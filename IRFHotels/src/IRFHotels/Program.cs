using IRFHotels.BOs;
using IRFHotels.Common;
using IRFHotels.DAL;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SQLite;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Description;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using IRFHotels.Properties;



using Microsoft.Practices.EnterpriseLibrary.Common.Configuration;
using Microsoft.Practices.EnterpriseLibrary.Logging;
using System.Diagnostics;


namespace IRFHotels
{
    class Program
    {
        internal static LogWriter Logger { get; private set; }
        static void Main(string[] args)
        {
            try
            {
                Program.Logger = (new LogWriterFactory()).Create();
            }
            catch (Exception ex)
            {
                Console.WriteLine(Resources.LoggingInitFailedUIMessage, ex.Message);
                return;
            }

            DataManager.CreateDatabaseIfNotExists();

            var aliveCheckingThread = new Thread(() =>
            {
                while (true)
                {
                    Monitoring.CheckAliveHotels();
                    Thread.Sleep(1000);
                }
            });

            aliveCheckingThread.Start();

            var host = new ServiceHost(typeof(Monitoring), new Uri(ConfigurationManager.AppSettings["MonitoringHostUri"]));
            ServiceMetadataBehavior smb = new ServiceMetadataBehavior();
            smb.HttpGetEnabled = true;
            smb.MetadataExporter.PolicyVersion = PolicyVersion.Policy15;
            host.Description.Behaviors.Add(smb);

            try
            {
                host.Open();
                if (host == null) { Program.Log(Resources.HostOpenFailedMessage, LogCategories.ProgramFail, LogEventIds.HostOpenFailedPrio, LogEventIds.HostOpenFailed, TraceEventType.Critical); }
                else
                {
                    Program.Log(Resources.HostOpenSuccesfulMessage, LogCategories.ProgramInfo, LogEventIds.HostOpenSuccesfulPrio, LogEventIds.HostOpenSuccesful, TraceEventType.Information);
                    ReadUserInput();
                    host.Close();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                host.Abort();
            }

        }

        static private void Menu()
        {
            Console.WriteLine("===== SERVER ======");
            Console.WriteLine("1) List connected hotels");
            Console.WriteLine("2) Add new hotel");
            Console.WriteLine("3) Delete a hotel");
            Console.WriteLine("4) Quit");
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
                            Monitoring.ListAliveHotels();
                            break;
                        case 2:
                            Console.Clear();
                            Console.WriteLine("Add new hotel.");
                            Console.Write("Provide the id: ");
                            var id = Convert.ToInt32(Console.ReadLine());
                            Console.Write("Provide the name: ");
                            var name = Console.ReadLine();
                            DataManager.InsertNewHotel(new Hotel() { FreeRoomCount = 0, Name = name, Id = id });
                            Console.WriteLine("Hotel successfully added. Press ENTER to continue.");
                            Console.ReadLine();
                            break;
                        case 3:
                            Console.Clear();
                            Console.WriteLine("Delete a hotel.");
                            Console.WriteLine("Provide the id: ");
                            var idToDelete = Convert.ToInt32(Console.ReadLine());
                            DataManager.DeleteHotelById(idToDelete);
                            Console.WriteLine("Hotel successfully deleted. Press ENTER to continue.");
                            Console.ReadLine();
                            break;
                        case 4:
                            Environment.Exit(0);
                            break;
                        default:
                            break;
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine("Invalid user input. Press ENTER to continue." + e.Message);
                    Console.ReadLine();
                }
            } while (result != 4);
        }
        internal static void Log(string message, string category, int priority, int eventId, TraceEventType severity)
        {
            if(Program.Logger.IsLoggingEnabled())
            {
                Program.Logger.Write(message, category, priority, eventId, severity);
            }
           
        }

    }
}
