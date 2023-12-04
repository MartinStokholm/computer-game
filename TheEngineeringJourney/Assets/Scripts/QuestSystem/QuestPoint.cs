using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CircleCollider2D))]
public class QuestPoint : MonoBehaviour
{
    [Header("Quest")] 
    [SerializeField] private QuestInfoSO _questInfoForPoint;

    [Header("Config")] 
    [SerializeField] private bool StartingPoint = true;
    [SerializeField] private bool EndingPoint = true;
    
    private bool _playerIsNear;
    private string _questId;
    private QuestState _questState;
    private QuestIcon _questIcon;

    private void Awake()
    {
        _questId = _questInfoForPoint.id;
        _questIcon = GetComponentInChildren<QuestIcon>();
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

        if (_questState.Equals(QuestState.CAN_START) && StartingPoint)
        {
            GameManager.Instance.QuestEvents.StartQuest(_questId);
        } 
        else if (_questState.Equals(QuestState.CAN_FINISH) && EndingPoint)
        {
            GameManager.Instance.QuestEvents.FinishQuest(_questId);
        }
    }

    private void QuestStateChange(Quest quest)
    {
        if (!quest._info.Equals(_questId)) return;
        
        _questState = quest.State;
        _questIcon.SetState(_questState, StartingPoint, EndingPoint);
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
