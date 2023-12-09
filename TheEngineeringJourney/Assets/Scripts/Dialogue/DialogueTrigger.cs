using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueTrigger : MonoBehaviour
{
    [Header("Visual Cue")] 
    [SerializeField] private GameObject _visualCue;

    [Header("Ink JSON")] 
    [SerializeField] private TextAsset _inkJSON;
    
    private bool _playerIsNear;

    private void Awake()
    {
        _playerIsNear = false;
        _visualCue.SetActive(false);
    }

    private void Update()
    {
        if (_playerIsNear)
        {
            _visualCue.SetActive(true);
        }
        else
        {
            _visualCue.SetActive(false);
        }
    }
    
    private void OnEnable()
    {
        GameManager.Instance.InputEvents.OnSubmitPressed += SubmitPressed;
    }
    
    private void OnDisable()
    {
        GameManager.Instance.InputEvents.OnSubmitPressed -= SubmitPressed;
    }

    private void SubmitPressed()
    {
        if (!_playerIsNear) return;
        
        switch (_playerIsNear)
        {
            case true when !DialogueManager.Instance.IsDialoguePlaying:
                DialogueManager.Instance.EnterDialogueMode(_inkJSON);
                break;
            case false :
                break;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag(Settings.PlayerTag)){
            _playerIsNear = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag(Settings.PlayerTag)){
            _playerIsNear = false;
        }
    }
}
