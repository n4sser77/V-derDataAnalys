using VäderDataAnalys.Models;
using VäderDataAnalys.Services;

namespace VäderDataAnalys
{
    public class Program
    {
        public static async Task Main(string[] args)
        {

            DataExtractService dataExtractService = new DataExtractService();

            //await dataExtractService.ProcessFile();
            //Console.WriteLine("data extract complete!!!");

            await dataExtractService.ProcessFilteredData();

            await dataExtractService.CalculateAverageTempPerDay();

        }
    }
}
