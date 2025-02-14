using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;

namespace VäderDataAnalys.Models
{
    /// <summary>
    /// Per row in the file
    /// </summary>
    public class WeatherModel
    {
        public string Date { get; set; }
        public string? Time { get; set; }
        public double Temperature { get; set; }
        public int Humidity { get; set; }
        public string Position { get; set; }
        public double AverageTemprature { get; set; }
        public double AverageHumidity { get; set; }

    }
    public class AverageWeather
    {
        public string Date { get; set; }
        public string Position { get; set; }
        public double AverageHumidity { get; set; }
        public double AverageTemprature { get; set; }


        public double GetMoldRisk()
        {
            // ((humidity - 78) * (Temp/15))/0,22    -     %risk mögel

            const double HumidityThreshold = 78.0;   // Ingen mögelrisk under denna fuktighet
            const double TemperatureScale = 15.0;    // Temperatur normaliseringsfaktor
            const double RiskDivisor = 0.22;         // Formelns skalningsfaktor
            const double MaxRawRisk = 200.0;         // Maxvärde för rå mögelrisk

            // Beräkna den råa mögelrisken enligt given formel
            double rawRisk = ((AverageHumidity - HumidityThreshold) * (AverageTemprature / TemperatureScale)) / RiskDivisor;

            // Normalisera till en skala mellan 0 och 100
            double scaledRisk = (rawRisk / MaxRawRisk) * 100;

            // Begränsa resultatet mellan 0 och 100
            return Math.Clamp(scaledRisk, 0, 100);
        }

        
        public static AverageWeather? GetMeteoroLogicalWinter(Dictionary<string,AverageWeather> weatherData)
        {
            // get höst temp
            var outsideWeatherData = weatherData.Where(d => d.Value.Position == "Ute");


            // kolla om det håller i fem dygn
            int streak = 1;
            AverageWeather? result = null;
            foreach (var item in outsideWeatherData)
            {
                if (streak >= 5)
                {
                    // höst är här
                    return result;
                }

                // kollar efter dagar under 10-grader
                if (item.Value.AverageTemprature <= 0.00)
                {
                    // om ja, vi har streak
                    if (streak == 1) result = item.Value;
                    streak++;
                }
                else
                {
                    // reset streak
                    streak = 1;

                }
            }
            return result;

            // är det innan eller efter 1 aug?

        }

        public static AverageWeather? GetMeteoroLogicalAutumn(Dictionary<string, AverageWeather> weatherData)
        {
            // get höst temp
            var outsideWeatherData = weatherData.Where(d => d.Value.Position == "Ute");


            // kolla om det håller i fem dygn
            int streak = 1;
            AverageWeather? result = null;
            foreach (var item in outsideWeatherData)
            {
                if (streak >= 5)
                {
                    // höst är här
                    return result;
                }

                // kollar efter dagar under 10-grader
                if (item.Value.AverageTemprature < 10.00)
                {
                    // om ja, vi har streak
                    if (streak == 1) result = item.Value;
                    streak++;
                }
                else
                {
                    // reset streak
                    streak = 1;

                }
            }
            return result;

            // är det innan eller efter 1 aug?

        }
    }




}
