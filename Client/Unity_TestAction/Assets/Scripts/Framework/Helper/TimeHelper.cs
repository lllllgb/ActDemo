using System;

namespace AosBaseFramework
{
	public static class TimeHelper
	{
		private static readonly long epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).Ticks;
		/// <summary>
		/// 客户端时间
		/// </summary>
		/// <returns></returns>
		public static long ClientNow()
		{
			return (DateTime.UtcNow.Ticks - epoch) / 10000;          
        }
        

        public static long ClientNowSeconds()
		{
			return (DateTime.UtcNow.Ticks - epoch) / 10000000;
		}

		/// <summary>
		/// 登陆前是客户端时间,登陆后是同步过的服务器时间
		/// </summary>
		/// <returns></returns>
		public static long Now()
		{
			return ClientNow();
		}
        /// <summary>
        /// 时间戳转格林威治时间 13位
        /// </summary>
        /// <param name="dateTime"></param>
        /// <param name="isTime"></param>
        /// <returns></returns>
        public static DateTime GreenwishTime(long dateTime,bool isTime)
        {
            DateTime epoch;
            if (isTime) { epoch = new DateTime(1970, 1, 1, 0, 0, 0); }
            else { epoch = new DateTime(1970, 1, 1, 8, 0, 0); }          

            return new DateTime(new DateTime(dateTime * 10000).Ticks + epoch.Ticks);
        }
    }

}