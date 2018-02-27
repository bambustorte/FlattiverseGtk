using System;
using System.Windows.Forms;


namespace FlattiverseGtk {
    public class WindowMainForms : System.Windows.Forms.Form {

        System.Windows.Forms.Panel panel1;
        System.Windows.Forms.Label label1;

        public WindowMainForms() {

            this.Name = "Flattiverse";

            panel1 = new Panel();
            label1 = new Label();

            this.panel1.Controls.Add(this.label1);
            this.panel1.Cursor = System.Windows.Forms.Cursors.Default;
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(472, 32);
            this.panel1.TabIndex = 14;
            this.label1.Location = new System.Drawing.Point(172, 5);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(128, 23);
            this.label1.TabIndex = 0;
            this.label1.Text = "Panel.Dock = Top";

        }

    }
}
