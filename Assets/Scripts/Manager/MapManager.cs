using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using System;

public class MapManager : BaseManager<MapManager>
{
    public GameObject MapObj = null;
    public Tilemap tilemap = null;

    public string seed;

    public void SetMapObj(GameObject mapObj)
    {
        this.MapObj = mapObj;
        this.tilemap = mapObj.GetComponent<Tilemap>();
    }

    public void SetSeed(string seed)
    {
        this.seed = seed;
    }

    public void GenerateTestMap()
    {
        TileDataConfig tileDataConfig = LoadManager.Instance.LoadConfig<TileDataConfig>();
        TileData tileData = tileDataConfig.GetTileData(1);
        Log(tileData.tilePath);

        Tile tile = LoadManager.Instance.Load<Tile>(tileData.tilePath);
        int[,] mapDatas = new int[10,10];
        ClearMap();
        for (int i = -mapDatas.GetLength(0); i < mapDatas.GetLength(0); i++)
        {
            for (int j = -mapDatas.GetLength(1); j < mapDatas.GetLength(1); j++)
            {
                tilemap.SetTile(new Vector3Int(i, j), tile);
            }
        }
    }

    public void ClearMap()
    {
        this.tilemap.ClearAllTiles();
    }

    private void GenerateMap()
    {
        
    }

    //���ɻ�������
    private void GenerateBaseMap()
    {

    }
    
    //���ɰ�Χ
    private void GenerateEnchat()
    {

    }

    //���ɳ�����
    private void GenerateStartingPoint()
    {

    }

    //�����յ�
    private void GenerateEndPoint()
    {

    }

    Vector2Int[] RandomChunkLayout(int count)
    {
        var result = new Vector2Int[count];
        result[0] = new Vector2Int(0,0);
        var edge = new List<Vector2Int>();
        edge.AddRange(GetNeighbors4(result[0]));

        var random = new System.Random(233333);
        for(int i = 1; i < count; i++)
        {
            var curr = edge[random.Next(0, edge.Count)];
            result[i] = curr;
            edge.Remove(curr);

            foreach(var neighbor in GetNeighbors4(curr))
            {
                if(Array.Exists(result, v => v.Equals(neighbor))) continue;
                if(edge.Contains(neighbor)) continue;
                edge.Add(neighbor);
            }
        }
        
        return result;
    }

    List<Vector2Int> GetNeighbors4(Vector2Int center)
    {
        var result = new List<Vector2Int>();
        result.Add(new Vector2Int(center.x + 1, center.y));
        result.Add(new Vector2Int(center.x - 1, center.y));
        result.Add(new Vector2Int(center.x, center.y + 1));
        result.Add(new Vector2Int(center.x, center.y - 1));
        return result;
    }
}
