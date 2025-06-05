using Godot;
using System;

public partial class Manager : Node
{
    private int days;

    [Signal] 
    public delegate void DayPassedEventHandler(int days);
    public override void _Ready()
    {
        GetNode<Timer>("DayTimer").Timeout += () =>
        {
            days++;
            EmitSignal(SignalName.DayPassed, days);
        };
    }
    public override void _Process(double delta)
    {
        if (Input.IsActionJustPressed("Deselect All Resources"))
        {
            Tile.selected = null;
        }
    }
}
