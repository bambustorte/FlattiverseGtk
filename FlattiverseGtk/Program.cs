using System;
using Gtk;

namespace FlattiverseGtk {
    class MainClass {
        public static void Main(string[] args) {
            String email = "";
            String password = "";

            if (args.Length == 2){
                email = args[0];
                password = args[1];
            }
             
            Application.Init();

            Controller controller = new Controller(email, password);

            Application.Run();
        }

        public static void Stop(){
            Application.Quit();
        }
    }
}