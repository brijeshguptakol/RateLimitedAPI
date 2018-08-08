using System.Collections.Generic;
using System.Web.Http;
using DAL.Factory;
using RateLimitingAPI.Common;

namespace WebAPI.Controllers
{
    public class RoomController : ApiController
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

            return manager.GetByRoom(id, sortOrder);
        }

        #endregion


    }
}
