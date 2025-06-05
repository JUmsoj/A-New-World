using Godot;
using System;
using Godot.Collections;

public partial class Resource : Node
{ 

	public static Resources parent;
	public string type;
	public int amount { get; set; }
	public Label label { get; set; }
	private int productionRate;
	  
	
	public override void _Ready()
	{
		amount = 0;
		
		type = (string)GetMeta("res");
		GD.Print("A type has been succesfully initialized, specifically: " + type);
	}
	
	public void Register(Label label, ref Dictionary<string, Resource> dict)
	{
		try
		{
			this.label = label;
			if (dict != null)
			{
				dict[type] = this;
				GD.Print($"{dict[type].type} is added to records");
			}
		}
		catch(Exception e)
		{
			GD.Print($"{e} on line 24");
		}
	   
		
	}
	public void Produce()
	{
		amount += productionRate;
		GD.Print($"Added {amount} to {type}");
		label.Text = $"{type}: {amount}";
	}
	public void Produce(int amount)
	{
		this.amount += amount;
		GD.Print($"Added {amount} to {type}");
		label.Text = $"{type}: {amount}";

	}
	
}
