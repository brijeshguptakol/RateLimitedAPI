using RateLimitingAPI.Common.Manager;

namespace DAL.Factory
{
    public interface IManagerFactory
    {
        /// <summary>
        /// Creates a Hotel Manager
        /// </summary>
        /// <returns></returns>
        IHotelManager CreateHotelManager();
    }
}
