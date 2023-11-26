using System.Collections;
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

    private int ratioValueTotal = 0;
    private List<chanceBoundaries> chanceBoundariesList = new List<chanceBoundaries>();
    private List<SpawnableObjectsByLevel<T>> spawnableObjectsByLevelList;

    /// <summary>
    /// Constructor
    /// </summary>
    public RandomSpawnableObject(List<SpawnableObjectsByLevel<T>> spawnableObjectsByLevelList)
    {
        this.spawnableObjectsByLevelList = spawnableObjectsByLevelList;
    }

    public T GetItem()
    {
        var upperBoundary = -1;
        ratioValueTotal = 0;
        chanceBoundariesList.Clear();
        var spawnableObject = default(T);

        foreach (var spawnableObjectsByLevel in spawnableObjectsByLevelList)
        {
            // check for current level
            if (spawnableObjectsByLevel.MapLevel != GameManager.Instance.GetCurrentMapLevel()) continue;
            foreach (var spawnableObjectRatio in spawnableObjectsByLevel.SpawnAbleObjectRatios)
            {
                var lowerBoundary = upperBoundary + 1;

                upperBoundary = lowerBoundary + spawnableObjectRatio.Ratio - 1;

                ratioValueTotal += spawnableObjectRatio.Ratio;

                // Add spawnable object to list;
                chanceBoundariesList.Add(new chanceBoundaries() { spawnableObject = spawnableObjectRatio.MapObject, lowBoundaryValue = lowerBoundary, highBoundaryValue = upperBoundary });

            }
        }

        if (chanceBoundariesList.Count == 0) return default;

        var lookUpValue = Random.Range(0, ratioValueTotal);

        // loop through list to get seleted random spawnable object details
        foreach (var spawnChance in chanceBoundariesList.Where(spawnChance => lookUpValue >= spawnChance.lowBoundaryValue && lookUpValue <= spawnChance.highBoundaryValue))
        {
            spawnableObject = spawnChance.spawnableObject;
            break;
        }


        return spawnableObject;
    }

}
