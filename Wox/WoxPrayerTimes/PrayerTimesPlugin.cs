using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Wox.Plugin;
using J = Newtonsoft.Json.JsonPropertyAttribute;
using R = Newtonsoft.Json.Required;
using N = Newtonsoft.Json.NullValueHandling;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Net.Http;
using System.Diagnostics;

namespace WoxPrayerTimes
{
    public class PrayerTimesPlugin : IPlugin, IContextMenu
    {
        private PluginInitContext _context;

        private readonly List<(string, string)> Cities = new List<ValueTuple<string, string>>() {
                ( "istanbul", "İstanbul"),
                ("ankara", "Ankara" ),
                ( "bursa", "Bursa" ),
                ( "erzurum", "Erzurum" ),
                ( "eskisehir", "Eskişehir" ),
                ( "gaziantep", "Gaziantep" ),
                ( "izmir", "İzmir" ),
                ( "kayseri", "Kayseri" ),
                ( "konya", "Konya" ),
                ( "sakarya", "Sakarya" ),
                ("tekirdag", "Tekirdağ" )
            };

        public void Init(PluginInitContext context)
        {
            this._context = context;
        }

        public PrayerTimesDaily GetPrayerTimes(string city)
        {
            using (var http = new HttpClient())
            {
                var content = http
                    .GetStringAsync(GetDailyUrl(city))
                    .ConfigureAwait(false)
                    .GetAwaiter()
                    .GetResult();

                var prayerTimes = JsonConvert.DeserializeObject<PrayerTimesDaily>(content);
                return prayerTimes;
            }
        }

        private static string GetDailyUrl(string city)
        {
            return $"http://api.namazvakti.guneysu.xyz/{city}/daily";
        }

        public List<Result> Query(Query query)
        {

            List<Result> results = new List<Result>();

            switch (query.FirstSearch)
            {
                case "":
                case " ":
                    return GetCitiesResults();

                case "istanbul":
                case "ankara":
                case "bursa":
                case "erzurum":
                case "eskisehir":
                case "gaziantep":
                case "izmir":
                case "kayseri":
                case "konya":
                case "sakarya":
                case "tekirdag":
                    return FillPrayerTimes(query);
                default:
                    break;
            }

            return results;
        }

        private List<Result> GetCitiesResults()
        {
            return Cities.Select(kv =>
            {
                var (city, title) = kv;

                return new Result()
                {
                    Title = title,
                    Action = context =>
                    {
                        _context.API.ChangeQuery($"prayer {city}");
                        return false;
                    },
                    ContextData = city
                };
            }).ToList();
        }

        private List<Result> FillPrayerTimes(Query query)
        {
            var prayerTimes = GetPrayerTimes(query.FirstSearch);
            var results = new List<Result>()
            {
                PrayerTimeResult("Hicrî", prayerTimes.Hijri),
                PrayerTimeResult("Milâdi", prayerTimes.Gregorian),
                PrayerTimeResult("İmsak", prayerTimes.Fajr),
                PrayerTimeResult("Güneş", prayerTimes.Sunrise),
                PrayerTimeResult("Öğle", prayerTimes.Dhuhr),
                PrayerTimeResult("İkindi", prayerTimes.Asr),
                PrayerTimeResult("Akşam", prayerTimes.Maghrib),
                PrayerTimeResult("Yatsı", prayerTimes.Isha)
            };
            return results;
        }

        public Result PrayerTimeResult(string title, string subtitle) => new Result()
        {
            Title = title,
            SubTitle = subtitle,
            Action = e => true
        };

        public List<Result> LoadContextMenus(Result selectedResult)
        {
            return new List<Result>()
            {
            new Result()
            {
                Title = "Open in Browser",
                Action = ctx =>
                {
                    //_context.API.ChangeQuery(GetDailyUrl(selectedResult.ContextData.ToString()), true);
                    //_context.API.ShowMsg("Hello");
                    Process.Start(GetDailyUrl(selectedResult.ContextData.ToString()));
                    return false;
                }
            }
            };

        }
    }
}
