using Godot;
using System;
using Godot.Collections;
using System.Collections.Generic;


public partial class Tile : Button
{
    public string sceneMode;
    private Resources resources;
    public int TotalBalance;
    [Export] Timer daytimer;
    public static constructionProject selected;
    private Godot.Collections.Dictionary<string, int> TileResource, toBeBuilt, timeRemaining;
    [Export] private int maxBuildings;

    [Export] public int Population { get; set; }
    private Godot.Collections.Dictionary<string, Label> tileStats;
    [Export] public  bool activated { get; set; }
    private VBoxContainer tileStatsVBox;

    //Dictionary to contain the buildings currently present in this tile 
    public Godot.Collections.Dictionary<string, int> buildingsCurrentlyPresent;
    private Label buildingCounter;
    public bool Established { get; set; }
    private bool confirmed = false;
    private int timeToEstablish = 1;
    private bool isSettingsShown = false;
    /*private readonly Label example = new Label()
    {
        Text = "Hello",
        Visible = false
    };*/
    

    // status variables 
    [Export] public int multiplier { get; set; }
    public int tileRunning { get; set; }

    [Signal] public delegate void displayDataEventHandler(Tile tile);

    public Godot.Collections.Dictionary<string, Variant> Save()
    {
        return new Godot.Collections.Dictionary<string, Variant>
        {
            {" multiplier", multiplier },
            { "population", Population },
            { "buildings" , buildingsCurrentlyPresent },
            { "InQueue", toBeBuilt },
            { "Times",timeRemaining },
            { "Selected", TileResource },
            { "identifier", "example"}
        };
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
    private void sellResources()
    {
        for (int i = 100; i < Population; i += 100)
        {
            if (Resources.Base.Production["gun"].amount < 50) Population -= 50;
            else Resources.Base.Production["gun"].amount -= 50;
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
        
     
    }
    void BuildForOneDay(string building)
    {


        if (toBeBuilt[building] != 0)
        {
            timeRemaining[building]--;

            GD.Print($"{Name} has been built for one day");
            if (timeRemaining[building] % Resources.Base.availableProjects[building].timeToProduce == 0 && timeRemaining[building] >= 0)
            {
                Establish(Resources.Base.availableProjects[building]);
            }
        }
       
    }
    public void OnButtonPress(bool toggle)
    {
        EmitSignal(SignalName.displayData, this);
        switch (sceneMode) 
        {
            case "economy":
                
                
                if (selected != null && !(Resources.Base.GetParent() as Manager).bulldozingMode)
                {
                    if (!TileResource.TryGetValue(selected.name, out int _)) TileResource[selected.name] = 1;
                    else TileResource[selected.name]++;
                    Modulate = Colors.Green;
                    buildingCounter.Visible = true;
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
                // add division movement code here
                break;
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
                switch(child.Name)
                {
                    case "buildingAmount":
                        buildingCounter = child as Label;
                        buildingCounter.Visible = false;
                        break;
                    

                }
            
        }
    }
    void OnConfirm()
    {
        if (TileResource.Count > 0 && activated)
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
                        
                        if (!toBeBuilt.TryGetValue(building, out int _)) toBeBuilt[building] = 0;
                        toBeBuilt[building]++;

                        TileResource[building]--;
                        Modulate = Colors.Red;

                        
                    }
                    else
                    {
                        GD.Print("Cannot build building, moving on");
                        break;
                    }
                }
                // uses total time for all buildings to be completed as the time stored
                // in the dictionary.
                if (!timeRemaining.TryGetValue(building, out int _)) timeRemaining[building] = 0;
                timeRemaining[building] += Resources.Base.availableProjects[building].timeToProduce * num;
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
        daytimer.Timeout += sellResources;
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
        buildingCounter.Visible = false;
        GD.Print($"{buildingCounter}'s status is {buildingCounter.Visible}");
    }

}
