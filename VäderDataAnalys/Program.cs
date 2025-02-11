using VäderDataAnalys.Models;
using VäderDataAnalys.Services;

namespace VäderDataAnalys
{
    public class Program
    {
        public static async Task Main(string[] args)
        {

            DataExtractService dataExtractService = new DataExtractService();

            await dataExtractService.ExtractData();
            Console.WriteLine("data extract complete!!!");

        }
    }
}
