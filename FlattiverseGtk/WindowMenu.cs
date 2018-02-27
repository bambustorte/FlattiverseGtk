using System;
using Gtk;
using Flattiverse;
using System.Collections.Generic;

namespace FlattiverseGtk {
    public partial class WindowMenu : Gtk.Dialog {
        Client client;
        ComboBox designsDropdown;
        ComboBox universesDropdown;

        public WindowMenu(Client client) {
            this.client = client;

            Modal = true;

            Entry name = new Entry("myShip");
            VBox.Add(name);


            ////////////////////////
            List<String> designEntries = new List<string>();
            foreach(ControllableDesign c in client.Designs) {
                designEntries.Add(c.Name);
            }
            designsDropdown = new ComboBox(designEntries.ToArray());
            if (designEntries.Count > 0)
                designsDropdown.Active = 0;
            VBox.Add(designsDropdown);
            ////////////////////////


            ////////////////////////
            List<String> universeEntries = new List<string>();
            foreach (UniverseGroup c in client.GetUniverseGroups()) {
                universeEntries.Add(c.Name);
            }
            universesDropdown = new ComboBox(universeEntries.ToArray());
            if (universeEntries.Count > 0)
                universesDropdown.Active = 0;
            VBox.Add(universesDropdown);
            ////////////////////////


            Button b = new Button("Select");
            b.Clicked += (sender, e) => {
                client.JoinGroup(universesDropdown.ActiveText);
                client.SelectShipAndCreateScanner(name.Text, designsDropdown.ActiveText);
                Destroy();
            };
            VBox.Add(b);

            ShowAll();
        }
    }
}
