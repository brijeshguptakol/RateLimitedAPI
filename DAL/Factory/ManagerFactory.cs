using RateLimitingAPI.Common.Manager;

namespace DAL.Factory
{
    public class ManagerFactory : IManagerFactory
    {
        #region Private Fields

        private static HotelManager m_HotelManager;

        private static object m_lock = new object();

        private static ManagerFactory m_factory;

        #endregion

        #region Constructor

        private ManagerFactory()
        {
        }

        #endregion

        #region Public Properties

        public static ManagerFactory Instance
        {
            get
            {
                if (m_factory == null)
                {
                    lock (m_lock)
                    {
                        if (m_factory == null)
                        {
                            m_factory = new ManagerFactory();
                        }
                    }

                }

                return m_factory;
            }
        }

        #endregion

        #region IManagerFactory

        public IHotelManager CreateHotelManager()
        {
            return new HotelManager();
        }

        #endregion
    }
}
