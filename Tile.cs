using Godot;
using System;

public partial class Tile : Button
{
    [Export] Timer daytimer;
    public static Resource selected;
    private Resource TileResource;
    bool activated;
    public bool confirmed { get; set; }
    int time = 1;
    void Establish()
    {
        TileResource = selected;
        daytimer.Timeout += () => GetNode<Resources>("../").Produce(5, TileResource.type);
        GD.Print($"{Name} has been established");

        confirmed = true;
    }
    void BuildForOneDay()
    {
        time--;
        if (time <= 0)
        {
            Establish();
        }
    }
    public void OnButtonPress(bool toggle)
    {
        activated = toggle;
        if(selected != null) GD.Print(activated ? $"{selected.type} is selected as {Name}'s resource" : $"{selected.type} is deselected as {Name}'s resource");
    }
    void OnConfirm()
    {
        if (activated)
        {
            GD.Print($"{selected.type} is confirmed as {Name}'s resource");
            time = 5;
           daytimer.Timeout += BuildForOneDay;
        }
    }
}
