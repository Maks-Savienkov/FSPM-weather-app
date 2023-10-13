using System.Collections.Generic;

namespace ConsoleApp1
{
    public class WeatherForecast
    {
        public string cod { get; set; }
        public int? message { get; set; }
        public int? cnt { get; set; }
        public City city { get; set; }
        public IList<WeatherInfo> list { get; set; }
    }
}
