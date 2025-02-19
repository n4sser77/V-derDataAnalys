using VäderDataAnalys.Models.Interfaces;

namespace VäderDataAnalys.Models;

    public class AverageWeather : IAverageWeather
    {
        public string? Date { get; set; }
        public string Position { get; set; }
        public double AverageHumidity { get; set; }
        public double AverageTemprature { get; set; }
        public double AverageMoldRisk { get; set; }
        public string Month { get; set; }


        public double GetMoldRisk()
        {
            // ((humidity - 78) * (Temp/15))/0,22    -     %risk mögel

            const double HumidityThreshold = 78.0;   // Ingen mögelrisk under denna fuktighet
            const double TemperatureScale = 15.0;    // Temperatur normaliseringsfaktor
            const double RiskDivisor = 0.22;         // Formelns skalningsfaktor
            const double MaxRawRisk = 200.0;         // Maxvärde för rå mögelrisk

            // Beräkna den råa mögelrisken enligt  formel
            double rawRisk = ((AverageHumidity - HumidityThreshold) * (AverageTemprature / TemperatureScale)) / RiskDivisor;

            // Normalisera till en skala mellan 0 och 100
            double scaledRisk = (rawRisk / MaxRawRisk) * 100;

            // Begränsa resultatet mellan 0 och 100
            return Math.Clamp(scaledRisk, 0, 100);
        }


        public delegate AverageWeather? MyDelegate(Dictionary<string,AverageWeather> weatherData, double breakpoint = 0);
        public static AverageWeather? GetMeteoroLogicalWinter(Dictionary<string, AverageWeather> weatherData)
        {
            MyDelegate winterDelegate = CalculateMeteroLogical;

            return winterDelegate(weatherData,0.00);
        }

        public static AverageWeather? GetMeteoroLogicalAutumn(Dictionary<string, AverageWeather> weatherData)
        {
            MyDelegate autumnDelegate = CalculateMeteroLogical;

            return autumnDelegate(weatherData, 10.00);
        }

        private static AverageWeather? CalculateMeteroLogical(Dictionary<string, AverageWeather> weatherData, double breakpoint = 0.00)
        {
            // get höst temp
            var outsideWeatherData = weatherData.Where(d => d.Value.Position == "Ute");


            // kolla om det håller i fem dygn
            int streak = 1;
            AverageWeather? result = null;
            foreach (var item in outsideWeatherData)
            {
                if (streak >= 5)
                {
                    // höst är här
                    return result;
                }

                // kollar efter dagar under 10-grader
                if (item.Value.AverageTemprature < breakpoint)
                {
                    // om ja, vi har streak
                    if (streak == 1) result = item.Value;
                    streak++;
                }
                else
                {
                    // reset streak
                    streak = 1;

                }
            }
            return result;
        }

        public static Dictionary<string, AverageWeather> GetMonthReport(Dictionary<string, AverageWeather> weatherData)
        {
            // Gruppindelning baserad på både månad och position
            var groupedByMonthAndPosition = weatherData.Values
                .GroupBy(w => new { w.Month, w.Position });

            var monthReport = new Dictionary<string, AverageWeather>();

            foreach (var group in groupedByMonthAndPosition)
            {
                // Beräkna genomsnitt för varje grupp
                double avgTemp = group.Select(w => w.AverageTemprature).DefaultIfEmpty(0).Average();
                double avgHumidity = group.Select(w => w.AverageHumidity).DefaultIfEmpty(0).Average();
                double avgMoldRisk = group.Select(w => w.GetMoldRisk()).DefaultIfEmpty(0).Average();

                // Skapa en ny nyckel t.ex. "2024-02-Inne" eller "2024-02-Ute"
                string key = $"{group.Key.Month}-{group.Key.Position}";

                // Skapa ett nytt AverageWeather-objekt med de aggregerade värdena
                monthReport[key] = new AverageWeather
                {
                    Month = group.Key.Month,
                    Position = group.Key.Position,
                    AverageTemprature = avgTemp,
                    AverageHumidity = avgHumidity,
                    // Om din klass har en property för risk, annars lägg in på ett sätt som passar
                    AverageMoldRisk = avgMoldRisk
                };
            }

            // without date
            return monthReport;
        }

    }

