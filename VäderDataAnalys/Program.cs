using VäderDataAnalys.Models;
using VäderDataAnalys.Services;
using VäderDataAnalys.Menu;
namespace VäderDataAnalys
{
    public class Program
    {
        public static Dictionary<string, AverageWeather> balls = new Dictionary<string, AverageWeather>();
        public static async Task Main(string[] args)
        {

            DataExtractService data = new DataExtractService();

            

            

            //await data.ProcessFile();
            //Console.WriteLine("data extract complete!!!");

            await data.ProcessFilteredData();

            var averageWeather = await data.CalculateAverageTempPerDay();
            

            balls = averageWeather;

            await MainMenu.Print(averageWeather);




        }
    }
}
