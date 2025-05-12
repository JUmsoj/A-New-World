using Godot;
using System;
using Godot.Collections;

public partial class Resource : Button
{ 

    public static Resources parent;
    public string type;
    public int amount { get; set; }
    public Label label { get; set; }
    [Export] private int productionRate;
      
    [Export] public Dictionary<string, int> cost; // odd is resource instance, even is int
    public override void _Ready()
    {
        amount = 0;
        cost = [];
        type = (string)GetMeta("res");
    }
    void OnPress()
    {
        Tile.selected = this;
    }
    public int Pay()
    {
        GD.Print($"{type}'s cost is being paid for a construction project");
        parent ??= (GetNode<Control>(Resources.root) as Resources);
       
        foreach (var pair in cost)
        {
            if (parent.Production[pair.Key].amount >= pair.Value)
            {
                parent.Produce(-1 * pair.Value, pair.Key);

            }
            else
            {
                foreach(var pai in cost) 
                parent.Produce(pai.Value, pai.Key); // replace this refund functions with a function that removes pounds from the balance
                GD.Print($"A production facility that produces {type} has failed to build");
                return -1;
            }
        }
        return 0;
    }
    public void Register(Label label, Dictionary<string, Resource> dict)
    {
        try
        {
            this.label = label;
            dict[type] = this;
        }
        catch(Exception e)
        {
            GD.Print($"{e} on line 24");
        }
       
        
    }
    public void Produce()
    {
        amount += productionRate;
        GD.Print($"Added {amount} to {type}");
        label.Text = $"{type}: {amount}";
    }
    private void Deselect()
    {
        if(GetHashCode() == Tile.selected.GetHashCode())
        {
            Tile.selected = null;
        }
    }
}
