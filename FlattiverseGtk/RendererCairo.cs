using System;
using Gtk;
using Cairo;
using System.Threading;
using System.Collections.Generic;


namespace FlattiverseGtk {
    public class RendererCairo {

        public static Color BLACK = new Color(0, 0, 0);
        public static Color RED = new Color(1, 0, 0);
        public static Color PINK = new Color(1, 0, 1);
        public static Color BLUE = new Color(0, 0, 1);
        public static Color WHITE = new Color(1, 1, 1);
        public static Color YELLOW = new Color(1, 1, 0);
        public static Color ORANGE = new Color(1, 0.9, 0);
        public static Color GRAY = new Color(0.5, 0.5, 0.5);
        public static Color PURPLE = new Color(0.7, 0, 1);

        ImageSurface imageSurface;
        ReaderWriterLock imageSurfaceLock = new ReaderWriterLock();

        Client client;
        int width;
        int height;
        int centerX, centerY;
        float displayFactor;
        float shipRadius;

        public RendererCairo(Client client, int width, int height) {
            this.client = client;
            this.width = width;
            this.height = height;

            imageSurface = new ImageSurface(Format.RGB24, width, height);

            centerX = width / 2;
            centerY = height / 2;

            int radarScreenMinDimension = Math.Min(width, height);
            displayFactor = radarScreenMinDimension / 500f;

            shipRadius = client.GetShipSize();
        }

        public ImageSurface ImageSurface {
            get {
                imageSurfaceLock.AcquireReaderLock(20);
                ImageSurface ret = new ImageSurface(imageSurface.Data, Format.RGB24, width, height, 4 * width);
                imageSurfaceLock.ReleaseReaderLock();
                return ret;
            }
            set {
                imageSurfaceLock.AcquireWriterLock(20);

                imageSurface = value;

                imageSurfaceLock.ReleaseWriterLock();
            }
        }

        public void RenderScene() {
            List<Flattiverse.Unit> units = client.Units;


            


            Context context = new Cairo.Context(ImageSurface);
            Clear(context);

            DrawShip(context, 0);
            DrawUnits(context, units);



            context.GetTarget().Dispose();
            context.Dispose();
            context = null;

        }

        void DrawCrosshair(Context context) {
            context.Save();

            context.SetSourceColor(WHITE);
            context.Translate(centerX, centerY);
            context.MoveTo(-10, -10);
            context.LineTo(10, 10);
            context.MoveTo(-10, 10);
            context.LineTo(10, -10);
            context.Stroke();

            context.Restore();
        }

        private void DrawShip(Context context, double angle) {
            context.Save();

            context.Translate(centerX, centerY);
            context.SetSourceColor(PURPLE);
            context.Arc(0, 0, shipRadius * displayFactor, angle, Math.PI * 2 + angle);
            context.ClosePath();
            context.Stroke();

            context.LineWidth = 1;
            context.SetSourceColor(RED);
            context.LineTo(0, 0);

            context.Stroke();

            context.Restore();
        }

        void DrawUnits(Context context, List<Flattiverse.Unit> units) {

            context.Save();

            foreach (Flattiverse.Unit u in units) {
                float uX = centerX + u.Position.X * displayFactor;
                float uY = centerY + u.Position.Y * displayFactor;
                float uR = u.Radius * displayFactor;

                switch (u.Kind) {
                    case Flattiverse.UnitKind.Sun:
                        context.SetSourceColor(ORANGE);

                        //context.Arc(uX, uY, (((Sun)u).Coronas[0]).Radius * displayFactor, 0, 360);
                        context.Stroke();

                        context.SetSourceColor(YELLOW);
                        break;
                    case Flattiverse.UnitKind.Moon:
                        context.SetSourceColor(WHITE);
                        break;

                    case Flattiverse.UnitKind.Planet:
                        context.SetSourceColor(GRAY);
                        break;

                    default:
                        context.SetSourceColor(PINK);
                        break;
                }
                context.Arc(uX, uY, uR, 0, 360);
                context.Stroke();
            }

            context.Restore();
        }

        void Clear(Context context) {
            context.SetSourceColor(BLACK);
            context.Paint();
        }

        public int CenterX {
            get {
                return this.centerX;
            }
        }

        public int CenterY {
            get {
                return this.centerY;
            }
        }
    }
}
