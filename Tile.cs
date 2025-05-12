using Godot;
using System;

public partial class Tile : Button
{
    public int TotalBalance;
    [Export] Timer daytimer;
    public static Resource selected;
    private Resource TileResource;
    bool activated;
    public bool confirmed { get; set; }
    private int timeToEstablish = 1;
    void Establish()
    {
        
        TileResource = selected;

        daytimer.Timeout += TileResource.Produce;
        GD.Print($"{Name} has been established");

        confirmed = true;
    }
    void BuildForOneDay()
    {
        if (!confirmed)
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
        activated = toggle;
        if(selected != null) GD.Print(activated ? $"{selected.type} is selected as {Name}'s resource" : $"{selected.type} is deselected as {Name}'s resource");
    }
    void OnConfirm()
    {
        if (activated && selected.Pay() == 0)
        {
            GD.Print($"{selected.type} is confirmed as {Name}'s resource");
            
            timeToEstablish = 5;
           daytimer.Timeout += BuildForOneDay;
        }
    }
}
