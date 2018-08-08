using System.Collections.Generic;

namespace RateLimitingAPI.Common.Manager
{
    public interface IHotelManager
    {
        IList<Hotel> Get();

        IList<Hotel> GetByCity(string city, string sortOrder);

        IList<Hotel> GetByRoom(string room, string sortOrder);
    }
}
