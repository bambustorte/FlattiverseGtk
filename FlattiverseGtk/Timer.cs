using System;
using System.Threading;

namespace FlattiverseGtk {
    public class Timer {
        public delegate void GraphicsChanged();
        public event GraphicsChanged GraphicsUpdate;


        public Timer() {

        }

        public void Sleep(){
            while (Client.running) {
                System.Threading.Thread.Sleep(1000);
                GraphicsUpdate();
            }
        }
    }
}
