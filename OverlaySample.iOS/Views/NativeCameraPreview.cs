using System;
using System.Linq;
using AVFoundation;
using CoreGraphics;
using Foundation;
using OverlaySample.Controls;
using UIKit;

namespace OverlaySample.iOS.Views
{
    public class NativeCameraPreview : UIView
    {
        AVCaptureVideoPreviewLayer previewLayer;
        CameraOptions cameraOptions;
        AVCaptureDevice camera;

        public event EventHandler<EventArgs> Tapped;

        public AVCaptureSession CaptureSession { get; private set; }

        public bool IsPreviewing { get; set; }



        public NativeCameraPreview(CameraOptions options)
        {
            cameraOptions = options;
            IsPreviewing = false;
            Initialize();
            camera = AVCaptureDevice.DefaultDeviceWithMediaType(AVMediaType.Video);
            if (camera.IsFocusModeSupported(AVCaptureFocusMode.ContinuousAutoFocus))
            {
                camera.LockForConfiguration(out NSError error);
                if (error == null)
                {
                    camera.FocusMode = AVCaptureFocusMode.ContinuousAutoFocus;
                    camera.UnlockForConfiguration();
                }
            }
        }

        public override void Draw(CGRect rect)
        {
            base.Draw(rect);
            previewLayer.Frame = rect;
        }

        protected virtual void OnTapped()
        {
            var eventHandler = Tapped;
            if (eventHandler != null)
            {
                eventHandler(this, new EventArgs());
            }
        }

        public override void TouchesBegan(NSSet touches, UIEvent evt)
        {
            base.TouchesBegan(touches, evt);
            if (camera == null || !camera.IsFocusModeSupported(AVCaptureFocusMode.AutoFocus))
                return;

            var touch = touches.AnyObject as UITouch;
            if (touch == null)
                return;

            var uiCameraPreviewLayer = new AVCaptureVideoPreviewLayer(CaptureSession);
            var location = touch.LocationInView(this);
            var convertedLocation = uiCameraPreviewLayer.CaptureDevicePointOfInterestForPoint(location);
            ConfigureFocusAtPoint(convertedLocation);
        }

        private void ConfigureFocusAtPoint(CoreGraphics.CGPoint point)
        {
            if (camera.LockForConfiguration(out NSError error))
            {
                if (camera.FocusPointOfInterestSupported && camera.IsFocusModeSupported(AVCaptureFocusMode.AutoFocus))
                {
                    camera.FocusPointOfInterest = point;
                    camera.FocusMode = AVCaptureFocusMode.AutoFocus;
                }
                camera.UnlockForConfiguration();
            }
        }

        void Initialize()
        {
            CaptureSession = new AVCaptureSession();
            previewLayer = new AVCaptureVideoPreviewLayer(CaptureSession)
            {
                Frame = Bounds,
                VideoGravity = AVLayerVideoGravity.ResizeAspectFill
            };

            var videoDevices = AVCaptureDevice.DevicesWithMediaType(AVMediaType.Video);
            var cameraPosition = (cameraOptions == CameraOptions.Front) ? AVCaptureDevicePosition.Front : AVCaptureDevicePosition.Back;
            var device = videoDevices.FirstOrDefault(d => d.Position == cameraPosition);

            if (device == null)
            {
                return;
            }

            NSError error;
            var input = new AVCaptureDeviceInput(device, out error);
            CaptureSession.AddInput(input);
            Layer.AddSublayer(previewLayer);
            CaptureSession.StartRunning();
            IsPreviewing = true;
        }
    }
}