using System;
using System.Threading.Tasks;
using CoreNotes.Redis.Core;
using CoreNotes.Redis.RedisHelper;
using Microsoft.AspNetCore.Mvc;

namespace CoreNotes.Redis.Controllers
{
    [Route("api/[Controller]")]
    [ApiController]
    public class RedisTestController
    {
        [HttpGet("EnqueueMsg")]
        public async Task<ApiResultObject> EnqueueMsgAsync(string redisKey, string redisValue)
        {
            ApiResultObject obj = new ApiResultObject();
            try
            {
                long enqueueLong = default;
                for (int i = 0; i < 1000; i++)
                {
                    enqueueLong = await MyRedisSubPublishHelper.EnqueueListLeftPushAsync(redisKey, redisValue + i);
                }
                obj.Code = ResultCode.Success;
                obj.Data = "入队的数据长度：" + enqueueLong;
                obj.Msg = "入队成功！";
            }
            catch (Exception ex)
            {

                obj.Msg = $"入队异常，原因：{ex.Message}";
            }
            return obj;
        }
        [HttpGet("DequeueMsg")]
        public async Task<ApiResultObject> DequeueMsgAsync(string redisKey)
        {
            ApiResultObject obj = new ApiResultObject();
            try
            {
                string dequeueMsg = await MyRedisSubPublishHelper.DequeueListPopRightAsync(redisKey);
                obj.Code = ResultCode.Success;
                obj.Data = $"出队的数据是：{dequeueMsg}";
                obj.Msg = "入队成功！";
            }
            catch (Exception ex)
            {
                obj.Msg = $"入队异常，原因：{ex.Message}";
            }
            return obj;
        }
    }
}