using Godot;
using System;
using Godot.Collections;
using System.Reflection.Metadata.Ecma335;
// Resources cannot be produced directly, a blueprint for a 
// facility that produces said resource must be built
// the class for it is defined below
public partial class constructionProject : Button
{
    
    [Export] public string name { get; set; }
    [Export] public Dictionary<string, int> Cost, Produce;
    [Export] public int timeToProduce;
    public Dictionary<string, int> Territories { get; set; }



    // simulates one operating day of a given facility that follows this blueprint
    // will update the stats accordingly
    public override void _GuiInput(InputEvent @event)
    {
        if(@event is InputEventMouseButton eventmousebutton)
        {
            if(eventmousebutton.ButtonIndex == MouseButton.Right && eventmousebutton.Pressed)
            {
                Manager.instance.fillInData(this);
            }
        }
    }
    public void Operate(Tile tile)
    {
        if (tile != null)
        {
            // update tileStats variables below:
            tile.tileRunning++;
            // edit it on labels below:


            if (Territories[tile.Name] != 0 && tile.activated)
            {
                //  changes the amounts  of the products that will be produced this day. including pounds
                foreach (var (resource, amount) in Produce)
                {
                    int newAmount = tile.multiplier != 0 ? tile.multiplier * amount : amount;
                    // if it isnt pounds use the regular Produce() function that uses
                    // the dictionary Production to get the Resources and Labels
                    if (resource != "balance")
                    {
                        Resources.Base.Production[resource].Produce(newAmount);
                        GD.Print($"A tile of {tile.Name} has succesfully fufilled resource expectations for the day");
                    }
                    // uniquely handles pounds with the SpendPounds function
                    else
                    {
                        Resources.Base.SpendPounds(-newAmount);
                    }
                }
            }
        }
    }
    public override void _Ready()
    {
        Territories = [];
    }
    public int InvestResources()
    {
        // creates the necessary references to dictionary 
        ref Dictionary<string, Resource> Production  = ref Resources.Base.Production;
        GD.Print($"{name}'s cost is being paid for a construction project");
        
       
        // loops through the Cost dictionary
        foreach(var (resource, amt) in Cost)
        {
           
            // if the resource isnt pounds; use the Production dictionary.
            if (resource != "balance")
            {
                // if there is even one type of resource that the total supply doesnt meet
                // the construction costs, it will be scrapped entirely
                if (Production[resource].amount < amt)
                {
                    
                        GD.Print($"Cannot afford {name}");
                        return -1;
                }
                // if it is the final iteration, it will go through to the next one;
                continue;
            }
            // if it is balance, use the SpendPounds function to 
            // get the variable and perform the necssary updates on the Labels
            else
            {
                if (!Resources.Base.SpendPounds(amt, false))
                {
                    GD.Print($"Cannot afford {name}");
                    return -1;
                }
                else continue;
            }
        }
        // it invests the respective resources into the project,
        // depleting them from the total supply
    
        foreach (var (resource, cost) in Cost)
        {
            if (resource != "balance") Production[resource].ChangeSupply(ResourceActions.Spend, cost);
            else Resources.Base.SpendPounds(cost);
            
        }
        GD.Print($"Succesfully invested resources in {name}");
        return 0;


      
      
    }
    // self explanatory
    public void OnClick()
    {
        Tile.selected = this;
        GetTree().CallGroup("States", "OnSelect");
        Manager.instance.fillInData(this);
        GD.Print($"{Name} has been clicked and selected");
    }
}
