using Godot;
using Godot.Collections;
using System;

public partial class Resources : Control
{
    public Dictionary<string, Resource> Production;
    public static string root;
    public T FindCompatibleResource<T>(string type, string Path) where T : Node
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
        root = GetPath();
        Production = [];
        foreach (var child in GetNode<Control>("ResourceTypes").GetChildren())
        {
             GD.Print((child as Resource).type);
             (child as Resource).Register(FindCompatibleResource<Label>(((Resource)child).type , "Labels2"), Production);
        }
        
    }
    public override void _Process(double delta)
    {
       // add code for show settings here 
    }
    void ShowSettings()
    {
        // add code for a menu including the following items:
        /*
        Upkeep cost
        Whether it was public or private
        what this land produces and how much this produces daily
        The amount of colonists working on it(is a factor in the production rate
        a button to disable & enable it(temporarily reducing upkeep and freezing production)
        a button to demolish it (destroys it completely)
        etc..
        */
    }
    
    public void Produce(int amount, string type)
    {
        Label label = Production[type].label;
        Production[type].amount += amount;
        label.Text = $"{type} : {Production[type].amount}";
        
    }
}
