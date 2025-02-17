namespace VäderDataAnalys.Models.Interfaces;

public interface IAverageWeather
{
    public string? Date { get; set; }
    public string Position { get; set; }
    public double AverageHumidity { get; set; }
    public double AverageTemprature { get; set; }
    public double AverageMoldRisk { get; set; }
    public string Month { get; set; }


    public double GetMoldRisk();
}
