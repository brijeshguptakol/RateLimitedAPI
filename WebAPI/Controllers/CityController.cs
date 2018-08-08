using DAL.Factory;
using System.Collections.Generic;
using System.Web.Http;
using RateLimitingAPI.Common;

namespace WebAPI.Controllers
{
    public class CityController : ApiController
    {
        [HttpGet]
        public IList<Hotel> Get(string id)
        {
            return GetInternal(id);
        }

        [HttpGet]
        public IList<Hotel> Get(string id, string sortOrder)
        {
            return GetInternal(id, sortOrder);
        }

        #region Private Methods

        private IList<Hotel> GetInternal(string id, string sortOrder = null)
        {
            var manager = ManagerFactory.Instance.CreateHotelManager();

            return manager.GetByCity(id, sortOrder);
        }

        #endregion
    }
}
