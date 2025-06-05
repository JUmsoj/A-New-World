using Godot;
using System;
using Godot.Collections;

public partial class constructionProject : Button
{
    [Export] public string name { get; set; }
    [Export] public Dictionary<string, int> Cost, Produce;
    [Export] public int timeToProduce;
    private bool isSettingsShown = false;
    public void Operate()
    {
        foreach(var (resource, amount) in Produce)
        {
            if (resource != "balance")
            {
                Resources.Base.Production[resource].Produce(amount);
                GD.Print($"A tile of {name} has succesfully fufilled resource expectations for the day");
            }
            else
            {
                Resources.Base.Balance += amount;
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
    private void ShowSettings()
    {
        if(isSettingsShown)
        {

        }
    }
    public int InvestResources()
    {
        
        ref Dictionary<string, Resource> Production  = ref Resources.Base.Production;
        GD.Print($"{name}'s cost is being paid for a construction project");
        
        int temp = 0;
        foreach(var (resource, amt) in Cost)
        {
            temp++;
            if (resource != "balance")
            {
                if (Production[resource].amount < amt)
                {
                    
                        GD.Print($"Cannot afford {name}");
                        return -1;
                }
                else if (temp == Cost.Count)
                {
                    
                        GD.Print($"Can afford {name}");
                        break;
                }
            }
            else
            {
                if (Resources.Base.Balance < amt)
                {
                    GD.Print($"Cannot afford {name}");
                    return -1;
                }
                else continue;
            }
        }
    
        foreach (var (resource, amount) in Cost)
        {
            Production[resource].Produce(-amount);
        }
        return 0;


      
      
    }
    public void OnClick()
    {
        Tile.selected = this;
    }
}
