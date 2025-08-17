using Godot;
using System;
using Godot.Collections;


/* Notes 
 * Create another partial class for the Military side of Tiles
 * each tile will have a dictionary for the # of friendly and enemy divisions
 * there will be a formula for the actual battle, which will happen when the offense timer runs out for the attacking side.
 * Eahc tile will have a control integer between 1-100 (clamp it).
 * When the control bar reaches 100% for either side, the fight gets taken elsewhere
 * Each battle will have a certain limit for the amount of divisions that can fight at any given type based on the terrain, all other divisions will do reduced damage.
 * Eahc battle will have a bar for who's winning, depending on the commanders tactic, "retreat" or "pursuit" will always be the final tactic before the division loses or wins. This is all based on the divisions morale, based on the damage they're taking
 * There will also be a supply mechanic, based on the factories in each tile and the "collected" supplies (supplies colected from excess in other tiles) , you will either get increased or decreased stats for the divisions in that tile.
 * This will be completed by the end of August, hopefully.
 */
public partial class Military : Area2D
{
    public static Military SelectedDivision;
    public int Damage, Defence;
    bool selected;
    private const string Infantry = "infantry";
    
    private Label defenceIndicator, attackIndicator;
    public override void _Input(InputEvent @event)
    {
        if(@event is InputEventMouseButton && selected)
        {
            Move((@event as InputEventMouseButton).Position);
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
    
}
public partial class Tile : Button
{
    public Dictionary<string, int> Divisions;
    public Dictionary<string, int> DivisionsInTraining;


}
