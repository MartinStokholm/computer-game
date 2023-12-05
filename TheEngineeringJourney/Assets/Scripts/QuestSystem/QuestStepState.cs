using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class QuestStepState
{
    public string State;

    public QuestStepState(string state)
    {
        State = state;
    }

    public QuestStepState()
    {
        State = "";
    }
}
