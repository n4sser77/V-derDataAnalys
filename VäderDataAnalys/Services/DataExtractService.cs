using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.IO;
using VäderDataAnalys.Models;

namespace VäderDataAnalys.Services;

public class DataExtractService
{
    private string rootPath = @"..\..\..\";
    private string DataFileName = "tempdata5-med-fel.txt";
    // private string FilterdDataPath = @"..\..\..";
    public List<WeatherModel> Data { get; set; }
    public async Task ProcessFile(string path = "")
    {
        if (!string.IsNullOrEmpty(path))
        {
            rootPath = path;
        }

        Regex pattern = new Regex(@"^(2016-05|2017-01)");

        StreamWriter sw = new StreamWriter(path + "Filtered-Data.txt");
        if (!File.Exists(path + "Filtered-Data.txt"))
        {
            File.Create(path);
        }



        int count = 0;
        await foreach (var line in ReadFileLinesAsync(rootPath, DataFileName))
        {
            if (pattern.IsMatch(line))
            {
                Console.WriteLine(line);
            }
            else
            {
                await sw.WriteLineAsync(line);
            }
            count++;
            //if (count % 1000 == 0)  // Log every 1000 lines
            //{
            //    Console.WriteLine($"Processed {count} lines...");
            //}
        }
    }

    private async IAsyncEnumerable<string> ReadFileLinesAsync(string path, string fileName)
    {


        if (!File.Exists(Path.Combine(path, "Filtered-Data.txt")))
        {
            Console.WriteLine("Path: " + Path.Combine(path, "Filtered-Data.txt"));
            throw new FileNotFoundException();

        }

        Console.WriteLine("File exists");

        using StreamReader sr = new StreamReader(Path.Combine(path, "Filtered-Data.txt"));

        while (!sr.EndOfStream)
        {

            yield return await sr.ReadLineAsync();
        }


    }






    public async Task ProcessFilteredData()
    {
        Regex pattern1 = new Regex(@"(?<Date>^\d{4}-\d{2}-\d{2}) (?<Time>\d{2}:\d{2}:\d{2}),(?<Position>Inne|Ute),(?<Temprature>\-?\d+\.?\d*),(?<Humidity>\d{2})");
        try
        {
            await foreach (var line in ReadFileLinesAsync(rootPath, "Filtered -Data.txt"))
            {
                var match = pattern1.Match(line);

                var v = match.Groups["Date"].Value;
            }

        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
        }
    }

}
