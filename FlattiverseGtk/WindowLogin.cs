using System;
using Gtk;

namespace FlattiverseGtk {
    public partial class WindowLogin : Gtk.Dialog {

        public static String email = "";
        public static String password = "";

        public WindowLogin(String defEmail, String defPass) {
            Modal = true;

            VBox v = VBox;

            Entry entryEmail = new Entry(defEmail);
            v.Add(entryEmail);

            Entry entryPassword = new Entry(defPass);
            entryPassword.Visibility = false;
            v.Add(entryPassword);

            Button b = new Button("login");
            b.Clicked += (sender, ev) => {
                email = entryEmail.Text;
                password = entryPassword.Text;
                Destroy();
            };
            v.Add(b);

            Add(v);
            ShowAll();
        }
    }
}
