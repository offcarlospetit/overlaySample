using System;
using System.Threading.Tasks;
using AVFoundation;
using CoreGraphics;
using CoreMedia;
using CoreVideo;
using Foundation;
using UIKit;


namespace OverlaySample.iOS.Services
{
    public class CameraService
    {
        private AVCaptureSession captureSession;
        private AVCaptureStillImageOutput stillImageOutput;
        private AVCaptureVideoPreviewLayer previewLayer;
        AVCaptureDevice currentDevice = AVCaptureDevice.DefaultDeviceWithMediaType(AVMediaType.Video);


        public CameraService()
        {
            ConfigureCamera();
            ConfigureSession();
        }

        void ConfigureSession()
        {
            captureSession = new AVCaptureSession();

            // ... (otros códigos de configuración)

            // Aquí es donde realizarías las modificaciones para el efecto espejo
            if (currentDevice.Position == AVCaptureDevicePosition.Front)
            {
                var connection = stillImageOutput.ConnectionFromMediaType(AVMediaType.Video); // Cambiamos captureOutput por stillImageOutput
                if (connection != null && connection.SupportsVideoMirroring)
                {
                    connection.VideoMirrored = true;
                }
            }
        }

        private void ConfigureCamera()
        {
            captureSession = new AVCaptureSession();
            var videoDevice = AVCaptureDevice.DefaultDeviceWithMediaType(AVMediaType.Video);
            if (videoDevice == null)
            {
                // No camera available. Handle appropriately.
                return;
            }

            var videoInput = AVCaptureDeviceInput.FromDevice(videoDevice);
            if (captureSession.CanAddInput(videoInput))
            {
                captureSession.AddInput(videoInput);
            }

            stillImageOutput = new AVCaptureStillImageOutput();
            if (captureSession.CanAddOutput(stillImageOutput))
            {
                captureSession.AddOutput(stillImageOutput);
            }

            previewLayer = new AVCaptureVideoPreviewLayer(captureSession)
            {
                Frame = UIScreen.MainScreen.Bounds
            };
        }

        public AVCaptureVideoPreviewLayer GetPreviewLayer()
        {
            return previewLayer;
        }

        public async Task<UIImage> CaptureImageAsync()
        {
            var videoConnection = stillImageOutput.ConnectionFromMediaType(AVMediaType.Video);
            var sampleBuffer = await stillImageOutput.CaptureStillImageTaskAsync(videoConnection);

            var jpegImageAsNsData = AVCaptureStillImageOutput.JpegStillToNSData(sampleBuffer);
            var jpegAsByteArray = new byte[jpegImageAsNsData.Length];
            System.Runtime.InteropServices.Marshal.Copy(jpegImageAsNsData.Bytes, jpegAsByteArray, 0, Convert.ToInt32(jpegImageAsNsData.Length));

            var image = new UIImage(jpegImageAsNsData);
            image = MirrorImage(image);

            return image;
        }

        public void StartSession()
        {
            if (!captureSession.Running)
            {
                captureSession.StartRunning();
            }
        }

        public void StopSession()
        {
            if (captureSession.Running)
            {
                captureSession.StopRunning();
            }
        }

        public UIImage MirrorImage(UIImage image)
        {
            var cgImage = image.CGImage;

            var context = new CGBitmapContext(IntPtr.Zero, cgImage.Width, cgImage.Height, cgImage.BitsPerComponent, cgImage.BytesPerRow, cgImage.ColorSpace, cgImage.BitmapInfo);
            context.TranslateCTM(cgImage.Width, 0);
            context.ScaleCTM(-1.0f, 1.0f);
            context.DrawImage(new CGRect(0, 0, cgImage.Width, cgImage.Height), cgImage);

            var mirroredImage = UIImage.FromImage(context.ToImage());
            return mirroredImage;
        }
    }
}

