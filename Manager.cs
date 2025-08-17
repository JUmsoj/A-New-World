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
    private Godot.Collections.Dictionary<string, Variant> tileSaveDict ;
    [Signal] 
    public delegate void DayPassedEventHandler(int days);
    public static Node manager;
    public Control SelectedInterface { get; set; }
    private VBoxContainer TileInformation;
    public Godot.Collections.Dictionary<string, Label> Labels { get; set; }
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
        var daysLabel = GetNode<Label>("DaysLabel");
        DayPassed += (days) =>
        {
            daysLabel.Text = $"Current Day : {days}";
        };
        // sets the labels that display the selected Tile's statistics
        // (population, buildings, etc)
        TileInformation = GetNode<VBoxContainer>("TileInfo");
        SelectedInterface = GetNode<Control>("GameScene");
        GetTree().SetGroup("States", "sceneMode", "economy");
        Labels = new Godot.Collections.Dictionary<string, Label> {
            { "Pop", TileInformation.GetNode<Label>("Test") },
             { "BuildingInfo", TileInformation.GetNode<Label>("BuildingInfo")}
        };
        GetTree().SetGroup("States", "buildingCounter", Labels["BuildingInfo"]);


    }
    public override void _UnhandledKeyInput(InputEvent @event)
    {
        if(@event is InputEventKey keyEvent)
        {
            if(keyEvent.Keycode == Key.J)
            {
                SaveAndLoadTest("hello", "sigma");
            }
        }
    }
    public void fillInData(Tile tile)
    {
        
        Labels["Pop"].Text = $"Population: {tile.Population}";
        
    }
    // saves all the data within a group's collective "Save" method.
    public void SaveData(string group, params Variant[] varsToSaveBinarily)
    {
        using var File = FileAccess.Open("user://savegame.save", FileAccess.ModeFlags.Write);
       
        

        
        GD.Print(FileAccess.GetOpenError().ToString());
        
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
               
                
                
            }
        }
        File.Close();
        if (varsToSaveBinarily.Length > 0)
        {
            using FileAccess binaryFile = FileAccess.Open("user://binsavegame.bin", FileAccess.ModeFlags.Write);
            foreach(var item in varsToSaveBinarily)
            {
                binaryFile.StoreVar(item);
            }
            binaryFile.Close();
        }

    }
    public void SaveData(Godot.Collections.Dictionary<string, Variant> data)
    {
        var File = FileAccess.Open("user://savegame.save", FileAccess.ModeFlags.Write);
        var jsonData = Json.Stringify(data);
        File.StoreLine(jsonData);
    }
    
    private void SaveAndLoadTest(string content, string name)
    {
        using var saveFile = FileAccess.Open($"user://{name}.txt", FileAccess.ModeFlags.Write);
        saveFile.StoreString(content);
        saveFile.Close();
        using var loadFile = FileAccess.Open($"user://{name}.txt", FileAccess.ModeFlags.Read);
        GD.Print(loadFile.GetAsText());
        loadFile.Close();
    }
    public void SingleLoadGame( string group)
    {
        GD.Print("Loading");
        if (!FileAccess.FileExists("user://savegame.save"))
        {
           // GD.Print("Here?");
            return;
        }
        foreach(var node in GetTree().GetNodesInGroup(group))
        {
            node.QueueFree();
        }
        
        GD.Print("Why");
       using var File = FileAccess.Open("user://savegame.save", FileAccess.ModeFlags.Read);
        GD.Print($"LoadingOpenError: {FileAccess.GetOpenError().ToString()}");
        // GD.Print($"File Reading: {File.GetAsText()}");
        
       //  GD.Print("Here right.");
        while (File.GetPosition() < File.GetLength())
        {
            
            // GD.Print("Filing");
            
            var json = new Json();
            var currentLine = File.GetLine();
           //  GD.Print($"Line {File.GetPosition()}/{File.GetLength()} : {currentLine}");

            var result = json.Parse(currentLine);
            if (result != Error.Ok)
            {
                GD.Print($"Parsing Error: {json.GetErrorMessage()} & {json.GetErrorLine()}");
                
            }
           
            else
            {
                // GD.Print($"Line {currentLine} was succesfully parsed");
                var Data = (Godot.Collections.Dictionary<string, Variant>)json.Data;
                // use a switch case with Data["identifier"] to identify what you need to decode
                // for this one we'll use a test key/value pair
                // GD.Print("ello");
                if (Data.TryGetValue("identifier", out var value))
                {

                   
                        var strVal = (string)value;
                        GD.Print("FOUBN");
                        switch (strVal)
                        {
                            case "example":
                                GD.Print("What");
                                 PackedScene tile = GD.Load<PackedScene>("res://Square.tscn");
                                    var newTile = tile.Instantiate<Tile>();
                                    newTile.Position = new Vector2((float)Data["X"], (float)Data["Y"]);
                                     newTile.AddToGroup("States");
                                    foreach(var (Key, val) in Data)
                                    {
                                        if (Key == "X" || Key == "Y") continue;
                                        else  newTile.Set(Key, val);
                                        
                                    }
                                    /*foreach(var (_, signal) in (Godot.Collections.Dictionary<string, Variant>)Data["Signals"])
                                    {
                                        Godot.Collections.Array<Godot.Collections.Dictionary> temp = (Godot.Collections.Array<Godot.Collections.Dictionary>)signal;
                                        
                                        for(int i = 0; i < temp.Count; i++)
                                        {
                                            Signal Signal = (Signal)temp[i]["signal"];
                                            
                                            Callable? callable = (Callable)temp[i]["callable"];
                                            if (callable == null) continue;

                                            newTile.Connect(Signal.Name, (Callable)callable);
                                        }
                                    }*/
                                    newTile.Set(Tile.PropertyName.daytimer, GetNode<Timer>("DayTimer"));
                                    SelectedInterface.AddChild(newTile);

                                    
                                break;
                        }
                   
                    
                   
                }
                else
                {
                    GD.Print("How");
                }

            }

        }
        File.Close();
        
        GetTree().SetGroup("States", "daytimer", GetNode<Timer>("DayTimer"));
       
        
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
                Control Interface = GetNode<Control>("WarScene");
                Interface.Visible = true;
                SelectedInterface = Interface;

                GetTree().SetGroup("States", "sceneMode", "war");
                SaveData("States");
                SingleLoadGame("States");
                
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
