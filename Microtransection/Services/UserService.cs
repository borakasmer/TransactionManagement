using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microtransection.DB;
using Newtonsoft.Json;
using RabbitMQ.Client;

public class UserService : IUserService
{
    private readonly DenoContext _context;
    public UserService(DenoContext context)
    {
        _context = context;
    }
    public List<Users> GetAll()
    {       
        return _context.Users.ToList();
    }

    public string InsertUser(UserShop data)
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

            var stocData = JsonConvert.SerializeObject(data);
            var body = Encoding.UTF8.GetBytes(stocData);

            channel.BasicPublish(exchange: "",
                                 routingKey: "User",
                                 basicProperties: null,
                                 body: body);
            return $"[x] Sent {data.name}";
        }
    }
}