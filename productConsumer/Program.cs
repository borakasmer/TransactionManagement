using System;
using System.Text;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using productConsumer.DB;
using System.Linq;
using System.Collections.Generic;
namespace productConsumer
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
                channel.QueueDeclare(queue: "Product",
                                     durable: false,
                                     exclusive: false,
                                     autoDelete: false,
                                     arguments: null);

                var consumer = new EventingBasicConsumer(channel);
                consumer.Received += (model, ea) =>
                {
                    var body = ea.Body.Span;
                    var data = Encoding.UTF8.GetString(body);
                    ProductConsumer product = JsonConvert.DeserializeObject<ProductConsumer>(data);
                    Console.WriteLine(" [x] Received {0}", product.Name + " : " + product.Price);

                    using (DenoContext context = new DenoContext())
                    {
                        if (!context.Products.Any(pro => pro.Name == product.Name && pro.Price == product.Price))
                        {
                            Products productModel = new Products();
                            productModel.Name = product.Name;
                            productModel.Price = product.Price;
                            productModel.IsActive = false;
                            context.Products.Add(productModel);
                            context.SaveChanges();

                            UserDetailQueue userDetail = new UserDetailQueue();
                            userDetail.ProductId = productModel.Id;
                            userDetail.UserId = product.UserId;
                            userDetail.IsActive = false;

                            List<TransactionHistory> listTransaction = product.TransactionList;
                            listTransaction.Add(new TransactionHistory()
                            {
                                TableName = "Products",
                                ID = productModel.Id,
                                State = TransactionState.Pending,
                                Step = TransactionStep.Product,
                                Type = TransactionType.SqlDB
                            });
                            userDetail.TransactionList = listTransaction;

                            Console.WriteLine(PushRabbitMQ(userDetail));
                        }
                        else
                        {
                            int productID = context.Products.FirstOrDefault(pro => pro.Name == product.Name && pro.Price == product.Price).Id;
                            UserDetailQueue userDetail = new UserDetailQueue();
                            userDetail.ProductId = productID;
                            userDetail.UserId = product.UserId;
                            userDetail.IsActive = false;

                            List<TransactionHistory> listTransaction = product.TransactionList;
                            userDetail.TransactionList = listTransaction;

                            Console.WriteLine(PushRabbitMQ(userDetail));
                        }
                    }
                    //-------------------------

                };
                channel.BasicConsume(queue: "Product",
                                     autoAck: true,
                                     consumer: consumer);

                Console.WriteLine(" Press [enter] to exit.");
                Console.ReadLine();
            }
        }

        public static string PushRabbitMQ(UserDetails data)
        {
            var factory = new ConnectionFactory() { HostName = "localhost" };
            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                channel.QueueDeclare(queue: "UserDetail",
                                     durable: false,
                                     exclusive: false,
                                     autoDelete: false,
                                     arguments: null);

                var stocData = JsonConvert.SerializeObject(data);
                var body = Encoding.UTF8.GetBytes(stocData);

                channel.BasicPublish(exchange: "",
                                     routingKey: "UserDetail",
                                     basicProperties: null,
                                     body: body);
                return $"[x] Sent UserDetail => UserID: {data.UserId} ProductID: {data.ProductId}";
            }
        }
    }
}
