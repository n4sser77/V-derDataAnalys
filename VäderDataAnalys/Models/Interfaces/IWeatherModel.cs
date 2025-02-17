namespace VäderDataAnalys.Models.Interfaces
{
    public interface IWeatherModel
    {
        public string Date { get; set; }
        public string? Time { get; set; }
        public double Temperature { get; set; }
        public int Humidity { get; set; }
        public string Position { get; set; }
        public double AverageTemprature { get; set; }
        public double AverageHumidity { get; set; }
        public string Month { get; set; }
    }
}