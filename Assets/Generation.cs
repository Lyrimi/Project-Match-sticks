using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.Tilemaps;
using Unity.Mathematics;

public class Generation : MonoBehaviour
{
    public ZoneTemplate[] Zones;
    public GameObject Player;
    public Tilemap TMap;
    public int RenderDistance = 1;
    public int ChunkSize = 32;
    Vector2Int curChunk;
    int frame = 0;
    int curZone = 0;

    public Tile TempTile;

    private void Start()
    {
        StartCoroutine(PlayTimeGenration());
    }

    Vector2Int getChunk(GameObject obj)
    {

        int xpos = (int)Math.Round(obj.transform.position.x);
        int ypos = (int)Math.Round(obj.transform.position.y);

        int xCCord = xpos / ChunkSize;
        int yCCord = ypos / ChunkSize;
        if (xpos < 0)
        {
            xCCord -= 1;
        }
        if (ypos < 0)
        {
            yCCord -= 1;
        }
        return new Vector2Int(xCCord, yCCord);
    }

    bool doesChunkExist(Vector2Int chunk) 
    {
        return (TMap.GetTile(new Vector3Int(chunk.x * ChunkSize, chunk.y * ChunkSize, 0)) != null);
    }

    void placeTile(int x, int y)
    {
        int DTC = x * x + y * y;
        while (true)
        {
            if (DTC > Zones[curZone].ZoneRadius * Zones[curZone].ZoneRadius)
            {
                curZone++;
            }
            else if (curZone == 0)
            {
                break;
            }
            else if (DTC < Zones[curZone - 1].ZoneRadius * Zones[curZone - 1].ZoneRadius)
            {
                curZone--;
            }
            else { break; }
        }
        TMap.SetTile(new Vector3Int(x, y, 0), Zones[curZone].FloorTile);
    }

    void GenerateChunk(Vector2Int chunk)
    {
        Vector2Int tileoffset = chunk * ChunkSize;
        //Debug.Log(tileoffset + " | " + frame);
        for (int x = 0; x < ChunkSize; x++)
        {
            for (int y = 0; y < ChunkSize; y++)
            {
                placeTile(x + tileoffset.x, y + tileoffset.y);
            }
        }
    }

    void LocalGeneration()
    {
        curChunk = getChunk(Player);  
        for (int x = 1-RenderDistance; x < RenderDistance; x++)
        {
            for (int y = 1 - RenderDistance; y < RenderDistance; y++)
            {
                Vector2Int chunkOffset = new Vector2Int(x, y);
                //print(doesChunkExist(curChunk + chunkOffset) + " | " + (curChunk+chunkOffset) + " | " + frame);
                GenerateChunk(curChunk + chunkOffset);
            }
        }

    }
    private void FixedUpdate()
    {

    }

    IEnumerator PlayTimeGenration()
    {
        while (true)
        {
            yield return new WaitForFixedUpdate();
            LocalGeneration();
            frame++;
        }
    }
}
