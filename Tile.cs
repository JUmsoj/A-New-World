using Godot;
using System;

public partial class Tile : Button
{
    private Resources resources;
    public int TotalBalance;
    [Export] Timer daytimer;
    public static constructionProject selected;
    private constructionProject TileResource;
    bool activated;
    public bool confirmed { get; set; }
    private int timeToEstablish = 1;
    void Establish()
    {


        daytimer.Timeout += TileResource.Operate;
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
        if(selected != null) GD.Print(activated ? $"{selected.name} is selected as {Name}'s resource" : $"{selected.name} is deselected as {Name}'s resource");
    }
    
    void OnConfirm()
    {
        TileResource = selected;
        if (activated && selected.InvestResources() == 0)
        {
            GD.Print($"{selected.name} is confirmed as {Name}'s resource");
            
            timeToEstablish = selected.timeToProduce;
           daytimer.Timeout += BuildForOneDay;
        }
    }
}
