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
        public static void WriteToFile(this List<AverageWeather> aw)
        {
            string text = "Montly report\n";
            var reports = aw;
            text += $"{"Month",-10} {"AverageTemp",-15} {"AverageHumid",-15} {"AverageMoldRisk",-18} {"Position",-15}\n";
            foreach (var report in reports)
            {
                text += $"{report.Month,-10} {report.AverageTemprature.ToString("F2"),-15} {report.AverageHumidity.ToString("F2"),-15} {report.AverageMoldRisk.ToString("F2"),-18} {report.Position,-15}\n";
            }
            text += $"\n Metrological Autumn: {AverageWeather.GetMeteoroLogicalAutumn(aw).Date}";
            text += $"\n Metrological Winter: {AverageWeather.GetMeteoroLogicalWinter(aw).Date}";
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
        public static AverageWeather? GetMeteoroLogicalWinter(List<AverageWeather> weatherData)
        {
            // get höst temp
            var outsideWeatherData = weatherData.Where(d => d.Position == "Ute");


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
                if (item.AverageTemprature <= 0.00)
                {
                    // om ja, vi har streak
                    if (streak == 1) result = item;
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
        public static AverageWeather? GetMeteoroLogicalAutumn(List<AverageWeather> weatherData)
        {
            // get höst temp
            var outsideWeatherData = weatherData.Where(d => d.Position == "Ute");


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
                if (item.AverageTemprature < 10.00)
                {
                    // om ja, vi har streak
                    if (streak == 1) result = item;
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

        public static List<AverageWeather?> GetMonthReport(Dictionary<string, AverageWeather> weatherData)
        {
            var groupedByMonth = weatherData
                .GroupBy(d => d.Value.Month)
                .ToDictionary(g => g.Key, g => g.ToList());



            List<AverageWeather> averageWeatherPerMonth = new();

            double averageTempPerMonthInne = 0;
            double averageTempPerMonthUte = 0;
            double averageHumidityPerMonthInne = 0;
            double averageHumidityPerMonthUte = 0;

            double averageMoldRiskPerMonthInne = 0;
            double averageMoldRiskPerMonthUte = 0;

            // Print grouped data
            foreach (var group in groupedByMonth)
            {
                //  Console.WriteLine($"Date: {group.Key,-20}, Entries: {group.Value.Count,-30}");

                averageTempPerMonthInne = group.Value.Where(d => d.Value.Position == "Inne").Select(e => e.Value.AverageTemprature).DefaultIfEmpty(0).Average();
                averageTempPerMonthUte = group.Value.Where(d => d.Value.Position == "Ute").Select(e => e.Value.AverageTemprature).DefaultIfEmpty(0).Average();
                averageHumidityPerMonthInne = group.Value.Where(d => d.Value.Position == "Inne").Select(e => e.Value.AverageHumidity).DefaultIfEmpty(0).Average();
                averageHumidityPerMonthUte = group.Value.Where(d => d.Value.Position == "Ute").Select(e => e.Value.AverageHumidity).DefaultIfEmpty(0).Average();

                averageMoldRiskPerMonthInne = group.Value.Where(d => d.Value.Position == "Inne").Select(e => e.Value.GetMoldRisk()).DefaultIfEmpty(0).Average();
                averageMoldRiskPerMonthUte = group.Value.Where(d => d.Value.Position == "Ute").Select(e => e.Value.GetMoldRisk()).DefaultIfEmpty(0).Average();


                string monthIndex = "06";
                string month = "";
                foreach (var item in group.Value.AsEnumerable())
                {

                    if (item.Key.EndsWith("Inne"))
                    {
                        var averagePerMonthInne = new AverageWeather
                        {
                            Date = item.Value.Date,
                            Month = group.Key,
                            AverageHumidity = averageHumidityPerMonthInne,
                            AverageTemprature = averageTempPerMonthInne,
                            AverageMoldRisk = averageMoldRiskPerMonthInne,
                            Position = "Inne"
                        };
                        averageWeatherPerMonth.Add(averagePerMonthInne);
                    }
                    else
                    {
                        var averagePerMonthUte = new AverageWeather
                        {
                            Date = item.Value.Date,
                            Month = group.Key,
                            AverageTemprature = averageTempPerMonthUte,
                            AverageHumidity = averageHumidityPerMonthUte,
                            AverageMoldRisk = averageMoldRiskPerMonthInne,
                            Position = "Ute"
                        };

                        
                            averageWeatherPerMonth.Add(averagePerMonthUte);
                    }


                }


            };


            return averageWeatherPerMonth;
        }
    }




}
