using System;
using Flattiverse;
using System.Threading;
using System.IO;

namespace FlattiverseGtk {
    public class Controller {

        Client client;
        WindowMain window;
        Thread clientThread;

        public Controller(String email, String password) {
            
            WindowLogin windowLogin = new WindowLogin(email, password);
            windowLogin.Run();

            if (!File.Exists("benchmark.bin")) {
                PerformanceMark newBenchmark = Connector.DoBenchmark();
                File.WriteAllBytes("benchmark.bin", Connector.SaveBenchmark());
            } else {
                Connector.LoadBenchmark(File.ReadAllBytes("benchmark.bin"));
            }

            Connect(WindowLogin.email, WindowLogin.password);


            clientThread = new Thread(client.MainLoop);
            clientThread.Name = "clientThread";

            WindowMenu windowMenu = new WindowMenu(client);
            windowMenu.Run();

            try {
                //System.Windows.Forms.Application.Run(new WindowForms(this, client));
                System.Windows.Forms.Application.Run(new WindowFullscreen(this, client));
            }catch{}


            //window = new WindowMain(this, client);

        }

        public void Run(){
            if (client == null)
                return;
            //window.Show();
        }

        void Connect(String email, String password){
            client = Client.GetInstance(this, email, password);
        }

        public void StartAll(){
            clientThread.Start();
        }

        public void StopAll(){
            System.Windows.Forms.Application.Exit();
            if (client != null)
                client.Stop();
            MainClass.Stop();
        }
    }
}