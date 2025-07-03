using Godot;
using System;

public partial class Manager : Node
{
    private int days;

    [Signal] 
    public delegate void DayPassedEventHandler(int days);
    public Control SelectedInterface { get; set; }
    public bool bulldozingMode { get; set; }
    public override void _Input(InputEvent @event)
    {
        if (@event is InputEventKey)
        {
            InputEventKey KeyPressed = @event as InputEventKey;
            if(KeyPressed.Keycode == Key.Space)
            {
                Tile.selected = null;
            }
        }
    }
    public override void _Ready()
    {
        GetNode<Timer>("DayTimer").Timeout += () =>
        {
            days++;
            EmitSignal(SignalName.DayPassed, days);
        };
    }
    
    public void setInterface(Control selectedInterface)
    {
        SelectedInterface.Visible = false;
        selectedInterface.Visible = true;
    }
}
