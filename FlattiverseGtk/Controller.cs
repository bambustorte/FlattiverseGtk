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


            WindowMenu windowMenu = new WindowMenu(client);
            windowMenu.Run();


            window = new WindowMain(this, client);
            window.Show();

        }

        void Connect(String email, String password){
            client = Client.GetInstance(this, email, password);
        }

        public void StartAll(){
            clientThread.Start();
        }

        public void StopAll(){
            client.Stop();
            MainClass.Stop();
        }
    }
}