using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.Tilemaps;
using Unity.Mathematics;
using System.Runtime.ConstrainedExecution;
using UnityEngine.UI;
using TMPro;
using UnityEditor.ShaderGraph.Internal;

public class Generation : MonoBehaviour
{
    public ZoneTemplate[] Zones;
    public GameObject Player;
    public Tilemap TMap;
    public Tilemap DecoMap;
    public Tilemap ObjectMap;
    public int RenderDistance = 1;
    public int ChunkSize = 32;
    //public Slider progressSlider;
    //public TextMeshProUGUI DebugText;
    public GameObject Bush;
    public GameObject Stick;

    Vector2Int curChunk;
    int curZone = 0;

    

    public static int Seed;

    float RandomSeedNumber(int seed, int x, int y)
    {
        x = x * 17;
        y = y * 31;

        x = ((x >> 15) ^ x) * 0x45d9f3b;
        x = ((x >> 15) ^ x) * 0x45d9f3b;
        x = ((x >> 15) ^ x) * 0x45d9f3b;

        y = ((y >> 15) ^ y) * 0x45d9f3b;
        y = ((y >> 15) ^ y) * 0x45d9f3b;
        y = ((y >> 15) ^ y) * 0x45d9f3b;

        return RandomSeedNumber(seed + y + x);
    }
    float RandomSeedNumber(int seed) 
    {
        int cur = seed;
        cur = ((cur >> 15) ^ cur) * 0x45d9f3b;
        cur = ((cur >> 15) ^ cur) * 0x45d9f3b;
        cur = ((cur >> 15) ^ cur) * 0x45d9f3b;
        cur = cur & 0x7FFFFFFF;
        return (float)cur/int.MaxValue;
    }

    
    private void Awake()
    {  
        QualitySettings.vSyncCount = 0;  // VSync must be disabled
        Application.targetFrameRate = 70;


    }

    private float test() 
    {
        DateTime starttime = DateTime.Now;
        for (int index = 0; index < int.MaxValue; index++)
        {
            int combined = Zones[1].ZoneRadius * Zones[2].ZoneRadius;
        }
        DateTime endtime = DateTime.Now;
        var UnAssinged = (endtime - starttime).Ticks;
        int[] ZonesRadius = new int[Zones.Length];

        int i = 0;
        foreach (ZoneTemplate Zone in Zones)
        {
            ZonesRadius[i] = Zone.ZoneRadius;
            i++;
        }

        starttime = DateTime.Now;
        for (int index = 0; index < int.MaxValue; index++)
        {
            int combined = ZonesRadius[1] * ZonesRadius[2];
        }
        endtime = DateTime.Now;
        var Assinged = (endtime - starttime).Ticks;

        return (1f - (float)Assinged / (float)UnAssinged) *100f;
    }

    private void Start()
    {
        Seed = UnityEngine.Random.Range(0,int.MaxValue);

        print($"Current Seed : {Seed}");

        UnityEngine.Random.InitState(Seed);
        int TotalSticks = 0;
        int TotalBushes = 0;
        int TotalEnemys = 0;
        foreach (ZoneTemplate Zone in Zones) 
        {
            TotalSticks += Zone.StickQuouta;
            TotalBushes += Zone.BushQuouta;
            TotalEnemys += Zone.EnemyQuouta;
        }

        //progressSlider.maxValue = TotalSticks+TotalBushes+TotalEnemys;

        for (int i = 0; i < Zones.Length; i++)
        {
            int lowerRadius = 0;
            int upperRadius = Zones[i].ZoneRadius;
            if (i == 0)
            {
                lowerRadius = 0;
            }
            else
            {
                lowerRadius = Zones[i-1].ZoneRadius;
            }


            for (int s = 0;  s< Zones[i].StickQuouta; s++)
            {
                //DebugText.text = $"Generating Sticks {s+1}/{Zones[i].StickQuouta}";
                Vector2 RandPoint = RandomPointInRing(lowerRadius, upperRadius);
                //progressSlider.value += 1;
                Instantiate(Stick, new Vector3Int((int)RandPoint.x, (int)RandPoint.y, (int)Stick.transform.position.z), Quaternion.Euler(new Vector3(0, 0, 0)));
            }

            for (int s = 0; s < Zones[i].BushQuouta; s++)
            {
                //DebugText.text = $"Generating Sticks {s + 1}/{Zones[i].BushQuouta}";
                Vector2 RandPoint = RandomPointInRing(lowerRadius, upperRadius);
                //progressSlider.value += 1;
                Instantiate(Bush, new Vector3Int((int)RandPoint.x, (int)RandPoint.y, (int)Bush.transform.position.z), Quaternion.Euler(new Vector3(0, 0, 0)));
            }

        }

    }

    IEnumerator StartGeneration() 
    {
        yield return new WaitForFixedUpdate();

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
        //while (true)
        //{
        //    if (DTC > Zones[curZone].ZoneRadius*Zones[curZone].ZoneRadius)
        //    {
        //        curZone++;
        //        break;
        //    }
        //    else if (curZone == 0)
        //    {
        //        break;
        //    }
        //    else if (DTC < Zones[curZone - 1].ZoneRadius*Zones[curZone - 1].ZoneRadius)
        //    {
        //        curZone--;
        //        break;
        //    }
        //    else { break; }
        //}

        if (DTC > Zones[curZone].ZoneRadius * Zones[curZone].ZoneRadius)
        {
            curZone++;
        }
        else if (curZone == 0) { }
        else if (DTC < Zones[curZone - 1].ZoneRadius * Zones[curZone - 1].ZoneRadius)
        {
            curZone--;
        }

        TMap.SetTile(new Vector3Int(x, y, 0), Zones[curZone].FloorTile);

        if (Zones[curZone].Decorations.Length != 0)
        {
            int i = 0;
            foreach (Tile Deco in Zones[curZone].Decorations)
            {
                float chance = 1 / Zones[curZone].Chances[i];
                if (RandomSeedNumber(Seed,x,y) < chance)
                {
                    DecoMap.SetTile(new Vector3Int(x, y, 0), Deco);
                }
            }
        }
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
        LocalGeneration();
    }

    public Vector2 RandomPointInRingMath(float lowerRadius, float upperRadius, float minRadians, float maxRadians)
    {
        
        float angle = UnityEngine.Random.Range(minRadians, maxRadians);
        float squareLength = UnityEngine.Random.Range(Mathf.Pow(lowerRadius, 2), Mathf.Pow(upperRadius, 2));
        return new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * Mathf.Sqrt(squareLength);
    }

    public Vector2 RandomPointInRing(float lowerRadius, float upperRadius)
    {
        return RandomPointInRingMath(lowerRadius, upperRadius, 0, Mathf.PI * 2);
    }
}
