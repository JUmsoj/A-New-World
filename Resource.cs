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
    public void Produce(int amount)
    {
        this.amount += amount;
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
