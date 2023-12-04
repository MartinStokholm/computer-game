using System.Collections.Generic;

[System.Serializable]
public class SpawnableObjectsByLevel<T>
{
    public MapLevelSO MapLevel;
    public List<SpawnableObjectRatio<T>> SpawnAbleObjectRatios;
}
