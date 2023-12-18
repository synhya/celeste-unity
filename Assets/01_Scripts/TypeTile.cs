

using UnityEngine;
using UnityEngine.Tilemaps;

public enum TileType
{
    None,
    Grey,
    Grass,
    Ice,
    Fire,
}

[CreateAssetMenu(fileName = "IndexTile", menuName = "Tile/IndexTile")]
public class TypeTile : Tile
{
    public TileType Type = TileType.Grey;
    
    public override void RefreshTile(Vector3Int position, ITilemap tilemap)
    {
        base.RefreshTile(position, tilemap);
    }

    public override void GetTileData(Vector3Int position, ITilemap tilemap, ref TileData tileData)
    {
        base.GetTileData(position, tilemap, ref tileData);
    }
}


