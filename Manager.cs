using Godot;
using System;

public partial class Manager : Node
{
    private int days;

    [Signal] 
    public delegate void DayPassedEventHandler(int days);
    [Signal] public delegate void DeselectEventHandler();
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
            EmitSignal(SignalName.Deselect);
        }
    }
}
