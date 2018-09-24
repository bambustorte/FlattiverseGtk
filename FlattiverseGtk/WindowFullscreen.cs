using System;
using Flattiverse;
using FlattiverseGtk;
using System.Collections.Generic;
using System.Threading;
using System.Windows.Forms;
using System.Drawing;



public partial class WindowFullscreen : System.Windows.Forms.Form {

    Controller controller;
    Client client;

    Panel drawingArea = new Canvas();

    float zoomFactor = 1000f;
    float centerX;
    float centerY;
    bool sizeSet = false;


    public WindowFullscreen(Controller controller, Client client) {
        this.controller = controller;
        this.client = client;

        Build();

        client.TickEvent += delegate {};

        client.MessageServer.NewMessageEvent += NewMessage;
    }

    void Build() {

        FormBorderStyle = FormBorderStyle.None;
        WindowState = FormWindowState.Maximized;

        //todo: make instructions screen
        //todo: add energy bar
        
        drawingArea.MouseClick += DrawingClicked;
        drawingArea.MouseWheel += MouseWheelHandler;
        drawingArea.KeyDown += KeyDownEventHandler;
        drawingArea.Paint += DrawHandler;
        client.Scanner.ScanUpdate += drawingArea.Invalidate;

        //todo message feed

        Controls.Add(drawingArea);

        drawingArea.Select();
    }

    void DrawHandler(object sender, PaintEventArgs e) {
        if(!sizeSet){
            Console.WriteLine("dkfja");
            drawingArea.Size = Size;
            centerX = drawingArea.Width / 2;
            centerY = drawingArea.Height / 2;
            sizeSet = true;
        }

        if (!Client.running)
            return;

        e.Graphics.Clear(Color.Black);

        // Compute magnification factor
        int radarScreenMinDimension = Math.Min(drawingArea.Width, drawingArea.Height);
        // Make screen at least 2000x2000 flattiverse miles with Ship at center
        // i.e. minimum screenPixels corresponds to 2000 flattiverse miles
        float displayFactor = radarScreenMinDimension / zoomFactor;

        float shipRadius = client.GetShipSize() * displayFactor;

        Graphics g = e.Graphics;

        //ship
        {
            g.DrawEllipse(
                Pens.Purple,
                centerX - shipRadius,
                centerY - shipRadius,
                shipRadius * 2,
                shipRadius * 2
            );
            Point one = new Point((int)centerX - 10, (int)centerY - 10);
            Point two = new Point((int)((centerX - 10) + client.ShipEnergyPercent/5),(int)centerY - 10);
            g.DrawString(client.ShipEnergyPercent.ToString(), 
                         new Font(System.Drawing.FontFamily.GenericMonospace, 10), 
                         Brushes.White, 10, 10);
            
            g.DrawString(client.Ship.WeaponProductionStatus.ToString(),
                         new Font(System.Drawing.FontFamily.GenericMonospace, 10),
                         Brushes.White, 200, 10);


            g.DrawLine(Pens.Green, one, two);
        }

        foreach (Unit u in client.Units) {
            float uX = centerX + u.Position.X * displayFactor;
            float uY = centerY + u.Position.Y * displayFactor;
            float uR = u.Radius * displayFactor;
            Pen pen;

            String naem = "";
            naem += u.Name + " <" + u.Kind + ">";

            switch (u.Kind) {
                case UnitKind.Sun:

                    foreach (Corona c in ((Sun)u).Coronas) {
                        float r = c.Radius * displayFactor;
                        g.DrawEllipse(Pens.Orange, uX - r, uY - r, r*2, r*2);
                    }
                    
                    pen = Pens.Yellow;
                    break;
                case UnitKind.Planet:
                    pen = Pens.SandyBrown;
                    break;
                case UnitKind.Moon:
                    pen = Pens.LightGray;
                    break;
                case UnitKind.Meteoroid:
                    pen = Pens.Beige;
                    break;
                case UnitKind.Buoy:
                    pen = Pens.Cyan;
                    Console.WriteLine(((Buoy)u).Message);
                    break;
                case UnitKind.Shot:
                    pen = Pens.Red;
                    break;
                case UnitKind.MissionTarget:
                    MissionTarget tar = ((MissionTarget)u);
                    naem += tar.SequenceNumber;

                    if (tar.DominationRadius > 0) {
                        Color col = tar.Team != null ? tar.Team.Color : Color.White;
                        Pen tempen = new Pen(col);

                        g.DrawArc(tempen, uX, uY, tar.DominationRadius, tar.DominationRadius, 0, 360);
                        g.DrawArc(tempen, centerX - uX, centerY - uY, tar.DominationRadius, tar.DominationRadius, 0, (tar.DominationTicks /350f));
                    }
                    pen = Pens.Cyan;
                    break;
           
                case UnitKind.PlayerShip:
                    pen = new Pen(((PlayerShip)u).Team.Color);
                    break;
                default:
                    pen = Pens.Pink;
                    break;
            }


            g.DrawString(naem, new Font(System.Drawing.FontFamily.GenericMonospace, 10), Brushes.White, uX, uY);

            g.DrawEllipse(pen, uX - uR, uY - uR, uR * 2, uR * 2);
        }
    }

