using System;
using Android.Content;
using Android.Hardware;
using OverlaySample.Controls;
using OverlaySample.Droid.Renderers;
using OverlaySample.Droid.Views;
using Plugin.Permissions;
using Plugin.Permissions.Abstractions;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: ExportRenderer(typeof(CameraPreview), typeof(CameraPreviewRenderer))]
namespace OverlaySample.Droid.Renderers
{
    public class CameraPreviewRenderer : ViewRenderer<CameraPreview, NativeCameraPreview>
    {
        NativeCameraPreview cameraPreview;

        public CameraPreviewRenderer(Context context) : base(context)
        {
        }

        public void OnAutoFocus(bool success, Camera camera)
        {
            var parameters = camera.GetParameters();
            if (parameters.FocusMode != Android.Hardware.Camera.Parameters.FocusModeContinuousPicture)
            {
                parameters.FocusMode = Android.Hardware.Camera.Parameters.FocusModeContinuousPicture;

                if (parameters.MaxNumFocusAreas > 0)
                {
                    parameters.FocusAreas = null;
                }
                camera.SetParameters(parameters);
                camera.StartPreview();
            }
        }

        protected override async void OnElementChanged(ElementChangedEventArgs<CameraPreview> e)
        {
            base.OnElementChanged(e);


            if (e.OldElement != null)
            {
                // Des-suscribir de eventos del elemento anterior
                e.OldElement.OnCapture -= OnCaptureRequested;
            }


            if (Control == null)
            {
                cameraPreview = new NativeCameraPreview(Context);
                SetNativeControl(cameraPreview);
            }

            if (e.OldElement != null)
            {
                // Unsubscribe
                cameraPreview.Click -= OnCameraPreviewClicked;
            }
            if (e.NewElement != null)
            {

                try
                {
                    var status = await CrossPermissions.Current.CheckPermissionStatusAsync(Permission.Camera);
                    if (status != PermissionStatus.Granted)
                    {
                        if (await CrossPermissions.Current.ShouldShowRequestPermissionRationaleAsync(Permission.Camera))
                        {
                           // await DisplayAlert("Need location", "Gunna need that location", "OK");
                        }

                        var results = await CrossPermissions.Current.RequestPermissionsAsync(Permission.Camera);
                        //Best practice to always check that the key exists
                        if (results.ContainsKey(Permission.Camera))
                            status = results[Permission.Camera];
                    }

                    if (status == PermissionStatus.Granted)
                    {
                        Control.Preview = Camera.Open((int)e.NewElement.Camera);
                    }
                    else if (status != PermissionStatus.Unknown)
                    {
                      // await DisplayAlert("Location Denied", "Can not continue, try again.", "OK");
                    }
                }
                catch (Exception ex)
                {

                   // LabelGeolocation.Text = "Error: " + ex;
                }


                // Subscribe
                e.NewElement.OnCapture += OnCaptureRequested;
                cameraPreview.Click += OnCameraPreviewClicked;
            }
        }

        private void OnCaptureRequested(object sender, EventArgs e)
        {
            cameraPreview?.Capture(data =>
            {
                // Aquí, "data" es tu imagen como un array de bytes.
                // Puedes hacer lo que quieras con este dato.
            });
        }


        void OnCameraPreviewClicked(object sender, EventArgs e)
        {
            if (cameraPreview.IsPreviewing)
            {
                cameraPreview.Preview.StopPreview();
                cameraPreview.IsPreviewing = false;
            }
            else
            {
                cameraPreview.Preview.StartPreview();
                cameraPreview.IsPreviewing = true;
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                Control.Preview.Release();
            }
            base.Dispose(disposing);
        }

        public void Capture()
        {
            
        }

        public event EventHandler OnCapture;
    }
}