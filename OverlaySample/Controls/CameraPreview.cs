using System;
using Xamarin.Forms;

namespace OverlaySample.Controls
{
    public enum CameraOptions
    {
        Rear,
        Front
    }

    public class CameraPreview : View
    {
        public static readonly BindableProperty CameraProperty = BindableProperty.Create(
            propertyName: "Camera",
            returnType: typeof(CameraOptions),
            declaringType: typeof(CameraPreview),
            defaultValue: CameraOptions.Rear);

        public CameraOptions Camera
        {
            get { return (CameraOptions)GetValue(CameraProperty); }
            set { SetValue(CameraProperty, value); }
        }

        public void Capture()
        {
            Application.Current.MainPage.Dispatcher.BeginInvokeOnMainThread(() =>
            {
                // Invocar evento
                OnCapture?.Invoke(this, EventArgs.Empty);
            });
        }

        public event EventHandler OnCapture;
    }
}
