using Godot;
using System;

public enum TypesOfSaves
{
    TILE,
    DIVISION
}
public partial class Manager : Node
{
    private int days;

    [Signal] 
    public delegate void DayPassedEventHandler(int days);
    public static Node manager;
    public Control SelectedInterface { get; set; }
    private VBoxContainer TileInformation;
    
    public bool bulldozingMode { get; set; }
    private bool toggle = false;
    public static Manager instance;
    
    // Assigns a callback to the Timeout signal, whcihc tracks the current day in the game
    public override void _Ready()
    {
        instance = this;
        GetNode<Timer>("DayTimer").Timeout += () =>
        {
            days++;
            EmitSignal(SignalName.DayPassed, days);
            
        };
        // sets the labels that display the selected Tile's statistics
        // (population, buildings, etc)
        TileInformation = GetNode<VBoxContainer>("TileInfo");
        SelectedInterface = GetNode<Control>("GameScene");




    }
    public void fillInData(Tile tile)
    {
        Godot.Collections.Dictionary<string, Label> Labels = new Godot.Collections.Dictionary<string, Label>
        {
            { "Pop", TileInformation.GetNode<Label>("Test") }
        };
        Labels["Pop"].Text = $"Population: {tile.Population}";
    }
    // saves all the data within a group's collective "Save" method.
    public void SaveData(string group)
    {
        var File = FileAccess.Open("user://savegame.save", FileAccess.ModeFlags.Write);
        foreach(var node in GetTree().GetNodesInGroup(group))
        {
            if(string.IsNullOrEmpty(node.SceneFilePath))
            {
                GD.Print("not instanced");
                return;
            }
            else if(!node.HasMethod("Save") && node.Name != "TESTSAVE")
            {
                GD.Print($" {node.Name} does not have a save method");
                return;
            }
            else
            {
                
                var Data = Json.Stringify(node.Name != "TESTSAVE"  ?  node.Call("Save") : new Godot.Collections.Dictionary<string, Variant>
                {
                     {"identifier", "example" }
                });
                File.StoreLine(Data);
                GD.Print(File.GetAsText());
            }
        }
        
    }
    public void SaveData(Godot.Collections.Dictionary<string, Variant> data)
    {
        var File = FileAccess.Open("user://savegame.save", FileAccess.ModeFlags.Write);
        var jsonData = Json.Stringify(data);
        File.StoreLine(jsonData);
    }
    public void LoadGame()
    {
        GD.Print("LOADING GAME");
            if(!FileAccess.FileExists("user://savegame.save"))
            {
                GD.Print("Save file does not exist, returning...");
                return;
            }
            using var File = FileAccess.Open("user://savegame.save", FileAccess.ModeFlags.Read);
            while (File.GetPosition() < File.GetLength())
            {
                
                 var json = new Json();
                var result = json.Parse(File.GetLine());
                if (result != Error.Ok)
                {
                    GD.Print("Error with savinf");
                }
                else
                {
                    var Data = (Godot.Collections.Dictionary<string, Variant>)json.Data;
                    // use a switch case with Data["identifier"] to identify what you need to decode
                    // for this one we'll use a test key/value pair
                    if (Data.TryGetValue("identifier", out var value))
                    {
                        var strVal = (string)value;
                        switch (strVal)
                        {
                            case "example":
                                GD.Print("What");
                                break;
                            
                        } 
                    }
                else
                {
                    GD.Print("Hi");
                }
                   
                }
                        
            }
        GD.Print("Loaded");

    }
    public void LoadGame(string type)
    {
        if (!FileAccess.FileExists("user://savegame.save"))
        {
            return;
        }
        using var File = FileAccess.Open("user://savegame.save", FileAccess.ModeFlags.Read);
        while (File.GetPosition() < File.GetLength())
        {
            
            var json = new Json();
            var result = json.Parse(File.GetLine());
            if (result != Error.Ok)
            {
                GD.Print("Error with savinf");
            }
            else
            {
                var Data = (Godot.Collections.Dictionary<string, Variant>)json.Data;
                // use a switch case with Data["identifier"] to identify what you need to decode
                // for this one we'll use a test key/value pair
                if (Data.TryGetValue("identifier", out var value))
                {
                    if ((string)value == type)
                    {
                        var strVal = (string)value;
                        switch (strVal)
                        {
                            case "example":
                                GD.Print("What");
                                break;
                        }
                    }
                    else
                    {
                        continue;
                    }
                }

            }

        }
    }
    public override void _Process(double delta)
    {
        if(Input.IsActionJustPressed("Deselect All Resources"))
        {
            Tile.selected = null;
            GetTree().CallGroup("States", "OnDeselect");
        }
    }
    // sets the interface depending on a signal string argument
    public void setInterface(string selectedInterface)
    {
        SelectedInterface.Visible = false;
        switch (selectedInterface)
        {
            case "war":
                /*Control Interface = GetNode<Control>("WarInterface");
                Interface.Visible = true;
                SelectedInterface = Interface;
                GetTree().SetGroup("States", "sceneMode", "war");
                break;*/
                SaveData("States");
                LoadGame();
                break;
            case "construct":
                Control AltInterface1 = GetNode<Control>("GameScene");
                AltInterface1.Visible = true;
                SelectedInterface = AltInterface1;
                GetTree().SetGroup("States", "sceneMode", "economy");
                break;

        }
    }
    public override void _EnterTree()
    {
        manager = this;
    }
}
