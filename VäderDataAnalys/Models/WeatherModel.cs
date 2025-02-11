using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;

namespace VäderDataAnalys.Models
{
    /// <summary>
    /// Per row in the file
    /// </summary>
    public class WeatherModel
    {
        public string Date { get; set; }
        public string Time { get; set; }
        public string Position { get; set; }
        public double Temperature { get; set; }
        public int Humidity { get; set; }

    }



   
}
