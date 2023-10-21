using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(fileName = "CurrentPlayer", menuName = "Scriptable Objects/Player/Current Player")]
public class CurrentPlayerSO : ScriptableObject
{
    [FormerlySerializedAs("playerDetails")] public PlayerDetailsSO PlayerDetails;
    [FormerlySerializedAs("playerName")] public string PlayerName;

}