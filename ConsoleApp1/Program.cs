using System;
using System.Data.SQLite;
using System.Net.Http;
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
            SQLiteConnection sqlite_conn = CreateConnection();
            CreateTables(sqlite_conn);
            await PresentMenuAsync(sqlite_conn);
            
        }

        static async Task PresentMenuAsync(SQLiteConnection sqlite_conn) 
        {
            Console.WriteLine("MENU:");
            Console.WriteLine("1. Show and save todays forecast");
            Console.WriteLine("2. Show all saved forecast");
            switch (Console.ReadLine())
            {
                case "1":
                    await ShowAndSaveTodaysForeacast(sqlite_conn);
                    break; 
                case "2":
                    ShowAllForecasts(sqlite_conn);
                    break;
                default:
                    break;
            }
        }

        private static void ShowAllForecasts(SQLiteConnection sqlite_conn)
        {
            WeatherRepository weatherRepository = new(sqlite_conn);
            foreach (var weather in weatherRepository.GetAll())
            {
                Console.WriteLine(
                    "=================================\n"
                    + $"city: {weather.City},\n"
                    + $"date: {weather.DateTime},\n"
                    + $"temp: {weather.Temp},\n"
                    + $"humidity: {weather.Humidity}\n"
                    + "================================="
                );
            }
        }

        static async Task ShowAndSaveTodaysForeacast(SQLiteConnection sqlite_conn) 
        {
            await getWeather(sqlite_conn);

            async Task getWeather(SQLiteConnection sqlite_conn)
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

                WeatherRepository weatherRepository = new(sqlite_conn);
                weatherRepository.Save(weatherForecast);
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
    }
}
