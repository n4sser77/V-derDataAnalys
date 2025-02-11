using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using VäderDataAnalys.Models;

namespace VäderDataAnalys.Services
{
    public class DataExtractService
    {
        public string Path { get; set; } = @"C:\Users\nasse\Desktop\code\System24\ApplikationsArkitektur\VäderDataAnalys\VäderDataAnalys\tempdata5-med-fel.txt";
        public string AllLines { get; set; }
        public List<WeatherModel> Data { get; set; }
        public async Task<string> ExtractData(string path = "")
        {
            if (!string.IsNullOrEmpty(path))
            {
                Path = path;
            }
            using StreamReader sr = new StreamReader(Path);

            int i = 0;
            while (!sr.EndOfStream)
            {
                var line = await sr.ReadLineAsync();
                AllLines += line;
                i++;
                Console.WriteLine("Lines read: " + i);
            }

            return AllLines;
        }

        public void FilterData()
        {
            Regex pattern = new Regex("^(2016-05|2017-01)");

            var match = pattern.Match(AllLines);

            if(match.Success)
            {
                Console.WriteLine(match.Value);
            }

        }

    }
}
