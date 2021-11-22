using Microsoft.AspNetCore.Mvc;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mime;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using AutoMapper;
using Microsoft.Extensions.Logging;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace PostmanApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RabbitController : ControllerBase
    {
        public IConfiguration Configuration { get; }
        private ILogger<RabbitController> _logger;
        private readonly IMapper _mapper;

        public RabbitController(IConfiguration config, ILogger<RabbitController> logger, IMapper mapper)
        {
            Configuration = config;
            _logger = logger;
            _mapper = mapper;

            //ConnectionFactory factory1 = new ConnectionFactory();
            //factory1.UserName = "guest";
            //factory1.Password = "guest";
            //factory1.VirtualHost = "/";
            //factory1.HostName = "localhost:5671";
            ////factory.Uri = "amqp://user:pass@hostName:port/vhost";
            ////var endpoints = new System.Collections.Generic.List<AmqpTcpEndpoint> {
            ////    //new AmqpTcpEndpoint("hostname"),
            ////    new AmqpTcpEndpoint("localhost")
            ////};
            //IConnection conn = factory1.CreateConnection();
            //IModel channel1 = conn.CreateModel();
            //channel1.Close();
            //conn.Close();
        }

        //// GET: api/<RabbitController>
        //[HttpGet]
        //public async Task<IEnumerable<string>> Get()
        //{
        //    return new string[] { "value1", "value2" };
        //}

        //// GET api/<RabbitController>/5
        //[HttpGet("{id}")]
        //public string Get(int id)
        //{
        //    //var factory = new ConnectionFactory() { HostName = "localhost" };
        //    //using (var connection = factory.CreateConnection())
        //    //{
        //    //    using (var channel = connection.CreateModel())
        //    //    {
        //    //        channel.QueueDeclare(queue: "hello",
        //    //            durable: false,
        //    //            exclusive: false,
        //    //            autoDelete: false,
        //    //            arguments: null);
        //    //        var consumer = new EventingBasicConsumer(channel);
        //    //        consumer.Received += (model, ea) =>
        //    //        {
        //    //            var body = ea.Body.ToArray();
        //    //            var message = Encoding.UTF8.GetString(body);
        //    //            Debug.WriteLine(" [x] Received {0}", message);
        //    //        };
        //    //        channel.BasicConsume(queue: "hello",
        //    //            autoAck: true,
        //    //            consumer: consumer);
        //    //    }
        //    //}
        //    return "value";
        //}

        // POST api/<RabbitController>
        [HttpPost]
        [Consumes(MediaTypeNames.Application.Json, MediaTypeNames.Application.Xml)]
        [Produces(MediaTypeNames.Application.Json, MediaTypeNames.Application.Xml)]
        public async Task Post([FromBody] string value)
        {
            var factory = new ConnectionFactory() { HostName = "localhost" };
            using (var connection = factory.CreateConnection())
            {
                using (var channel = connection.CreateModel())
                {
                    channel.QueueDeclare(queue: "hello",
                        durable: false,
                        exclusive: false,
                        autoDelete: false,
                        arguments: null);

                    string message = "Hello World!";
                    var body = Encoding.UTF8.GetBytes(message);

                    channel.BasicPublish(exchange: "",
                        routingKey: "hello",
                        basicProperties: null,
                        body: body);
                    Console.WriteLine(" [x] Sent {0}", message);
                }
            }
        }

        //// PUT api/<RabbitController>/5
        //[HttpPut("{id}")]
        //public async Task Put(int id, [FromBody] string value)
        //{
        //}

        //// DELETE api/<RabbitController>/5
        //[HttpDelete("{id}")]
        //public async Task Delete(int id)
        //{
        //}
    }
}
