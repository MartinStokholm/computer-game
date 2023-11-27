using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnTest : MonoBehaviour
{
    private List<SpawnableObjectsByLevel<EnemyDetailsSO>> testLevelSpawnList;
    private RandomSpawnableObject<EnemyDetailsSO> randomEnemyHelperClass;
    private List<GameObject> instantiatedEnemyList = new List<GameObject>();

    private void OnEnable()
    {
        // subscribe to change of room
        StaticEventHandler.OnRoomChanged += StaticEventHandler_OnRoomChanged;
    }

    private void OnDisable()
    {
        // unsubscribe to change of room
        StaticEventHandler.OnRoomChanged -= StaticEventHandler_OnRoomChanged;
    }


    private void StaticEventHandler_OnRoomChanged(RoomChangedEventArgs roomChangedEventArgs)
    {
        // Destroy any spawned enemies
        if (instantiatedEnemyList is not null && instantiatedEnemyList.Count > 0)
        {
            foreach (GameObject enemy in instantiatedEnemyList)
            {
                Destroy(enemy);
            }
        }
        
        var roomTemplate = MapBuilder.Instance.GetRoomTemplate(roomChangedEventArgs.Room.TemplateID);
        if (roomTemplate is null) return;
        testLevelSpawnList = roomTemplate.EnemiesByLevelList;

        // Create RandomSpawnableObject helper class
        randomEnemyHelperClass = new RandomSpawnableObject<EnemyDetailsSO>(testLevelSpawnList);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            Debug.Log("Spawn Enemy with T");
            EnemyDetailsSO enemyDetails = randomEnemyHelperClass.GetItem();

            if (enemyDetails is not null)
                instantiatedEnemyList.Add( Instantiate(enemyDetails.enemyPrefab, PlayerUtils.GetSpawnPosition(GameUtilities.GetMouseWorldPosition()), Quaternion.identity));
        }
    }
}
