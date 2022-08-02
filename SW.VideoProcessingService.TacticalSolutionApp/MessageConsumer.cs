namespace SW.VideoProcessingService.TacticalSolutionApp
{
    using System;
    using RabbitMQ.Client;
    using RabbitMQ.Client.Events;
    using System.Text;
    using SW.VideoProcessingService.TacticalSolutionApp.Enums;

    public class MessageConsumer
    {
        public void Consume(string queueName)
        {
            var factory = new ConnectionFactory() { HostName = "localhost" };
            using (var connection = factory.CreateConnection())
            {
                using (var channel = connection.CreateModel())
                {
                    channel.QueueDeclare(queue: queueName,
                        durable: false,
                        exclusive: false,
                        autoDelete: false,
                        arguments: null);
                    var consumer = new EventingBasicConsumer(channel);
                    int messageCounter = 0;
                    consumer.Received += (model, ea) =>
                    {
                        var body = ea.Body.ToArray();
                        var message = Encoding.UTF8.GetString(body);
                        messageCounter++;
                        Console.WriteLine($" [{messageCounter}] Received {message}");
                        new MessageReader().Read(message);
                        Console.WriteLine($" [{messageCounter}] Done");

                        channel.BasicAck(deliveryTag:ea.DeliveryTag,
                                        multiple: false);
                    };
                    channel.BasicConsume(queue: queueName,
                        autoAck: false,
                        consumer: consumer);
                    Console.WriteLine(" Press [enter] to exit.");
                    Console.ReadLine();
                }
            }
        }
        /// <summary>
        /// Method that is consume from topic exchager really easy
        /// </summary>
        /// <param name="exchangeName">Name for exchange</param>
        /// <param name="routingKey">Routing key that will be </param>
        internal void ConsumeFromTopicExchange(string exchangeName, string routingKey)
        {
            var factory = new ConnectionFactory() { HostName = "localhost" };
            using (var connection = factory.CreateConnection())
            {
                using (var channel = connection.CreateModel())
                {
                    channel.ExchangeDeclare(exchangeName, ExchangeType.Topic);

                    var queueName = channel.QueueDeclare().QueueName;

                    channel.QueueBind(queue: queueName,
                        exchange: exchangeName,
                        routingKey: routingKey);

                    Console.WriteLine($" [{routingKey}] Waiting for logs.");

                    var consumer = new EventingBasicConsumer(channel);
                    int messageCounter = 0;

                    consumer.Received += (model, ea) =>
                    {
                        var body = ea.Body.ToArray();
                        var message = Encoding.UTF8.GetString(body);
                        var routingKey = ea.RoutingKey;

                        messageCounter++;
                        Console.WriteLine($" [{messageCounter}] Received '{routingKey}': {message}");
                    };
                    channel.BasicConsume(queue: queueName,
                        autoAck: false,
                        consumer: consumer);


                    Console.WriteLine(" Press [enter] to exit.");
                    Console.ReadLine();
                }
            }
        }

        public void ConsumeFromExchange(string exchangeName)
        {
            var factory = new ConnectionFactory() { HostName = "localhost" };
            using (var connection = factory.CreateConnection())
            {
                using (var channel = connection.CreateModel())
                {
                    channel.ExchangeDeclare(exchange: "logs", type: ExchangeType.Fanout);

                    var queName = channel.QueueDeclare().QueueName;

                    channel.QueueBind(queue: queName,
                        exchange: exchangeName,
                        routingKey: "");

                    Console.WriteLine(" [*] Waiting for logs.");
;
                    var consumer = new EventingBasicConsumer(channel);

                    int messageCounter = 0;
                    consumer.Received += (model, ea) =>
                    {
                        var body = ea.Body.ToArray();
                        var message = Encoding.UTF8.GetString(body);
                        messageCounter++;
                        Console.WriteLine($" [{messageCounter}] Received {message}");
                        new MessageReader().Read(message);
                        Console.WriteLine($" [{messageCounter}] Done");

                        channel.BasicAck(deliveryTag: ea.DeliveryTag,
                                        multiple: false);
                    };

                    channel.BasicConsume(queue: queName,
                        autoAck: false,
                        consumer: consumer);
                    Console.WriteLine(" Press [enter] to exit.");
                    Console.ReadLine();
                }
            }
        }

        public void ConsumeFromDirectExchange(string exchangeName, string severity)
        {
            var factory = new ConnectionFactory() { HostName = "localhost" };
            using (var connection = factory.CreateConnection())
            {
                using (var channel = connection.CreateModel())
                {
                    channel.ExchangeDeclare(exchangeName, ExchangeType.Direct);

                    var queueName = channel.QueueDeclare().QueueName;

                    channel.QueueBind(queue: queueName,
                        exchange: exchangeName,
                        routingKey: severity);

                    Console.WriteLine(" [*] Waiting for messages.");

                    var consumer = new EventingBasicConsumer(channel);
                    int messageCounter = 0;

                    consumer.Received += (model, ea) =>
                    {
                        var body = ea.Body.ToArray();
                        var message = Encoding.UTF8.GetString(body);
                        var routingKey = ea.RoutingKey;

                        messageCounter++;
                        Console.WriteLine($" [{messageCounter}] Received '{routingKey}': {message}");
                    };
                    channel.BasicConsume(queue: queueName,
                        autoAck: false,
                        consumer: consumer);

                    
                    Console.WriteLine(" Press [enter] to exit.");
                    Console.ReadLine();
                }
            }
        }

         void OldMain(string[] args)
        {
            new MessageConsumer().Consume("Hello");
            new MessageConsumer().ConsumeFromExchange("logs");

            Console.WriteLine("Please, choose your routing key: [1]:info, [2]:warning, [3]:error");
            var routingKey = GetRoutingKey( Console.ReadLine());
            new MessageConsumer().ConsumeFromDirectExchange("direct_logs", routingKey);

            Console.WriteLine("Please, choose your routing key: [1]:'fly.orange', [2]:'fox.red', [3]:'*.red', [4]:'fly.*', [other]:'*.*' ");
            var routingKeyTopic = GetRoutingKeyForTopic(Console.ReadLine());
            new MessageConsumer().ConsumeFromTopicExchange("topic_logs", routingKeyTopic);
        }

        private  string GetRoutingKeyForTopic(string str)
        {
            switch (str)
            {
                case "1":
                    return "fly.orange";
                case "2":
                    return "fox.red";
                case "3":
                    return "*.red";
                case "4":
                    return "fly.*";
                default:
                    return "*.*";
            }
        }

        private  string GetRoutingKey(string str)
        {
            switch (str)
            {
                case "1":
                    return Severity.info.ToString();
                case "2":
                    return Severity.warning.ToString();
                default:
                    return Severity.error.ToString();
            }
        }
    }
}
