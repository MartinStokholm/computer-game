using UnityEngine;

[DisallowMultipleComponent]
public class WeaponShootEffect : MonoBehaviour
{
    private ParticleSystem _shootEffectParticleSystem;

    private void Awake()
    {
        _shootEffectParticleSystem = GetComponent<ParticleSystem>();
    }

    /// <summary>
    /// Set the Shoot Effect from the passed in WeaponShootEffectSO and aimAngle
    /// </summary>
    public void SetShootEffect(WeaponShootEffectSO shootEffect, float aimAngle)
    {
        // Set shoot effect color gradient
        SetShootEffectColorGradient(shootEffect.ColorGradient);

        // Set shoot effect particle system starting values
        SetShootEffectParticleStartingValues(
            shootEffect.Duration, 
            shootEffect.StartParticleSize, 
            shootEffect.StartParticleSpeed, 
            shootEffect.StartLifetime, 
            shootEffect.EffectGravity, 
            shootEffect.MaxParticleNumber);

        // Set shoot effect particle system particle burst particle number
        SetShootEffectParticleEmission(shootEffect.EmissionRate, shootEffect.BurstParticleNumber);

        // Set emitter rotation
        SetEmitterRotation(aimAngle);

        // Set shoot effect particle sprite
        SetShootEffectParticleSprite(shootEffect.Sprite);

        // Set shoot effect lifetime min and max velocities
        SetShootEffectVelocityOverLifeTime(shootEffect.VelocityOverLifetimeMin, shootEffect.VelocityOverLifetimeMax);

    }

    /// <summary>
    /// Set the shoot effect particle system color gradient
    /// </summary>
    private void SetShootEffectColorGradient(Gradient gradient)
    {
        // Set colour gradient
        var colorOverLifetimeModule = _shootEffectParticleSystem.colorOverLifetime;
        colorOverLifetimeModule.color = gradient;
    }

    /// <summary>
    /// Set shoot effect particle system starting values
    /// </summary>
    private void SetShootEffectParticleStartingValues(float duration, float startParticleSize, float startParticleSpeed, float startLifetime, float effectGravity, int maxParticles)
    {
        var mainModule = _shootEffectParticleSystem.main;
        
        mainModule.duration = duration;
        mainModule.startSize = startParticleSize;
        mainModule.startSpeed = startParticleSpeed;
        mainModule.startLifetime = startLifetime;
        mainModule.gravityModifier = effectGravity;
        mainModule.maxParticles = maxParticles;
    }

    /// <summary>
    /// Set shoot effect particle system particle burst particle number
    /// </summary>
    private void SetShootEffectParticleEmission(int emissionRate, float burstParticleNumber)
    {
        var emissionModule = _shootEffectParticleSystem.emission;

        // Set particle burst number
        var burst = new ParticleSystem.Burst(0f, burstParticleNumber);
        emissionModule.SetBurst(0, burst);

        // Set particle emission rate
        emissionModule.rateOverTime = emissionRate;
    }

    /// <summary>
    /// Set shoot effect particle system sprite
    /// </summary>
    private void SetShootEffectParticleSprite(Sprite sprite)
    {
        // Set particle burst number
        var textureSheetAnimationModule = _shootEffectParticleSystem.textureSheetAnimation;

        textureSheetAnimationModule.SetSprite(0, sprite);

    }

    /// <summary>
    /// Set the rotation of the emitter to match the aim angle
    /// </summary>
    private void SetEmitterRotation(float aimAngle)
    {
        transform.eulerAngles = new Vector3(0f, 0f, aimAngle);
    }

    /// <summary>
    /// Set the shoot effect velocity over lifetime
    /// </summary>
    private void SetShootEffectVelocityOverLifeTime(Vector3 minVelocity, Vector3 maxVelocity)
    {
        var velocityOverLifetimeModule = _shootEffectParticleSystem.velocityOverLifetime;
        
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