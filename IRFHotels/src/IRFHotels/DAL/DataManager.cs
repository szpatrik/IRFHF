using IRFHotels.BOs;
using IRFHotels.Properties;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SQLite;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IRFHotels.DAL
{
    public class DataManager
    {
        private static string DATABASE = ConfigurationManager.AppSettings["DatabaseFile"];
        private static string CONNSTRING = ConfigurationManager.AppSettings["DBConnectionString"];

        public static void CreateDatabaseIfNotExists()
        {
            if (File.Exists(DATABASE)) return;
            SQLiteConnection.CreateFile(DATABASE);

            var conn = new SQLiteConnection(CONNSTRING);
            conn.Open();

            var q = "CREATE TABLE hotels (id int primary key, name varchar(40), freeroomcount int)";
            SQLiteCommand cmd = new SQLiteCommand(q, conn);
            cmd.ExecuteNonQuery();
            Program.Log(Resources.NewDatabase, LogCategories.ProgramInfo, LogEventIds.HotelManagementPrio, LogEventIds.HotelManagement, TraceEventType.Information);
            conn.Close();

        }

        public static void InsertNewHotel(Hotel hotel)
        {
            var conn = new SQLiteConnection(CONNSTRING);
            conn.Open();

            var q = "INSERT INTO hotels (id, name, freeroomcount) VALUES ("+hotel.Id+",\""+hotel.Name+"\","+hotel.FreeRoomCount+")";
            SQLiteCommand cmd = new SQLiteCommand(q, conn);
            cmd.ExecuteNonQuery();
            Program.Log(Resources.HotelInsert, LogCategories.ProgramInfo, LogEventIds.HotelManagementPrio, LogEventIds.HotelManagementPrio, TraceEventType.Information);
            conn.Close();
        }

        public static void SetFreeRoomCount(int hotel, int count)
        {
            var conn = new SQLiteConnection(CONNSTRING);
            conn.Open();

            var q = "UPDATE hotels SET freeroomcount ="+ count+" WHERE id = "+hotel;
            SQLiteCommand cmd = new SQLiteCommand(q, conn);
            cmd.ExecuteNonQuery();

            conn.Close();
        }

        public static bool CheckIsKnownHotel(int hotel)
        {
            var conn = new SQLiteConnection(CONNSTRING);
            conn.Open();

            var q = "SELECT id FROM hotels WHERE id = "+hotel;
            SQLiteCommand cmd = new SQLiteCommand(q, conn);
            var res = cmd.ExecuteScalar();

            conn.Close();

            return res == null ? false : true;
        }

        public static int GetFreeRoomCount(int hotel)
        {
            var conn = new SQLiteConnection(CONNSTRING);
            conn.Open();

            var q = "SELECT freeroomcount FROM hotels WHERE id = " + hotel;
            SQLiteCommand cmd = new SQLiteCommand(q, conn);
            var res = (int)cmd.ExecuteScalar();

            conn.Close();

            return res;
        }

        public static Hotel GetHotelById(int hotel)
        {
            Hotel result = null;

            var conn = new SQLiteConnection(CONNSTRING);
            conn.Open();

            var q = "SELECT * FROM hotels WHERE id = " + hotel;
            SQLiteCommand cmd = new SQLiteCommand(q, conn);
            using(var reader = cmd.ExecuteReader())
            {
                reader.Read();
                var id = reader.GetInt32(0);
                var name = reader.GetString(1);
                var freeRoomCount = reader.GetInt32(2);
                result = new Hotel() { Id = id, Name = name, FreeRoomCount = freeRoomCount };
            }

            conn.Close();

            return result;
        }

        public static void DeleteHotelById(int hotel)
        {
            var conn = new SQLiteConnection(CONNSTRING);
            conn.Open();
            var q = "DELETE FROM hotel WHERE id = " + hotel;
            SQLiteCommand cmd = new SQLiteCommand(q, conn);
            cmd.ExecuteNonQuery();
            Program.Log(Resources.HotelDelete, LogCategories.ProgramInfo, LogEventIds.HotelManagementPrio, LogEventIds.HotelManagementPrio, TraceEventType.Information);
            conn.Close();
        }
    }
}
