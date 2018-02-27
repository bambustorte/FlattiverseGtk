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

    Button buttonJoin;
    Button buttonQuit;
    ProgressBar energyBar;
    Panel drawingArea;
    RichTextBox messages;
    TableLayoutPanel table;

    float zoomFactor = 1000f;


    public WindowForms(Controller controller, Client client) {
        this.controller = controller;
        this.client = client;

        Build();

        client.TickEvent += delegate {
            energyBar.Value = (int)client.Ship.Energy;
        };

        client.GetMessageServer().NewMessageEvent += NewMessage;
        drawingArea.Paint += RadarScreenPaintEventHandler;
        drawingArea.Resize += RadarScreenResizedHandler;
        client.GetScanner().ScanUpdate += ScanUpdate;
        Resize += WindowResized;
    }

    void Build(){

        table = new TableLayoutPanel();
        table.Size = Size;
        table.ColumnCount = 3;
        table.RowCount = 3;


        buttonQuit = new Button();
        buttonQuit.Text = "Quit";
        buttonQuit.Click += OnButtonQuitClicked;
        table.Controls.Add(buttonQuit, 0, 0);

        buttonJoin = new Button();
        buttonJoin.Text = "Join";
        buttonJoin.Click += OnButtonJoinClicked;
        table.Controls.Add(buttonJoin, 1, 0);

        energyBar = new ProgressBar();
        energyBar.Maximum = (int)client.Ship.EnergyMax;
        energyBar.Value = energyBar.Maximum;
        table.Controls.Add(energyBar, 2, 0);

        TableLayoutPanel subTable = new TableLayoutPanel();
        subTable.ColumnCount = 1;
        subTable.RowCount = 2;
        TextBox zoom = new TextBox();
        zoom.Text = zoomFactor.ToString();
        Button app = new Button();
        app.Text = "Apply";
        app.Click += (sender, e) => { this.zoomFactor = float.Parse(zoom.Text); };
        subTable.Controls.Add(zoom, 0, 0);
        subTable.Controls.Add(app, 0,1);
        table.Controls.Add(subTable, 0, 1);

        drawingArea = new Panel();
        drawingArea.Size = new Size(400, 200);
        drawingArea.MouseClick += DrawingClicked;
        drawingArea.MouseWheel += (sender, e) => {
            Console.WriteLine(e.Delta);
        };
        table.Controls.Add(drawingArea, 1, 1);

        messages = new RichTextBox();
        messages.Size = drawingArea.Size;
        table.Controls.Add(messages, 1, 2);

        table.CellBorderStyle = TableLayoutPanelCellBorderStyle.Single;


        Controls.Add(table);
    }

    protected void OnButtonQuitClicked(object sender, EventArgs e) {
        Dispose();
        client.Stop();
    }

    void WindowResized(object sender, EventArgs e){
        table.Size = Size;
    }

    private void RadarScreenResizedHandler(object sender, EventArgs e) {
        drawingArea.Refresh();
    }

    void RadarScreenPaintEventHandler(object sender, PaintEventArgs e){
        if (!Client.running)
            return;

        e.Graphics.Clear(Color.Black);

        // Compute magnification factor
        int radarScreenMinDimension = Math.Min(drawingArea.Width, drawingArea.Height);
        // Make screen at least 2000x2000 flattiverse miles with Ship at center
        // i.e. minimum screenPixels corresponds to 2000 flattiverse miles
        float displayFactor = radarScreenMinDimension / zoomFactor;

        float centerX = drawingArea.Width / 2;
        float centerY = drawingArea.Height / 2;

        float shipRadius = client.GetShipSize() * displayFactor;

        Graphics g = e.Graphics;
        g.DrawEllipse(Pens.Purple,
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

    void DrawingClicked(object sender, MouseEventArgs e){
        client.Move(new Vector());


        client.Move(
            new Vector((float)e.X, e.Y)
            - new Vector(drawingArea.Width / 2, drawingArea.Height / 2)
        );

    }

    private void NewMessage(FlattiverseMessage flattiverseMessage) {

        //if(!Client.running){
        //    return;
        //}

        if (InvokeRequired) {
            Console.WriteLine("INVOKE REQUIRED");
            try {
                Invoke(
                (MethodInvoker)delegate {
                    NewMessage(flattiverseMessage);
                    }
                );
            }catch(Exception ex){
                Console.WriteLine(ex.Message);
            }

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
