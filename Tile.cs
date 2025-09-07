using Godot;
using System;
using Godot.Collections;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Security.Cryptography.X509Certificates;


public partial class Tile : Button
{

    [Export] public int qualityOfLife { get; set; }
    public string sceneMode;
    private Resources resources;
    public int TotalBalance;
    [Export] Timer daytimer;
    public static constructionProject selected;
    private Godot.Collections.Dictionary<string, int> TileResource, toBeBuilt, timeRemaining;
    [Export] private int maxBuildings;
   

    [Export] public int Population { get; set; }
    private Godot.Collections.Dictionary<string, Label> tileStats;
    [Export] public bool activated { get; set; } = true;
    private VBoxContainer tileStatsVBox;

    //Dictionary to contain the buildings currently present in this tile 
    public Godot.Collections.Dictionary<string, int> buildingsCurrentlyPresent;
    private Label buildingCounter;
    public bool Established { get; set; }
    private bool confirmed = false;
    private int timeToEstablish = 1;
    private bool isSettingsShown = false;
    [Export] Godot.Collections.Dictionary<string, int> NaturalResources;
    /*private readonly Label example = new Label()
    {
        Text = "Hello",
        Visible = false
    };*/
    

    // status variables 
    [Export] public int multiplier { get; set; }
    public int tileRunning { get; set; }

    [Signal] public delegate void displayDataEventHandler(Tile tile);


    public override void _UnhandledInput(InputEvent @event)
    {
        if(@event is InputEventKey keyevent)
        {
            if(keyevent.Keycode == Key.Space && keyevent.Pressed && sceneMode == "war")
            {
                Military.SelectedDivisionType = null;
            }
        }
    }
    public override void _GuiInput(InputEvent @event)
    {
        
        
        if (Manager.instance.isJustPressed(@event, MouseButton.Right, out InputEventMouseButton _))
        {
            
            
                GD.Print($"Right clicked on {Name}");
                switch (sceneMode) {
                    case "war":
                       
                        Manager.instance.fillInData(this);
                        break;
                    case "economy":
                        Update(toBeBuilt, buildingsCurrentlyPresent, selected);
                        break;
                }
            
        }
    }
   
    public void Move(string type, Tile destination)
    {
        int timeToMove = 5;
        int day = Manager.instance.days;
        Divisions[type]--;
        GD.Print($"One division of type {type} is moving from {this.Name} to {destination.Name}");
        Manager.instance.DayPassed += (int days) =>
        {
            if(days == day + timeToMove)
            {
                destination.Divisions[type]++;
            }
        };
        
    }
        
    
    public Godot.Collections.Dictionary<string, Variant> Save()
    {
        var dict = new Godot.Collections.Dictionary<string, Variant>
        {
            {" multiplier", multiplier },
            { "population", Population },
            { "buildings" , buildingsCurrentlyPresent },
            { "InQueue", toBeBuilt },
            { "Times",timeRemaining },
            { "Selected", TileResource },
            { "identifier", "example"},
            { "X", GlobalPosition.X },
            { "Y", GlobalPosition.Y },
            {"sceneMode", sceneMode },
            { "name", Name },
            { "Divisions", Divisions }
        };
        return dict;
    }
    public bool EngageWIthNatives(int nativeAttack, int nativeDefence)
    {
        double friendlyAttack = (0.25 * Divisions["Infantry"]);
        double friendlyDefence = 0.5 * Divisions["Infantry"];
        if (friendlyAttack < nativeAttack || friendlyDefence < nativeAttack) return false;
        return true;
    }
    private Godot.Collections.Dictionary<string, Variant> GetAllConnections()
    {
        Godot.Collections.Dictionary<string, Variant> dict = [];
        foreach(var signal in GetSignalList())
        {
            if (GetSignalConnectionList(signal["name"].As<string>()).Count != 0)
            dict[(string)signal["name"]] = GetSignalConnectionList((string)signal["name"]);
        }
        return dict;
    }
    public void NativeRebellion()
    {
        if (!Divisions.ContainsKey("Infantry")) Divisions["Infantry"] = 0;

       
        bool result;
        var rng = new RandomNumberGenerator();
        int num = rng.RandiRange(0, 10);
        if(num < 5)
        {
            result = EngageWIthNatives(5, 5);
        }
        else if(num > 5 && num < 8)
        {
            result = EngageWIthNatives(5, 5);
        }
        else
        {
            return;
        }
        if (result && Divisions["Infantry"] > 0) Divisions["Infantry"]--;

        else if(activated)
        {
            Population -= 50;
            activated = false;
            return;
        }
    }
    private int amountOfBuildings()
    {
        
        int total = 0;
        foreach(var building in buildingsCurrentlyPresent)
        {
            total += building.Value;
        }
        return total;
    }
    public void sellResources()
    {
        if (activated)
        {
            for (int i = 100; i < Population; i += 100)
            {
                foreach (var (need, amount) in Manager.instance.popNeeds)
                {
                    if (Resources.Base.Production[need].amount < amount)
                    {
                        qualityOfLife -= 10;
                        break;

                    }
                    else if (Resources.Base.Production[need].amount - amount > 100)
                    {
                        qualityOfLife++;
                    }
                    Resources.Base.Production[need].ChangeSupply(ResourceActions.Spend, 50);
                }
            }
            if (qualityOfLife <= 0)
            {
                Population -= 10;
            }
            if (Population <= 0)
            {
                activated = false;
                GD.Print($"{Name} is now deactivated");
            }

            else
            {
                if (!activated)
                {
                    activated = true;
                    GD.Print($"{Name} is now deactivated");
                }
            }
            Manager.instance.fillInData(this);
        }
    }
    private bool checkIfAllConstructionsAreComplete()
    {
        foreach(var val in toBeBuilt.Values)
        {
            if(val > 0)
            {
                return false;
            }
        }
        return true;
    }

