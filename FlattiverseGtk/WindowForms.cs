using System;
using Flattiverse;
using FlattiverseGtk;
using System.Collections.Generic;
using System.Threading;
using System.Windows.Forms;
using System.Drawing;

public partial class WindowForms : System.Windows.Forms.Form {

    Controller controller;
    Client client;
    RendererCairo renderer;

    Button buttonJoin;
    Button buttonQuit;
    ProgressBar energyBar;
    Panel drawingArea;
    RichTextBox messages;


    public WindowForms(Controller controller, Client client) {
        this.controller = controller;
        this.client = client;

        Build();

        client.TickEvent += delegate {
            energyBar.Value = (int)client.Ship.Energy;
        };

        drawingArea.Click += (sender, e) => {
            //client.Move(
            //    new Vector((float)args.Event.X, (float)args.Event.Y)
            //    - new Vector(renderer.CenterX, renderer.CenterY)
            //);
        };

        client.GetMessageServer().NewMessageEvent += NewMessage;
        drawingArea.Paint += RadarScreenPaintEventHandler;
        drawingArea.Resize += RadarScreenResizedHandler;
        client.GetScanner().ScanUpdate += ScanUpdate;
    }

    void Build(){
        TableLayoutPanel table = new TableLayoutPanel();
        table.ColumnCount = 3;
        table.RowCount = 3;


        buttonQuit = new Button();
        buttonQuit.Text = "Quit";
        buttonQuit.Click += OnButtonJoinClicked;
        table.Controls.Add(buttonQuit, 0, 0);

        buttonJoin = new Button();
        buttonJoin.Text = "Join";
        buttonJoin.Click += OnButtonQuitClicked;
        table.Controls.Add(buttonJoin, 1, 0);

        energyBar = new ProgressBar();
        energyBar.Maximum = (int)client.Ship.EnergyMax;
        energyBar.Value = energyBar.Maximum;
        table.Controls.Add(energyBar, 2, 0);

        drawingArea = new Panel();
        table.Controls.Add(drawingArea, 1, 1);

        messages = new RichTextBox();
        table.Controls.Add(messages, 1, 2);

        Controls.Add(table);
    }

    protected void OnButtonQuitClicked(object sender, EventArgs e) {
        Dispose();
        client.Stop();
    }

    private void RadarScreenResizedHandler(object sender, EventArgs e) {
        drawingArea.Refresh();
    }

    void RadarScreenPaintEventHandler(object sender, PaintEventArgs e){
        if (!Client.running)
            return;

        // Compute magnification factor
        int radarScreenMinDimension = Math.Min(drawingArea.Width, drawingArea.Height);
        // Make screen at least 2000x2000 flattiverse miles with Ship at center
        // i.e. minimum screenPixels corresponds to 2000 flattiverse miles
        float displayFactor = radarScreenMinDimension / 2000f;

        float centerX = drawingArea.Width / 2;
        float centerY = drawingArea.Height / 2;

        float shipRadius = client.GetShipSize() * displayFactor;

        Graphics g = e.Graphics;
        g.DrawEllipse(Pens.White,
            centerX - shipRadius, centerY - shipRadius,
            shipRadius * 2, shipRadius * 2);


        foreach (Unit u in client.Map.UnitList) {
            float uX = centerX + u.Position.X * displayFactor;
            float uY = centerY + u.Position.Y * displayFactor;
            float uR = u.Radius * displayFactor;
            g.DrawEllipse(Pens.Green, uX - uR, uY - uR, uR * 2, uR * 2);
        }
    }

    void ScanUpdate(){
        drawingArea.Invalidate();
    }

    protected void OnButtonJoinClicked(object sender, EventArgs e) {
        buttonJoin.Enabled = false;
        try {
            client.JoinGame();
            controller.StartAll();
        } catch (GameException gE) {
            Console.WriteLine(gE.Message);
        }
    }

    private void NewMessage(FlattiverseMessage flattiverseMessage) {

        //if(!Client.running){
        //    return;
        //}

        if (InvokeRequired) {
            Console.WriteLine("INVOKE REQUIRED");
            //Invoke(new MethodInvoker(NewMessage));
        } else {
            if (messages.Text == "") {

                List<FlattiverseMessage> all = client.GetMessageServer().Messages;
                List<string> lines = new List<string>();
                foreach (FlattiverseMessage message in all)
                    lines.Add(message.ToString());
                String s = "";
                foreach (String m in lines) {
                    s += m;
                    s += "\n";
                }
                messages.Text += s;
            }

            messages.Text += flattiverseMessage.ToString();
            messages.Text += "\n";

            messages.SelectionStart = messages.Text.Length;
            messages.ScrollToCaret();
            messages.Refresh();
        }
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
