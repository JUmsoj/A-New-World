using Godot;
using System;
using Godot.Collections;

public partial class Military : Control
{
	public static Military SelectedDivision;
	public int Damage, Defence;
	bool selected;
	private const string Infantry = "infantry";
	public static string SelectedDivisionType;
	private Label defenceIndicator, attackIndicator;
	public static Vector2 lineOrigin, lineEnding;
	public static bool lineExists = false;
	[Export] public Label tileInLBL { get; set; }
	public string Type { get; set; }
	public Tile tileExistsIn { get; set; }
	
	public override void _Input(InputEvent @event)
	{
		
		
		if (@event is InputEventMouseMotion eventmousemotion)
		{
			if(lineExists) 
			lineEnding = eventmousemotion.Position;
		}
		if(Manager.instance.isJustPressed(@event, Key.R, out InputEventKey _))
		{
			GD.Print("R key pressed");
			lineEnding = GetGlobalMousePosition();
		}	
		
	}
	public override void _Draw()
	{
		if(lineExists)
		{
			DrawLine(lineOrigin, lineEnding, Colors.Black);
		}
	}
	public void EnemyEngagement()
	{
	   
		Timer attackTimer = new()
		{
			WaitTime = 5
		};
		Manager.instance.AddChild(attackTimer);
	}
	private void Move(Vector2 position)
	{
		// also increase the amount of divisions of this type in there
		Position += position;
	}
	private void Move(Tile tile)
	{
		tile.Divisions[Infantry]++;
	}
	public void OnClick(bool toggle)
	{
        GD.Print("Drawing Line");
        SelectedDivision = this;
		lineOrigin = GetNode<Button>("Button").Position;
		lineEnding = GetGlobalMousePosition();
		lineExists = toggle;
	}
    public override void _Ready()
    {
		GD.Print($"Position of {Name}: {Position}");
		GD.Print($"{Name} has been deployed for duty");
		tileInLBL.Text = $"Tile: {tileExistsIn.Name}";
    }
	public override void _Process(double delta)
	{
		QueueRedraw();
	}
}
public partial class Tile : Button
{
	public Dictionary<string, int> Divisions;
	public Dictionary<string, int> DivisionsInTraining;
	private bool mouseIn = false;
    public override void _Notification(int what)
    {
        if(what == NotificationMouseEnter && Military.lineExists && ButtonPressed)
		{
			var Division = Military.SelectedDivision;
			Division.tileExistsIn.Move(Division.Type, this);
			Military.lineExists = false;
		}
    }
	 
}
