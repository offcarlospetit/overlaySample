using System;
using Android.Content;
using Android.Graphics;
using Android.Views;
using OverlaySample.Controls;

namespace OverlaySample.Droid.Views
{

    public class NativeOverlayView : View
    {
        Bitmap windowFrame;


        float overlayOpacity = 0.5f;


        bool showOverlay = false;
        public bool ShowOverlay
        {
            get { return showOverlay; }
            set
            {
                bool repaint = !showOverlay;
                showOverlay = value;
                if (repaint)
                {
                    Redraw();
                }
            }
        }


        public float Opacity
        {
            get { return overlayOpacity; }
            set
            {
                overlayOpacity = value;
                Redraw();
            }
        }

        Color overlayColor = Color.Gray;
        public Color OverlayBackgroundColor
        {
            get { return overlayColor; }
            set
            {
                overlayColor = value;
                Redraw();

            }
        }

        OverlayShape overlayShape = OverlayShape.Circle;

        public OverlayShape Shape
        {
            get { return overlayShape; }
            set
            {
                overlayShape = value;
                Redraw();

            }
        }


        public NativeOverlayView(Context context, bool showOverlay = false) : base(context)
        {
            ShowOverlay = showOverlay;
            SetWillNotDraw(false);
        }

        protected override void OnDraw(Canvas canvas)
        {
            base.OnDraw(canvas);
            if (ShowOverlay)
            {
                if (windowFrame == null)
                {
                    CreateWindowFrame();
                }
                canvas.DrawBitmap(windowFrame, 0, 0, null);
            }
        }
        void Redraw()
        {
            if (ShowOverlay)
            {
                windowFrame?.Recycle();
                windowFrame = null;
                Invalidate();
            }
        }
        void CreateWindowFrame()
        {
            float width = this.Width;
            float height = this.Height;

            windowFrame = Bitmap.CreateBitmap((int)width, (int)height, Bitmap.Config.Argb8888);
            Canvas osCanvas = new Canvas(windowFrame);
            Paint paint = new Paint(PaintFlags.AntiAlias)
            {
                Color = OverlayBackgroundColor,
                Alpha = (int)(255 * Opacity)
            };

            RectF outerRectangle = new RectF(0, 0, width, height);

            osCanvas.DrawRect(outerRectangle, paint);

            paint.SetXfermode(new PorterDuffXfermode(PorterDuff.Mode.Clear));


            switch (Shape)
            {
                case OverlayShape.Circle:

                    float radius = Math.Min(width, height) * 0.45f;
                    osCanvas.DrawCircle(width / 2, (height / 2), radius, paint);

                    break;
                case OverlayShape.Square:
                    float rectHeight = Math.Min(width, height) * 0.7f;  // Ajusta este valor según lo desees, es el 70% del menor tamaño.
                    float rectWidth = rectHeight * 1.6f;  // Relación de aspecto de 1.6

                    // Asegúrate de que el rectángulo se ajusta dentro del canvas
                    if (rectWidth > width)
                    {
                        rectWidth = width * 0.9f;  // Usamos el 90% del ancho del canvas
                        rectHeight = rectWidth / 1.6f;
                    }

                    float left = (width - rectWidth) / 2;
                    float top = (height - rectHeight) / 2;
                    osCanvas.DrawRect(left, top, left + rectWidth, top + rectHeight, paint);
                    break;
                case OverlayShape.Doc:
                    float margin = 90f; // Define un margen para el borde. Puedes ajustar este valor según lo desees.
                    float docWidth = width - 2 * margin;  // Ancho total menos los márgenes de ambos lados
                    float docHeight = height - 2 * margin;  // Altura total menos los márgenes de arriba y abajo

                    float docLeft = margin;
                    float docTop = margin;

                    osCanvas.DrawRect(docLeft, docTop, docLeft + docWidth, docTop + docHeight, paint);
                    break;
                default:
                    float sideLengths = Math.Min(width, height) * 0.9f;  // Adjust this multiplier as needed.
                    float lefts = (width - sideLengths) / 2;
                    float tops = (height - sideLengths) / 2;
                    osCanvas.DrawRect(lefts, tops, lefts + sideLengths, tops + sideLengths, paint);
                    break;
            }


        }

        protected override void OnLayout(bool changed, int l, int t, int r, int b)
        {
            base.OnLayout(changed, l, t, r, b);
            windowFrame?.Recycle();
            windowFrame = null;
        }
    }
}
