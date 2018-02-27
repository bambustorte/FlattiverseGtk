
// This file has been generated by the GUI designer. Do not modify.

public partial class WindowMain
{
	private global::Gtk.VBox vbox2;

	private global::Gtk.HBox hbox2;

	private global::Gtk.Button buttonQuit;

	private global::Gtk.Button buttonJoin;

	private global::Gtk.HBox hbox1;

	private global::Gtk.DrawingArea drawingarea1;

	private global::Gtk.ScrolledWindow GtkScrolledWindow;

	private global::Gtk.TextView messages;

	protected virtual void Build()
	{
		global::Stetic.Gui.Initialize(this);
		// Widget WindowMain
		this.Name = "WindowMain";
		this.Title = global::Mono.Unix.Catalog.GetString("MainWindow");
		this.WindowPosition = ((global::Gtk.WindowPosition)(4));
		this.AllowGrow = false;
		// Container child WindowMain.Gtk.Container+ContainerChild
		this.vbox2 = new global::Gtk.VBox();
		this.vbox2.Name = "vbox2";
		this.vbox2.Spacing = 6;
		// Container child vbox2.Gtk.Box+BoxChild
		this.hbox2 = new global::Gtk.HBox();
		this.hbox2.Name = "hbox2";
		this.hbox2.Spacing = 6;
		// Container child hbox2.Gtk.Box+BoxChild
		this.buttonQuit = new global::Gtk.Button();
		this.buttonQuit.CanFocus = true;
		this.buttonQuit.Name = "buttonQuit";
		this.buttonQuit.UseUnderline = true;
		this.buttonQuit.Label = global::Mono.Unix.Catalog.GetString("quit");
		this.hbox2.Add(this.buttonQuit);
		global::Gtk.Box.BoxChild w1 = ((global::Gtk.Box.BoxChild)(this.hbox2[this.buttonQuit]));
		w1.Position = 0;
		w1.Expand = false;
		w1.Fill = false;
		// Container child hbox2.Gtk.Box+BoxChild
		this.buttonJoin = new global::Gtk.Button();
		this.buttonJoin.CanFocus = true;
		this.buttonJoin.Name = "buttonJoin";
		this.buttonJoin.UseUnderline = true;
		this.buttonJoin.Label = global::Mono.Unix.Catalog.GetString("Join");
		this.hbox2.Add(this.buttonJoin);
		global::Gtk.Box.BoxChild w2 = ((global::Gtk.Box.BoxChild)(this.hbox2[this.buttonJoin]));
		w2.Position = 1;
		w2.Expand = false;
		w2.Fill = false;
		this.vbox2.Add(this.hbox2);
		global::Gtk.Box.BoxChild w3 = ((global::Gtk.Box.BoxChild)(this.vbox2[this.hbox2]));
		w3.Position = 0;
		w3.Expand = false;
		w3.Fill = false;
		// Container child vbox2.Gtk.Box+BoxChild
		this.hbox1 = new global::Gtk.HBox();
		this.hbox1.Name = "hbox1";
		this.hbox1.Spacing = 6;
		// Container child hbox1.Gtk.Box+BoxChild
		this.drawingarea1 = new global::Gtk.DrawingArea();
		this.drawingarea1.WidthRequest = 600;
		this.drawingarea1.HeightRequest = 300;
		this.drawingarea1.Name = "drawingarea1";
		this.hbox1.Add(this.drawingarea1);
		global::Gtk.Box.BoxChild w4 = ((global::Gtk.Box.BoxChild)(this.hbox1[this.drawingarea1]));
		w4.Position = 0;
		this.vbox2.Add(this.hbox1);
		global::Gtk.Box.BoxChild w5 = ((global::Gtk.Box.BoxChild)(this.vbox2[this.hbox1]));
		w5.Position = 1;
		// Container child vbox2.Gtk.Box+BoxChild
		this.GtkScrolledWindow = new global::Gtk.ScrolledWindow();
		this.GtkScrolledWindow.Name = "GtkScrolledWindow";
		this.GtkScrolledWindow.ShadowType = ((global::Gtk.ShadowType)(1));
		// Container child GtkScrolledWindow.Gtk.Container+ContainerChild
		this.messages = new global::Gtk.TextView();
		this.messages.HeightRequest = 300;
		this.messages.CanFocus = true;
		this.messages.Name = "messages";
		this.messages.Editable = false;
		this.messages.AcceptsTab = false;
		this.messages.WrapMode = ((global::Gtk.WrapMode)(2));
		this.GtkScrolledWindow.Add(this.messages);
		this.vbox2.Add(this.GtkScrolledWindow);
		global::Gtk.Box.BoxChild w7 = ((global::Gtk.Box.BoxChild)(this.vbox2[this.GtkScrolledWindow]));
		w7.Position = 2;
		this.Add(this.vbox2);
		if ((this.Child != null))
		{
			this.Child.ShowAll();
		}
		this.DefaultWidth = 681;
		this.DefaultHeight = 661;
		this.Show();
		this.DeleteEvent += new global::Gtk.DeleteEventHandler(this.OnDeleteEvent);
		this.buttonQuit.Clicked += new global::System.EventHandler(this.OnButtonQuitClicked);
		this.buttonJoin.Clicked += new global::System.EventHandler(this.OnButtonJoinClicked);
		this.drawingarea1.ExposeEvent += new global::Gtk.ExposeEventHandler(this.OnDrawingarea1ExposeEvent);
	}
}
