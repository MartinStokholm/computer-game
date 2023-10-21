using Cinemachine;
using UnityEngine;
using UnityEngine.Serialization;

[RequireComponent(typeof(CinemachineTargetGroup))]
public class CinemachineTarget : MonoBehaviour
{
    private CinemachineTargetGroup _cinemachineTargetGroup;
    
    //#region Tooltip
    //[FormerlySerializedAs("cursorTarget")]
    //[Tooltip("Populate with the CursorTarget game object")]
    //#endregion Tooltip
    //[SerializeField] private Transform _cursorTarget;

    private void Awake()
    {
        _cinemachineTargetGroup = GetComponent<CinemachineTargetGroup>();
    }

    private void Start()
    {
        SetCinemachineTargetGroup();
    }

    private void SetCinemachineTargetGroup()
    {
        var player = new CinemachineTargetGroup.Target
            { weight = 1f, radius = 1f, target = GameManager.Instance.Player.transform };
        
        //var cursor = new CinemachineTargetGroup.Target { weight = 1f, radius = 1f, target = _cursorTarget };
        
        var cinemachineTargetGroups = new[] { player };
        _cinemachineTargetGroup.m_Targets = cinemachineTargetGroups;
    }
    
    //private void Update()
    //{
    //    _cursorTarget.position = GameUtilities.GetMouseWorldPosition();
    //}
}
