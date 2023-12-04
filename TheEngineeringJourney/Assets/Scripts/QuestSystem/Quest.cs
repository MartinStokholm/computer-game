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
      _questStepStates = Array.ConvertAll(_info.QuestPrerequisites, _ => new QuestStepState());
   }


   public void MoveToNextStep() => ++_questStepIndex;

   public bool CurrentStepExists() => _questStepIndex < _info.QuestPrerequisites.Length;

   public void InstantiateCurrentQuestStep(Transform parentTransform)
   {
      var questStepPrefab = GetCurrentQuestStepPrefab();
      
      if (questStepPrefab is null) return;

      var questStep = Object.Instantiate<GameObject>(questStepPrefab, parentTransform);
   }

   private GameObject GetCurrentQuestStepPrefab()
   {
      if (CurrentStepExists()) return _info.QuestStepPrefabs[_questStepIndex];

      Debug.LogWarning("Tried to get quest step prefab, but stepIndex was out of range indicating that "
                       + $"there's no current step: QuestId={_info.id}, stepIndex= {_questStepIndex}");

      return null;
   }
   
   
}
