using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;
using CommandService.EventProcessing;
using System.Threading;
using System.Threading.Tasks;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Text;
namespace CommandService.AsyncDataService
{
    public class MessageBusSubscriber : BackgroundService
    {
        private readonly IConfiguration _config;
        private readonly IEventProcessor _eventProcessor;
        private  IConnection _connection;
        private  IModel _channel;
        private string _queueName;
        public MessageBusSubscriber(IConfiguration config, IEventProcessor eventProcessor)
        {
            _config = config;
            _eventProcessor = eventProcessor;
            InitializeRabbitMQ();
        }

        private void InitializeRabbitMQ()
        {
            var factory = new ConnectionFactory()
            {
                
                HostName = _config["RabbitMQHost"],
                Port = int.Parse(_config["RabbitMQPort"])
            };
            
            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();
            _channel.ExchangeDeclare(exchange: "trigger", type: ExchangeType.Fanout);
            _queueName = _channel.QueueDeclare().QueueName;
            _channel.QueueBind(queue: _queueName, exchange: "trigger", routingKey: "");
            _connection.ConnectionShutdown += RabbitMQ_ConnectionShutdown;
            Console.WriteLine("---> Listening on the message buss");
        }

        protected override async Task  ExecuteAsync(CancellationToken stoppingToken)
        {
            stoppingToken.ThrowIfCancellationRequested();
            var consumer = new EventingBasicConsumer(_channel);
            consumer.Received += (ModuleHandle, ea) => 
            {
                Console.WriteLine("---> Event received!");

                var body = ea.Body;
                var notification = Encoding.UTF8.GetString(body.ToArray());
                _eventProcessor.ProcessEvent(notification);
            };

            _channel.BasicConsume(queue: _queueName, autoAck: true, consumer: consumer);
        }

        public override void Dispose()
        {
            Console.WriteLine("---> Message bus disposed");
            if(_channel.IsOpen)
            {
                _channel.Close();
                _connection.Close();
            }

            base.Dispose();
        }

        private void RabbitMQ_ConnectionShutdown(object sender, ShutdownEventArgs e)
        {
            Console.WriteLine("---> RabbitMQ connection shutdown");
        }
    }
}