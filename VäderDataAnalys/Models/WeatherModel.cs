using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using VäderDataAnalys.Services;

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
        public string Month { get; set; }

    }

    public static class WeatcherExtension
    {
        
        public static void WriteToFile(this Dictionary<string, AverageWeather> aw)
        {
            //aw is without date
            string text = "Montly report\n";
            var reports = aw;
            text += $"{"Month",-10} {"AverageTemp",-15} {"AverageHumid",-15} {"AverageMoldRisk",-18} {"Position",-15}\n";
            foreach (var report in reports)
            {
                text += $"{report.Value.Month,-10} {report.Value.AverageTemprature.ToString("F2"),-15} {report.Value.AverageHumidity.ToString("F2"),-15} {report.Value.AverageMoldRisk.ToString("F2"),-18} {report.Value.Position,-15}\n";
            }

            // use global data from service
            var fallDay = AverageWeather.GetMeteoroLogicalAutumn(DataExtractService.AverageWeatherData);
            var winterDay = AverageWeather.GetMeteoroLogicalWinter(DataExtractService.AverageWeatherData);

            text += $"\n Metrological Autumn: {fallDay.Date}";
            text += $"\n Metrological Winter: {winterDay.Date}";
            text += $"\n ((humidity - 78) * (Temp/15))/0,22    -     % risk mögel";
            File.WriteAllText(Path.Combine(@"..\..\..\", "Montly_Report.txt"), text);
        }
        
    }
    public class AverageWeather
    {
        public string Date { get; set; }
        public string Position { get; set; }
        public double AverageHumidity { get; set; }
        public double AverageTemprature { get; set; }
        public double AverageMoldRisk { get; set; }
        public string Month { get; set; }


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


        public static AverageWeather? GetMeteoroLogicalWinter(Dictionary<string, AverageWeather> weatherData)
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


        public static Dictionary<string, AverageWeather> GetMonthReport(Dictionary<string, AverageWeather> weatherData)
        {
            // Gruppindelning baserad på både månad och position
            var groupedByMonthAndPosition = weatherData.Values
                .GroupBy(w => new { w.Month, w.Position });

            var monthReport = new Dictionary<string, AverageWeather>();

            foreach (var group in groupedByMonthAndPosition)
            {
                // Beräkna genomsnitt för varje grupp
                double avgTemp = group.Select(w => w.AverageTemprature).DefaultIfEmpty(0).Average();
                double avgHumidity = group.Select(w => w.AverageHumidity).DefaultIfEmpty(0).Average();
                double avgMoldRisk = group.Select(w => w.GetMoldRisk()).DefaultIfEmpty(0).Average();

                // Skapa en ny nyckel t.ex. "2024-02-Inne" eller "2024-02-Ute"
                string key = $"{group.Key.Month}-{group.Key.Position}";

                // Skapa ett nytt AverageWeather-objekt med de aggregerade värdena
                monthReport[key] = new AverageWeather
                {
                    Month = group.Key.Month,
                    Position = group.Key.Position,
                    AverageTemprature = avgTemp,
                    AverageHumidity = avgHumidity,
                    // Om din klass har en property för risk, annars lägg in på ett sätt som passar
                    AverageMoldRisk = avgMoldRisk
                };
            }

            // without date
            return monthReport;
        }

    }




}
