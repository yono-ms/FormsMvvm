using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace FormsMvvm
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
        }

        protected override void OnAppearing()
        {
            Trace.WriteLine($"{GetType().Name} {MethodBase.GetCurrentMethod().Name}");
        }

        protected override void OnDisappearing()
        {
            Trace.WriteLine($"{GetType().Name} {MethodBase.GetCurrentMethod().Name}");
        }
    }
}
