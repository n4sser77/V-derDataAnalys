using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using VäderDataAnalys.Models.Interfaces;

namespace VäderDataAnalys.Models;

/// <summary>
/// Per row in the file
/// </summary>
public class WeatherModel : IWeatherModel
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

