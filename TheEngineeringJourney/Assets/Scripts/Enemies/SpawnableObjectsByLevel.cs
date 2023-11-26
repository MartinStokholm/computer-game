using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SpawnableObjectsByLevel<T>
{
    public MapLevelSO MapLevel;
    public List<SpawnableObjectRatio<T>> SpawnAbleObjectRatios;
}
