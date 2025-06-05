using Godot;
using System;

public partial class Tile : Button
{
    private Resources resources;
    public int TotalBalance;
    [Export] Timer daytimer;
    public static constructionProject selected;
    private constructionProject TileResource = null;
    bool activated;
    public bool Established { get; set; }
    private bool confirmed = false;
    private int timeToEstablish = 1;
   
    void Establish()
    { 
        daytimer.Timeout += TileResource.Operate;
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
    
    void OnConfirm()
    {
        if (activated && selected.InvestResources() == 0 && TileResource != null)
        {
            GD.Print($"{selected.name} is confirmed as {Name}'s resource");
            confirmed = true;
            timeToEstablish = selected.timeToProduce;
            daytimer.Timeout += BuildForOneDay;
        }
    }
    
}
