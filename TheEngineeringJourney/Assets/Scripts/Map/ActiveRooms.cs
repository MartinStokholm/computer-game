using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class ActivateRooms : MonoBehaviour
{
    #region Header POPULATE WITH MINIMAP CAMERA
    [Header("POPULATE WITH MINIMAP CAMERA")]
    #endregion Header
    [SerializeField] private Camera miniMapCamera;

    private Camera cameraMain;

    // Start is called before the first frame update
    private void Start()
    {
        // Cache main camera
        cameraMain = Camera.main;

        InvokeRepeating("EnableRooms", 0.5f, 0.75f);
    }

    private void EnableRooms()
    {
        // if currently showing the dungeon map UI don't process
        if (GameManager.Instance.GameState == GameState.MapOverviewMap)
            return;

        GameUtilities.CameraWorldPositionBounds(out Vector2Int miniMapCameraWorldPositionLowerBounds, out Vector2Int miniMapCameraWorldPositionUpperBounds, miniMapCamera);

        GameUtilities.CameraWorldPositionBounds(out Vector2Int mainCameraWorldPositionLowerBounds, out Vector2Int mainCameraWorldPositionUpperBounds, cameraMain);


        // Iterate through dungeon rooms
        foreach (KeyValuePair<string, Room> keyValuePair in MapBuilder.Instance.MapBuilderRoomDictionary)
        {
            Room room = keyValuePair.Value;

            // If room is within miniMap camera viewport then activate room game object
            if ((room.LowerBounds.x <= miniMapCameraWorldPositionUpperBounds.x && room.LowerBounds.y <= miniMapCameraWorldPositionUpperBounds.y) && (room.UpperBounds.x >= miniMapCameraWorldPositionLowerBounds.x && room.UpperBounds.y >= miniMapCameraWorldPositionLowerBounds.y))
            {
                room.InstantiatedRoom.gameObject.SetActive(true);

                // If room is within main camera viewport then activate environment game objects
                if ((room.LowerBounds.x <= mainCameraWorldPositionUpperBounds.x && room.LowerBounds.y <= mainCameraWorldPositionUpperBounds.y) && (room.UpperBounds.x >= mainCameraWorldPositionLowerBounds.x && room.UpperBounds.y >= mainCameraWorldPositionLowerBounds.y))
                {
                    room.InstantiatedRoom.ActivateEnvironmentGameObjects();
                }
                else
                {
                    room.InstantiatedRoom.DeactivateEnvironmentGameObjects();
                }


            }
            else
            {
                room.InstantiatedRoom.gameObject.SetActive(false);
            }

        }
    }

    #region Validation
#if UNITY_EDITOR
    private void OnValidate()
    {
        EditorUtilities.ValidateCheckNullValue(this, nameof(miniMapCamera), miniMapCamera);
    }
#endif
    #endregion
}