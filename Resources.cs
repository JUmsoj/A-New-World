using Godot;
using Godot.Collections;
using System;
using System.Collections;
using System.Collections.Generic;

public partial class Resources : Control
{
    public static Resources Base;
    public bool stillGoing { get; set; } = false;
    private Vector2 lineOrigin, lineEnding;
    [Export] public int Balance { get; set; }
    [Export] public Label balanceIndicator;
    public Godot.Collections.Dictionary<string, Resource> Production;
    public static string root;
   
    public Godot.Collections.Dictionary<string, constructionProject> availableProjects { get; set; }
    /// <summary>
    /// subtracts or simulates the subtraction from the balance by the amount  specified in the amount paramater, and returns true or false depending on if 
    /// you can afford the loss (does it go negative when you subtract it). However, if purchaseAuto is false, it doesn't actually subtract.
    /// </summary>
    public override void _GuiInput(InputEvent @event)
    {
        if(@event is InputEventMouseMotion eventmotion)
        {
            lineEnding = eventmotion.Position;
        }
        else if(@event is InputEventMouseButton eventbutton)
        {
            if(!stillGoing && eventbutton.Pressed)
            {
                GD.Print($"hello sarr");
                stillGoing = true;
                lineOrigin = eventbutton.Position;
            }
            else if(stillGoing && !eventbutton.Pressed)
            {
                stillGoing = false;
            }
        }
    }
    public override void _Process(double delta)
    {
        QueueRedraw();
    }
    public static IEnumerable findTypeInList<T>(List<object> list)
    {
        foreach(var item in list)
        {
            if(item is T) yield return item;
        }
    }
    public bool SpendPounds(int amount, bool PurchaseAuto = true)
    {
        if (Balance < amount) return false;
        else
        {
            if (PurchaseAuto)
            {
                Balance -= amount;
                balanceIndicator.Text = $"Balance: {Balance}";
            }
            return true;
            
        }
    }
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="type"></param>
    /// <param name="Path"></param>
    /// <returns></returns>
    public T FindCompatibleResource<T>(string type, string Path) where T : Node
    {
      
        foreach (var child in GetNode(Path).GetChildren())
        {
            string ChildType = (string)child.GetMeta("res");
            // GD.Print("Loop Went Through");
            if (child is T && ChildType == type)
            {
               // GD.Print("Child matches type");
                
                return child as T;

            }
        }
        // GD.Print("Returned Null");

        return null;
    }
    public override void _Draw()
    {
        if (stillGoing) {
            DrawLine(lineOrigin, lineEnding, Colors.Black);
        }
    }
    public override void _Ready()
    {

        Balance = 1000;
        balanceIndicator.Text = $"Balance: {Balance}";
        
        Base = this;
        root = GetPath();
        Production = [];
        availableProjects = [];
        foreach (var child in GetNode<Node>("ResourceTypes").GetChildren())
        {
            Resource ChildResource = child as Resource;
            string childType = ChildResource.type;

            // GD.Print("The main loop has detected" + (child as Resource).type + "As one of the Resources");
             ChildResource.Register(FindCompatibleResource<Label>(childType , "Labels"), ref Production);
        }
        foreach(var child in GetNode<Node>("ProjectTypes").GetChildren())
        {
            constructionProject Project = child as constructionProject;
            availableProjects[Project.name] = Project;
        }
        
    }
    public void OnConfirm() => GetTree().CallGroup("States", "OnConfirm");
    
   
}
