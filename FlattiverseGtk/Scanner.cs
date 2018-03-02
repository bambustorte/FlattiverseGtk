using System;
using System.Collections.Generic;
using Flattiverse;
using FlattiverseGtk;

//fixme could also scan nothing at all
public class Scanner {
    public delegate void ScanResults();
    public event ScanResults ScanUpdate;

    Ship ship;
    List<Unit> scannedUnits;
    int scanAngle = 0;
    Map map;
    Client client;

    public Scanner(Ship ship, Map map) {
        this.ship = ship;
        this.map = map;
        scannedUnits = new List<Unit>();
        this.client = Client.GetInstance(null, "", "");
        client.TickEvent += Scan;
    }

    public void Scan() {
        Scan(scanAngle += 90, 300);

        foreach (Unit u in scannedUnits) {
            u.Tag = new Tag(client.ticks);
        }

        map.CleanList();
        map.Insert(scannedUnits);

        if (ScanUpdate != null)
            ScanUpdate();
    }

    public void Scan(float degree, float range) {
        scannedUnits = ship.Scan(degree, range);
    }

    public List<Unit> UnitList {
        get {
            return scannedUnits;
        }
    }

    public override String ToString() {
        String units = "";

        foreach (Unit unit in scannedUnits) {
            units += "\n[" + unit.Kind + "] ";
            units += "<" + unit.Name + ">";
            units += "(" + unit.Position.X + ", " + unit.Position.Y + ")";
            units += ", " + unit.Radius;
        }

        return units;
    }
}

