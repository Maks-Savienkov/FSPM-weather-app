﻿using System;

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
            this.City = city;
            this.DateTime = dateTime;
            this.Temp = temp;
            this.Humidity = humidity;
        }
    }
}
