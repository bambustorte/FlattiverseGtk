using System;
using Gtk;
using System.Threading;

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
            Thread main = new Thread(controller.Run);
            main.Start();

            try {
                Application.Run();
            }catch{}
        }

        public static void Stop(){
            Application.Quit();
        }
    }
}