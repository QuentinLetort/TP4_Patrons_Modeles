using System.Globalization;
using System.IO;
using System;
using System.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace FileDump
{
    class FileDump
    {
        private static string[] severities = new string[]{
            "warning",
            "error",
            "critical"
        };

        private static string defaultFileName = "default.log";

        static void Main(string[] args)
        {
            string filePath = (args.Length > 0) ? args[0] : defaultFileName;

            var factory = new ConnectionFactory() { HostName = "localhost" };

            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            using (StreamWriter writer = File.AppendText(filePath))
            {
                channel.ExchangeDeclare(exchange: "direct_logs", type: "direct");
                string queueName = channel.QueueDeclare().QueueName;

                foreach (string severity in severities)
                {
                    channel.QueueBind(queue: queueName, exchange: "direct_logs", routingKey: severity);
                }

                var consumer = new EventingBasicConsumer(channel);
                consumer.Received += (model, ea) =>
                {
                    var body = ea.Body;
                    var message = Encoding.UTF8.GetString(body);
                    var routingKey = ea.RoutingKey;
                    var date = DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss", DateTimeFormatInfo.InvariantInfo);
                    writer.WriteLine($"[{date}] [{routingKey.ToUpper()}]: {message}");
                    writer.Flush();
                };

                channel.BasicConsume(queue: queueName, autoAck: true, consumer: consumer);
                Console.WriteLine($" Redirection of logs to {filePath}");
                Console.WriteLine(" Press [enter] to exit.");
                Console.ReadLine();
            }

        }
    }
}
