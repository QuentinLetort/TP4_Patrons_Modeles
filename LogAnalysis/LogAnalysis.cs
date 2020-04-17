using System.Collections.Generic;
using System;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Threading;
using System.Globalization;

namespace LogAnalysis
{
    class LogAnalysis
    {
        private static string[] severities = new string[]{
            "info",
            "warning",
            "error",
            "critical"
        };

        private static Dictionary<string, int> logs = InitializeLogsDict();

        static Dictionary<string,int> InitializeLogsDict()
        {
            var result = new Dictionary<string, int>();
            foreach (string severity in severities)
            {
                result[severity] = 0;
            }
            return result;
        }

        static void ShowAnalysis()
        {
            foreach (var log in logs)
            {
                var value = (log.Value > 0) ? log.Value + " logs" : log.Value + " log";
                Console.WriteLine($"{CultureInfo.CurrentCulture.TextInfo.ToTitleCase(log.Key)} : {value}");
            }
            Console.WriteLine();
        }

        static void Main(string[] args)
        {
            var factory = new ConnectionFactory() { HostName = "localhost" };
            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
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
                    var routingKey = ea.RoutingKey;
                    logs[routingKey]++;
                };

                channel.BasicConsume(queue: queueName, autoAck: true, consumer: consumer);
                while (true)
                {
                    ShowAnalysis();
                    Thread.Sleep(10000);
                }
            }
        }
        
    }
}
