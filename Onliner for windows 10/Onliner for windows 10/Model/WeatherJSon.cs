using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Onliner_for_windows_10.Model
{
    public class WeatherJSon
    {
        public string city { get; set; }
        public List<Forecast> forecast { get; set; }
        public Now now { get; set; }
        public Today today { get; set; }
        
    }
    public class Forecast
    {
        public string dateTextDayOfWeek { get; set; }
        public string dayOfMonth { get; set; }
        public DayTemperature dayTemperature { get; set; }
        public string falloutIcon { get; set; }
        public string humidityTitle { get; set; }
        public NightTemperature nightTemperature { get; set; }
        public string phenomena { get; set; }
        public string pressureTitle { get; set; }
        public string weekend { get; set; }
        public Wind wind { get; set; }
        public string windDirection { get; set; }
    }
    #region Forecast
    public class DayTemperature
    {
        public string max { get; set; }
        public string min { get; set; }
    }
    public class NightTemperature
    {
        public string max { get; set; }
        public string min { get; set; }
    }
    public class Now
    {
        public string falloutIcon { get; set; }
        public string phenomena { get; set; }
        public string pressureTitle { get; set; }
        public string temperature { get; set; }
        public Wind wind { get; set; }
        public string windDirection { get; set; }
    }
    #endregion

    #region Today
    public class Today
    {
        public string date { get; set; }
        public Day day { get; set; }
        public Evening evening { get; set; }
        public Morning morning { get; set; }
        public Night night { get; set; }
    }

    public class Day
    {
        public string falloutIcon { get; set; }
        public string phenomena { get; set; }
        public string rusDateTime { get; set; }
        public string temperature { get; set; }
    }
    public class Evening
    {
        public string falloutIcon { get; set; }
        public string phenomena { get; set; }
        public string rusDateTime { get; set; }
        public string temperature { get; set; }
    }
    public class Morning
    {
        public string falloutIcon { get; set; }
        public string phenomena { get; set; }
        public string rusDateTime { get; set; }
        public string temperature { get; set; }
    }
    public class Night
    {
        public string falloutIcon { get; set; }
        public string phenomena { get; set; }
        public string rusDateTime { get; set; }
        public string temperature { get; set; }
    }
    #endregion

    #region Wind
    public class Wind
    {
        public Direction direction { get; set; }
        public string gustsSpeed { get; set; }
        public List<string> speed { get; set; }
        public string unstable { get; set; }
    }

    public class Direction
    {
        public Abbr abbr { get; set; }
        public string title { get; set; }
    }

    public class Abbr
    {
        public string eng { get; set; }
        public string rus { get; set; }
    }
    #endregion
    
}
