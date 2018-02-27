using System;
using Gtk;
using Gdk;
using Cairo;
using Flattiverse;
using FlattiverseGtk;
using System.Collections.Generic;
using System.Threading;

public partial class WindowMain : Gtk.Window {
    
    Controller controller;
    Client client;
    RendererCairo renderer;

    public WindowMain(Controller controller, Client client) : base(Gtk.WindowType.Toplevel) {
        this.controller = controller;
        this.client = client;

        Build();

        client.GetMessageServer().NewMessageEvent += NewMessage;
    }

    protected void OnDrawingarea1ExposeEvent(object o, ExposeEventArgs args) {
        DrawingArea da = (DrawingArea)o;
        if (renderer == null)
            renderer = new RendererCairo(client, drawingarea1);
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
        } catch (GameException gE){
            Console.WriteLine(gE.Message);
        }
    }

    private void NewMessage(FlattiverseMessage flattiverseMessage) {
        
        //if(!Client.running){
        //    return;
        //}

        Application.Invoke(delegate {
            if(messages.Buffer.Text == ""){
                
                List<FlattiverseMessage> all = client.GetMessageServer().Messages;
                List<string> lines = new List<string>();
                foreach (FlattiverseMessage message in all)
                    lines.Add(message.ToString());
                String s = "";
                foreach(String m in lines){
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
        switch(args.Event.Key){
            case Gdk.Key.w:
            case Gdk.Key.W:
                client.Move(270);
                break;
            case Gdk.Key.a:
            case Gdk.Key.A:
                client.Move(180);
                break;
            case Gdk.Key.s:
            case Gdk.Key.S:
                client.Move(90);
                break;
            case Gdk.Key.d:
            case Gdk.Key.D:
                client.Move(0);
                break;
        }
    }
}
