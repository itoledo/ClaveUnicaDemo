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

        Uri callbackUrl = new Uri("claveunicademo://");

        public async void LoginClicked(object sender, EventArgs args)
        {
            var authUrl = new Uri("https://claveunicademo.azurewebsites.net/auth/claveunica");

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

        public async void LogoutClicked(object sender, EventArgs args)
        {
            var claveUnicaLogout = "https://claveunicademo.azurewebsites.net/auth/claveunicalogout";
            var authUrl = new Uri($"https://accounts.claveunica.gob.cl/api/v1/accounts/app/logout?redirect={claveUnicaLogout}");

            try
            {
                var r = await WebAuthenticator.AuthenticateAsync(authUrl, callbackUrl);
                RUN.Text = Nombres.Text = Email.Text = string.Empty;
                await DisplayAlert("Exito", "Sesión de Clave Única cerrada exitosamente.", "Ok");
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine($"excepcion: {e}");
                await DisplayAlert("Error", "Ocurrió algún error al cerrar su sesión de Clave Única, por favor intente nuevamente.", "Ok");
            }
        }

        public async void Logo_Tapped(object sender, EventArgs args)
        {
            await Launcher.OpenAsync("https://www.birdie.cl/");
        }
    }
}
