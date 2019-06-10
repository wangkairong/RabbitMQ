using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using RabbitMQ.Client;
namespace MessageQueueClient
{
    class Program
    {
        static void Main(string[] args)
        {
            //生产者
            ConnectionFactory factory = new ConnectionFactory();
            factory.HostName = "127.0.0.1";
            //默认端口
            factory.Port = 5672;

            using (IConnection conn=factory.CreateConnection())
            {
                using (IModel channel=conn.CreateModel())
                {
                    //在MQ上定义一个持久化队列，如果名称相同不会重复创建
                    channel.QueueDeclare("MyRabbitMQ", true, false, false, null);
                    while (true)
                    {
                        string message = string.Format("{0}", Console.ReadLine());

                        byte[] buffer = Encoding.UTF8.GetBytes(message);

                        IBasicProperties basicProperties = channel.CreateBasicProperties();
                        basicProperties.DeliveryMode = 2;
                        channel.BasicPublish("", "MyRabbitMQ",basicProperties,buffer);

                        Console.WriteLine("入队成功:"+message);

                    }
                }
            }
        }
    }
}
