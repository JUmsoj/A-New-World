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
    private constructionProject TileResource = null;
   
 
    bool activated;
    public Godot.Collections.Dictionary<string, Label> tileStats { get; set; }
    public bool Established { get; set; }
    private bool confirmed = false;
    private int timeToEstablish = 1;
    private bool isSettingsShown = false;

    // status variables 
    [Export] public int multiplier { get; set; }
    public int tileRunning { get; set; }

    void Establish()
    { 
        daytimer.Timeout += () => TileResource.Operate(this);
        GD.Print($"{Name} has been established");
        daytimer.Timeout -= BuildForOneDay;
       
    }
    void BuildForOneDay()
    {
        if (!Established)
        {
            timeToEstablish--;
            
            GD.Print($"{Name} has been built for one day");
            if (timeToEstablish <= 0)
            {
                Establish();
            }
        }
    }
    public void OnButtonPress(bool toggle)
    {
        if (!confirmed && selected != null)
        {
            activated = toggle;
            TileResource = TileResource != null ? null : selected;
            GD.Print(activated ? $"{selected.name} is selected as {Name}'s resource" : $"{selected.name} is deselected as {Name}'s resource");
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
        if (activated && selected.InvestResources() == 0 && TileResource != null)
        {
            GD.Print($"{selected.name} is confirmed as {Name}'s resource");
            confirmed = true;
            timeToEstablish = selected.timeToProduce;
            daytimer.Timeout += BuildForOneDay;
        }
        else
        {
            TileResource = null;
            activated = false;
            confirmed = false;
            GD.Print($"Failed to build on {Name}");
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
