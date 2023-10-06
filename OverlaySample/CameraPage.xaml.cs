using System;
using System.Collections.Generic;
using OverlaySample.Controls;
using Xamarin.Forms;

namespace OverlaySample
{
    public partial class CameraPage : ContentPage
    {
        private string overlayType;
        public CameraPage(string overlayType)
        {
            InitializeComponent();
            this.overlayType = overlayType;
        }
        protected override void OnAppearing()
        {
            base.OnAppearing();

            overlayView.OverlayBackgroundColor = Color.FromHex("#34393b");
            if (overlayType == "DNI")
            {
                overlayView.Shape = OverlayShape.Square;
            }
            else if (overlayType == "Documento")
            {
                overlayView.Shape = OverlayShape.Doc;
            }
        }

        void Handle_ValueChanged(object sender, Xamarin.Forms.ValueChangedEventArgs e)
        {
            
            
        }

        void Handle_Toggled(object sender, Xamarin.Forms.ToggledEventArgs e)
        {
            
        }


        void Handle_Camera(object sender, Xamarin.Forms.ToggledEventArgs e)
        {
            if (e.Value)
            {
                cameraPreview.Camera = CameraOptions.Front;
            }
            else
            {
                cameraPreview.Camera = CameraOptions.Rear;
            }
        }
    }
}
