using PlatformService.Dtos;
using Microsoft.Extensions.Configuration;
using RabbitMQ.Client;
using System.Text.Json;
using System.Text;
using System;
namespace PlatformService.AsyncDataServices
{
    public class MessageBusClient : IMessageBusClient
    {
        private readonly IConfiguration _config;
        private readonly IConnection _connection;
        private readonly IModel _channel;
        public MessageBusClient(IConfiguration configuration)
        {
            _config = configuration;
            var factory = new ConnectionFactory()
            {
                HostName = _config["RabbitMQHost"],
                Port = int.Parse(_config["RabbitMQPort"])
            };
            try
            {
                _connection = factory.CreateConnection();
                _channel = _connection.CreateModel();
                _channel.ExchangeDeclare(exchange: "trigger", type: ExchangeType.Fanout);
                _connection.ConnectionShutdown += RabbitMQ_ConnectionShutdown;
                Console.WriteLine("---> Connected to message bus");
            }
            catch(Exception ex)
            {
                Console.WriteLine("---> Could not connect to the message bus"+ex.Message);
            }
        }
        public void PublishMessage(PlatformPublishDto platformPublishDto)
        {
            var message = JsonSerializer.Serialize(platformPublishDto);
            if(_connection.IsOpen)
            {
                Console.WriteLine("---> RabbitMQ connection Open. Sending Message ...");
                SendMessage(message);
            }
            else{
                Console.WriteLine("---> RabbitMQ connection closed. Sending message failed");
            }
        }

        private void SendMessage(string message)
        {
            var body = Encoding.UTF8.GetBytes(message);
            _channel.BasicPublish(exchange: "trigger", routingKey: "", basicProperties: null, body: body);
            Console.WriteLine("---> Message sent");
        }

        public void Dispose()
        {
            Console.WriteLine("---> Message bus disposed");
            if(_channel.IsOpen)
            {
                _channel.Close();
                _connection.Close();
            }
        }

        private void RabbitMQ_ConnectionShutdown(object sender, ShutdownEventArgs e)
        {
            Console.WriteLine("---> RabbitMQ connection shutdown");
        }
    }
}