    // removes one of type Project from the building queue and into the actual Tile.
    // hence it "Establishes" a building.
    void Establish(constructionProject Project)
    {
        daytimer.Timeout += () => Project.Operate(this);
        GD.Print($"A project of {Project.name} has been established in {Name}");
        toBeBuilt[Project.name]--;
        if (buildingsCurrentlyPresent.TryGetValue(Project.name, out int _)) buildingsCurrentlyPresent[Project.name]++;
        else buildingsCurrentlyPresent[Project.name] = 1;
        if (Project.Territories.TryGetValue(Name, out int _)) Project.Territories[Name]++;
        else Project.Territories[Name] = 1;
        if (checkIfAllConstructionsAreComplete()) Modulate = Colors.Black;
        Update(toBeBuilt, buildingsCurrentlyPresent, selected);

    }
    void BuildForOneDay(string building)
    {

        
            if (toBeBuilt.TryGetValue(building, out int val) && val >= 0)
            {
                timeRemaining[building]--;

                GD.Print($"{Name} has been built for one day");
                if (timeRemaining[building] % Resources.Base.availableProjects[building].timeToProduce == 0)
                {
                    Establish(Resources.Base.availableProjects[building]);
                }
            }


        
    }
    private void Update(Godot.Collections.Dictionary<string, int> trackingVar, Godot.Collections.Dictionary<string, int> Present, constructionProject selected)
    {
        buildingCounter.Text = $"{selected.name} details: ({trackingVar[selected.name]}){(Present.TryGetValue(selected.name, out int val) ? buildingsCurrentlyPresent[selected.name] : val)}/{maxBuildings}";

    }
    private void Update(Godot.Collections.Dictionary<string, int> trackingVar, constructionProject selected)
    {
        buildingCounter.Text = trackingVar.TryGetValue(selected.name, out int _) ? $"{selected.name} details: {trackingVar[selected.name]}/{maxBuildings}" : $"0/{maxBuildings}";
    }
    public void OnButtonPress(bool toggle)
    {
        EmitSignal(SignalName.displayData, this);
        GD.Print("Pressed");
        switch (sceneMode)
        {
            case "economy":
                
                
                if (selected != null && !(Resources.Base.GetParent() as Manager).bulldozingMode)
                {
                    if (!TileResource.TryGetValue(selected.name, out int _)) TileResource[selected.name] = 1;
                    else TileResource[selected.name]++;
                    Update(TileResource, buildingsCurrentlyPresent, selected);

                    var newBox = GetThemeStylebox("normal").Duplicate() as StyleBoxFlat;
                    newBox.BgColor = Colors.Green;
                    AddThemeStyleboxOverride("normal", newBox);
                    
                    
                }
                else if (!(Resources.Base.GetParent() as Manager).bulldozingMode)
                {
                    // code to select which building should be destroyed
                }
                else
                {
                    GD.Print("Error : Land Already Occupied or nothing is selected");
                }
                break;
            case "war":
                var div = Military.SelectedDivisionType;
                foreach (var (good, cost) in Manager.instance.DivisionCosts[div])
                {
                    if (cost > Resources.Base.Production[good].amount)
                    {
                        return;
                    }
                    else
                    {
                        continue;
                    }
                }
                
                foreach (var (good, cost) in Manager.instance.DivisionCosts[div]) {
                    Resources.Base.Production[good].ChangeSupply(ResourceActions.Spend, cost);
                }
                if (DivisionsInTraining.TryGetValue(div, out int _)) DivisionsInTraining[div]++;
                else DivisionsInTraining[div] = 1;
                PackedScene div_tangible = GD.Load<PackedScene>("res://Unit.tscn");
                var division = div_tangible.Instantiate<Military>();
                
                division.tileExistsIn = this;
                division.Position = new Vector2(196, 200);
                division.MoveToFront();
                division.Name = "hi";
                division.Visible = true;
                division.Type = div;
                FindParent("WarScene").AddChild(division);
                
                GD.Print("Training soldiers");
                daytimer.Timeout += TrainOneDivision;
                break;

        }
        
    }
    private void TrainOneDivision()
    {
        var div = Military.SelectedDivisionType;
        if (DivisionsInTraining[div] > 0) {
            for (int i = 0; i < DivisionsInTraining[div]; i++)
            {
                if (Divisions.TryGetValue(div, out int _)) Divisions[div]++;
                else Divisions[div] = 1;
                
                GD.Print($"Divisions in {Name}: {Divisions[div]}");
                DivisionsInTraining[div]--;
            }

        }
        else
        {
            daytimer.Timeout -= TrainOneDivision;
        }
    }
    // use this for tile data that is displayed within a child of the Tile Node itself.
    // add another case for the switch function and the name of the Label has to match the one 
    // on the case
    public void setUpStats()
    {
        
        foreach(var child in GetNode<VBoxContainer>("tileStats").GetChildren())
        {
            GD.Print("Found tileStats");

            // use this for stats that are located inside the Tile button itself
                /*switch(child.Name)
                {
                    case "buildingAmount":
                        buildingCounter = child as Label;
                        buildingCounter.Visible = false;
                        break;
                    

                }*/
            
        }
    }
    private bool isAnythingToBeBuilt()
    {
        foreach(var (good, amt) in TileResource)
        {
            if (amt > 0)
            {
                return true;
            }
        }
        return false;
    }
    void OnConfirm()
    {
        GD.Print("selection Confirmed");
        bool prematurelyEnded = false;
        int amount = 0;
            if (isAnythingToBeBuilt() && activated)
            {
                // iterates through the types buildings currently selected to be built

                foreach (var (building, num) in TileResource)
                {
                    // the number of buildings to be built
                    for (int i = 0; i < num; i++)
                    {
                        if (Resources.Base.availableProjects[building].InvestResources() == 0 && amountOfBuildings() < maxBuildings)
                        {
                            GD.Print($"{selected.name} is confirmed as [one of] {Name}'s resource");

                            if (!toBeBuilt.ContainsKey(building)) toBeBuilt[building] = 0;
                            toBeBuilt[building]++;
                            GD.Print(toBeBuilt[building]);
                            
                            TileResource[building]--;

                            AddThemeStyleboxOverride("hover", new StyleBoxFlat()
                            {
                                BgColor = Colors.Red
                            });


                        }
                        else
                        {
                            GD.Print("Cannot build building, moving on");
                            prematurelyEnded = true;
                            amount = i;
                            break;
                        }
                    }
                    // uses total time for all buildings to be completed as the time stored
                    // in the dictionary.
                    if (!timeRemaining.TryGetValue(building, out int _)) timeRemaining[building] = 0;
                    timeRemaining[building] += Resources.Base.availableProjects[building].timeToProduce * (!prematurelyEnded ? num : amount);
                    daytimer.Timeout += () => BuildForOneDay(building);
                }
            }
        
        
    }
    public override void _Process(double delta)
    {
        if (IsHovered() && Input.IsActionJustPressed("showTileInfo"))
        {



        }
    }
   
       
    public void RandomRebellion(int days)
    {
        
            if (days % 5 == 0)
            {
                NativeRebellion();
            }
        
    }
    public void ExtractResources()
    {
        foreach(var ( resource, amount) in NaturalResources)
        {
            Resources.Base.Production[resource].ChangeSupply(ResourceActions.Produce, amount);
        }
    }
    public override void _Ready()
    {
        
        // sets up Tile collections
        buildingsCurrentlyPresent = [];
        toBeBuilt = [];
        
        timeRemaining = [];
        //displayData += Manager.instance.fillInData;
        TileResource = [];
        // tileStatsVBox = GetTree().Root.GetNode<VBoxContainer>("tileInfo");
        // sets up statistics to be displayed when ShowSettings()
        // is called
        setUpStats();
        

        // tileStats["example"] = example;

        // GD.Print(tileStats["example"]);

        // GD.Print(tileStats["population"].Text);

        // daytimer.Timeout += sellResources;
        DivisionsInTraining = [];
        Divisions = [];
    }
    // displays the tile statistics by setting the Visible property of the TIleStats VBoxCOntainer
    // to true or false
    /*private void ShowSettings()
    {
        if (tileStats.TryGetValue("population", out Label label)) label.Text = $"Population: {Population}";
        else GD.Print($"Key population Does not exist");
    }*/
    public void OnSelect()
    {

        buildingCounter.Visible = true;
        buildingCounter.Text = buildingsCurrentlyPresent.TryGetValue(selected.name, out int _) ? $"{buildingsCurrentlyPresent[selected.name]}/{maxBuildings}" : $"0/{maxBuildings}";
    }
    public void OnDeselect()
    {
        /*buildingCounter.Visible = false;
        GD.Print($"{buildingCounter}'s status is {buildingCounter.Visible}");*/
    }

}
