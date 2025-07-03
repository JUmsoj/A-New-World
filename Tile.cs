using Godot;
using System;
using Godot.Collections;
using System.Collections.Generic;


public partial class Tile : Button
{

    private Resources resources;
    public int TotalBalance;
    [Export] Timer daytimer;
    public static constructionProject selected;
    private Godot.Collections.Dictionary<string, int> TileResource, toBeBuilt, timeRemaining;
    
   
 
    bool activated;
    public Godot.Collections.Dictionary<string, Label> tileStats { get; set; }

    //Dictionary to contain the buildings currently present in this tile 
    public Godot.Collections.Dictionary<string, int> buildingsCurrentlyPresent;
    public bool Established { get; set; }
    private bool confirmed = false;
    private int timeToEstablish = 1;
    private bool isSettingsShown = false;

    // status variables 
    [Export] public int multiplier { get; set; }
    public int tileRunning { get; set; }
    [Export] private List<string> resourceSpecificTileStatuses;

    void Establish(constructionProject Project)
    {
        daytimer.Timeout += () => Project.Operate(this);
        GD.Print($"A project of {Project.name} has been established in {Name}");
        toBeBuilt[Project.name]--;
        if(buildingsCurrentlyPresent.TryGetValue(Project.name, out int _)) buildingsCurrentlyPresent[Project.name]++;
        if (Project.Territories.TryGetValue(Name, out int _)) Project.Territories[Name]++;
        else Project.Territories[Name] = 1;
     
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
        if (selected != null && !(Resources.Base.GetParent() as Manager).bulldozingMode)
        {
            if (!TileResource.TryGetValue(selected.name, out int _)) TileResource[selected.name] = 1;
            else TileResource[selected.name]++;
        }
        else if(!(Resources.Base.GetParent() as Manager).bulldozingMode)
        {
            // code to select which building should be destroyed
        }
        else
        {
            GD.Print("Error : Land Already Occupied or nothing is selected");
        }
    }
    public void setUpStats()
    {
        tileStats = [];
        foreach(var child in GetNode<VBoxContainer>("tileStats").GetChildren())
        {
            GD.Print("Found tileStats");

            if (child.GetType() == typeof(Label) && !resourceSpecificTileStatuses.Contains(child.Name))
            {
                GD.Print($"A Label that states {child.Name} has been found as a status label for a Tile.");

                tileStats[child.Name] = child as Label;
            }
            else
            {
                switch(child.Name)
                {
                    case "Thing":

                        break;
                }
            }
        }
    }
    void OnConfirm()
    {
        if (TileResource.Count > 0)
        {
            foreach (var (building, num) in TileResource)
            {
                for (int i = 0; i < num; i++)
                {
                    if (Resources.Base.availableProjects[building].InvestResources() == 0)
                    {
                        GD.Print($"{selected.name} is confirmed as [one of] {Name}'s resource");
                        
                        if (!toBeBuilt.TryGetValue(building, out int _)) toBeBuilt[building] = 0;
                        toBeBuilt[building]++;

                        TileResource[building]--;


                        
                    }
                    else
                    {
                        break;
                    }
                }
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
            isSettingsShown = !isSettingsShown;
            ShowSettings();
        }
    }
    
    public override void _Ready()
    {
        // sets up Tile collections
        buildingsCurrentlyPresent = [];
        toBeBuilt = [];
        timeRemaining = [];
        resourceSpecificTileStatuses = [];
        TileResource = [];
        // sets up statistics to be displayed when ShowSettings()
        // is called
        setUpStats();
    }
    // displays the tile statistics by setting the Visible property of the TIleStats VBoxCOntainer
    // to true or false
    private void ShowSettings()
    {
        if (isSettingsShown)
        {
            foreach (var (_, label) in tileStats)
            {
                label.Show();
            }
        }
        else
        {
            foreach (var (_, label) in tileStats)
            {
                label.Hide();
            }
        }
    }

}
