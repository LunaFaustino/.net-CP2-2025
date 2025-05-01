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
    public static class UserSender
    {
        private static readonly ConnectionFactory _factory;
        private static readonly IConnection _connection;
        private static readonly IModel _channel;

        private const string UserExchange = "user_exchange";
        private const string UserRoutingKey = "user.info";

        static UserSender()
        {
            _factory = new ConnectionFactory() { HostName = "localhost", Port = 5672 };
            _connection = _factory.CreateConnection();
            _channel = _connection.CreateModel();

            _channel.ExchangeDeclare(exchange: UserExchange, type: ExchangeType.Direct);

            Console.WriteLine("[UserSender] Iniciado.");
        }

        public static void SendUserInfo(string fullName, string address, string rg, string cpf, DateTime registrationDate)
        {
            var userMessage = new UserMessage
            {
                FullName = fullName,
                Address = address,
                RG = rg,
                CPF = cpf,
                RegistrationDate = registrationDate,
                Timestamp = DateTime.Now
            };

            var message = JsonConvert.SerializeObject(userMessage);
            var body = Encoding.UTF8.GetBytes(message);

            _channel.BasicPublish(exchange: UserExchange,
                                 routingKey: UserRoutingKey,
                                 basicProperties: null,
                                 body: body);

            Console.WriteLine($"[UserSender] Enviada informação sobre o usuário: {fullName}");
        }

        public static void Close()
        {
            _channel.Close();
            _connection.Close();
        }
    }
}
