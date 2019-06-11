using System;
using System.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
namespace NETCOREMessageQueueClient
{
    class Program
    {
        static void Main(string[] args)
        {
            string exchangeName = "TestTopicChange";
            string queueName1 = "NetCoreRabbotMQ1";
            string queueName2 = "NetCoreRabbotMQ2";
            string routeKey = "TestRouteKey.*";
            //创建连接工厂
            ConnectionFactory factory = new ConnectionFactory
            {
               
                HostName="127.0.0.1",
                Port=5672
            };
            //创建连接
            var connection = factory.CreateConnection();
            //创建通道
            var channel = connection.CreateModel();
            //定义一个Direct类型交换机
            channel.ExchangeDeclare(exchangeName, ExchangeType.Topic, false, false, null);
            //声明队列
            channel.QueueDeclare(queueName1, false , false, false, null);
            //channel.QueueDeclare(queueName2, false, false, false, null);
            //将队列绑定到交换机
            channel.QueueBind(queueName1, exchangeName, routeKey, null);
            //channel.QueueBind(queueName2, exchangeName, "", null);
            //生成2个队列得消费者
            //ConsumerGenerator(queueName1);
            //ConsumerGenerator(queueName2);
            Console.WriteLine("\nRabbitMQ连接成功，请输入消息，输入Exit退出！");
            string input;
            do
            {
                input = Console.ReadLine();
                var sendByte = Encoding.UTF8.GetBytes(input);
                //发布消息
                channel.BasicPublish(exchangeName, "TestRouteKey.one",null,sendByte);
            } while (input.Trim().ToLower()!="exit");

            channel.Close();
            connection.Close();
        }

        private static void ConsumerGenerator(string queueName1)
        {
            //创建连接工厂
            ConnectionFactory factory = new ConnectionFactory
            {
                HostName = "127.0.0.1",
                Port = 5672

            };
            //创建连接
            var conn = factory.CreateConnection();
            //创建通道
            var channel = conn.CreateModel();
            //事件基本消费者
            EventingBasicConsumer consumer = new EventingBasicConsumer(channel);
            //接收到消息事件

            consumer.Received += (ch, ea) =>
            {
                var message = Encoding.UTF8.GetString(ea.Body);
                Console.WriteLine($"Queue:{queueName1}收到消息：{message}");
                //确认消息已被消费
                channel.BasicAck(ea.DeliveryTag, false);
            };

            //启动消费这 设置为手动应答消息
            channel.BasicConsume(queueName1, false, consumer);
            Console.WriteLine($"Queue:{queueName1},消费者已启动");
        }
    }
}
