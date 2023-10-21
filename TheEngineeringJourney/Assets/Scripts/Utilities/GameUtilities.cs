using UnityEngine;

public static class GameUtilities
{
    public static Camera mainCamera;
    
    public static float GetAngleFromVector(Vector3 vector)
    {
        var radians = Mathf.Atan2(vector.y, vector.x);
        return radians * Mathf.Rad2Deg;
    }

    public static Vector3 GetMouseWorldPosition()
    {
        mainCamera ??= Camera.main;

        var mouseScreenPosition = Input.mousePosition;

        mouseScreenPosition.x = Mathf.Clamp(mouseScreenPosition.x, 0f, Screen.width);
        mouseScreenPosition.y = Mathf.Clamp(mouseScreenPosition.y, 0f, Screen.width);

        var worldPosition = mainCamera.ScreenToWorldPoint(mouseScreenPosition);

        worldPosition.z = 0f;

        return worldPosition;
    }
}
