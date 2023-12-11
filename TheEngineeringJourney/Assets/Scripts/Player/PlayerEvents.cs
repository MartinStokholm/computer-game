using System;
using UnityEngine;

public class PlayerEvents : MonoBehaviour
{
    public event Action<int> OnExperienceGained;
    public void ExperienceGained(int experience)
    {
        OnExperienceGained?.Invoke(experience);
    }

    public event Action<int> OnPlayerLevelChange;
    public void PlayerLevelChange(int level)
    {
        OnPlayerLevelChange?.Invoke(level);
    }

    public event Action<int> OnPlayerExperienceChange;
    public void PlayerExperienceChange(int experience)
    {
        OnPlayerExperienceChange?.Invoke(experience);
    }
}
