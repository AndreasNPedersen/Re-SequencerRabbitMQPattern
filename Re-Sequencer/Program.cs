using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

namespace Re_Sequencer
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Sequencer sequencer = new Sequencer();
            var factory = new ConnectionFactory { HostName = "localhost" };
            using var connection = factory.CreateConnection();
            using var channel = connection.CreateModel();
            channel.ExchangeDeclare(exchange: "Agg", type: ExchangeType.Direct);
            channel.QueueDeclare(queue: "Agg",
                                 durable: false,//should be true in prod
                                 exclusive: false,
                                 autoDelete: false,
                                 arguments: null);
            channel.QueueBind("Agg", "Agg", "input");

            channel.BasicQos(prefetchSize: 0, prefetchCount: 1, global: false);

            Console.WriteLine(" [*] Waiting for messages.");

            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += (model, ea) =>
            {
                byte[] body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                Console.WriteLine($" [x] Received {message}");
                int id = Convert.ToInt32(ea.BasicProperties.CorrelationId);
                //publish
                sequencer.SendInSequence(channel,id,message);
                Console.WriteLine(" [x] Done");

            };
            channel.BasicConsume(queue: "Agg",
                                 autoAck: true,
                                 consumer: consumer);

            Console.WriteLine(" Press [enter] to exit.");
            Console.ReadLine();
        }
    }
}