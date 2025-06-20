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


    void Establish(constructionProject Project)
    {
        daytimer.Timeout += () => Project.Operate(this);
        GD.Print($"A project of {Project.name} has been established in {Name}");
        toBeBuilt[Project.name] = 0;
        daytimer.Timeout -= () => BuildForOneDay(Project.name);
       
    }
    void BuildForOneDay(string building)
    {
        
            

        timeRemaining[building]--;

         GD.Print($"{Name} has been built for one day");
         if (timeRemaining[building] <= 0)
         {
            Establish(Resources.Base.availableProjects[building]);
         }
    }
    public void OnButtonPress(bool toggle)
    {
        if (selected != null)
        {
            if (!TileResource.TryGetValue(selected.name, out int _)) TileResource[selected.name] = 1;
            else TileResource[selected.name]++;
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
            
            if (child.GetType() == typeof(Label))
            {
                GD.Print($"A Label that states {child.Name} has been found as a status label for a Tile.");

                tileStats[child.Name] = child as Label;

            }
        }
    }
    void OnConfirm()
    {
        foreach (var (building, num) in TileResource)
        {
            if (Resources.Base.availableProjects[building].InvestResources() == 0)
            {
                GD.Print($"{selected.name} is confirmed as {Name}'s resource");
                timeRemaining[building] += Resources.Base.availableProjects[building].timeToProduce * num;
                toBeBuilt[building]++;
                TileResource[building]--;
                
                
                daytimer.Timeout += () => BuildForOneDay(building);
            }
            else
            {
                TileResource = null;
                activated = false;
                confirmed = false;
                GD.Print($"Failed to build on {Name}");
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
        buildingsCurrentlyPresent = [];
    }
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
