using System;
using System.Collections.Concurrent;

namespace WebAPI.RateLimiter
{
    public class RateLimiter
    {

        private static object m_lock = new object();

        private static ConcurrentDictionary<string, APICallHistory> m_apiCallHistories;

        /// <summary>
        /// Minimum Average Threshold.
        /// </summary>
        private decimal m_MinimumAvgThreshold;


        #region Constructor

        static RateLimiter()
        {
            m_apiCallHistories = new ConcurrentDictionary<string, APICallHistory>();
        }

        /// <summary>
        /// Creates an instance of RateLimiter
        /// </summary>
        /// <param name="timeInterval">time interval in millisecond. -1 signifies no restriction and 0 signifies no access</param>
        /// <param name="maxRequestAllowed">max number of request allowed per time interval. -1 signifies no restriction and 0 signifies no access</param>
        public RateLimiter(int timeInterval, int maxRequestAllowed)
        {
            //Lets consider 0 to be blocked and -1 to be no restriction. So, both timeInterval and maxRequestAllowed should be either equal to -1 , 0 or both are greater then 0. This is corner case scenario.
            if (timeInterval == 0)
            {
                if (maxRequestAllowed != 0)
                {
                    throw new ArgumentOutOfRangeException("Both time interval and max request allowed should be 0");
                }
            }
            else if (maxRequestAllowed == 0)
            {
                throw new ArgumentOutOfRangeException("Both time interval and max request allowed should be 0");
            }

            if (timeInterval == -1)
            {
                if (maxRequestAllowed != -1)
                {
                    throw new ArgumentOutOfRangeException("Both time interval and max request allowed should be -1");
                }
            }
            else if (maxRequestAllowed == -1)
            {
                throw new ArgumentOutOfRangeException("Both time interval and max request allowed should be -1");
            }

            // if the above scenarios are fine, than assign the values to instance variables
            TimeInterval = timeInterval;
            MaxRequestAllowed = maxRequestAllowed;
            m_MinimumAvgThreshold = timeInterval / (decimal)maxRequestAllowed;
        }

        #endregion

        /// <summary>
        /// Gets the time interval in millisecond
        /// </summary>
        public long TimeInterval { get; }

        /// <summary>
        /// Gets the max number of request allowed per time interval
        /// </summary>
        public long MaxRequestAllowed { get; }

        #region Public methods

        public bool IsCallAllowed(string requestUrl, DateTime requestTime)
        {
            lock (m_lock)
            {
                bool isAllowed = false;
                APICallHistory apiCallHistory = null;
                var newApiCallHistory = new APICallHistory();
                var historyExists = m_apiCallHistories.TryGetValue(requestUrl, out apiCallHistory);

                if (historyExists)
                {
                    var timeElapsedSinceLastCall = requestTime.Subtract(apiCallHistory.LastCallTime).TotalMilliseconds;

                    newApiCallHistory.LastCallTime = requestTime;
                    newApiCallHistory.CurrentCount = apiCallHistory.CurrentCount + 1;

                    newApiCallHistory.AverageTime =
                        ((apiCallHistory.CurrentCount * apiCallHistory.AverageTime) + (decimal)timeElapsedSinceLastCall) /
                        newApiCallHistory.CurrentCount;

                    isAllowed = newApiCallHistory.AverageTime >= m_MinimumAvgThreshold;

                    //Reset history if time elapsed since last call is greater than allowed time interval
                    if (timeElapsedSinceLastCall > TimeInterval)
                    {
                        newApiCallHistory.Reset(TimeInterval);
                        isAllowed = true;
                    }
                }
                else
                {
                    newApiCallHistory = new APICallHistory
                    {
                        LastCallTime = requestTime,
                        CurrentCount = 1,
                        AverageTime = TimeInterval
                    };

                    isAllowed = true;
                }

                //If call is allowed, than update the history
                if (isAllowed)
                {
                    m_apiCallHistories[requestUrl] = newApiCallHistory;
                }

                return isAllowed;
            }
        }

        #endregion
    }
}