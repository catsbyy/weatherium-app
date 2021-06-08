using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WeatheriumApp.Helper;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using static WeatheriumApp.Models.WeatherObject;
using Newtonsoft.Json;
using Xamarin.Essentials;
using System.Globalization;
using WeatheriumApp.Models;

namespace WeatheriumApp.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class CurrentWeatherPage : ContentPage
    {
        public CurrentWeatherPage()
        {
            InitializeComponent();
            //"en-US", "de-DE", "ar-TN" 
            CultureInfo.DefaultThreadCurrentCulture = new CultureInfo("en-US");
            GetCoordinates();
        }

        private string Location { get; set; } = "Kyiv, Ukraine";
        public double Latitude { get; set; }
        public double Longtitude { get; set; }

        private async void GetCoordinates()
        {
            try
            {
                var request = new GeolocationRequest(GeolocationAccuracy.Best);
                var location = await Geolocation.GetLocationAsync(request);

                if (location != null)
                {
                    Latitude = location.Latitude;
                    Longtitude = location.Longitude;
                    Location = await GetCity(location);

                    GetWeatherInfo();
                    GetBackground();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.StackTrace);
            }
        }
        private async Task<string> GetCity(Location location)
        {
            var places = await Geocoding.GetPlacemarksAsync(location);
            var currentPlace = places?.FirstOrDefault();

            if (currentPlace != null)
                return $"{currentPlace.Locality},{currentPlace.CountryName}";

            return null;
        }
        private async void GetBackground()
        {
            var url = $"https://api.unsplash.com/search/photos?query={Location}&client_id={ApiKeys.apiUnsplash}&page=1&per_page=20";

            var result = await ApiCaller.Get(url, ApiKeys.apiUnsplash);

            if (result.Successful)
            {
                var bgInfo = JsonConvert.DeserializeObject<Root>(result.Response);

                if (bgInfo != null && bgInfo.results.Count > 0)
                {
                    bgImg.Source = ImageSource.FromUri(new Uri(bgInfo.results[new Random().Next(0, bgInfo.results.Count - 1)].urls.full));
                }
            }
        }

        private async void GetWeatherInfo()
        {
            string url = $"http://api.openweathermap.org/data/2.5/weather?q={Location}&appid={ApiKeys.apiOpenWeather}&units=metric";

            var result = await ApiCaller.Get(url);

            if (result.Successful)
            {
                try
                {
                    var weatherInfo = JsonConvert.DeserializeObject<WeatherInfo>(result.Response);
                    descriptionTxt.Text = weatherInfo.weather[0].description.ToUpper();
                    iconImg.Source = $"w{weatherInfo.weather[0].icon}";

                    RegionInfo cultureInfo = new RegionInfo(weatherInfo.sys.country);
                    string country = cultureInfo.EnglishName;
                    cityTxt.Text = weatherInfo.name + ", " + country;
                    Location = cityTxt.Text;
                    temperatureTxt.Text = weatherInfo.main.temp.ToString("0") + "°C";
                    minmaxTempTxt.Text = weatherInfo.main.temp_min.ToString("0") + "° / " + weatherInfo.main.temp_max.ToString("0") + "°";
                    realFeelTxt.Text = weatherInfo.main.feels_like.ToString("0") + "°";
                    humidityTxt.Text = $"{weatherInfo.main.humidity}%";
                    pressureTxt.Text = $"{weatherInfo.main.pressure} hpa";
                    windTxt.Text = $"{weatherInfo.wind.speed} m/s";
                    cloudinessTxt.Text = $"{weatherInfo.clouds.all}%";

                    dateTxt.Text = DateTime.Today.ToLongDateString().ToUpper();

                    GetForecast();
                }
                catch (Exception ex)
                {
                    await DisplayAlert("Weather Info", ex.Message, "OK");
                }
            }
            else 
            {
                await DisplayAlert("Weather Info", "No weather information found", "OK");
            }
        }
        public static DateTime JavaTimeStampToDateTime(double javaTimeStamp)
        {
            // Java timestamp is milliseconds past epoch
            System.DateTime dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
            dtDateTime = dtDateTime.AddMilliseconds(javaTimeStamp).ToLocalTime();
            return dtDateTime;
        }

        private async void GetForecast()
        {
            var url = $"http://api.openweathermap.org/data/2.5/forecast?q={Location}&appid=71bb05d58da8e4bad5346f165d6026be&units=metric";
            var result = await ApiCaller.Get(url);

            if (result.Successful)
            {
                try
                {
                    var forcastInfo = JsonConvert.DeserializeObject<ForecastInfo>(result.Response);

                    List<List> allList = new List<List>();

                    foreach (var list in forcastInfo.list)
                    {
                        var date = DateTime.Parse(list.dt_txt);

                        if (date > DateTime.Now && date.Hour == 0 && date.Minute == 0 && date.Second == 0)
                            allList.Add(list);
                    }

                    dayOneTxt.Text = DateTime.Parse(allList[0].dt_txt).ToString("dddd");
                    dateOneTxt.Text = DateTime.Parse(allList[0].dt_txt).ToString("dd MMM");
                    iconOneImg.Source = $"w{allList[0].weather[0].icon}";
                    tempOneTxt.Text = allList[0].main.temp.ToString("0") + "°";

                    dayTwoTxt.Text = DateTime.Parse(allList[1].dt_txt).ToString("dddd");
                    dateTwoTxt.Text = DateTime.Parse(allList[1].dt_txt).ToString("dd MMM");
                    iconTwoImg.Source = $"w{allList[1].weather[0].icon}";
                    tempTwoTxt.Text = allList[1].main.temp.ToString("0") + "°";

                    dayThreeTxt.Text = DateTime.Parse(allList[2].dt_txt).ToString("dddd");
                    dateThreeTxt.Text = DateTime.Parse(allList[2].dt_txt).ToString("dd MMM");
                    iconThreeImg.Source = $"w{allList[2].weather[0].icon}";
                    tempThreeTxt.Text = allList[2].main.temp.ToString("0") + "°";

                    dayFourTxt.Text = DateTime.Parse(allList[3].dt_txt).ToString("dddd");
                    dateFourTxt.Text = DateTime.Parse(allList[3].dt_txt).ToString("dd MMM");
                    iconFourImg.Source = $"w{allList[3].weather[0].icon}";
                    tempFourTxt.Text = allList[3].main.temp.ToString("0") + "°";

                    dayFiveTxt.Text = DateTime.Parse(allList[4].dt_txt).ToString("dddd");
                    dateFiveTxt.Text = DateTime.Parse(allList[4].dt_txt).ToString("dd MMM");
                    iconFiveImg.Source = $"w{allList[4].weather[0].icon}";
                    tempFiveTxt.Text = allList[4].main.temp.ToString("0") + "°";

                }
                catch (Exception ex)
                {
                    await DisplayAlert("Weather Info", ex.Message, "OK");
                }
            }
            else
            {
                await DisplayAlert("Weather Info", "No forecast information found", "OK");
            }
        }
    }
}