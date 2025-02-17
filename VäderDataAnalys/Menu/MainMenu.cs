using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VäderDataAnalys.Models;

namespace VäderDataAnalys.Menu
{
    public  static class MainMenu
    {
        // private readonly Dictionary<string, AverageWeather> _weatherData;

            
        public static async Task Print(Dictionary<string, AverageWeather> weatherData)
        {


            while (true)
            {
                Console.WriteLine("Weahther data app");

                Console.WriteLine("[A]ll | [S]earch | sort by [H]umidity | sort by [M]old risk | [F] Meteorological autumn | [R]eport");
                var k = Console.ReadKey(true);
                switch (k.Key)
                {
                    // Medeltemperatur och luftfuktighet per dag, för valt datum (sökmöjlighet med validaering
                    case ConsoleKey.A:
                        Console.Clear();
                        ListAllToMenu(SortByTemprature(weatherData));
                        break;
                    case ConsoleKey.H:
                        Console.Clear();
                        Console.WriteLine("Sort By humidity");
                        ListAllToMenu(SortByHumidity(weatherData));
                        break;
                    case ConsoleKey.M:
                        Console.Clear();
                        Console.WriteLine("Sort By mold risk");
                        ListAllToMenu(SortByMoldRisk(weatherData));
                        break;
                    case ConsoleKey.S:
                        Console.Clear();
                        Search(weatherData);
                        break;
                    case ConsoleKey.F:
                        Console.Clear();
                        Console.WriteLine("Meteorological autumn date");
                        var fallDay = AverageWeather.GetMeteoroLogicalAutumn(weatherData);
                        Console.WriteLine($"{fallDay.Date,-15} {"Average temprature: " + fallDay.AverageTemprature.ToString("F2"),-15} | {"Average humidity: " + fallDay.AverageHumidity.ToString("F2"),-15}");
                        break;
                    case ConsoleKey.W:
                        Console.Clear();
                        Console.WriteLine("Meteorological autumn date");
                        var winterDay = AverageWeather.GetMeteoroLogicalWinter(weatherData);
                        Console.WriteLine($"{winterDay.Date,-15} {"Average temprature: " + winterDay.AverageTemprature.ToString("F2"),-15} | {"Average humidity: " + winterDay.AverageHumidity.ToString("F2"),-15}");
                        break;
                    case ConsoleKey.R:
                        Console.Clear();
                        Console.WriteLine("Montly report");
                        var reports = AverageWeather.GetMonthReport(weatherData);
                        Console.WriteLine($"{"Month",-10} {"AverageTemp",-15} {"AverageHumid",-15} {"Position",-15}");
                        foreach (var report in reports)
                        {
                            Console.WriteLine($"{report.Value.Month,-10} {report.Value.AverageTemprature.ToString("F2"),-15} {report.Value.AverageHumidity.ToString("F2"),-15} {report.Value.Position,-15}");
                        }
                        Console.WriteLine("Report saved!");
                        reports.WriteToFile();
                        break;
                    default:
                        Thread.Sleep(50);
                        Console.Clear();
                        break;
                }

            }
        }

        private static void Search(Dictionary<string, AverageWeather> weatherData)
        {
            Console.Write("Search: ");
            var searchString = Console.ReadLine();
            var filterdWeather = weatherData.Where(d => d.Value.Date.Contains(searchString));
            ListAllToMenu(filterdWeather.ToDictionary());
        }

        private static void ListAllToMenu(Dictionary<string, AverageWeather> weatherData)
        {
            var sortedByTemp = weatherData.OrderByDescending(d => d.Value.AverageTemprature);

            Console.WriteLine($"{"Date",-15}{"Avg Temp",-15}{"Avg Humid",-15}{"Position",-10}{"Mold_Risk",-10}");
            bool isOn = false;
            foreach (var item in sortedByTemp)
            {

                Console.WriteLine($"{item.Value.Date,-15}{item.Value.AverageTemprature.ToString("F2"),-15}{item.Value.AverageHumidity.ToString("F2"),-15}{item.Value.Position,-10}{item.Value.GetMoldRisk().ToString("F2"),-10}");
                isOn = !isOn;
            }

        }

        private static Dictionary<string, AverageWeather> SortByTemprature(Dictionary<string, AverageWeather> weatherData)
        {
            var sorted = weatherData.OrderByDescending(d => d.Value.AverageTemprature);
            return sorted.ToDictionary();
        }
        private static Dictionary<string, AverageWeather> SortByHumidity(Dictionary<string, AverageWeather> weatherData)
        {
            var sorted = weatherData.OrderBy(d => d.Value.AverageHumidity);
            return sorted.ToDictionary();
        }

        private static Dictionary<string, AverageWeather> SortByMoldRisk(Dictionary<string, AverageWeather> weatherData)
        {
            var sorted = weatherData.OrderBy(d => d.Value.GetMoldRisk());
            return sorted.ToDictionary();
        }




    }
}
