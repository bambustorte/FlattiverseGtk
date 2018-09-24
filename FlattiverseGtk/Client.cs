using System;
using Flattiverse;
using System.Collections.Generic;
using FlattiverseGtk;
using System.Threading;

public class Client {
    public static bool running = true;

    Connector connector;
    String playerName;
    int playerLevel;
    Player player;
    Scanner scanner;
    UniverseGroup ug;
    Ship ship;
    static Client instance = null;
    Controller controller;
    MessageServer messageServer;
    Map map;
    Thread messageServerThread;
    public delegate void TickHandler();
    public event TickHandler TickEvent;
    bool canMove = true;
    Vector moveVector;
    Vector stillVector;
    public long ticks = (long.MinValue+100);
    List<Probe> probes = new List<Probe>();


    public static Client GetInstance(Controller controller, String email, String password) {
        if (instance == null)
            instance = new Client(controller, email, password);
        return instance;
    }

    private Client(Controller controller, String email, String password) {
        try {
            this.controller = controller;
            connector = new Connector(email, password);
            if(connector == null)
                Console.WriteLine("NULL!!");
            messageServer = new MessageServer(this);
            player = connector.Player;
            playerName = player.Name;
            playerLevel = player.Level;
            map = new Map(this);

            moveVector = new Vector();
            stillVector = new Vector();

            messageServerThread = new Thread(messageServer.Run);
            messageServerThread.Name = "messageServerThread";
            messageServerThread.Start();

            foreach (Player p in connector.Players)
                if (p.Name == "Towelie")
                    p.Chat("peter");

        } catch (Exception e) {
            Console.WriteLine(e.ToString());
            Stop();
            return;
        }
        Console.WriteLine("connected");
    }

    public override string ToString() {
        return string.Format(connector.Player.Name);
    }

    public void MainLoop() {
        if (connector == null)
            return;

        using(UniverseGroupFlowControl ugfc = ug.GetNewFlowControl()){
            //ug.Chat("switched state to peaceful");

             

            while (Client.running) {
                ugfc.PreWait();
                ugfc.Wait();

                try {
                    Move();
                    if(TickEvent != null)
                        TickEvent();
                    ticks++;
                    ugfc.Commit();
                    canMove = true;
                } catch (Exception exception) {
                    try {
                        Console.WriteLine(exception.Message);
                        if(!ship.IsAlive || ShipEnergyPercent < 0.5){
                            try{
                                ship.Kill();
                            }catch{}
                            ship.Continue();
                        }
                    }catch (Exception e){
                        Console.WriteLine(e.Message);
                    }
                }
            }
        }
    }

    public Connector Connector {
        get {
            return connector;
        }
    }

    public MessageServer MessageServer {
        get {
            return messageServer;
        }
    }

    public void BuildProbe(){
        Probe d = (Probe)ship.Build("sonde1", "test" + ((new Random()).Next()), 0);
        probes.Add(d);
    }

    public void SelectShipAndCreateScanner(String shipName, String modelName) {
        ship = ug.RegisterShip(modelName, shipName);
        scanner = new Scanner(ship, map);
    }

    public List<Flattiverse.Unit> GetScan() {
        return scanner.UnitList;
    }

    public UniverseGroup JoinGroup(String name) {
        ug = connector.UniverseGroups[name];
        if (name.Equals("DOM II")) {
            ug.Join(name, ug.Teams["Signalling Orange"]);
            return ug;
        }
        if (name.Equals("DOM I")) {
            ug.Join(name, ug.Teams["Blue"]);
            return ug;
        }
            
        ug.Join(name, ug.Teams["None"]);
        return ug;
    }

    public Team[] GetTeams (String universeGr){
        return (connector.UniverseGroups[universeGr].Teams).List.ToArray();
    }

    public void ShipContinue() {
        ship.Continue();
    }

    public List<UniverseGroup> GetUniverseGroups() {
        List<UniverseGroup> groups = new List<UniverseGroup>();
        foreach (UniverseGroup gr in connector.UniverseGroups) {
            groups.Add(gr);
        }
        return groups;
    }

    public List<ControllableDesign> Designs {
        get {
            return connector.QueryDesigns();
        }
    }

    public int ShotsAvailable {
        get {
            return (int)ship.WeaponProductionStatus;
        }
    }

    public void Shoot(Vector direction){
        
        if (ShotsAvailable > 0) {
            
            float maxTime = ship.WeaponShot.Time.Limit;
            float maxSpeed = ship.WeaponShot.Speed.Limit;
            float shootLimit = maxTime * maxSpeed;
            //float needTime = direction.Length / maxSpeed;
            direction.Length = maxSpeed;

            ship.Shoot(direction, (int)maxTime);

        }
    }

    public void Stop() {
        Client.running = false;
        MessageServer.running = false;
        if (connector != null) {
            connector.Close();
        }
        controller.StopAll();
    }

    internal void GetMessages(FlattiverseMessage flattiverseMessage) {
        connector.NextMessage(out flattiverseMessage);
    }

    public float GetShipSize() {
        return (ship == null) ? 0 : ship.Radius;
    }

    public void SetMoveVector(Vector v){
        this.moveVector = v;
        moveVector.Length = ship.EngineAcceleration.Limit;
    }

    public void Move(){
        if (!canMove)
            return;
        canMove = false;
        ship.Move(moveVector);
    }

    public Scanner Scanner {
        get {
            return scanner;
        }
    }

    public Map Map {
        get {
            return map;
        }
    }

    public List<Unit> Units {
        get {
            return map.Units;
        }
    }

    public Ship Ship {
        get {
            return ship;
        }
    }

    public float ShipEnergyPercent {
        get {
            return (ship.Energy / ship.EnergyMax) * 100;
        }
    }
}