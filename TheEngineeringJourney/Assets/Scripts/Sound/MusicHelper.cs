using UnityEngine;

public static class MusicHelper
{
    /// <summary>
    /// Convert the linear volume scale to decibels
    /// </summary>
    public static float LinearToDecibels(int linear)
    {
        const float linearScaleRange = 20f;

        // formula to convert from the linear scale to the logarithmic decibel scale
        return Mathf.Log10(linear / linearScaleRange) * 20f;
    }
}