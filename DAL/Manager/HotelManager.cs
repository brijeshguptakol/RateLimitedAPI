using System;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using RateLimitingAPI.Common;
using RateLimitingAPI.Common.Manager;

namespace DAL
{
    public class HotelManager : IHotelManager
    {
        private const  string m_HotelDbPath = "DAL.hoteldb.csv";

        private static IList<Hotel> m_AllHotels;

        #region Constructors

        static HotelManager()
        {
            m_AllHotels = GetInternal();
        }

        #endregion

        #region Public Methods

        public IList<Hotel> Get()
        {

            return m_AllHotels;
        }

        public IList<Hotel> GetByCity(string city, string sortOrder)
        {

            var hotelsFilteredByCity = m_AllHotels.Where(x => x.City.Equals(city, StringComparison.InvariantCultureIgnoreCase));
            var sortedHotels = ApplySorting(hotelsFilteredByCity, sortOrder);
            return sortedHotels;
        }

        public IList<Hotel> GetByRoom(string room, string sortOrder)
        {
            var hotelsFilteredByRoom = m_AllHotels.Where(x => x.Room.Equals(room, StringComparison.InvariantCultureIgnoreCase));
            var sortedHotels = ApplySorting(hotelsFilteredByRoom, sortOrder);
            return sortedHotels;
        }

        #endregion

        #region Private Methods

        private IList<Hotel> ApplySorting(IEnumerable<Hotel> hotels, string sortOrder)
        {
            if (!string.IsNullOrEmpty(sortOrder))
            {
                if (sortOrder.Equals("asc", StringComparison.InvariantCultureIgnoreCase))
                {
                    return hotels.OrderBy(x => x.Price).ToList();
                }

                if (sortOrder.Equals("desc", StringComparison.InvariantCultureIgnoreCase))
                {
                    return hotels.OrderByDescending(x => x.Price).ToList();
                }
            }

            return hotels.ToList();
        }

        private static IList<Hotel> GetInternal()
        {

            return GetHotelListFromAssembly();
        }

        /// <summary>
        /// Gets the Hotels list from assembly
        /// </summary>
        private static IList<Hotel> GetHotelListFromAssembly()
        {
            var assembly = Assembly.GetExecutingAssembly();

            var hotels = new List<Hotel>();

            using (var stream = assembly.GetManifestResourceStream(m_HotelDbPath))
            {
                if (stream != null)
                {
                    using (var reader = new StreamReader(stream))
                    {
                        //First Line contains the column labels
                        var metaData = reader.ReadLine();

                        //Read all the columns for respective column values.
                        //column 1 - CITY
                        //column 2 - HOTELID
                        //column 3 - ROOM
                        //column 4 - PRICE

                        while (!reader.EndOfStream)
                        {
                            var line = reader.ReadLine();
                            var values = line.Split(',');

                            hotels.Add(new Hotel
                            {
                                City = values[0],
                                HotelId = Int32.Parse(values[1]),
                                Room = values[2],
                                Price = Int32.Parse(values[3])
                            });

                        }
                    }


                }
                else
                {
                    throw new ArgumentNullException("hotel db.");
                }
            }

            return hotels;
        }

        #endregion
    }
}
