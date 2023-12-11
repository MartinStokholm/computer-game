using UnityEngine;

[RequireComponent(typeof(Health))]
[DisallowMultipleComponent]
public class ReceiveContactDamage : MonoBehaviour
{
    #region Header
    [Header("The contact damage amount to receive")]
    #endregion
    [SerializeField] private int _contactDamageAmount;
    private Health _health;

    private void Awake()
    {
        _health = GetComponent<Health>();
    }

    public void TakeContactDamage(int damageAmount = 0)
    {
        if (_contactDamageAmount > 0)
            damageAmount = _contactDamageAmount;

        _health.TakeDamage(damageAmount);
    }

    #region Validation
#if UNITY_EDITOR
    private void OnValidate()
    {
        EditorUtilities.ValidateCheckPositiveValue(this, nameof(_contactDamageAmount), _contactDamageAmount, true);
    }
#endif
    #endregion
}