using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu(fileName ="Zone",menuName ="ZoneTemplate")]
public class ZoneTemplate : ScriptableObject
{
    public int ZoneRadius;
    public string ZoneName;
    public int ZoneType;
    public int StickQuouta;
    public int BushQuouta;
    public int EnemyQuouta;
    public Tile FloorTile;

}
