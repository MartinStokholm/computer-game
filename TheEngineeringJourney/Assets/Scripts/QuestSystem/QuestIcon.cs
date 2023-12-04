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

    public void SetState(QuestState newState, bool startingPoint, bool endingPonint)
    {
        SetStateToFalse();
        Debug.Log($"Quest icon: {newState}");
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
                if (endingPonint) 
                    _requirementsNotMetToFinishIcon.SetActive(true);
                break;
            case QuestState.CAN_FINISH:
                if (endingPonint) 
                    _canFinishIcon.SetActive(true);
                break;
            case QuestState.FINISHED:
                break;
            default:
                Debug.LogWarning($"Quest State not recognized by switch statement for quest icon {newState}");
                break;
        }
    }

    private void SetStateToFalse()
    {
        _requirementsNotMetToStartIcon.SetActive(false);
        _canStartIcon.SetActive(false);
        _requirementsNotMetToFinishIcon.SetActive(false);
        _canFinishIcon.SetActive(false);
    }
}
