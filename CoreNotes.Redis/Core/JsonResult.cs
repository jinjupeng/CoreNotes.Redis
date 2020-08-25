namespace CoreNotes.Redis.Core
{
    public class ApiResultObject
    {
        public int Code { get; set; }
        public string Data { get; set; }
        public string Msg { get; set; }
    }

    public class ResultCode
    {
        /// <summary>
        /// 成功
        /// </summary>
        public const int Success = 200000;
        /// <summary>
        /// 内部未知错误
        /// </summary>
        public const int ServerError = 500000;
        /// <summary>
        /// 未登录
        /// </summary>
        public const int NotLogin = 500001;

        /// <summary>
        /// 无权限
        /// </summary>
        public const int Forbidden = 403000;
        /// <summary>
        /// 未找到
        /// </summary>
        public const int NotFund = 404000;
    }
}