using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerLevelManager : MonoBehaviour
{
    [Header("Configuration")] 
    [SerializeField] private int _startingLevel = 1;

    [SerializeField] private int _startingExperience = 0;

    private int _level;
    private int _experience;

    private void Awake()
    {
        _level = _startingLevel;
        _experience = _startingExperience;
    }

    private void OnEnable()
    {
        GameManager.Instance.PlayerEvents.OnExperienceGained += ExperienceGained;
    }

    private void OnDisable() 
    {
        GameManager.Instance.PlayerEvents.OnExperienceGained -= ExperienceGained;
    }

    private void Start()
    {
        GameManager.Instance.PlayerEvents.PlayerLevelChange(_level);
        GameManager.Instance.PlayerEvents.PlayerExperienceChange(_experience);
    }
    
    private void ExperienceGained(int experience) 
    {
        _experience += experience;
        // check if we're ready to level up
        while (_experience >= Settings.ExperienceToLevelUp) 
        {
            _experience -= Settings.ExperienceToLevelUp;
            _level++;
            GameManager.Instance.PlayerEvents.PlayerLevelChange(_level);
        }
        GameManager.Instance.PlayerEvents.PlayerExperienceChange(_experience);
    }
}
