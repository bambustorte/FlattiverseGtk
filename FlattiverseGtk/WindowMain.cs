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
        

        if(!Client.running){
            return;
        }

        Application.Invoke(delegate {
            if(messages.Buffer.Text.Length == 0){
                
                List<FlattiverseMessage> all = client.GetMessageServer().Messages;
                List<string> lines = new List<string>();
                foreach (FlattiverseMessage message in all)
                    lines.Add(message.ToString());
                messages.Buffer.Text = lines.ToArray().ToString();
                String s = "";

                foreach(String m in lines){
                    s += m;
                    s += "\n";
                }
            }

            messages.Buffer.Text += flattiverseMessage.ToString();
            messages.Buffer.Text += "\n";
        });
    }
}
