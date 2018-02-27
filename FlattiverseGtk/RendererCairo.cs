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
        public static Color GRAY = new Color(0.5, 0.5, 0.5);
        public static Color PURPLE = new Color(0.7, 0, 1);

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
            units = client.Map.UnitList;
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
            context.SetSourceColor(PURPLE);
            context.Arc(0, 0, shipRadius, angle, Math.PI*2+angle);
            context.ClosePath();
            context.FillPreserve();

            context.LineWidth = 1;
            context.SetSourceColor(RED);
            context.LineTo(0, 0);

            context.Stroke();

            context.Restore();
        }

        void DrawUnits() {
            context.Save();

            foreach (Flattiverse.Unit u in units) {
                switch(u.Kind){
                    case Flattiverse.UnitKind.Sun:
                        DrawSun(u);
                        break;

                    case Flattiverse.UnitKind.Moon:
                        DrawMoon(u);
                        break;

                    case Flattiverse.UnitKind.Planet:
                        DrawPlanet(u);
                        break;

                    default:
                        DrawUnit(u);
                        break;
                }
            }

            context.Restore();
        }

        void DrawSun(Flattiverse.Unit u) {
            context.Save();

            float uX = centerX + u.Position.X * displayFactor;
            float uY = centerY + u.Position.Y * displayFactor;
            float uR = u.Radius * displayFactor;

            context.Arc(uX, uY, uR, 0, 360);
            context.SetSourceColor(WHITE);
            context.StrokePreserve();
            context.SetSourceColor(YELLOW);
            context.Fill();

            context.Restore();
        }

        void DrawMoon(Flattiverse.Unit u) {
            context.Save();

            float uX = centerX + u.Position.X * displayFactor;
            float uY = centerY + u.Position.Y * displayFactor;
            float uR = u.Radius * displayFactor;

            context.Arc(uX, uY, uR, 0, 360);
            context.SetSourceColor(WHITE);
            context.StrokePreserve();
            context.SetSourceColor(WHITE);
            context.Fill();

            context.Restore();
        }

        void DrawPlanet(Flattiverse.Unit u) {
            context.Save();

            float uX = centerX + u.Position.X * displayFactor;
            float uY = centerY + u.Position.Y * displayFactor;
            float uR = u.Radius * displayFactor;

            context.Arc(uX, uY, uR, 0, 360);
            context.SetSourceColor(WHITE);
            context.StrokePreserve();
            context.SetSourceColor(GRAY);
            context.Fill();

            context.Restore();
        }

        void DrawUnit(Flattiverse.Unit u) {
            context.Save();

            float uX = centerX + u.Position.X * displayFactor;
            float uY = centerY + u.Position.Y * displayFactor;
            float uR = u.Radius * displayFactor;

            context.Arc(uX, uY, uR, 0, 360);
            context.SetSourceColor(WHITE);
            context.StrokePreserve();
            context.SetSourceColor(PINK);
            context.Fill();

            context.Restore();
        }

        void Clear(){
            context.SetSourceColor(BLACK);
            context.Paint();
        }
    }
}
