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
    public static class UserReceiver
    {
        private static readonly ConnectionFactory _factory;
        private static readonly IConnection _connection;
        private static readonly IModel _channel;

        private const string UserExchange = "user_exchange";
        private const string UserQueue = "user_queue";
        private const string ValidatedUserRoutingKey = "user.validated";

        static UserReceiver()
        {
            _factory = new ConnectionFactory() { HostName = "localhost", Port = 5672 };
            _connection = _factory.CreateConnection();
            _channel = _connection.CreateModel();

            _channel.ExchangeDeclare(exchange: UserExchange, type: ExchangeType.Direct);

            _channel.QueueDeclare(queue: UserQueue, durable: true, exclusive: false, autoDelete: false, arguments: null);

            _channel.QueueBind(queue: UserQueue, exchange: UserExchange, routingKey: ValidatedUserRoutingKey);

            Console.WriteLine("[UserReceiver] Iniciado.");
        }

        public static void Start()
        {
            var consumer = new EventingBasicConsumer(_channel);

            consumer.Received += (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                var userMessage = JsonConvert.DeserializeObject<UserMessage>(message);

                Console.WriteLine($"[UserReceiver] Recebida mensagem validada sobre o usuário: {userMessage.FullName}");
                Console.WriteLine($"Nome Completo: {userMessage.FullName}");
                Console.WriteLine($"Endereço: {userMessage.Address}");
                Console.WriteLine($"RG: {userMessage.RG}");
                Console.WriteLine($"CPF: {userMessage.CPF}");
                Console.WriteLine($"Data de Registro: {userMessage.RegistrationDate}");
                Console.WriteLine($"Data/Hora: {userMessage.Timestamp}");
                Console.WriteLine(new string('-', 50));

                _channel.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);
            };

            _channel.BasicConsume(queue: UserQueue,
                                 autoAck: false,
                                 consumer: consumer);

            Console.WriteLine("[UserReceiver] Aguardando mensagens...");
            Console.WriteLine("Pressione qualquer tecla para encerrar.");
            Console.ReadKey();

            _channel.Close();
            _connection.Close();
        }
    }
}
