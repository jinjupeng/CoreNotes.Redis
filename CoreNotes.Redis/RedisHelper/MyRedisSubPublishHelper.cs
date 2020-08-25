using System;
using System.Threading;
using System.Threading.Tasks;
using StackExchange.Redis;

namespace CoreNotes.Redis.RedisHelper
{
    public class MyRedisSubPublishHelper
    {
        private static readonly string redisConnectionStr = "127.0.0.1:6379,connectTimeout=10000,connectRetry=3,syncTimeout=10000";
        private static readonly ConnectionMultiplexer ConnectionMultiplexer;
        static MyRedisSubPublishHelper()
        {
            ConnectionMultiplexer = ConnectionMultiplexer.Connect(redisConnectionStr);
        }


        #region 发布订阅

        /// <summary>
        /// 订阅消息
        /// </summary>
        /// <param name="topticName"></param>
        /// <param name="handler"></param>
        public void SubScribe(string topticName, Action<RedisChannel, RedisValue> handler = null)
        {
            ISubscriber subscriber = ConnectionMultiplexer.GetSubscriber();
            ChannelMessageQueue channelMessageQueue = subscriber.Subscribe(topticName);
            channelMessageQueue.OnMessage(channelMessage =>
            {
                if (handler != null)
                {
                    string redisChannel = channelMessage.Channel;
                    string msg = channelMessage.Message;
                    handler.Invoke(redisChannel, msg);
                }
                else
                {
                    string msg = channelMessage.Message;
                    Console.WriteLine($"订阅到消息: { msg},Channel={channelMessage.Channel}");
                }
            });
        }

        /// <summary>
        /// 发布消息
        /// </summary>
        /// <param name="topticName"></param>
        /// <param name="message"></param>
        public void PublishMessage(string topticName, string message)
        {
            ISubscriber subscriber = ConnectionMultiplexer.GetSubscriber();
            // 返回订阅数
            long publishLong = subscriber.Publish(topticName, message);
            Console.WriteLine($"发布消息成功：{publishLong}");
        }
        #endregion

        #region 入队出队

        /// <summary>
        /// 入队
        /// </summary>
        /// <param name="queueName"></param>
        /// <param name="redisValue"></param>
        /// <returns></returns>
        public static async Task<long> EnqueueListLeftPushAsync(RedisKey queueName, RedisValue redisValue)
        {
            return await ConnectionMultiplexer.GetDatabase().ListLeftPushAsync(queueName, redisValue);
        }


        /// <summary>
        /// 出队
        /// </summary>
        /// <param name="queueName"></param>
        /// <returns></returns>
        public static async Task<string> DequeueListPopRightAsync(RedisKey queueName)
        {
            IDatabase database = ConnectionMultiplexer.GetDatabase();
            int count = (await database.ListRangeAsync(queueName)).Length;
            if (count <= 0)
            {
                throw new Exception($"队列{queueName}数据为零");
            }
            string redisValue = await database.ListRightPopAsync(queueName);
            if (!string.IsNullOrEmpty(redisValue))
                return redisValue;
            else
                return string.Empty;
        }
        #endregion

        #region 分布式锁

        /// <summary>
        /// 加锁
        /// </summary>
        /// <param name="key"></param>
        /// <param name="expireTimeSeconds"></param>
        public static void LockByRedis(string key, int expireTimeSeconds = 10)
        {
            try
            {
                IDatabase database = ConnectionMultiplexer.GetDatabase();
                while (true)
                {
                    expireTimeSeconds = expireTimeSeconds > 20 ? 10 : expireTimeSeconds;
                    bool lockFlag = database.LockTake(key, Thread.CurrentThread.ManagedThreadId, TimeSpan.FromSeconds(expireTimeSeconds));
                    if (lockFlag)
                    {
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Redis加锁异常:原因{ex.Message}");
            }
        }

        /// <summary>
        /// 解锁
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static bool UnLockByRedis(string key)
        {
            try
            {
                IDatabase database = ConnectionMultiplexer.GetDatabase();
                return database.LockRelease(key, Thread.CurrentThread.ManagedThreadId);
            }
            catch (Exception ex)
            {
                throw new Exception($"Redis解锁异常:原因{ex.Message}");
            }
        }
        #endregion
    }
}