namespace SW.VideoProcessingService.TacticalSolutionApp
{
    using RabbitMQ.Client;
    using RabbitMQ.Client.Events;
    using System;
    using System.Text;
    using System.Threading.Tasks;

    public class Consumer : IDisposable
    {
        private IConnection brokerConnection;
        private IModel connectionModel;
        private string Message { get; set; }

        public Consumer()
        {
        }

        internal void ConsumeFromQueue(string queueName, string exchangeName, string routingKey)
        {
            var factory = new ConnectionFactory() { HostName = "localhost" };
            this.brokerConnection = factory.CreateConnection();
            this.connectionModel = this.brokerConnection.CreateModel();

            Console.WriteLine($"Connection for the '{queueName}' was created \r\n(exchangeName: '{exchangeName}', routingKey: '{routingKey}').");

            connectionModel.BasicQos(
                prefetchSize: 0,
                prefetchCount: 1,
                global: true); 
            
            connectionModel.ExchangeDeclare(exchangeName, ExchangeType.Direct);

            connectionModel.QueueBind(
                queue: queueName,
                exchange: exchangeName,
                routingKey: routingKey);

            var consumer = new EventingBasicConsumer(connectionModel);

            consumer.Received += (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var message = GetMessage(body);
                var routingKey = ea.RoutingKey;

                this.Message = message;

                this.Proseccing().Wait();

                Console.WriteLine($" [] Received from '{queueName}': {message}");

                connectionModel.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);
            };

            connectionModel.BasicConsume(
                queue: queueName,
                autoAck: false,
                consumer: consumer);
        }

        public void Dispose()
        {
            brokerConnection?.Dispose();
            connectionModel?.Dispose();
        }

        internal void Consume(string exchangeName, string routingKey)
        {
            Console.WriteLine($"Successfully created Connection Model to RabbitMq");

            var queueName = connectionModel.QueueDeclare().QueueName;

            this.ConsumeFromQueue(
                    queueName: queueName,
                    exchangeName: exchangeName,
                    routingKey: routingKey);

        }

        private string GetMessage(byte[] body)
        {
            return Encoding.UTF8.GetString(body);
        }

        internal async Task Proseccing()
        {
            var x = this.Message.Substring(0, 1);
            var proseccingDuration = Convert.ToInt32(x);
            await Task.Delay(proseccingDuration * 500);
        }
    }
}
