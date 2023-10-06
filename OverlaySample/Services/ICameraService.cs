using System;
namespace OverlaySample.Services
{
    public interface ICameraService
    {
        void Capture(Action<byte[]> onImageCaptured);
    }
}
