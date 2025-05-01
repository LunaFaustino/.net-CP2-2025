using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RabbitMQApp.Models;
using RabbitMQ.Client.Events;
using Newtonsoft.Json;

namespace RabbitMQApp.Receivers
{
    public static class FruitReceiver
    {
        private static readonly ConnectionFactory _factory;
        private static readonly IConnection _connection;
        private static readonly IModel _channel;

        private const string FruitExchange = "fruit_exchange";
        private const string FruitQueue = "fruit_queue";
        private const string ValidatedFruitRoutingKey = "fruit.validated";

        static FruitReceiver()
        {
            _factory = new ConnectionFactory() { HostName = "localhost", Port = 5672 };
            _connection = _factory.CreateConnection();
            _channel = _connection.CreateModel();

            _channel.ExchangeDeclare(exchange: FruitExchange, type: ExchangeType.Direct);

            _channel.QueueDeclare(queue: FruitQueue, durable: true, exclusive: false, autoDelete: false, arguments: null);

            _channel.QueueBind(queue: FruitQueue, exchange: FruitExchange, routingKey: ValidatedFruitRoutingKey);

            Console.WriteLine("[FruitReceiver] Iniciado.");
        }

        public static void Start()
        {
            var consumer = new EventingBasicConsumer(_channel);

            consumer.Received += (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                var fruitMessage = JsonConvert.DeserializeObject<FruitMessage>(message);

                Console.WriteLine($"[FruitReceiver] Recebida mensagem validada sobre a fruta: {fruitMessage.Name}");
                Console.WriteLine($"Nome: {fruitMessage.Name}");
                Console.WriteLine($"Descrição: {fruitMessage.Description}");
                Console.WriteLine($"Data/Hora: {fruitMessage.Timestamp}");
                Console.WriteLine(new string('-', 50));

                _channel.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);
            };

            _channel.BasicConsume(queue: FruitQueue,
                                 autoAck: false,
                                 consumer: consumer);

            Console.WriteLine("[FruitReceiver] Aguardando mensagens...");
            Console.WriteLine("Pressione qualquer tecla para encerrar.");
            Console.ReadKey();

            _channel.Close();
            _connection.Close();
        }
    }
}
