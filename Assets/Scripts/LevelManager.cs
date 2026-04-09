using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections.Generic;

public class LevelManager : MonoBehaviour
{
    public static LevelManager instance { get; private set; }

    Dictionary<(Vector3Int pos, int level), TileData> tiles;
    Dictionary<int, int> tileCounts;
    private Grid grid;

    private void Awake()
    {
        // singleton
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;

        // variables
        grid = GetComponent<Grid>();

        tiles = new();
        tileCounts = new();

        // methods
        RegisterTiles();
    }

    public Vector3Int GetGridPos(Vector3 pos)
    {
        return grid.WorldToCell(pos);
    }

    public TileData? GetTile(Vector3Int pos, int level)
    {
        if (tiles.TryGetValue((pos, level), out var tile)) return tile;
        return null;
    }

    void RegisterTiles()
    {
        tiles.Clear();
        tileCounts.Clear();

        Level[] levels = GetComponentsInChildren<Level>();

        foreach (Level lvl in levels)
        {
            tileCounts[lvl.level] = 0;

            foreach (TilemapLayer tl in lvl.tLayers)
            {
                AddTiles(lvl.level, tl.tMap, tl.tType);
            }
        }

        PrintTileCounts();
    }

    void AddTiles(int level, Tilemap tilemap, TileType tType)
    {
        BoundsInt bounds = tilemap.cellBounds;
        int count = 0;

        foreach (Vector3Int pos in bounds.allPositionsWithin)
        {
            if (!tilemap.HasTile(pos)) continue;

            Vector3 world = tilemap.GetCellCenterWorld(pos);
            Vector3Int worldPos = grid.WorldToCell(world);

            tiles[(worldPos, level)] = new TileData(tType);
            count++;
        }

        tileCounts[level] += count;
    }

    void PrintTileCounts()
    {
        string toPrint =  " === ";

        foreach (var x in tileCounts) toPrint += $"(Level: {x.Key}, Tiles: {x.Value}) === ";
        toPrint += $"(Total: {tiles.Count})";

        Debug.Log(toPrint);
    }
}
