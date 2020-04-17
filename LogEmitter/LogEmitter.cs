using System.Linq;
using System.Collections.Generic;
using System;
using RabbitMQ.Client;
using System.Text;

namespace LogEmitter
{
    class LogEmitter
    {
        private static Dictionary<string, string> logMessage = new Dictionary<string, string>
        {
            {"info", "Info message"},
            {"warning", "Warning message!"},
            {"error", "Error message!!"},
            {"critical", "Critical message!!!"}
        };
        static void Main(string[] args)
        {
            var factory = new ConnectionFactory() { HostName = "localhost" };
            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                channel.ExchangeDeclare(exchange: "direct_logs", type: "direct");

                int nbLogs = (args.Length > 0 && Int32.TryParse(args[0], out int result) && result > 0) ? result : 1;
                Random random = new Random();

                for (int i = 0; i < nbLogs; i++)
                {
                    var log = logMessage.ElementAt(random.Next(0, logMessage.Count));
                    var body = Encoding.UTF8.GetBytes(log.Value);
                    channel.BasicPublish(exchange: "direct_logs",
                                         routingKey: log.Key,
                                         basicProperties: null,
                                         body: body);
                    Console.WriteLine($" [x] Log sent: [{log.Key}]: {log.Value}");
                }
                Console.WriteLine(" Press [enter] to exit.");
                Console.ReadLine();
            }
        }
    }
}
