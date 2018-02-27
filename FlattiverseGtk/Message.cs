using System;
namespace FlattiverseGtk {
    public class Message : EventArgs {

        String message;

        public Message(String message) {
            this.message = message;
        }
    }
}
