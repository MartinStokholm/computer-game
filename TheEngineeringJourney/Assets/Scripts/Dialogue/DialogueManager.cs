using System;
using System.Collections;
using System.Collections.Generic;
using Ink.Runtime;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;

public class DialogueManager : SingletonMonobehaviour<DialogueManager>
{
    [Header("Params")]
    [SerializeField] public float TypingSpeed = 0.04f;
    
    [Header("Dialogue UI")] 
    [SerializeField] private GameObject _dialoguePanel;

    [SerializeField] private TextMeshProUGUI _dialogueText;

    [Header("Choices UI")] 
    [SerializeField] private GameObject[] _choices;

    private TextMeshProUGUI[] _choicesText;
    
    private Story _currentStory;
    public bool _isDialoguePlaying { get; private set;}
    private bool _canContinueToNextLine = false;
    private Coroutine displayLineCoroutine;

    private void Start()
    {
        _isDialoguePlaying = false;
        _dialoguePanel.SetActive(false);

        _choicesText = new TextMeshProUGUI[_choices.Length];
        var index = 0;
        foreach (var choice in _choices) 
        {
            _choicesText[index] = choice.GetComponentInChildren<TextMeshProUGUI>();
            index++;
        }
        //_choicesText = _choices.Select(choice => choice.GetComponentInChildren<TextMeshProUGUI>()).ToArray();
    }

    private void Update()
    {
       if (!_isDialoguePlaying) return;
       
       //ContinueStory();
    }

    public void EnterDialogueMode(TextAsset inkJSON)
    {
        _currentStory = new Story(inkJSON.text);
        _isDialoguePlaying = true;
        _dialoguePanel.SetActive(_isDialoguePlaying);
    }

    private IEnumerator ExitDialogueMode()
    {
        yield return new WaitForSeconds(0.2f);
        
        _isDialoguePlaying = false;
        _dialoguePanel.SetActive(_isDialoguePlaying);
        _dialogueText.text = "";
    }

    private void ContinueStory()
    {
        if (_currentStory.canContinue)
        {
            if (displayLineCoroutine != null) 
            {
                StopCoroutine(displayLineCoroutine);
            }
            var nextLine = _currentStory.Continue();
            
            if (string.IsNullOrEmpty(nextLine) && !_currentStory.canContinue)
            {
                StartCoroutine(ExitDialogueMode());
            }// otherwise, handle the normal case for continuing the story
            else 
            {
                // handle tags
                //HandleTags(_currentStory.currentTags);
                displayLineCoroutine = StartCoroutine(DisplayLine(nextLine));
            }
        }
        else
        {
            StartCoroutine(ExitDialogueMode());
        }
    }
    
    private IEnumerator DisplayLine(string line) 
    {
        // set the text to the full line, but set the visible characters to 0
        _dialogueText.text = line;
        _dialogueText.maxVisibleCharacters = 0;
        // hide items while text is typing
        //continueIcon.SetActive(false);
        //HideChoices();

        _canContinueToNextLine = false;

        bool isAddingRichTextTag = false;

        // display each letter one at a time
        foreach (var letter in line.ToCharArray())
        {
            // if the submit button is pressed, finish up displaying the line right away
            // if (InputManager.GetInstance().GetSubmitPressed()) 
            // {
            //     dialogueText.maxVisibleCharacters = line.Length;
            //     break;
            // }

            // check for rich text tag, if found, add it without waiting
            if (letter == '<' || isAddingRichTextTag) 
            {
                isAddingRichTextTag = true;
                if (letter == '>')
                {
                    isAddingRichTextTag = false;
                }
            }
            // if not rich text, add the next letter and wait a small time
            else 
            {
                //PlayDialogueSound(dialogueText.maxVisibleCharacters, dialogueText.text[dialogueText.maxVisibleCharacters]);
                _dialogueText.maxVisibleCharacters++;
                yield return new WaitForSeconds(TypingSpeed);
            }
        }

        // actions to take after the entire line has finished displaying
        //continueIcon.SetActive(true);
        DisplayChoices();

        _canContinueToNextLine = true;
    }

    private void DisplayChoices()
    {
        var currentChoices = _currentStory.currentChoices;

        if (currentChoices.Count > _choices.Length)
        {
            Debug.LogError($"More choices were given than the UI can support. Number of choices given {currentChoices.Count}");
        }

        var index = 0;
        foreach (var choice in currentChoices)
        {
            _choices[index].gameObject.SetActive(true);
            _choicesText[index].text = choice.text;
            ++index;
        }

        for (var i = index; i < _choices.Length; i++)
        {
            _choices[i].gameObject.SetActive(true);
        }
        
        StartCoroutine(SelectFirstChoice());
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
        Debug.Log("Got dialog event");
        if (!_isDialoguePlaying) return;
        
        switch (_isDialoguePlaying)
        {
            case true:
                Debug.Log("Should show dialog");
                ContinueStory();
                break;
            case false:
                break;
        }
    }

    private IEnumerator SelectFirstChoice()
    {
        EventSystem.current.SetSelectedGameObject(null);
        yield return new WaitForEndOfFrame();
        EventSystem.current.SetSelectedGameObject(_choices[0].gameObject);
    }

    public void MakeChoice(int choiceIndex)
    {
        //if (!_canContinueToNextLine) return;
        
        _currentStory.ChooseChoiceIndex(choiceIndex);
        // NOTE: The below two lines were added to fix a bug after the Youtube video was made
        //InputManager.GetInstance().RegisterSubmitPressed(); // this is specific to my InputManager script
        //ContinueStory();
    }
}
