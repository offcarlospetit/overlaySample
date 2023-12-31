﻿using System;
using System.Collections.Generic;
using Android.Content;
using Android.Hardware;
using Android.Runtime;
using Android.Views;

namespace OverlaySample.Droid.Views
{
    public sealed class NativeCameraPreview : ViewGroup, ISurfaceHolderCallback, Camera.IPictureCallback
    {
        SurfaceView surfaceView;
        ISurfaceHolder holder;
        Camera.Size previewSize;
        IList<Camera.Size> supportedPreviewSizes;
        Camera camera;
        IWindowManager windowManager;
        private Action<byte[]> imageAvailableCallback;
        public bool IsPreviewing { get; set; }

        public Camera Preview
        {
            get { return camera; }
            set
            {
                camera = value;
                if (camera != null)
                {
                    supportedPreviewSizes = Preview.GetParameters().SupportedPreviewSizes;
                    RequestLayout();
                    SetCameraAutoFocus();
                }
            }
        }

        public NativeCameraPreview(Context context)
            : base(context)
        {
            surfaceView = new SurfaceView(context);
            AddView(surfaceView);

            windowManager = Context.GetSystemService(Context.WindowService).JavaCast<IWindowManager>();

            IsPreviewing = false;
            holder = surfaceView.Holder;
            holder.AddCallback(this);
        }

        protected override void OnMeasure(int widthMeasureSpec, int heightMeasureSpec)
        {
            int width = ResolveSize(SuggestedMinimumWidth, widthMeasureSpec);
            int height = ResolveSize(SuggestedMinimumHeight, heightMeasureSpec);
            SetMeasuredDimension(width, height);

            if (supportedPreviewSizes != null)
            {
                previewSize = GetOptimalPreviewSize(supportedPreviewSizes, width, height);
            }
        }

        protected override void OnLayout(bool changed, int l, int t, int r, int b)
        {
            var msw = MeasureSpec.MakeMeasureSpec(r - l, MeasureSpecMode.Exactly);
            var msh = MeasureSpec.MakeMeasureSpec(b - t, MeasureSpecMode.Exactly);

            surfaceView.Measure(msw, msh);
            surfaceView.Layout(0, 0, r - l, b - t);
        }

        public void SurfaceCreated(ISurfaceHolder holder)
        {
            try
            {
                if (Preview != null)
                {
                    Preview.SetPreviewDisplay(holder);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(@"          ERROR: ", ex.Message);
            }
        }

        public void SurfaceDestroyed(ISurfaceHolder holder)
        {
            if (Preview != null)
            {
                Preview.StopPreview();
            }
        }

        public void SurfaceChanged(ISurfaceHolder holder, Android.Graphics.Format format, int width, int height)
        {
            if(Preview!=null)
            {
                try
                {
                    var parameters = Preview.GetParameters();
                    parameters.SetPreviewSize(previewSize.Width, previewSize.Height);
                    RequestLayout();

                    switch (windowManager.DefaultDisplay.Rotation)
                    {
                        case SurfaceOrientation.Rotation0:
                            camera.SetDisplayOrientation(90);
                            break;
                        case SurfaceOrientation.Rotation90:
                            camera.SetDisplayOrientation(0);
                            break;
                        case SurfaceOrientation.Rotation270:
                            camera.SetDisplayOrientation(180);
                            break;
                    }

                    Preview.SetParameters(parameters);
                    Preview.StartPreview();
                    SetCameraAutoFocus();
                    IsPreviewing = true;
                }
                catch(Exception ex)
                {

                }
              
            }

        }

        Camera.Size GetOptimalPreviewSize(IList<Camera.Size> sizes, int w, int h)
        {
            const double AspectTolerance = 0.1;
            double targetRatio = (double)w / h;

            if (sizes == null)
            {
                return null;
            }

            Camera.Size optimalSize = null;
            double minDiff = double.MaxValue;

            int targetHeight = h;
            foreach (Camera.Size size in sizes)
            {
                double ratio = (double)size.Width / size.Height;

                if (Math.Abs(ratio - targetRatio) > AspectTolerance)
                    continue;
                if (Math.Abs(size.Height - targetHeight) < minDiff)
                {
                    optimalSize = size;
                    minDiff = Math.Abs(size.Height - targetHeight);
                }
            }

            if (optimalSize == null)
            {
                minDiff = double.MaxValue;
                foreach (Camera.Size size in sizes)
                {
                    if (Math.Abs(size.Height - targetHeight) < minDiff)
                    {
                        optimalSize = size;
                        minDiff = Math.Abs(size.Height - targetHeight);
                    }
                }
            }

            return optimalSize;
        }

        public void Capture(Action<byte[]> imageAvailable)
        {
            if (camera != null)
            {
                imageAvailableCallback = imageAvailable;
                camera.TakePicture(null, null, this);
            }
        }

        public void OnPictureTaken(byte[] data, Camera camera)
        {
            imageAvailableCallback?.Invoke(data);
            camera.StartPreview(); // Reiniciar la vista previa después de tomar la foto. la vista previa después de tomar la foto.
        }

        internal byte[] CaptureImage()
        {
            throw new NotImplementedException();
        }

        private void SetCameraAutoFocus()
        {
            if (camera == null)
                return;

            var parameters = camera.GetParameters();
            var supportedFocusModes = parameters.SupportedFocusModes;

            if (supportedFocusModes.Contains(Camera.Parameters.FocusModeContinuousPicture))
            {
                parameters.FocusMode = Camera.Parameters.FocusModeContinuousPicture;
                camera.SetParameters(parameters);
            }
        }
    }
}
