using Godot;
using Godot.Collections;
using System;

public partial class Resources : Control
{
    public static Resources Base;
    [Export] public int Balance { get; set; }
    [Export] public Label balanceIndicator;
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
        Base = this;
        root = GetPath();
        Production = new Dictionary<string, Resource>();
        
        foreach (var child in GetNode<Node>("ResourceTypes").GetChildren())
        {
             GD.Print("The main loop has detected" + (child as Resource).type + "As one of the Resources");
             (child as Resource).Register(FindCompatibleResource<Label>(((Resource)child).type , "Labels"), ref Production);
        }
        
    }
   
    
    public void Produce(int amount, string type)
    {
        Label label = Production[type].label;
        Production[type].amount += amount;
        label.Text = $"{type} : {Production[type].amount}";
    }
}
