using System.Collections;
using UnityEngine;
using UnityEngine.Serialization;

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
    public int _currentHealth { get; private set; }
    private HealthEvent _healthEvent;
    private Player _player;
    private Coroutine _immunityCoroutine;
    private bool _isImmuneAfterHit = false;
    private float _immunityTime = 0f;
    private SpriteRenderer _spriteRenderer = null;
    private const float _spriteFlashInterval = 0.2f;
    private WaitForSeconds _waitForSecondsSpriteFlashInterval = new(_spriteFlashInterval);
    [HideInInspector] public bool IsDamageable = true;
    [FormerlySerializedAs("enemy")] [HideInInspector] public Enemy _enemy;

    private void Awake()
    {
        _healthEvent = GetComponent<HealthEvent>();
    }

    private void Start()
    {
        // Trigger a health event for UI update
        CallHealthEvent(0);

        // Attempt to load enemy / player components
        _player = GetComponent<Player>();
        _enemy = GetComponent<Enemy>();


        // Get player / enemy hit immunity details
        if (_player is not null)
        {
            // if (_player.PlayerDetails.IsImmuneAfterHit)
            // {
            //     _isImmuneAfterHit = true;
            //     //_immunityTime = _player.PlayerDetails.HitImmunityTime;
            //     _spriteRenderer = _player.SpriteRenderer;
            // }
        }
        else if (_enemy is not null)
        {
            if (_enemy.EnemyDetails.IsImmuneAfterHit)
            {
                _isImmuneAfterHit = true;
                _immunityTime = _enemy.EnemyDetails.HitImmunityTime;
                _spriteRenderer = _enemy.spriteRendererArray[0];
            }
        }

        // Enable the health bar if required
        if (_enemy != null && _enemy.EnemyDetails.IsHealthBarDisplayed == true && _healthBar != null)
        {
            _healthBar.EnableHealthBar();
        }
        else if (_healthBar != null)
        {
            _healthBar.DisableHealthBar();
        }
    }
    
    /// <summary>
    /// Set starting health 
    /// </summary>
    public void SetStartingHealth(int startingHealth)
    {
        StartingHealth = startingHealth;
        _currentHealth = startingHealth;
    }

    public void LoseHealth(int damage)
    {
        _currentHealth -= damage;
    }

    /// <summary>
    /// Public method called when damage is taken
    /// </summary>
    public void TakeDamage(int damageAmount)
    {
        Debug.Log("TakeDamage: " + damageAmount);
        if (!IsDamageable) return;
        
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

        Debug.Log($"Curren amount of health{_currentHealth}");
        CallHealthEvent(0);
    }
}