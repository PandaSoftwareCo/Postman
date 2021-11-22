using System;
using Newtonsoft.Json;

namespace PostmanApi
{
    public class WeatherForecast
    {
        [JsonProperty(PropertyName = "date")]
        public DateTime Date { get; set; }

        [JsonProperty(PropertyName = "temperatureC")]
        public int TemperatureC { get; set; }

        [JsonProperty(PropertyName = "temperatureF")]
        public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);

        [JsonProperty(PropertyName = "summary")]
        public string Summary { get; set; }
    }
}
