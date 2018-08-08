using System;
using System.Runtime.Serialization;

namespace WebAPI.RateLimiter
{
    [Serializable]
    [DataContract]
    public class APICallHistory
    {
        #region Constructor

        public APICallHistory()
        {
            LastCallTime = DateTime.Now;
        }

        #endregion


        #region Public Properties

        [DataMember(Name = "LastCallTime", Order = 1)]
        public DateTime LastCallTime { get; set; }


        [DataMember(Name = "CurrentCount", Order = 2)]
        public long CurrentCount { get; set; }

        [DataMember(Name = "AverageTime", Order = 3)]
        public decimal AverageTime { get; set; }

        public void Reset(decimal avegrageTime)
        {
            AverageTime = avegrageTime;
            CurrentCount = 1;
        }

        #endregion

    }


}