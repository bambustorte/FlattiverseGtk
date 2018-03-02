using System;
using Flattiverse;
using System.Collections.Generic;
using System.Threading;

namespace FlattiverseGtk {
    public class Map {
        Dictionary<string, Unit> mapUnits = new Dictionary<string, Unit>();
        ReaderWriterLock listLock = new ReaderWriterLock();
        Client client;

        public Map(Client client) {
            this.client = client;
            //this.client.TickEvent += CleanList;
        }

        public void Insert(List<Unit> units) {
            listLock.AcquireWriterLock(100);
            foreach (Unit u in units) {
                mapUnits[u.Name] = u;
            }
            listLock.ReleaseWriterLock();
        }

        public List<Unit> Units {
            get {
                listLock.AcquireReaderLock(100);
                List<Unit> units = new List<Unit>(mapUnits.Values);
                listLock.ReleaseReaderLock();
                return units;
            }
        }

        public void CleanList(){
            List<String> ToDelete = new List<String>();

            foreach(Unit u in Units){
                if (((Tag)u.Tag).TickCreatedTimestamp < client.ticks - 5)
                    ToDelete.Add(u.Name);
            }

            foreach(String s in ToDelete){
                listLock.AcquireWriterLock(100);
                mapUnits.Remove(s);
                listLock.ReleaseWriterLock();
            }
        }

        //public void MoveAway(){
        //    if (UnitList.Count == 0)
        //        return;
        //
        //    Unit unit = UnitList[0];
        //
        //    foreach(Unit u in this.UnitList){
        //        if (unit.Position.Length > u.Position.Length)
        //            unit = u;
        //    }
        //
        //    client.SetMoveVector(-unit.Position);
        //}
    }
}
