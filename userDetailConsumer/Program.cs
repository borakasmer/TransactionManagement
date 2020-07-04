using System;
using System.Text;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using userDetailConsumer.DB;
using System.Linq;
namespace userDetailConsumer
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
                channel.QueueDeclare(queue: "UserDetail",
                                     durable: false,
                                     exclusive: false,
                                     autoDelete: false,
                                     arguments: null);

                var consumer = new EventingBasicConsumer(channel);
                consumer.Received += (model, ea) =>
                {
                    var body = ea.Body.Span;
                    var data = Encoding.UTF8.GetString(body);
                    UserDetailQueue userDetail = JsonConvert.DeserializeObject<UserDetailQueue>(data);
                    Console.WriteLine(" [x] Received {0}", userDetail.UserId + " : " + userDetail.ProductId);

                    using (DenoContext context = new DenoContext())
                    {
                        UserDetails userDetailModel = new UserDetails();
                        userDetailModel.UserId = userDetail.UserId;
                        userDetailModel.ProductId = userDetail.ProductId;
                        userDetailModel.CreatedDate = DateTime.Now;
                        userDetailModel.IsActive = true;
                        context.UserDetails.Add(userDetailModel);
                        context.SaveChanges();

                        foreach (TransactionHistory dataTransaction in userDetail.TransactionList)
                        {
                            string TableName = dataTransaction.TableName;
                            int ID = dataTransaction.ID;
                            //Different Resource Type Case SQL
                            //Strategy Design Pattern
                            if (dataTransaction.Type == TransactionType.SqlDB && dataTransaction.State == TransactionState.Pending)
                            {
                                context.ExecuteQuery($"UPDATE {TableName}  SET IsActive = 1 WHERE Id = {ID}");
                                dataTransaction.State = TransactionState.Completed;                             
                            }

                            //Run Different Bussines Logic By Step Name
                            if (dataTransaction.Step == TransactionStep.Product && dataTransaction.State==TransactionState.Completed)
                            {

                            }
                        }
                    }
                    //-------------------------

                };
                channel.BasicConsume(queue: "UserDetail",
                                     autoAck: true,
                                     consumer: consumer);

                Console.WriteLine(" Press [enter] to exit.");
                Console.ReadLine();
            }
        }
    }
}
