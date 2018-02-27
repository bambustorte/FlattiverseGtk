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
        //if (scannedUnits.Count == 0) {
        //    Scan(scanAngle += 90, 300);
        //    return;
        //}

        //float direction = scannedUnits.ToArray()[0].Position.Angle;
        //Vector v = Vector.FromAngleLength(0, ship.EngineAcceleration.Limit);
        //ship.Move(v);

        //Console.WriteLine(
        //    "\tvangle: " 
        //    + (float)v.Angle 
        //    + "\n\tunitangle: " 
        //    + scannedUnits.ToArray()[0].Position.Angle
        //);

        Scan(scanAngle += 90, 300);
        map.Insert(scannedUnits);

        //this.ScanUpdate += client.GetMessageServer().Run;

        if (ScanUpdate != null)
            ScanUpdate();
    }

    public void Scan(float degree, float range) {
        scannedUnits = ship.Scan(degree, range);
    }

    public List<Unit> GetUnitList (){
            return scannedUnits;
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

