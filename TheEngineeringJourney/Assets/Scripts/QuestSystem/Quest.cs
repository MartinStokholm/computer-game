using System;
using UnityEngine;
using Object = UnityEngine.Object;

public class Quest
{
   /// <summary>
   /// Contains the static information about the quest
   /// </summary>
   /// <returns></returns>
   public QuestInfoSO _info;

   public QuestState State;
   private int _questStepIndex;
   private QuestStepState[] _questStepStates;

   public Quest(QuestInfoSO questInfo)
   {
      _info = questInfo;
      State = QuestState.REQUIREMENTS_NOT_MET;
      _questStepIndex = 0;
      
      _questStepStates = new QuestStepState[_info.QuestStepPrefabs.Length];
      for (var i = 0; i < _questStepStates.Length; i++)
      {
         _questStepStates[i] = new QuestStepState();
      }
   }
   
   public Quest(QuestInfoSO questInfo, QuestState questState, int currentQuestStepIndex, QuestStepState[] questStepStates)
   {
      _info = questInfo;
      State = questState;
      _questStepIndex = currentQuestStepIndex;
      _questStepStates = questStepStates;

      // if the quest step states and prefabs are different lengths,
      // something has changed during development and the saved data is out of sync.
      if (_questStepStates.Length != _info.QuestStepPrefabs.Length)
      {
         Debug.LogWarning("Quest Step Prefabs and Quest Step States are "
                          + "of different lengths. This indicates something changed "
                          + "with the QuestInfo and the saved data is now out of sync. "
                          + $"Reset your data - as this might cause issues. QuestId: {_info.id}");
      }
   }


   public void MoveToNextStep() => ++_questStepIndex;

   public bool CurrentStepExists()
   {
      return _questStepIndex <= _info.QuestPrerequisites.Length;
   }

   public void InstantiateCurrentQuestStep(Transform parentTransform)
   {
      var questStepPrefab = GetCurrentQuestStepPrefab();

      if (questStepPrefab is null) return;
      
      var questStep = Object.Instantiate(questStepPrefab, parentTransform).GetComponent<QuestStep>();
      questStep.InitializeQuestStep(_info.id, _questStepIndex, _questStepStates[_questStepIndex].State);
   }

   private GameObject GetCurrentQuestStepPrefab()
   {
      if (CurrentStepExists())
      {
         return _info.QuestStepPrefabs[_questStepIndex];
      }

      Debug.LogWarning("Tried to get quest step prefab, but stepIndex was out of range indicating that "
                          + "there's no current step: QuestId=" + _info.id + ", stepIndex=" + _questStepIndex);

      return null;
   }
   
   public void StoreQuestStepState(QuestStepState questStepState, int stepIndex)
   {
      if (stepIndex < _questStepStates.Length)
      {
         _questStepStates[stepIndex].State = questStepState.State;
      }
      else 
      {
         Debug.LogWarning("Tried to access quest step data, but stepIndex was out of range: "
                          + "Quest Id = " + _info.id + ", Step Index = " + stepIndex);
      }
   }
   public QuestData GetQuestData() => new(State, _questStepIndex, _questStepStates);
}
