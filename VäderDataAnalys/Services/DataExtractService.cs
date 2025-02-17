using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.IO;
using VäderDataAnalys.Models;
using System.Globalization;

namespace VäderDataAnalys.Services;

public class DataExtractService
{
    private string rootPath = @"..\..\..\";
    private string DataFileName = "tempdata5-med-fel.txt";
    // private string FilterdDataPath = @"..\..\..";
    //public Dictionary<string, double> TempData { get; set; }
    //public Dictionary<string, int> HumidityData { get; set; }
    public List<WeatherModel> WeatherData { get; set; } = new List<WeatherModel>();
    public static Dictionary<string, AverageWeather> AverageWeatherData { get; set; } = new Dictionary<string, AverageWeather>();
    public async Task ProcessFile(string path = "")
    {
        if (!string.IsNullOrEmpty(path))
        {
            rootPath = path;
        }

        Regex pattern = new Regex(@"^(2016-05|2017-01)");

        using StreamWriter sw = new StreamWriter(rootPath + "Filtered-Data.txt");
        if (!File.Exists(rootPath + "Filtered-Data.txt"))
        {
            File.Create(rootPath);
        }



        int count = 0;
        await foreach (var line in ReadFileLinesAsync(rootPath, DataFileName))
        {
            if (line == null) return;

            if (pattern.IsMatch(line))
            {
                Console.WriteLine(line);
                continue;
            }

            await sw.WriteLineAsync(line);

            count++;

        }
    }


    private async IAsyncEnumerable<string> ReadFileLinesAsync(string path, string fileName)
    {


        if (!File.Exists(Path.Combine(path, fileName)))
        {
            Console.WriteLine("Path: " + Path.Combine(path, fileName));
            throw new FileNotFoundException();

        }



        using StreamReader sr = new StreamReader(Path.Combine(path, fileName));
        var line = await sr.ReadLineAsync();
        yield return line;
        while (line != null)
        {

            line = await sr.ReadLineAsync();
            yield return line;
        }


    }






    public async Task ProcessFilteredData()
    {
        Regex pattern1 = new Regex(@"(?<Date>^(?<Year>\d{4})-(?<Month>0[1-9]|1[0-2])-(?<Day>0[1-9]|1[0-9]|2[0-9]|3[0-1])) (?<Time>\d{2}:\d{2}:\d{2}),(?<Position>Inne|Ute),(?<Temprature>\-?\d+\.?\d*),(?<Humidity>\d{2})");
        try
        {

            await foreach (var line in ReadFileLinesAsync(rootPath, "Filtered-Data.txt"))
            {
                if (line == null) return;
                var match = pattern1.Match(line);

                if (match.Groups["Date"].Success == false || match.Groups["Time"].Success == false
                    || match.Groups["Position"].Success == false || match.Groups["Temprature"].Success == false || match.Groups["Humidity"].Success == false || match.Groups["Month"].Success == false)
                {
                    continue;
                }

                var row = new WeatherModel
                {
                    Date = match.Groups["Date"].Value,
                    Month = match.Groups["Month"].Value,
                    Time = match.Groups["Time"].Value,
                    Position = match.Groups["Position"].Value,
                    Temperature = double.Parse(match.Groups["Temprature"].Value, CultureInfo.InvariantCulture),
                    Humidity = int.Parse(match.Groups["Humidity"].Value)
                };

                WeatherData.Add(row);



            }
            // Console.WriteLine("List loaded");


        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message + " " + e.InnerException);
            throw;
        }
    }



    public async Task<Dictionary<string, AverageWeather>> CalculateAverageTempPerDay()
    {
        // Group by Date
        var groupedByDate = WeatherData
            .GroupBy(w => w.Date)
            .OrderBy(g => g.Key) // Ensures dates are in order
            .ToDictionary(g => g.Key, g => g.ToList());



        Dictionary<string, AverageWeather> averageWeather = new();

        double averageTempPerDayInne = 0;
        double averageTempPerDayUte = 0;
        double averageHumidityPerDayInne = 0;
        double averageHumidityPerDayUte = 0;



        // Print grouped data
        foreach (var group in groupedByDate)
        {
            //  Console.WriteLine($"Date: {group.Key,-20}, Entries: {group.Value.Count,-30}");

            averageTempPerDayInne = group.Value.Where(d => d.Position == "Inne").Select(e => e.Temperature).DefaultIfEmpty(0).Average();
            averageTempPerDayUte = group.Value.Where(d => d.Position == "Ute").Select(e => e.Temperature).DefaultIfEmpty(0).Average();
            averageHumidityPerDayInne = group.Value.Where(d => d.Position == "Inne").Select(e => e.Humidity).DefaultIfEmpty(0).Average();
            averageHumidityPerDayUte = group.Value.Where(d => d.Position == "Ute").Select(e => e.Humidity).DefaultIfEmpty(0).Average();



            string month = "";
            foreach (var i in group.Value.AsEnumerable())
            {
                month = i.Month;
                break;
            }

            var averagePerDayInne = new AverageWeather
            {
                Date = group.Key,
                AverageHumidity = averageHumidityPerDayInne,
                AverageTemprature = averageTempPerDayInne,
                Position = "Inne",
                Month = month


            };
            averageWeather.Add(group.Key + "-Inne", averagePerDayInne);
            var averagePerDayUte = new AverageWeather
            {
                Date = group.Key,
                AverageHumidity = averageHumidityPerDayUte,
                AverageTemprature = averageTempPerDayUte,
                Position = "Ute",
                Month = month

            };
            averageWeather.Add(group.Key + "-Ute", averagePerDayUte);


            // Console.WriteLine($"Average Temprature: {averageTempPerDay.ToString("F2")} | Average Humidity: {averageHumidityPerDay.ToString("F2")}");
            //foreach (var entry in group.Value)
            //{

            //    Console.WriteLine($" - {entry.Time,-10} | {entry.Position,-5} | Temp: {entry.Temperature,-3}°C | Humidity: {entry.Humidity,-3}%");
            //}
        }
        AverageWeatherData = averageWeather;
        return averageWeather;
    }



}
