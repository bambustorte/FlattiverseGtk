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

        Context context;
        DrawingArea drawingArea;
        Client client;
        int width;
        int height;
        int centerX, centerY;
        float displayFactor;
        float shipRadius;
        List<Flattiverse.Unit> units;
        Scanner scanner;

        public RendererCairo(Client client, DrawingArea da){
            this.client = client;
            this.context = Gdk.CairoHelper.Create(da.GdkWindow);
            da.GdkWindow.GetSize(out width, out height);
            drawingArea = da;

            int radarScreenMinDimension = Math.Min(width, height);
            displayFactor = radarScreenMinDimension / 500f;

            this.shipRadius = client.GetShipSize() * displayFactor;

            centerX = width / 2;
            centerY = height / 2;

            units = new List<Flattiverse.Unit>();
            scanner = client.GetScanner();

            FlattiverseGtk.Timer timer = new FlattiverseGtk.Timer();
            Thread timerThread = new Thread(timer.Sleep);
            timerThread.Name = "timerThread";
            timerThread.Start();

            client.GetScanner().ScanUpdate += NewScan;
            timer.GraphicsUpdate += RenderScene;
        }

        void NewScan(){
            units = scanner.GetUnitList();
        }

        public void RenderScene(){
            if (!Client.running)
                return;

            Application.Invoke(delegate {
                context = Gdk.CairoHelper.Create(drawingArea.GdkWindow);
                Clear();

                DrawShip(0);

                DrawUnits();

                //DrawCrosshair();

                context.GetTarget().Dispose();
                context.Dispose();
            });
        }

        void DrawCrosshair(){
            context.Save();

            context.SetSourceColor(WHITE);
            context.Translate(centerX, centerY);
            context.MoveTo(-10,-10);
            context.LineTo(10,10);
            context.MoveTo(-10,10);
            context.LineTo(10,-10);
            context.Stroke();

            context.Restore();
        }

        private void DrawShip(double angle) {
            context.Save();

            context.Translate(centerX, centerY);
            context.SetSourceColor(PINK);
            context.Arc(0, 0, shipRadius, angle, Math.PI*2+angle);
            context.ClosePath();
            context.FillPreserve();

            context.LineWidth = 1;
            context.SetSourceColor(RED);
            context.LineTo(0, 0);

            context.Stroke();

            context.Restore();
        }

        void DrawUnits(){
            context.Save();

            foreach (Flattiverse.Unit u in units) {
                float uX = centerX + u.Position.X * displayFactor;
                float uY = centerY + u.Position.Y * displayFactor;
                float uR = u.Radius * displayFactor;

                context.Arc(uX, uY, uR, 0, 360);
                context.SetSourceColor(WHITE);
                context.StrokePreserve();
                context.SetSourceColor(BLUE);
                context.Fill();
            }

            context.Restore();
        }

        void Clear(){
            context.SetSourceColor(BLACK);
            context.Paint();
        }
    }
}
