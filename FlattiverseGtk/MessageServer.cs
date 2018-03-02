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
        public static bool running = true;


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

        public void Run() {
            while (MessageServer.running) {
                FlattiverseMessage message;
                while (client.Connector.NextMessage(out message)) {
                    messageLock.AcquireWriterLock(100);
                    flattiverseMessages.Add(message);
                    messageLock.ReleaseWriterLock();
                    Console.WriteLine(message);
                    if (NewMessageEvent != null)
                        NewMessageEvent(message);
                }

            }
        }
    }
}
