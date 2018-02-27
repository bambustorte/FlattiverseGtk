using System;
using Flattiverse;
using System.Collections.Generic;

namespace FlattiverseGtk {
    public class Map {
        List<Unit> units;

        public Map() {
            units = new List<Unit>();
        }
        public void Insert(List<Unit> scannedUnits){
            foreach(Unit nU in scannedUnits){
                bool contains = false;
                foreach(Unit oU in units){
                    if(nU.Name == oU.Name){
                        contains = true;

                    }
                }

                if(!contains){
                    units.Add(nU);
                }
            }
        }
    }
}
