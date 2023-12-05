using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;

public class QuestManager : MonoBehaviour
{
    [FormerlySerializedAs("loadQuestState")]
    [Header("Config")]
    [SerializeField] private bool _loadQuestState = true;
    
    private Dictionary<string, Quest> _questMap;
    private int _playerLevel;

    private void Awake()
    {
        _questMap = CreateQuestMap();
    }

    private void OnEnable()
    {
        GameManager.Instance.QuestEvents.OnStartQuest += StartQuest;
        GameManager.Instance.QuestEvents.OnAdvanceQuest += AdvanceQuest;
        GameManager.Instance.QuestEvents.OnFinishQuest += FinishQuest;
        GameManager.Instance.QuestEvents.OnQuestStepStateChange += QuestStepStateChange;

        GameManager.Instance.PlayerEvents.OnPlayerLevelChange += PlayerLevelChange;
        StartCoroutine(DelayedStart());
    }
    
    private void OnDisable()
    {
        GameManager.Instance.QuestEvents.OnStartQuest -= StartQuest;
        GameManager.Instance.QuestEvents.OnAdvanceQuest -= AdvanceQuest;
        GameManager.Instance.QuestEvents.OnFinishQuest -= FinishQuest;
        GameManager.Instance.QuestEvents.OnQuestStepStateChange += QuestStepStateChange;
        
        GameManager.Instance.PlayerEvents.OnPlayerLevelChange -= PlayerLevelChange;
        
    }

    private IEnumerator DelayedStart()
    {
        yield return new WaitForSeconds(0.01f);

        // Call your Start method here
        Starts();
    }
    
    /// <summary>
    /// Broadcast the initial state of all quest on startup
    /// </summary>
    private void Starts()
    {
        foreach (var quest in _questMap.Values)
        {
            if (quest.State is QuestState.IN_PROGRESS)
            {
                quest.InstantiateCurrentQuestStep(transform);
            }

            GameManager.Instance.QuestEvents.QuestStateChange(quest);
        }
    }
    
    private void ChangeQuestState(string id, QuestState state)
    {
        var quest = GetQuestById(id);
        quest.State = state;
        GameManager.Instance.QuestEvents.QuestStateChange(quest);
    }
    
    private void QuestStepStateChange(string id, int stepIndex, QuestStepState questStepState)
    {
        var quest = GetQuestById(id);
        quest.StoreQuestStepState(questStepState, stepIndex);
        ChangeQuestState(id, quest.State);
    }

    private void StartQuest(string id)
    {
        Debug.Log($"Start Quest: {id}");
        var quest = GetQuestById(id);
        quest.InstantiateCurrentQuestStep(transform);
        ChangeQuestState(quest._info.id, QuestState.IN_PROGRESS);
    }
    
    /// <summary>
    /// Either progress or mark it able to finish quest
    /// </summary>
    private void AdvanceQuest(string id)
    {
        Debug.Log($"Advance Quest: {id}");
        var quest = GetQuestById(id);
        quest.MoveToNextStep();

        if (quest.CurrentStepExists())
        {
            quest.InstantiateCurrentQuestStep(transform);
        }
        else
        {
            ChangeQuestState(quest._info.id, QuestState.CAN_FINISH);
        }
    }
    
    private void FinishQuest(string id)
    {
        var quest = GetQuestById(id);
        ClaimRewards(quest);
        ChangeQuestState(quest._info.id, QuestState.FINISHED);
    }

    /// <summary>
    /// For implementing more type of rewards create new events
    /// </summary>
    private static void ClaimRewards(Quest quest)
    {
        GameManager.Instance.GoldEvents.GoldGained(quest._info.GoldReward);
        GameManager.Instance.PlayerEvents.ExperienceGained(quest._info.ExperienceReward);
    }

    private bool CheckRequirementsMet(Quest quest)
    {
        // Check PLayer Level requirements
        if (_playerLevel < quest._info.LevelRequirement)
            return false;
        
        // TODO: Add game level requirements completion of main levels

      
        // Check quest prerequisites for completion
        return quest._info.QuestPrerequisites
            .All(prerequisiteQuestInfo => GetQuestById(prerequisiteQuestInfo.id).State is QuestState.FINISHED);
    }

    private void Update()
    {
        var quest = _questMap.Values
             .Where(quest => quest.State is QuestState.REQUIREMENTS_NOT_MET && CheckRequirementsMet(quest))
             .ToList();
        
         quest.ForEach(x => Debug.Log( $"ID: {x._info.id}, Quest State: {QuestState.CAN_START}"));

        quest.ForEach(x => ChangeQuestState(x._info.id, QuestState.CAN_START));
    }

    private void PlayerLevelChange(int level) => 
        _playerLevel = level;



    /// <summary>
    /// Load all QuestInfoSO Scriptable Objects under the folder Assets/Resources/Quest
    /// </summary>
    /// <returns></returns>
    private Dictionary<string, Quest> CreateQuestMap()
    {
        var quests = Resources.LoadAll<QuestInfoSO>("Quests");
        var idToQuestMap = new Dictionary<string, Quest>();

        foreach (var questInfo in quests)
        {
            if (idToQuestMap.ContainsKey(questInfo.id))
            {
                Debug.LogWarning($"Duplicate ID found when creating quest map: {questInfo.id}");
            }

            idToQuestMap.Add(questInfo.id, LoadQuest(questInfo));
        }

        return idToQuestMap;
    }

    private Quest GetQuestById(string id)
    {
        var quest = _questMap[id];

        if (quest is null)
        {
            Debug.LogError("ID not found in the Quest Map: " + id);
        }
        return quest;
    }
    
    private void OnApplicationQuit()
    {
        foreach (var quest in _questMap.Values)
        {
            SaveQuest(quest);
        }
    }

    /// <summary>
    /// Save to PlayerPrefs but should be Save & Load system and write to a file, the cloud, etc.
    /// </summary>
    private static void SaveQuest(Quest quest)
    {
        try 
        {
            var questData = quest.GetQuestData();
            var serializedData = JsonUtility.ToJson(questData);

            PlayerPrefs.SetString(quest._info.id, serializedData);
            Debug.Log(serializedData);
        }
        catch (Exception e)
        {
            Debug.LogError("Failed to save quest with id " + quest._info.id + ": " + e);
        }
    }
    
    /// <summary>
    /// Load from PlayerPrefs but should be Save & Load system and write to a file, the cloud, etc.
    /// load quest from saved data or otherwise, initialize a new quest
    /// </summary>
    private Quest LoadQuest(QuestInfoSO questInfo)
    {
        Quest quest = null;
        try 
        {
            
            if (PlayerPrefs.HasKey(questInfo.id) && _loadQuestState)
            {
                var serializedData = PlayerPrefs.GetString(questInfo.id);
                var questData = JsonUtility.FromJson<QuestData>(serializedData);
                Debug.Log(serializedData);
                quest = new Quest(questInfo, questData.State, questData.QuestStepIndex, questData.QuestStepStates);
            }
            else 
            {
                quest = new Quest(questInfo);
            }
        }
        catch (Exception e)
        {
            Debug.LogError("Failed to load quest with id " + quest._info.id + ": " + e);
        }
        return quest;
    }
}
