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


    public static Client GetInstance(Controller controller, String email, String password) {
        if (instance == null)
            instance = new Client(controller, email, password);
        return instance;
    }

    private Client(Controller controller, String email, String password) {
        try {
            this.controller = controller;
            connector = new Connector("mahait13@hs-esslingen.de", "ichbineinpasswort");
            if(connector == null)
                Console.WriteLine("NULL!!");
            messageServer = new MessageServer(this);
            player = connector.Player;
            playerName = player.Name;
            playerLevel = player.Level;
            map = new Map(this);

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
        ug.Chat("switched state to peaceful");

            while (Client.running) {
                ugfc.PreWait();
                ugfc.Wait();

                try {
                    //Move(new Vector(1, 0));
                    if(TickEvent != null)
                        TickEvent();
                    ugfc.Commit();
                } catch (Exception exception) {
                    Console.WriteLine(exception.Message);
                    Client.running = false;
                }
            }
        }
    }

    public Connector GetConnector() {
        return connector;
    }

    public MessageServer GetMessageServer() {
        return messageServer;
    }

    public void SelectShipAndCreateScanner(String shipName, String modelName) {
        ship = ug.RegisterShip(modelName, shipName);
        scanner = new Scanner(ship, map);
    }

    public List<Flattiverse.Unit> GetScan() {
        return scanner.GetUnitList();
    }

    public UniverseGroup JoinGroup(String name) {
        ug = connector.UniverseGroups[name];
        ug.Join(name, ug.Teams["None"]);
        return ug;
    }

    public void JoinGame() {
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

    public void Stop() {
        Client.running = false;
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

    public void Move(Vector vector){
        vector.Length = ship.EngineAcceleration.Limit;
        ship.Move(vector);
    }

    public Scanner GetScanner(){
        return scanner;
    }

    public Map Map {
        get {
            return map;
        }
    }

    public Ship Ship {
        get {
            return ship;
        }
    }
}