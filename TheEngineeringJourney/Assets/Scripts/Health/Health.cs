using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(HealthEvent))]
[DisallowMultipleComponent]
public class Health : MonoBehaviour
{
    #region Header References
    [Space(10)]
    [Header("References")]
    #endregion
    #region Tooltip
    [Tooltip("Populate with the HealthBar component on the HealthBar gameobject")]
    #endregion
    [SerializeField] private HealthBar _healthBar;
    public int StartingHealth { get; private set; }
    private int _currentHealth;
    private HealthEvent _healthEvent;
    private Player _player;
    private Coroutine _immunityCoroutine;
    private bool _isImmuneAfterHit = false;
    private float _immunityTime = 0f;
    private SpriteRenderer _spriteRenderer = null;
    private const float _spriteFlashInterval = 0.2f;
    private WaitForSeconds _waitForSecondsSpriteFlashInterval = new(_spriteFlashInterval);
    [HideInInspector] public bool IsDamageable = true;
    [HideInInspector] public Enemy enemy;

    /// <summary>
    /// Set starting health 
    /// </summary>
    public void SetStartingHealth(int startingHealth)
    {
        StartingHealth = startingHealth;
        _currentHealth = startingHealth;
    }

    /// <summary>
    /// Get the starting health
    /// </summary>
    public int GetStartingHealth() => StartingHealth;

    public void LoseHealth(int damage)
    {
        _currentHealth -= damage;
    }

    /// <summary>
    /// Public method called when damage is taken
    /// </summary>
    public void TakeDamage(int damageAmount)
    {
        if (!(IsDamageable)) return;
        
        _currentHealth -= damageAmount;
        CallHealthEvent(damageAmount);

        PostHitImmunity();

        // Set health bar as the percentage of health remaining
        if (_healthBar == null) return;
        
        _healthBar.SetHealthBarValue((float)_currentHealth / StartingHealth);
    }
    
    /// <summary>
    /// Indicate a hit and give some post hit immunity
    /// </summary>
    private void PostHitImmunity()
    {
        // Check if gameobject is active - if not return
        if (!gameObject.activeSelf) return;

        // If there is post hit immunity then
        if (!_isImmuneAfterHit) return;
        
        if (_immunityCoroutine != null)
            StopCoroutine(_immunityCoroutine);

        // flash red and give period of immunity
        _immunityCoroutine = StartCoroutine(PostHitImmunityRoutine(_immunityTime, _spriteRenderer));
    }
    
    /// <summary>
    /// Coroutine to indicate a hit and give some post hit immunity
    /// </summary>
    private IEnumerator PostHitImmunityRoutine(float immunityTime, SpriteRenderer spriteRenderer)
    {
        var iterations = Mathf.RoundToInt(immunityTime / _spriteFlashInterval / 2f);

        IsDamageable = false;

        while (iterations > 0)
        {
            spriteRenderer.color = Color.red;

            yield return _waitForSecondsSpriteFlashInterval;

            spriteRenderer.color = Color.white;

            yield return _waitForSecondsSpriteFlashInterval;

            iterations--;

            yield return null;

        }

        IsDamageable = true;
    }
    
    private void CallHealthEvent(int damageAmount)
    {
        _healthEvent.CallHealthChangedEvent(((float)_currentHealth / StartingHealth), _currentHealth, damageAmount);
    }
    
    
    /// <summary>
    /// Increase health by specified percent
    /// </summary>
    public void AddHealth(int healthPercent)
    {
        var healthIncrease = Mathf.RoundToInt((StartingHealth * healthPercent) / 100f);

        var totalHealth = _currentHealth + healthIncrease;

        _currentHealth = totalHealth > StartingHealth 
            ? StartingHealth 
            : totalHealth;

        CallHealthEvent(0);
    }
}