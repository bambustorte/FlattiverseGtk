using System;
using System.Threading;
using System.Collections.Generic;
using Flattiverse;

namespace FlattiverseGtk {
    public class MessageServer {
        public delegate void FlattiverseChanged(FlattiverseMessage message);
        public event FlattiverseChanged NewMessageEvent;
        List<FlattiverseMessage> flattiverseMessages = new List<FlattiverseMessage>();
        ReaderWriterLock messageLock = new ReaderWriterLock();
        Client client;


        public MessageServer(Client client) {
            this.client = client;
        }

        public List<FlattiverseMessage> Messages {
            get {
                messageLock.AcquireReaderLock(100);
                List<FlattiverseMessage> listCopy = new List<FlattiverseMessage>(flattiverseMessages);
                messageLock.ReleaseReaderLock();
                return listCopy;
            }
        }

        public void Run(){
            while (Client.running) {
                FlattiverseMessage message;
                messageLock.AcquireWriterLock(100);
                while (client.GetConnector().NextMessage(out message)) {
                    Console.WriteLine(message);
                    flattiverseMessages.Add(message);
                    if (NewMessageEvent != null)
                        NewMessageEvent(message);
                }
                messageLock.ReleaseWriterLock();
            }
        }
    }
}
