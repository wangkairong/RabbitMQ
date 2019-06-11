using System;
using System.Text;
using System.Threading;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
namespace NETCOREMessageQueueServer
{
    class Program
    {
        static void Main(string[] args)
        {
            //创建连接工厂
            ConnectionFactory factory = new ConnectionFactory
            {
                HostName = "127.0.0.1",
                Port=5672
                
            };
            //创建连接
            IConnection conn = factory.CreateConnection();
            //创建通道
            IModel channel = conn.CreateModel();


            #region 不使用using

            //事件基本消费者
            EventingBasicConsumer consumer = new EventingBasicConsumer(channel);
            //接收到消息事件
            consumer.Received += (ch, ea) =>
            {
                var message = Encoding.UTF8.GetString(ea.Body);
                Console.WriteLine($"收到消息：{message}");

               // Console.WriteLine($"收到该消息[{ea.DeliveryTag}]延迟10S发送回执");
               // Thread.Sleep(1000);
                //确认消息已被消费
                channel.BasicAck(ea.DeliveryTag, false);
                //Console.WriteLine($"已发送回执[{ea.DeliveryTag}]");
            };
            //启动消费者 设置手动应答消息
            channel.BasicConsume("NetCoreRabbotMQ1", false, consumer);
            Console.WriteLine("消费者启动成功！");
            Console.ReadKey();
            channel.Close();
            conn.Close();
            #endregion



            #region 使用using
            //using (IConnection conn2 = factory.CreateConnection())
            //{
            //    using (IModel channel2 = conn2.CreateModel())
            //    {
            //        //在MQ上定义一个持久化队列，如果名称相同不会重复创建
            //        channel2.QueueDeclare("NetCoreRabbotMQ", false, false, false, null);
            //        //输入1，那如果接收一个消息，但是没有应答，则客户端不会收到下一个消息
            //        channel2.BasicQos(0, 1, false);
            //        Console.WriteLine("Listening...");

            //        //在队列定义一个消费者
            //        QueueingBasicConsumer consumer2 = new QueueingBasicConsumer(channel2);
            //        //消费队列 并设置为主动应答模式
            //        channel2.BasicConsume("NetCoreRabbotMQ", false, consumer2);


            //        while (true)
            //        {
            //            //阻塞函数 获取队列数据
            //            BasicDeliverEventArgs eventArgs = consumer2.Queue.Dequeue();

            //            byte[] bytes = eventArgs.Body;
            //            string str = Encoding.UTF8.GetString(bytes);

            //            Console.WriteLine("读取队列的消息：" + str);
            //            //回复确认
            //            channel2.BasicAck(eventArgs.DeliveryTag, false);
            //        }
            //    }
            //}
            #endregion
        }
    }
}
