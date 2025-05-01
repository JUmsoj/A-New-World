using Godot;
using System;
using Godot.Collections;

public partial class Resource : Button
{
    public string type;
    public int amount { get; set; }
    public Label label { get; set; }
    public override void _Ready()
    {
        type = (string)GetMeta("res");
    }
    void OnPress()
    {
        Tile.selected = this;
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
    private void Deselect()
    {
        if(GetHashCode() == Tile.selected.GetHashCode())
        {
            Tile.selected = null;
        }
    }
}
