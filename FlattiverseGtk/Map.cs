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
            //client.TickEvent += MoveAway;
        }

        //public void Insert(List<Unit> scannedUnits){
        //    foreach(Unit nU in scannedUnits){
        //        bool contains = false;
        //
        //        listLock.AcquireWriterLock(100);
        //        for (int i = 0; i < units.Count; i++){
        //            if(nU.Name == units[i].Name){
        //                contains = true;
        //                units[i] = nU;
        //            }
        //        }
        //
        //        if(!contains){
        //            units.Add(nU);
        //        }
        //        listLock.ReleaseWriterLock();
        //    }
        //}

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
