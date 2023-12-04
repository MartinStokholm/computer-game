using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerLevelUI : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private Slider _xpSilder;
    [SerializeField] private TextMeshProUGUI _xpText;
    [SerializeField] private TextMeshProUGUI _levelText;

    private void OnEnable()
    {
        GameManager.Instance.PlayerEvents.OnPlayerExperienceChange += PlayerExperienceChange;
        GameManager.Instance.PlayerEvents.OnPlayerLevelChange += PlayerLevelChange;
    }
    
    private void OnDisable()
    {
        GameManager.Instance.PlayerEvents.OnPlayerExperienceChange += PlayerExperienceChange;
        GameManager.Instance.PlayerEvents.OnPlayerLevelChange += PlayerLevelChange;
    }
    
    
    private void PlayerExperienceChange(int experience) 
    {
        _xpSilder.value = (float) experience / (float) 100;
        _xpText.text = experience + " / " + Settings.ExperienceToLevelUp;
    }

    private void PlayerLevelChange(int level) 
    {
        _levelText.text = "Level " + level;
    }
}
