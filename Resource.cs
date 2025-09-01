using Godot;
using System;
using Godot.Collections;
/*HOW TO CREATE A RESOURCE:
 * 1. you create a new child Node in ResourceTypes and name it
 * 2. you create a string metadata in the Node and you name it "res", the value of which will be the key that will allow other Nodes to access your Resource
 * 3. You create a label under Labels, name it your aforementioned Resource name with an identifier that it is a label
 * 4. create metadata on the newly created Label, name it "res" and the key of that Resource (the "res" property in the other Node)
 * 5. Now constructionProjects can reference your Resource using the Production dictionary, adding and subtracting to its supply using the Produce() function
*/
public enum ResourceActions
{
	Produce=35,
	Spend=50,
	
}
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
				// GD.Print($"{dict[type].type} is added to records");
			}
		}
		catch(Exception e)
		{
			GD.Print($"{e} on line 24");
		}
	   
		
	}
	
	
	/// <summary> Adds a specific amount to the resources supply </summary>
	public void Produce(int amount)
	{
		this.amount += amount;
		GD.Print($"Changed the amount of {type} by {amount}");
		label.Text = $"{type}: {this.amount}";

	}
	/// <summary>
	/// Changes the Supply of the corresponding Resource with an Action (add, subtract), and an amount
	/// </summary>
	
	public bool ChangeSupply(ResourceActions action, int amount)
	{
		
		switch(action)
		{
			case ResourceActions.Produce:
				this.amount += amount;
				break;
			case ResourceActions.Spend:
				this.amount -= amount;
				break;

                
        }
        label.Text = $"{type}: {this.amount}";
        return this.amount > amount;
    }
	
}
