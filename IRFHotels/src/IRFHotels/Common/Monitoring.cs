using IRFHotels.DAL;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IRFHotels.Common
{
    public class Monitoring : IMonitoring
    {
        public static ConcurrentDictionary<int, DateTime> ActiveHotels = new ConcurrentDictionary<int, DateTime>();

        public void ImAlive(int id, int freeRoomCount)
        {
            if (!DataManager.CheckIsKnownHotel(id)) return;
            if (ActiveHotels.ContainsKey(id))
            {
                ActiveHotels[id] = DateTime.Now;
                if (DataManager.GetFreeRoomCount(id) != freeRoomCount)
                {
                    DataManager.SetFreeRoomCount(id, freeRoomCount);
                }
            }
            else
            {
                if (ActiveHotels.TryAdd(id, DateTime.Now))
                {
                    /* Successfully added a new hotel to actives. */
                    DataManager.SetFreeRoomCount(id, freeRoomCount);
                }
                
            }
        }


        public static void CheckAliveHotels()
        {
            var timeToCheck = DateTime.Now;

            var timeout = TimeSpan.FromSeconds(5);

            foreach (var hotel in ActiveHotels)
            {
                if (hotel.Value < (timeToCheck - timeout))
                {
                    DateTime removedElement;

                    if (ActiveHotels.TryRemove(hotel.Key, out removedElement)) { /* Successfully removed due to timeout. */ }
                }
            }
        }

        public static void ListAliveHotels()
        {
            Console.Clear();
            foreach (var h in ActiveHotels)
            {
                var hotel = DataManager.GetHotelById(h.Key);
                Console.WriteLine(hotel.Id + " - " + hotel.Name + " - available rooms: " + hotel.FreeRoomCount);
            }
            Console.Write("Press ENTER to continue...");
            Console.ReadLine();
        }
    }
}
