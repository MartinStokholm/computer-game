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
        if (mainCamera == null) mainCamera = Camera.main;

        var mouseScreenPosition = Input.mousePosition;

        mouseScreenPosition.x = Mathf.Clamp(mouseScreenPosition.x, 0f, Screen.width);
        mouseScreenPosition.y = Mathf.Clamp(mouseScreenPosition.y, 0f, Screen.width);

        var worldPosition = mainCamera.ScreenToWorldPoint(mouseScreenPosition);

        worldPosition.z = 0f;

        return worldPosition;
    }
    
    /// <summary>
    /// Get the camera viewport lower and upper bounds
    /// </summary>
    public static void CameraWorldPositionBounds(out Vector2Int cameraWorldPositionLowerBounds, out Vector2Int cameraWorldPositionUpperBounds, Camera camera)
    {
        var worldPositionViewportBottomLeft = camera.ViewportToWorldPoint(new Vector3(0f, 0f, 0f));
        var worldPositionViewportTopRight = camera.ViewportToWorldPoint(new Vector3(1f, 1f, 0f));

        cameraWorldPositionLowerBounds = new Vector2Int((int)worldPositionViewportBottomLeft.x, (int)worldPositionViewportBottomLeft.y);
        cameraWorldPositionUpperBounds = new Vector2Int((int)worldPositionViewportTopRight.x, (int)worldPositionViewportTopRight.y);
    }
}
