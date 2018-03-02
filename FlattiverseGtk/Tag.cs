using System;
namespace FlattiverseGtk {
    public class Tag {

        long tickstamp;

        public Tag(long ticks) {
            tickstamp = ticks;
        }

        public long TickCreatedTimestamp {
            get{
                return tickstamp;
            }
        }
    }
}
