using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RabbitMQApp.Senders
{
    public static class FruitSender
    {
        private static readonly ConnectionFactory _factory;
        private static readonly IConnection _connection;
        private static readonly IModel _channel;

        private const string FruitExchange = "fruit_exchange";
        private const string FruitRoutingKey = "fruit.info";

        static FruitSender()
        {
            _factory = new ConnectionFactory() { HostName = "localhost", Port = 5672 };
            _connection = _factory.CreateConnection();
            _channel = _connection.CreateModel();

            _channel.ExchangeDeclare(exchange: FruitExchange, type: ExchangeType.Direct);

            Console.WriteLine("[FruitSender] Iniciado.");
        }

        public static void SendFruitInfo(string name, string description)
        {
            var fruitMessage = new FruitMessage
            {
                Name = name,
                Description = description,
                Timestamp = DateTime.Now
            };

            var message = JsonConvert.SerializeObject(fruitMessage);
            var body = Encoding.UTF8.GetBytes(message);

            _channel.BasicPublish(exchange: FruitExchange,
                                 routingKey: FruitRoutingKey,
                                 basicProperties: null,
                                 body: body);

            Console.WriteLine($"[FruitSender] Enviada informação sobre a fruta: {name}");
        }

        public static void Close()
        {
            _channel.Close();
            _connection.Close();
        }
    }
}
