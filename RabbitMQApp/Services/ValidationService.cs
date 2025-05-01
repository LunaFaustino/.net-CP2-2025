using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMQApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RabbitMQApp.Services
{
    public static class ValidationService
    {
        private static readonly ConnectionFactory _factory;
        private static readonly IConnection _connection;
        private static readonly IModel _channel;

        private const string FruitExchange = "fruit_exchange";
        private const string UserExchange = "user_exchange";
        private const string FruitValidationQueue = "fruit_validation_queue";
        private const string UserValidationQueue = "user_validation_queue";
        private const string FruitRoutingKey = "fruit.info";
        private const string UserRoutingKey = "user.info";
        private const string ValidatedFruitRoutingKey = "fruit.validated";
        private const string ValidatedUserRoutingKey = "user.validated";

        static ValidationService()
        {
            _factory = new ConnectionFactory() { HostName = "localhost", Port = 5672 };
            _connection = _factory.CreateConnection();
            _channel = _connection.CreateModel();

            _channel.ExchangeDeclare(exchange: FruitExchange, type: ExchangeType.Direct);
            _channel.ExchangeDeclare(exchange: UserExchange, type: ExchangeType.Direct);

            _channel.QueueDeclare(queue: FruitValidationQueue, durable: true, exclusive: false, autoDelete: false, arguments: null);
            _channel.QueueDeclare(queue: UserValidationQueue, durable: true, exclusive: false, autoDelete: false, arguments: null);

            _channel.QueueBind(queue: FruitValidationQueue, exchange: FruitExchange, routingKey: FruitRoutingKey);
            _channel.QueueBind(queue: UserValidationQueue, exchange: UserExchange, routingKey: UserRoutingKey);

            var fruitConsumer = new EventingBasicConsumer(_channel);
            fruitConsumer.Received += (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                var fruitMessage = JsonConvert.DeserializeObject<FruitMessage>(message);

                Console.WriteLine($"[Validation] Recebida mensagem de fruta: {fruitMessage.Name}");

                if (ValidateFruit(fruitMessage))
                {
                    Console.WriteLine($"[Validation] Fruta {fruitMessage.Name} validada com sucesso.");

                    var validatedMessage = JsonConvert.SerializeObject(fruitMessage);
                    var validatedBody = Encoding.UTF8.GetBytes(validatedMessage);

                    _channel.BasicPublish(exchange: FruitExchange,
                                         routingKey: ValidatedFruitRoutingKey,
                                         basicProperties: null,
                                         body: validatedBody);
                }
                else
                {
                    Console.WriteLine($"[Validation] Fruta {fruitMessage.Name} falhou na validação.");
                }

                _channel.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);
            };

            var userConsumer = new EventingBasicConsumer(_channel);
            userConsumer.Received += (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                var userMessage = JsonConvert.DeserializeObject<UserMessage>(message);

                Console.WriteLine($"[Validation] Recebida mensagem de usuário: {userMessage.FullName}");

                if (ValidateUser(userMessage))
                {
                    Console.WriteLine($"[Validation] Usuário {userMessage.FullName} validado com sucesso.");

                    var validatedMessage = JsonConvert.SerializeObject(userMessage);
                    var validatedBody = Encoding.UTF8.GetBytes(validatedMessage);

                    _channel.BasicPublish(exchange: UserExchange,
                                         routingKey: ValidatedUserRoutingKey,
                                         basicProperties: null,
                                         body: validatedBody);
                }
                else
                {
                    Console.WriteLine($"[Validation] Usuário {userMessage.FullName} falhou na validação.");
                }

                _channel.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);
            };

            _channel.BasicConsume(queue: FruitValidationQueue,
                                 autoAck: false,
                                 consumer: fruitConsumer);

            _channel.BasicConsume(queue: UserValidationQueue,
                                 autoAck: false,
                                 consumer: userConsumer);

            Console.WriteLine("[Validation] Serviço de validação iniciado.");
        }

        private static bool ValidateFruit(FruitMessage fruit)
        {
            return !string.IsNullOrWhiteSpace(fruit.Name) &&
                   !string.IsNullOrWhiteSpace(fruit.Description) &&
                   fruit.Timestamp != DateTime.MinValue;
        }

        private static bool ValidateUser(UserMessage user)
        {
            return !string.IsNullOrWhiteSpace(user.FullName) &&
                   !string.IsNullOrWhiteSpace(user.Address) &&
                   !string.IsNullOrWhiteSpace(user.RG) &&
                   !string.IsNullOrWhiteSpace(user.CPF) &&
                   user.CPF.Length == 11 &&
                   user.RegistrationDate != DateTime.MinValue &&
                   user.Timestamp != DateTime.MinValue;
        }

        public static void Start()
        {
            Console.WriteLine("[Validation] Serviço de validação em execução. Pressione qualquer tecla para encerrar.");
            Console.ReadKey();

            _channel.Close();
            _connection.Close();
        }
    }
}
