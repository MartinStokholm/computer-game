using UnityEngine;

[DisallowMultipleComponent]
public class AmmoHitEffect : MonoBehaviour
{
    private ParticleSystem ammoHitEffectParticleSystem;

    private void Awake()
    {
        ammoHitEffectParticleSystem = GetComponent<ParticleSystem>();
    }

    /// <summary>
    /// Set Ammo Hit Effect from passed in AmmoHitEffectSO details
    /// </summary>
    public void SetHitEffect(AmmoHitEffectSO ammoHitEffect)
    {
        // Set ammo effect color gradient
        SetHitEffectColorGradient(ammoHitEffect.ColorGradient);

        // Set hit effect particle system starting values
        SetHitEffectParticleStartingValues(
            ammoHitEffect.Duration,
            ammoHitEffect.StartParticleSize,
            ammoHitEffect.StartParticleSpeed,
            ammoHitEffect.StartLifetime,
            ammoHitEffect.EffectGravity,
            ammoHitEffect.MaxParticleNumber);

        // Set hit effect particle system particle burst particle number
        SetHitEffectParticleEmission(ammoHitEffect.EmissionRate, ammoHitEffect.BurstParticleNumber);

        // Set hit effect particle sprite
        SetHitEffectParticleSprite(ammoHitEffect.Sprite);

        // Set hit effect lifetime min and max velocities
        SetHitEffectVelocityOverLifeTime(ammoHitEffect.VelocityOverLifetimeMin, ammoHitEffect.VelocityOverLifetimeMax);

    }

    /// <summary>
    /// Set the ammo effect particle system color gradient
    /// </summary>
    private void SetHitEffectColorGradient(Gradient gradient)
    {
        // Set colour gradient
        var colorOverLifetimeModule = ammoHitEffectParticleSystem.colorOverLifetime;
        colorOverLifetimeModule.color = gradient;
    }

    /// <summary>
    /// Set hit effect particle system starting values
    /// </summary>
    private void SetHitEffectParticleStartingValues(float duration, float startParticleSize, float startParticleSpeed,
        float startLifetime, float effectGravity, int maxParticles)
    {
        var mainModule = ammoHitEffectParticleSystem.main;
        mainModule.duration = duration;
        mainModule.startSize = startParticleSize;
        mainModule.startSpeed = startParticleSpeed;
        mainModule.startLifetime = startLifetime;
        mainModule.gravityModifier = effectGravity;
        mainModule.maxParticles = maxParticles;
    }

    /// <summary>
    /// Set hit effect particle system particle burst particle number
    /// </summary>
    private void SetHitEffectParticleEmission(int emissionRate, float burstParticleNumber)
    {
        var emissionModule = ammoHitEffectParticleSystem.emission;

        var burst = new ParticleSystem.Burst(0f, burstParticleNumber);
        emissionModule.SetBurst(0, burst);

        // Set particle emission rate
        emissionModule.rateOverTime = emissionRate;
    }

    /// <summary>
    /// Set hit effect particle system sprite
    /// </summary>
    private void SetHitEffectParticleSprite(Sprite sprite)
    {
        // Set particle burst number
        var textureSheetAnimationModule = ammoHitEffectParticleSystem.textureSheetAnimation;

        textureSheetAnimationModule.SetSprite(0, sprite);
    }

    /// <summary>
    /// Set the ammo effect velocity over lifetime
    /// </summary>
    private void SetHitEffectVelocityOverLifeTime(Vector3 minVelocity, Vector3 maxVelocity)
    {
        var velocityOverLifetimeModule = ammoHitEffectParticleSystem.velocityOverLifetime;

        var minMaxCurveX = new ParticleSystem.MinMaxCurve
        {
            mode = ParticleSystemCurveMode.TwoConstants,
            constantMin = minVelocity.x,
            constantMax = maxVelocity.x
        };
        velocityOverLifetimeModule.x = minMaxCurveX;

        var minMaxCurveY = new ParticleSystem.MinMaxCurve
        {
            mode = ParticleSystemCurveMode.TwoConstants,
            constantMin = minVelocity.y,
            constantMax = maxVelocity.y
        };
        velocityOverLifetimeModule.y = minMaxCurveY;

        var minMaxCurveZ = new ParticleSystem.MinMaxCurve
        {
            mode = ParticleSystemCurveMode.TwoConstants,
            constantMin = minVelocity.z,
            constantMax = maxVelocity.z
        };
        velocityOverLifetimeModule.z = minMaxCurveZ;
    }
}