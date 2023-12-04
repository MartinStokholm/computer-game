using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class QuestManager : MonoBehaviour
{
    private Dictionary<string, Quest> _questMap;
    public QuestEvents QuestEvents;
    private void Awake()
    {
        _questMap = CreateQuestMap();
    }

    private void OnEnable()
    {
        GameManager.Instance.QuestEvents.OnStartQuest += StartQuest;
        GameManager.Instance.QuestEvents.OnAdvanceQuest += AdvanceQuest;
        GameManager.Instance.QuestEvents.OnFinishQuest += FinishQuest;
        //GameManager.Instance.QuestEvents.
    }
    
    private void OnDisable()
    {
        GameManager.Instance.QuestEvents.OnStartQuest -= StartQuest;
        GameManager.Instance.QuestEvents.OnAdvanceQuest -= AdvanceQuest;
        GameManager.Instance.QuestEvents.OnFinishQuest -= FinishQuest;
    }

    /// <summary>
    /// Broadcast the initial state of all quest on startup
    /// </summary>
    private void Start()
    {
        var questsToBroadcast = _questMap.Values
            .Where(quest => quest.State == QuestState.IN_PROGRESS)
            .ToList();
        
        questsToBroadcast.ForEach(quest =>
        {
            quest.InstantiateCurrentQuestStep(transform);
            GameManager.Instance.QuestEvents.QuestStateChange(quest);
        });
    }

    private void StartQuest(string id)
    {
        
    }
    
    private void AdvanceQuest(string id)
    {
        
    }
    
    private void FinishQuest(string id)
    {
        
    }
    
    

    /// <summary>
    /// Load all QuestInfoSO Scriptable Objects under the folder Assets/Resources/Quest
    /// </summary>
    /// <returns></returns>
    private Dictionary<string, Quest> CreateQuestMap()
    {
        var quests = Resources.LoadAll<QuestInfoSO>("Quest");

        var idToQuestMap = new Dictionary<string, Quest>();
        foreach (var questInfo in quests)
        {
            if (idToQuestMap.ContainsKey(questInfo.id))
            {
                Debug.LogWarning($"Duplicate ID found when creating quest map: {questInfo.id}");
            }
            
            idToQuestMap.Add(questInfo.id, new Quest(questInfo));
        }

        return idToQuestMap;
    }

    private Quest GetQuestById(string id)
    {
        var quest = _questMap[id];
        if (quest == null)
        {
            Debug.LogError("ID not found in the Quest Map: " + id);
        }
        return quest;
    }
}
