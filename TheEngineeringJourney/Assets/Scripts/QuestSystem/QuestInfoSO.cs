using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(fileName = "QuestInfo_", menuName = "Scriptable Objects/Quest")]
public class QuestInfoSO : ScriptableObject
{
    [field: SerializeField] public string id { get; private set; }

    [Header("General")] 
    public string DisplayName;
    
    [FormerlySerializedAs("levelRequirement")] [Header("Requirements")]
    public  int LevelRequirement;
    public QuestInfoSO[] QuestPrerequisites;

    [Header("Steps")] 
    [field: SerializeField]public GameObject[] QuestStepPrefabs;

    [Header("Rewards")] 
    public int GoldReward;

    public int ExperienceReward;
    
    private void OnValidate()
    {
        #if UNITY_EDITOR
        id = this.name;
        UnityEditor.EditorUtility.SetDirty(this);
        #endif
    }
}
