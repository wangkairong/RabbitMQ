using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using RabbitMQ.Client;
using RabbitMQ.Client.Events;
namespace MessageQueueServer
{
    class Program
    {
        static void Main(string[] args)
        {
            //消费者
            ConnectionFactory factory = new ConnectionFactory();
            factory.HostName="127.0.0.1";
            //默认端口
            factory.Port = 5672;
            using (IConnection conn=factory.CreateConnection())
            {
                using (IModel channel=conn.CreateModel())
                {
                    //在MQ上定义一个持久化队列，如果名称相同不会重复创建
                    channel.QueueDeclare("MyRabbitMQ", true, false, false, null);
                    //输入1，那如果接收一个消息，但是没有应答，则客户端不会收到下一个消息
                    channel.BasicQos(0,1,false);
                    Console.WriteLine("Listening...");

                    //在队列定义一个消费者
                    QueueingBasicConsumer consumer = new QueueingBasicConsumer(channel);
                    //消费队列 并设置为主动应答模式
                    channel.BasicConsume("MyRabbitMQ",false,consumer);


                    while (true)
                    {
                        //阻塞函数 获取队列数据
                        BasicDeliverEventArgs eventArgs = consumer.Queue.Dequeue();

                        byte [] bytes = eventArgs.Body;
                        string str = Encoding.UTF8.GetString(bytes);

                        Console.WriteLine("读取队列的消息：" + str);
                        //回复确认
                        channel.BasicAck(eventArgs.DeliveryTag, false);
                    }
                }
            }
        }
    }
}
