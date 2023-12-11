using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RandomSpawnableObject<T>
{
    private struct chanceBoundaries
    {
        public T spawnableObject;
        public int lowBoundaryValue;
        public int highBoundaryValue;
    }

    private int _ratioValueTotal = 0;
    private List<chanceBoundaries> _chanceBoundariesList = new List<chanceBoundaries>();
    private List<SpawnableObjectsByLevel<T>> _spawnableObjectsByLevelList;

    /// <summary>
    /// Constructor
    /// </summary>
    public RandomSpawnableObject(List<SpawnableObjectsByLevel<T>> spawnableObjectsByLevelList)
    {
        _spawnableObjectsByLevelList = spawnableObjectsByLevelList;
    }

    public T GetItem()
    {
        var upperBoundary = -1;
        _ratioValueTotal = 0;
        _chanceBoundariesList.Clear();
        var spawnableObject = default(T);

        foreach (var spawnableObjectsByLevel in _spawnableObjectsByLevelList)
        {
            // check for current level
            if (spawnableObjectsByLevel.MapLevel != GameManager.Instance.GetCurrentMapLevel()) continue;
            
            foreach (var spawnableObjectRatio in spawnableObjectsByLevel.SpawnAbleObjectRatios)
            {
                var lowerBoundary = upperBoundary + 1;

                upperBoundary = lowerBoundary + spawnableObjectRatio.Ratio - 1;

                _ratioValueTotal += spawnableObjectRatio.Ratio;

                // Add spawnable object to list;
                _chanceBoundariesList.Add(new chanceBoundaries() { spawnableObject = spawnableObjectRatio.MapObject, lowBoundaryValue = lowerBoundary, highBoundaryValue = upperBoundary });
            }
        }

        if (_chanceBoundariesList.Count == 0) return default;

        var lookUpValue = Random.Range(0, _ratioValueTotal);

        // loop through list to get seleted random spawnable object details
        foreach (var spawnChance in _chanceBoundariesList.Where(spawnChance => lookUpValue >= spawnChance.lowBoundaryValue && lookUpValue <= spawnChance.highBoundaryValue))
        {
            spawnableObject = spawnChance.spawnableObject;
            break;
        }


        return spawnableObject;
    }

}
