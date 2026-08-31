using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName ="Level",menuName = "Level")]
public class LevelData : ScriptableObject
{
    public int LevelIndex;
    public float time;
    public int Row;
    public int Column;
    public List<int> Data;
}

