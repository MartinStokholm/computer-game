using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestIcon : MonoBehaviour
{
    [Header("Icons")] 
    [SerializeField] private GameObject _requirementsNotMetToStartIcon;
    [SerializeField] private GameObject _canStartIcon;
    [SerializeField] private GameObject _requirementsNotMetToFinishIcon;
    [SerializeField] private GameObject _canFinishIcon;

    public void SetState(QuestState newState, bool startingPoint, bool endingPoint)
    {
        _requirementsNotMetToStartIcon.SetActive(false);
        _canStartIcon.SetActive(false);
        _requirementsNotMetToFinishIcon.SetActive(false);
        _canFinishIcon.SetActive(false);
        
        switch (newState)
        {
            case QuestState.REQUIREMENTS_NOT_MET:
                if (startingPoint) 
                    _requirementsNotMetToStartIcon.SetActive(true);
                break;
            case QuestState.CAN_START:
                if (startingPoint) 
                    _canStartIcon.SetActive(true);
                break;
            case QuestState.IN_PROGRESS:
                if (endingPoint) 
                    _requirementsNotMetToFinishIcon.SetActive(true);
                break;
            case QuestState.CAN_FINISH:
                if (endingPoint) 
                    _canFinishIcon.SetActive(true);
                break;
            case QuestState.FINISHED:
                _requirementsNotMetToStartIcon.SetActive(false);
                _canStartIcon.SetActive(false);
                _requirementsNotMetToFinishIcon.SetActive(false);
                _canFinishIcon.SetActive(false);
                break;
            default:
                Debug.LogWarning($"Quest State not recognized by switch statement for quest icon {newState}");
                break;
        }
    }
}
