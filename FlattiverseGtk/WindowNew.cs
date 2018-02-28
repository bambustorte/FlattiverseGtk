using System;
using Gtk;
using Gdk;
using Cairo;
using Flattiverse;
using FlattiverseGtk;
using System.Collections.Generic;
using System.Threading;

public partial class WindowNew : Gtk.Window {

    Controller controller;
    Client client;
    RendererCairo renderer;
    FlattiverseGtk.Timer timer = new FlattiverseGtk.Timer();
    public static bool rendering = false;

    public WindowNew(Controller controller, Client client) : base(Gtk.WindowType.Toplevel) {
        this.controller = controller;
        this.client = client;

        Thread timerThread = new Thread(timer.Sleep);
        timerThread.Name = "timerThread";
        timerThread.Start();

        Build();

        int width, height;
        drawingarea1.GdkWindow.GetSize(out width, out height);

        //if (renderer == null)
        renderer = new RendererCairo(client, width, height);

        client.TickEvent += delegate {
            progressbar1.Fraction = client.Ship.Energy / client.Ship.EnergyMax;
            drawingarea1.QueueDraw();
        };

        drawingarea1.AddEvents((int)EventMask.ButtonPressMask);
        drawingarea1.ButtonPressEvent += delegate (object o, ButtonPressEventArgs args) {
            client.SetMoveVector(
                new Vector((float)args.Event.X, (float)args.Event.Y)
                - new Vector(renderer.CenterX, renderer.CenterY)
            );
        };

        client.GetMessageServer().NewMessageEvent += NewMessage;


        Thread renderT = new Thread(renderer.RenderScene);

        timer.GraphicsUpdate += delegate {
            if (renderT.ThreadState == ThreadState.Running)
                return;
            rendering = true;
            renderT = new Thread(renderer.RenderScene);
            renderT.Name = "renderThread";
            renderT.Start();

        };

        //timer.GraphicsUpdate += drawingarea1.QueueDraw;//RenderScene;

    }

    protected void OnDrawingarea1ExposeEvent(object o, ExposeEventArgs args) {

        DrawingArea da = (DrawingArea)o;

        Context c = Gdk.CairoHelper.Create(da.GdkWindow);

        c.SetSourceSurface(renderer.ImageSurface, 0, 0);
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
            client.JoinGame();
            controller.StartAll();
        } catch (GameException gE) {
            Console.WriteLine(gE.Message);
        }
    }

    private void NewMessage(FlattiverseMessage flattiverseMessage) {

        Application.Invoke(delegate {
            if (messages.Buffer.Text == "") {

                List<FlattiverseMessage> all = client.GetMessageServer().Messages;
                List<string> lines = new List<string>();
                foreach (FlattiverseMessage message in all)
                    lines.Add(message.ToString());
                String s = "";
                foreach (String m in lines) {
                    s += m;
                    s += "\n";
                }
                messages.Buffer.Text += s;
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
}
