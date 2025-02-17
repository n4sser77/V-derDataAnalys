using VäderDataAnalys.Models;
using VäderDataAnalys.Services;
using VäderDataAnalys.Menu;
namespace VäderDataAnalys
{
    public class Program
    {
        public static async Task Main(string[] args)
        {

            DataExtractService data = new DataExtractService();

            

            

            //await data.ProcessFile();
            //Console.WriteLine("data extract complete!!!");

            await data.ProcessFilteredData();

            var averageWeather = await data.CalculateAverageTempPerDay();
            

            await MainMenu.Print(averageWeather);




        }
    }
}
