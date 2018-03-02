using System;
using Gtk;
//using Gdk;
using Cairo;
using Flattiverse;
using FlattiverseGtk;
using System.Collections.Generic;
using System.Threading;

public partial class WindowMain : Gtk.Window {
    
    public static Color BLACK = new Color(0, 0, 0);
    public static Color RED = new Color(1, 0, 0);
    public static Color PINK = new Color(1, 0, 1);
    public static Color BLUE = new Color(0, 0, 1);
    public static Color WHITE = new Color(1, 1, 1);
    public static Color YELLOW = new Color(1, 1, 0);
    public static Color ORANGE = new Color(0.5, 1, 0);
    public static Color CYAN = new Color(1, 0, 1);
    public static Color GRAY = new Color(0.5, 0.5, 0.5);
    public static Color PURPLE = new Color(0.7, 0, 1);


    Controller controller;
    Client client;
    //FlattiverseGtk.Timer timer = new FlattiverseGtk.Timer();
    //RendererCairo renderer;

    int daWidth, daHeight, rec = 0;
    int centerX, centerY;
    float displayFactor;
    float shipRadius;


    public WindowMain(Controller controller, Client client) : base(Gtk.WindowType.Toplevel) {
        Build();

        this.controller = controller;
        this.client = client;

        //Thread timerThread = new Thread(timer.Sleep);
        //timerThread.Name = "timerThread";
        //timerThread.Start();

        drawingarea1.GdkWindow.GetSize(out daWidth, out daHeight);

        centerX = daWidth / 2;
        centerY = daHeight / 2;

        int radarScreenMinDimension = Math.Min(daWidth, daHeight);
        displayFactor = radarScreenMinDimension / 500f;

        shipRadius = client.GetShipSize();

        //renderer = new RendererCairo(client, daWidth, daHeight);

        client.TickEvent += RenderScene;
        //timer.GraphicsUpdate += drawingarea1.QueueDraw;

        drawingarea1.AddEvents((int)Gdk.EventMask.ButtonPressMask);
        drawingarea1.ButtonPressEvent += delegate (object o, ButtonPressEventArgs args) {
            client.SetMoveVector(
                new Vector((float)args.Event.X, (float)args.Event.Y)
                - new Vector(centerX, centerY)
            );
        };

        client.MessageServer.NewMessageEvent += NewMessage;


        //timer.GraphicsUpdate += drawingarea1.QueueDraw;//RenderScene;

    }

    protected void OnDrawingarea1ExposeEvent(object o, ExposeEventArgs args) {
        DrawingArea da = (DrawingArea)o;
        RenderScene();
    }

    protected void OnDeleteEvent(object sender, DeleteEventArgs a) {
        controller.StopAll();
        a.RetVal = true;
    }

    protected void OnButtonQuitClicked(object sender, EventArgs e) {
        Destroy();
        client.Stop();
    }

    protected void OnButtonJoinClicked(object sender, EventArgs e) {
        buttonJoin.Sensitive = false;
        try {
            client.ShipContinue();
            controller.StartAll();
        } catch (GameException gE){
            Console.WriteLine(gE.Message);
        }
    }

    private void NewMessage(FlattiverseMessage flattiverseMessage) {

        Application.Invoke(delegate {
            if(messages.Buffer.Text == ""){
                
                List<FlattiverseMessage> all = client.MessageServer.Messages;
                List<string> lines = new List<string>();
                foreach (FlattiverseMessage message in all)
                    lines.Add(message.ToString());
                String s = "";
                foreach(String m in lines){
                    s += m;
                    s += "\n";
                }
                messages.Buffer.Text += s;
                return;
            }

            messages.Buffer.Text += flattiverseMessage.ToString();
            messages.Buffer.Text += "\n";

            messages.ScrollToIter(messages.Buffer.EndIter, 0, false, 0, 0);
        });
    }

    protected void OnDrawingarea1KeyPressEvent(object o, KeyPressEventArgs args) {
        //switch(args.Event.Key){
        //    case Gdk.Key.w:
        //    case Gdk.Key.W:
        //        client.Move(270);
        //        Console.WriteLine("moving up");
        //        break;
        //    case Gdk.Key.a:
        //    case Gdk.Key.A:
        //        client.Move(180);
        //        break;
        //    case Gdk.Key.s:
        //    case Gdk.Key.S:
        //        client.Move(90);
        //        break;
        //    case Gdk.Key.d:
        //    case Gdk.Key.D:
        //        client.Move(0);
        //        break;
        //}
    }


    public void RenderScene() {
        Application.Invoke(
            delegate {

                List<Flattiverse.Unit> units = client.Units;

                progressbar1.Fraction = client.Ship.Energy / client.Ship.EnergyMax;

                Context context = Gdk.CairoHelper.Create(drawingarea1.GdkWindow);

                Clear(context);
                DrawShip(context, 0);
                DrawUnits(context, units);

                context.GetTarget().Dispose();
                context.Dispose();
                context = null;
            }
        );

        //DrawCrosshair();

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

}
