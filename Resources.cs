using Godot;
using Godot.Collections;
using System;

public partial class Resources : Control
{
    public Dictionary<string, Resource> Production;

    T FindCompatibleResource<T>(string type, string Path) where T : Node
    {
       
        foreach (var child in GetNode(Path).GetChildren())
        {
            GD.Print("Loop Went Through");
            if (child.GetType() == typeof(T) && ((string)child.GetMeta("res")) == type)
            {
                GD.Print("Child matches type");
                
                return child as T;

            }
        }
        GD.Print("Returned Null");

        return null;
    }
    public override void _Ready()
    {
        Production = new Dictionary<string, Resource>();
        foreach (var child in GetNode<Control>("ResourceTypes").GetChildren())
        {
             GD.Print((child as Resource).type);
             (child as Resource).Register(FindCompatibleResource<Label>(((Resource)child).type , "Labels2"), Production);
        }
        
    }
    public void Produce(int amount, string type)
    {
        Label label = Production[type].label;
        Production[type].amount += amount;
        label.Text = $"{type} : {amount}";
        
    }
}
