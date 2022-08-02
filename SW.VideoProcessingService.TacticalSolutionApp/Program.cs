namespace SW.VideoProcessingService.TacticalSolutionApp
{
    using System;

    class Program
    {
        static void Main(string[] args)
        {
            var queueName = "queue_name";
            var exchangeForQueue = "queue_video_exchange";
            var routinKeyForQueue = "queue_routingKey";

            var consumer = new Consumer();
            
            consumer.ConsumeFromQueue(
                    queueName: queueName,
                    exchangeName: exchangeForQueue,
                    routingKey: routinKeyForQueue);

            SomeLogic();

            consumer.Dispose();
        }

        private static void SomeLogic()
        {
            ConsoleKey key;
            do
            {
                key = Console.ReadKey().Key;
                switch (key)
                {
                    case ConsoleKey.D1:
                        Console.WriteLine("First message");
                        break;
                    case ConsoleKey.D2:
                        Console.WriteLine("Second message");
                        break;
                    case ConsoleKey.D3:
                        Console.WriteLine("Third message");
                        break;
                    case ConsoleKey.D4:
                        Console.WriteLine("Fourth message");
                        break;
                    case ConsoleKey.D5:
                        Console.WriteLine("Fifth message");
                        break;

                    default:
                        Console.WriteLine("Help menu with all commands");
                        break;
                }
            }
            while (key != ConsoleKey.Escape);
        }
    }
}
