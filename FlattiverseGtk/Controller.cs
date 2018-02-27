using System;
using Flattiverse;
using System.Threading;

namespace FlattiverseGtk {
    public class Controller {

        Client client;
        WindowMain window;
        Thread clientThread;

        public Controller(String email, String password) {
            
            WindowLogin windowLogin = new WindowLogin(email, password);
            windowLogin.Run();

            Connect(WindowLogin.email, WindowLogin.password);


            clientThread = new Thread(client.MainLoop);
            clientThread.Name = "clientThread";

            if (client == null)
                return;

            WindowMenu windowMenu = new WindowMenu(client);
            windowMenu.Run();

            try {
                System.Windows.Forms.Application.Run(new WindowForms(this, client));
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
            if (client != null)
                client.Stop();
            MainClass.Stop();
        }
    }
}