using CoreNotes.Redis.RedisHelper;
using System;
using StackExchange.Redis;

namespace CoreNotes.Redis.SubscribeClient
{
    class Program
    {
        static void Main(string[] args)
        {
            /*
             * 注意：
             * 发布订阅模式使用，消费端必须保持在线，你用publish, subscribe假如消费端没在线，
             * 这时候发布了一个消息，过一分钟消费端开启的话，是接收不到这个消息的。
             */
            // 订阅消息
            new MyRedisSubPublishHelper().SubScribe("hello");

            Console.ReadKey();
        }
    }
}
