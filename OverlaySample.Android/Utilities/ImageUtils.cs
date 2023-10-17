using Android.Graphics;

namespace OverlaySample.Droid.Utilities
{
    public static class ImageUtils
    {
        public static Bitmap MirrorImage(Bitmap originalBitmap)
        {
            Matrix matrix = new Matrix();
            matrix.PreScale(-1.0f, 1.0f);
            return Bitmap.CreateBitmap(originalBitmap, 0, 0, originalBitmap.Width, originalBitmap.Height, matrix, true);
        }
    }
}