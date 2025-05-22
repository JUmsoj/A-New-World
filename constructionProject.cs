using Godot;
using System;
using Godot.Collections;

public partial class constructionProject : Node
{
    [Export] public string name { get; set; }
    [Export] public Dictionary<string, int> Cost, Produce;
    [Export] public int timeToProduce;
    public void Operate()
    {
        foreach(var (resource, amount) in Produce)
        {
            Resources.Base.Production[resource].Produce(amount);
        }
    }
    public int InvestResources()
    {
        
        ref Dictionary<string, Resource> Production  = ref Resources.Base.Production;
        GD.Print($"{name}'s cost is being paid for a construction project");
        Resources parent = (GetNode<Control>(Resources.root) as Resources);
        int temp = 0;
        foreach(var (resource, amt) in Cost)
        {
            temp++;
            if (Production[resource].amount < amt)
            {
                return -1;
            }
            else if (temp == Cost.Count)
            {
                goto CanAfford;
            }
        }
    CanAfford:
        foreach (var (resource, amount) in Cost)
        {
            Production[resource].Produce(amount);
        }
    return 0;


      
        
    }
}
