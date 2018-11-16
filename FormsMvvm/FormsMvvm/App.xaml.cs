using System;
using System.Diagnostics;
using System.Reflection;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

[assembly: XamlCompilation(XamlCompilationOptions.Compile)]
namespace FormsMvvm
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            MainPage = new NavigationPage(new MainPage());
        }

        protected override void OnStart()
        {
            Trace.WriteLine($"{GetType().Name} {MethodBase.GetCurrentMethod().Name}");
        }

        protected override void OnSleep()
        {
            Trace.WriteLine($"{GetType().Name} {MethodBase.GetCurrentMethod().Name}");
        }

        protected override void OnResume()
        {
            Trace.WriteLine($"{GetType().Name} {MethodBase.GetCurrentMethod().Name}");
        }
    }
}
