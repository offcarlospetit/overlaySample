using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OverlaySample.Controls;
using Xamarin.Forms;

namespace OverlaySample
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            overlayView.OverlayBackgroundColor = Color.FromHex("#fccccc");
            overlayView.Shape = OverlayShape.Circle;
        }

        void Handle_ValueChanged(object sender, Xamarin.Forms.ValueChangedEventArgs e)
        {

          
        }

        void Handle_Tapped(object sender, System.EventArgs e)
        {
            label.TextColor =(label.TextColor == Color.Black ? Color.Blue : Color.Black);

        }

        void Handle_Toggled(object sender, Xamarin.Forms.ToggledEventArgs e)
        {
            
        }
    }
}
