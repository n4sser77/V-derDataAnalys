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
    public Dictionary<string, double> TempData { get; set; }
    public Dictionary<string, int> HumidityData { get; set; }
    public List<WeatherModel> WeatherData { get; set; } = new List<WeatherModel>();
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

        Console.WriteLine("File exists");

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
        Regex pattern1 = new Regex(@"(?<Date>^(?<Year>\d{4})-(?<Month>\d{2})-(?<Day>\d{2})) (?<Time>\d{2}:\d{2}:\d{2}),(?<Position>Inne|Ute),(?<Temprature>\-?\d+\.?\d*),(?<Humidity>\d{2})");
        try
        {

            await foreach (var line in ReadFileLinesAsync(rootPath, "Filtered-Data.txt"))
            {
                if (line == null) return;
                var match = pattern1.Match(line);

                if (match.Groups["Date"].Success == false || match.Groups["Time"].Success == false
                    || match.Groups["Position"].Success == false || match.Groups["Temprature"].Success == false || match.Groups["Humidity"].Success == false)
                {
                    continue;
                }

                var row = new WeatherModel
                {
                    Date = match.Groups["Date"].Value,
                    Time = match.Groups["Time"].Value,
                    Position = match.Groups["Position"].Value,
                    Temperature = double.Parse(match.Groups["Temprature"].Value, CultureInfo.InvariantCulture),
                    Humidity = int.Parse(match.Groups["Humidity"].Value)
                };

                WeatherData.Add(row);



            }
            Console.WriteLine("List loaded");


        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message + " " + e.InnerException);
            throw;
        }
    }



    public async Task CalculateAverageTempPerDay()
    {
        // Group by Date
        var groupedByDate = WeatherData
            .GroupBy(w => w.Date)
            .OrderBy(g => g.Key) // Ensures dates are in order
            .ToDictionary(g => g.Key, g => g.ToList());

        // Print grouped data
        foreach (var group in groupedByDate)
        {
            Console.WriteLine($"Date: {group.Key, -20}, Entries: {group.Value.Count,-30}");

            foreach (var entry in group.Value)
            {
                Console.WriteLine($" - {entry.Time,-10} | {entry.Position,-5} | Temp: {entry.Temperature,-3}°C | Humidity: {entry.Humidity,-3}%");
            }
        }
    }
}
