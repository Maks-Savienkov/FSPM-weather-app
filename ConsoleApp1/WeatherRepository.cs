using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data.SQLite;
using System.Globalization;

namespace ConsoleApp1
{
    internal class WeatherRepository
    {
        private readonly SQLiteConnection sqlite_conn;

        public WeatherRepository(SQLiteConnection sqlite_conn) 
        {
            this.sqlite_conn = sqlite_conn;
        }

        public void Save(WeatherForecast weatherForecast) {
            SQLiteCommand sqlite_cmd = sqlite_conn.CreateCommand();
            foreach (var weather in weatherForecast?.List)
            {
                sqlite_cmd.CommandText = $"INSERT OR IGNORE INTO WeatherForecasts (town, date_time, temperature, humidity) "
                    + $"VALUES("
                    + $"'{weatherForecast.City.Name}',"
                    + $"'{weather.Dt}',"
                    + $"{weather.Main.Temp.Value.ToString(CultureInfo.InvariantCulture)},"
                    + $"{weather.Main.Humidity}"
                    + $");";
                sqlite_cmd.ExecuteNonQuery();
            }
        }

        public List<WeatherForecastDTO> GetAll()
        {
            SQLiteCommand sqlite_cmd = sqlite_conn.CreateCommand();
            sqlite_cmd.CommandText = $"SELECT * FROM WeatherForecasts;";
            SQLiteDataReader rdr = sqlite_cmd.ExecuteReader();

            var list = new List<WeatherForecastDTO>();

            while (rdr.Read())
            {
                NameValueCollection values = rdr.GetValues();

                list.Add(
                    new WeatherForecastDTO(
                        city: values.Get("town"),
                        dateTime: DateTimeOffset.FromUnixTimeSeconds(
                                long.Parse(values.Get("date_time"))
                            ).ToLocalTime().DateTime,
                        temp: double.Parse(values.Get("temperature"), CultureInfo.InvariantCulture),
                        humidity: double.Parse(values.Get("humidity"), CultureInfo.InvariantCulture))
                );
            }

            return list;
        }
    }
}
