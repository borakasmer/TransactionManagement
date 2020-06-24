using System;
using System.Text;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using userConsumer.DB;
using System.Linq;
using System.Collections.Generic;

namespace userConsumer
{
    //dotnet add package Microsoft.EntityFrameworkCore --version 3.1.5
    //dotnet add package Microsoft.EntityFrameworkCore.SqlServer
    //dotnet add package Microsoft.EntityFrameworkCore.Tools
    //dotnet ef dbcontext Scaffold "Server=tcp:192.168.1.7,1433;Database=Deno;User ID=****;Password=****;" Microsoft.EntityFrameworkCore.SqlServer --force -o DB -c DenoContext
    class Program
    {
        static void Main(string[] args)
        {
            var factory = new ConnectionFactory() { HostName = "localhost" };
            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                channel.QueueDeclare(queue: "User",
                                     durable: false,
                                     exclusive: false,
                                     autoDelete: false,
                                     arguments: null);

                var consumer = new EventingBasicConsumer(channel);
                consumer.Received += (model, ea) =>
                {
                    var body = ea.Body.Span;
                    var data = Encoding.UTF8.GetString(body);
                    UserShop user = JsonConvert.DeserializeObject<UserShop>(data);
                    Console.WriteLine(" [x] Received {0}", user.name + " : " + user.surname);

                    using (DenoContext context = new DenoContext())
                    {
                        if (!context.Users.Any(us => us.Name == user.name && us.Surname == user.surname))
                        {
                            Users userModel = new Users();
                            userModel.Name = user.name;
                            userModel.Surname = user.surname;
                            userModel.No = user.no;
                            userModel.IsActive = false;
                            context.Users.Add(userModel);
                            context.SaveChanges();

                            ProductConsumer product = new ProductConsumer();
                            product.Name = user.productName;
                            product.Price = user.productPrice;
                            product.UserId = userModel.Id;
                            product.IsActive = false;

                            List<TransactionHistory> listTransaction = new List<TransactionHistory>();
                            listTransaction.Add(new TransactionHistory() { TableName = "Users", ID = userModel.Id });
                            product.TransactionList = listTransaction;

                            Console.WriteLine(PushRabbitMQ(product));
                        }
                        else
                        {
                            int userID = context.Users.FirstOrDefault(us => us.Name == user.name && us.Surname == user.surname).Id;
                            ProductConsumer product = new ProductConsumer();
                            product.Name = user.productName;
                            product.Price = user.productPrice;
                            product.UserId = userID;
                            product.IsActive = false;

                            List<TransactionHistory> listTransaction = new List<TransactionHistory>();
                            product.TransactionList = listTransaction;

                            Console.WriteLine(PushRabbitMQ(product));
                        }
                    }
                    //-------------------------

                };
                channel.BasicConsume(queue: "User",
                                     autoAck: true,
                                     consumer: consumer);

                Console.WriteLine(" Press [enter] to exit.");
                Console.ReadLine();
            }
        }

        public static string PushRabbitMQ(ProductConsumer data)
        {
            var factory = new ConnectionFactory() { HostName = "localhost" };
            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                channel.QueueDeclare(queue: "Product",
                                     durable: false,
                                     exclusive: false,
                                     autoDelete: false,
                                     arguments: null);

                var stocData = JsonConvert.SerializeObject(data);
                var body = Encoding.UTF8.GetBytes(stocData);

                channel.BasicPublish(exchange: "",
                                     routingKey: "Product",
                                     basicProperties: null,
                                     body: body);
                return $"[x] Sent Product : {data.Name}";
            }
        }
    }
}
