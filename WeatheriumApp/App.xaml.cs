using System;
using WeatheriumApp.Views;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace WeatheriumApp
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            MainPage = new CurrentWeatherPage();
        }

        protected override void OnStart()
        {
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }
    }
}
