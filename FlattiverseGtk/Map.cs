using System;
using Flattiverse;
using System.Collections.Generic;
using System.Threading;

namespace FlattiverseGtk {
    public class Map {
        List<Unit> units;
        ReaderWriterLock listLock = new ReaderWriterLock();

        public Map() {
            units = new List<Unit>();
        }

        public void Insert(List<Unit> scannedUnits){
            foreach(Unit nU in scannedUnits){
                bool contains = false;

                listLock.AcquireWriterLock(100);
                for (int i = 0; i < units.Count; i++){
                    if(nU.Name == units[i].Name){
                        contains = true;
                        units[i] = nU;
                    }
                }

                if(!contains){
                    units.Add(nU);
                }
                listLock.ReleaseWriterLock();
            }
        }

        public List<Unit> UnitList {
            get {
                listLock.AcquireReaderLock(100);
                List<Unit> listCopy = new List<Unit>(units);
                listLock.ReleaseReaderLock();
                return listCopy;
            }

            set {
                listLock.AcquireWriterLock(100);
                units = value;
                listLock.ReleaseWriterLock();
            }
        }
    }
}
