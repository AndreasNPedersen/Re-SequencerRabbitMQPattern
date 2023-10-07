using RabbitMQ.Client;
using System.Text;

namespace Producer
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var factory = new ConnectionFactory { HostName = "localhost" };
            using var connection = factory.CreateConnection();
            using var channel = connection.CreateModel();

                for (int i = 10; i > 0; i--)
                {
                    var properties = channel.CreateBasicProperties();
                    properties.CorrelationId = i.ToString();
                    var message = "Hello World! : " + i;
                    var body = Encoding.UTF8.GetBytes(message);
                    channel.BasicPublish(exchange: "Agg",
                                         routingKey: "input",
                                         basicProperties: properties,
                                         body: body);
                    Console.WriteLine($" [x] Sent message: '{message}'");
                }
            

            Console.WriteLine(" Press [enter] to exit.");
            Console.ReadLine();
        }
    }
}