    protected void JoinHandler() {
        try {
            client.ShipContinue();
            controller.StartAll();
        } catch (GameException gE) {
            Console.WriteLine(gE.Message);
        }
    }

    void DrawingClicked(object sender, MouseEventArgs e) {
        if (e.Button == MouseButtons.Left) {
            client.SetMoveVector(
                new Vector((float)e.X, e.Y)
                - new Vector(drawingArea.Width / 2, drawingArea.Height / 2)
            );
        } else if(e.Button == MouseButtons.Right){
            client.Shoot(new Vector(e.X, e.Y) - new Vector(drawingArea.Width / 2, drawingArea.Height / 2));
        }
    }

    void MouseWheelHandler (object sender, MouseEventArgs e) {
        float number = ((e.Delta / Math.Abs(e.Delta)) * 100);
        zoomFactor += number + zoomFactor< 200 ? 200 : number;
        //todo display zoom factor
    }

    private void NewMessage(FlattiverseMessage flattiverseMessage) {
        if (InvokeRequired) {
            //Invoke(
            //    (MethodInvoker)delegate {
            //        NewMessage(flattiverseMessage);
            //    }
            //);
        } else {
            //todo display old messages
            //todo display messages at all
            //if (messages.Text == "") {
                //List<FlattiverseMessage> all = client.MessageServer.Messages;
                //List<string> lines = new List<string>();
                //foreach (FlattiverseMessage message in all)
                //    lines.Add(message.ToString());
                //String s = "";
                //foreach (String m in lines) {
                //    s += m;
                //    s += "\n";
                //}
                //messages.Text += s;
            //}

            //messages.Text += flattiverseMessage.ToString();
            //messages.Text += "\n";

            //messages.SelectionStart = messages.Text.Length;
            //messages.ScrollToCaret();
            //messages.Refresh();
        }
    }

    private void KeyDownEventHandler(object sender, KeyEventArgs e) {


        switch (e.KeyCode) {
            case Keys.Escape:
                Quit();
                break;

            case Keys.W:
                
                break;
            case Keys.A:
                
                break;
            case Keys.S:
                
                break;
            case Keys.D:
                
                break;
            case Keys.Space:
                Vector mv = new Vector(0, 0);
                mv.Length = 0;
                client.SetMoveVector(mv);
                break;

            case Keys.J:
                JoinHandler();
                break;
            case Keys.K:
                client.Ship.Kill();
                break;
            case Keys.O:
                client.BuildProbe();
                break;
        }
    }

    protected void Quit() {
        Dispose();
        //System.Windows.Forms.Application.Exit();
        controller.StopAll();
    }
}
