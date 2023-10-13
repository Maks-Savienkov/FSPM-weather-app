using System;
using System.Data.SQLite;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Security.Policy;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace ConsoleApp1
{
    internal class Program
    {
        private const string CherkasyURL = "https://api.openweathermap.org/data/2.5/forecast?appid=YOUR_OPEN_WEATHER_TOKEN&q=Cherkasy&cnt=5&units=metric";
        private const string KyivURL = "https://api.openweathermap.org/data/2.5/forecast?appid=YOUR_OPEN_WEATHER_TOKEN&q=Kyiv&cnt=5&units=metric";
        private static readonly HttpClient client = new();

        async static Task Main(string[] args)
        {
            await getWeather();
            SQLiteConnection sqlite_conn;

            async Task getWeather()
            {
                Console.WriteLine("Getting JSON...");
                var responseString = await client.GetStringAsync(CherkasyURL);
                Console.WriteLine("Parsing JSON...");
                WeatherForecast weatherForecast = JsonSerializer.Deserialize<WeatherForecast>(responseString);
                Console.WriteLine($"cod: {weatherForecast?.cod}");
                Console.WriteLine($"City: {weatherForecast?.city?.name}");
                Console.WriteLine($"list count: {weatherForecast?.list?.Count}");
                foreach (var weather in weatherForecast?.list)
                {
                    Console.WriteLine($"weather temp: {weather?.main?.temp}, date: {weather.dt_txt}");
                }

                sqlite_conn = CreateConnection();
                CreateTables(sqlite_conn);
                SQLiteCommand sqlite_cmd = sqlite_conn.CreateCommand();
                foreach (var weather in weatherForecast?.list)
                {
                    sqlite_cmd.CommandText = $"INSERT OR IGNORE INTO WeatherForecasts (town, date_time, temperature, humidity) "
                        + $"VALUES("
                        + $"'{weatherForecast.city.name}',"
                        + $"'{weather.dt}',"
                        + $"{weather.main.temp.Value.ToString(CultureInfo.InvariantCulture)},"
                        + $"{weather.main.humidity}"
                        + $");";
                    sqlite_cmd.ExecuteNonQuery();
                }
            }
        }

        static SQLiteConnection CreateConnection()
        {

            SQLiteConnection sqlite_conn;
            // Create a new database connection:
            sqlite_conn = new SQLiteConnection("Data Source=database.db; Version = 3; New = True; Compress = True; ");
            // Open the connection:
            try
            {
                sqlite_conn.Open();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            return sqlite_conn;
        }

        static void CreateTables(SQLiteConnection conn)
        {
            SQLiteCommand sqlite_cmd = conn.CreateCommand();

            sqlite_cmd.CommandText = "CREATE TABLE IF NOT EXISTS Genders (text TEXT);";
            sqlite_cmd.ExecuteNonQuery();

            sqlite_cmd.CommandText = "CREATE TABLE IF NOT EXISTS WeatherForecasts ("
                + "town        VARCHAR(50) NOT NULL,"
                + "date_time   INTEGER     NOT NULL,"
                + "temperature REAL        NOT NULL,"
                + "humidity    INTEGER     NOT NULL,"
                + "PRIMARY KEY (town, date_time)"
                + ");";
            sqlite_cmd.ExecuteNonQuery();
        }

        static void ReadData(SQLiteConnection conn)
        {
            SQLiteDataReader sqlite_datareader;
            SQLiteCommand sqlite_cmd;
            sqlite_cmd = conn.CreateCommand();
            sqlite_cmd.CommandText = "SELECT * FROM SampleTable";
            sqlite_datareader = sqlite_cmd.ExecuteReader();
            while (sqlite_datareader.Read())
            {
                string myreader = sqlite_datareader.GetString(0);
                Console.WriteLine(myreader);
            }
            conn.Close();
        }
    }
}
