using System;
using TimeZoneConverter;

namespace system
{
    public static class DateTimeOffsetExtensions
    {
        /// <summary>
        /// Converts a <see cref="DateTimeOffset"/> from the server to a user's local time with 
        /// his specific timezone.
        /// </summary>
        /// <param name="serverTime"></param>
        /// <param name="timeZoneId">The timezone to convert server time to.</param>
        /// <remarks>
        /// Posts are saved Utc time on server and are converted to <paramref name="timeZoneId"/>
        /// before displayed on the client.
        /// </remarks>
        public static DateTimeOffset ToLocalTime(this DateTimeOffset serverTime, string timeZoneId)
        {
            var userTimeZone = TZConvert.GetTimeZoneInfo(timeZoneId);
            return TimeZoneInfo.ConvertTime(serverTime, userTimeZone);
        }
    }
}
