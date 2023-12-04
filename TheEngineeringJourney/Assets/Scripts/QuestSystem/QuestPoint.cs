using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CircleCollider2D))]
public class QuestPoint : MonoBehaviour
{
    [Header("Quest")] 
    [SerializeField] private QuestInfoSO _questInfoForPoint;

    private bool _playerIsNear;
    private string _questId;
    private QuestState _questState;

    private void Awake()
    {
        _questId = _questInfoForPoint.id;
    }

    private void OnEnable()
    {
        GameManager.Instance.QuestEvents.OnQuestStateChange += QuestStateChange;
        GameManager.Instance.InputEvents.OnSubmitPressed += SubmitPressed;
    }
    
    private void OnDisable()
    {
        GameManager.Instance.QuestEvents.OnQuestStateChange -= QuestStateChange;
        GameManager.Instance.InputEvents.OnSubmitPressed -= SubmitPressed;
    }

    private void SubmitPressed()
    {
        if (!_playerIsNear) return;
        
        GameManager.Instance.QuestEvents.StartQuest(_questId);
        GameManager.Instance.QuestEvents.AdvanceQuest(_questId);
        GameManager.Instance.QuestEvents.FinishQuest(_questId);
    }

    public void QuestStateChange(Quest quest)
    {
        if (!quest._info.Equals(_questId)) return;
        
        _questState = quest.State;
        Debug.Log($"Quest with {_questId} update to state {_questState}");
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag(Settings.PlayerTag))
        {
            _playerIsNear = true;
        }
    }
    
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag(Settings.PlayerTag))
        {
            _playerIsNear = false;
        }
    }
}
