using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace ClaveUnicaDemo
{
    // Learn more about making custom code visible in the Xamarin.Forms previewer
    // by visiting https://aka.ms/xamarinforms-previewer
    [DesignTimeVisible(false)]
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
        }

        public async void LoginClicked(object sender, EventArgs args)
        {
            var authenticationUrl = "https://claveunicademo.azurewebsites.net/auth/claveunica";
            var authUrl = new Uri(authenticationUrl);
            var callbackUrl = new Uri("claveunicademo://");

            try
            {
                var r = await Xamarin.Essentials.WebAuthenticator.AuthenticateAsync(authUrl, callbackUrl);
                if (r.Properties.ContainsKey("run"))
                    RUN.Text = WebUtility.UrlDecode(r.Properties["run"]);
                if (r.Properties.ContainsKey("name"))
                    Nombres.Text = WebUtility.UrlDecode(r.Properties["name"]);
                if (r.Properties.ContainsKey("email"))
                    Email.Text = WebUtility.UrlDecode(r.Properties["email"]);
            } catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine($"excepcion: {e}");
                await DisplayAlert("Excepción", e.ToString(), "Ok");
            }
            //var token = r?.AccessToken ?? r?.IdToken;
        }

        public async void Logo_Tapped(object sender, EventArgs args)
        {
            await Launcher.OpenAsync("https://www.birdie.cl/");
        }
    }
}
