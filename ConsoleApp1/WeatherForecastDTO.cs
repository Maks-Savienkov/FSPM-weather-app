using System;

namespace ConsoleApp1
{
    internal class WeatherForecastDTO
    {
        public string City { get; set; }
        public DateTime DateTime { get; set; }
        public double Temp { get; set; }
        public double Humidity { get; set; }

        public WeatherForecastDTO(string city, DateTime dateTime, double temp, double humidity)
        {
            City = city;
            DateTime = dateTime;
            Temp = temp;
            Humidity = humidity;
        }
    }
}
