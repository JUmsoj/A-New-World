using Godot;
using System;
using Godot.Collections;
public partial class Military : Area2D
{
    public int Damage, Defence;
    bool selected;
   
    public override void _Input(InputEvent @event)
    {
        if(@event is InputEventMouseButton && selected)
        {
            Move((@event as InputEventMouseButton).Position);
        }
    }
    private void Move(Vector2 position)
    {
        Position += position;
    }
}
