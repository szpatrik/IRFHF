using IRFHotels.HotelClient.IRFHotelsMonitoringReference;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace IRFHotels.HotelClient
{
    class Program
    {
        private static int hotel;
        private static int freeRoomCount;

        static void Main(string[] args)
        {
            try
            {
                hotel = Convert.ToInt32(args[0]);
                freeRoomCount = Convert.ToInt32(args[1]);
            }
            catch (Exception)
            {
                Console.WriteLine("Invalid parameters: please supply hotel ID and number of free rooms (only numbers are accepted.)");
                Console.WriteLine("Example usage: \"IRFHotels.HotelClient.exe 0 1\"");
                Console.WriteLine("Application is now going to quit.");
                Console.ReadLine();
                Environment.Exit(-1);
            }

            var imAliveThread = new Thread(() =>
            {
                while (true)
                {
                    using (MonitoringClient client = new MonitoringClient())
                    {
                        client.ImAlive(hotel, freeRoomCount);
                    }
                    Thread.Sleep(3000);
                }
            });

            imAliveThread.Start();

            ReadUserInput();
        }

        static private void Menu()
        {
            Console.WriteLine("===== HOTEL CLIENT ("+hotel+" [available rooms: "+freeRoomCount+"]) ======");
            Console.WriteLine("1) Reserve a room");
            Console.WriteLine("2) Free up a room");
            Console.WriteLine("3) Quit");
            Console.Write("Choose a menu: ");
        }

        static private void ReadUserInput()
        {
            int result = -1;
            do
            {
                Console.Clear();
                Menu();
                try
                {
                    result = Convert.ToInt32(Console.ReadLine());

                    switch (result)
                    {
                        case 1:
                            if (freeRoomCount > 0) freeRoomCount--;
                            else
                            {
                                Console.WriteLine("No more free room.");
                                Console.ReadLine();
                            }
                            break;
                        case 2:
                            freeRoomCount++;
                            break;
                        case 3:
                            Environment.Exit(0);
                            break;
                        default:
                            break;
                    }
                }
                catch (Exception)
                {
                    Console.WriteLine("Invalid user input. Press ENTER to continue.");
                    Console.ReadLine();
                }
            } while (result != 3);
        }
    }
}
