using VäderDataAnalys.Services;

namespace VäderDataAnalys.Models
{
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




}
