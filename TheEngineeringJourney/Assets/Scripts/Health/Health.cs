using UnityEngine;

[DisallowMultipleComponent]
public class Health : MonoBehaviour
{
    private int _startingHealth;
    private int _currentHealth;

    /// <summary>
    /// Set starting health 
    /// </summary>
    public void SetStartingHealth(int startingHealth)
    {
        _startingHealth = startingHealth;
        _currentHealth = startingHealth;
    }

    /// <summary>
    /// Get the starting health
    /// </summary>
    public int GetStartingHealth() => _startingHealth;

    public void LoseHealth(int healthlost)
    {
        _currentHealth -= healthlost;
    }

}