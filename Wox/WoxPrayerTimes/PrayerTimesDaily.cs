using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using J = Newtonsoft.Json.JsonPropertyAttribute;
using R = Newtonsoft.Json.Required;
using N = Newtonsoft.Json.NullValueHandling;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Net.Http;

namespace WoxPrayerTimes
{

    public partial class PrayerTimesDaily
    {
        [J("fajr")] public string Fajr { get; set; }
        [J("sunrise")] public string Sunrise { get; set; }
        [J("dhuhr")] public string Dhuhr { get; set; }
        [J("asr")] public string Asr { get; set; }
        [J("maghrib")] public string Maghrib { get; set; }
        [J("isha")] public string Isha { get; set; }
        [J("hijri")] public string Hijri { get; set; }
        [J("gregorian")] public string Gregorian { get; set; }
    }
}
