using UnityEngine;

public class GameGrid
{
    public Node[,] Nodes { get; set; }
}

public struct Node
{
    public TileType TileType { get; set; }
}

public enum TileType { Wall, Room, Corridor, Door }