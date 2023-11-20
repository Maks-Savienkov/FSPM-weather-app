using System.Collections.Generic;
namespace ConsoleApp1
{
    public class WeatherForecast
    {
        public string Cod { get; set; }
        public int? Message { get; set; }
        public int? Cnt { get; set; }
        public City City { get; set; }
        public IList<WeatherInfo> List { get; set; }
    }
}
