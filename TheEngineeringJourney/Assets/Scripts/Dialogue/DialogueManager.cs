using System.Collections;
using Ink.Runtime;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class DialogueManager : SingletonMonobehaviour<DialogueManager>
{
    [Header("Params")]
    [SerializeField] public float TypingSpeed = 0.04f;
    
    [Header("Load Globals JSON")]
    [SerializeField] private TextAsset _loadGlobalsJSON;
    
    [Header("Dialogue UI")] 
    [SerializeField] private GameObject _dialoguePanel;

    [SerializeField] private TextMeshProUGUI _dialogueText;

    [Header("Choices UI")] 
    [SerializeField] private GameObject[] _choices;
    private TextMeshProUGUI[] _choicesText;
    
    [Header("Audio")]
    // [SerializeField] private DialogueAudioInfoSO defaultAudioInfo;
    // [SerializeField] private DialogueAudioInfoSO[] audioInfos;
    // [SerializeField] private bool makePredictable;
    // private DialogueAudioInfoSO currentAudioInfo;
    // private Dictionary<string, DialogueAudioInfoSO> audioInfoDictionary;
    // private AudioSource audioSource;
    
    private DialogueVariables _dialogueVariables;
    private Story _currentStory;
    public bool IsDialoguePlaying { get; private set;}
    private bool _canContinueToNextLine = false;
    private Coroutine displayLineCoroutine;

    // protected override void Awake()
    // {
    //     // dialogueVariables = new DialogueVariables(loadGlobalsJSON);
    //     // inkExternalFunctions = new InkExternalFunctions();
    //     //
    //     // audioSource = this.gameObject.AddComponent<AudioSource>();
    //     // currentAudioInfo = defaultAudioInfo;
    //     base.Awake();
    // }

    private void Start()
    {
        IsDialoguePlaying = false;
        _dialoguePanel.SetActive(false);

        _choicesText = new TextMeshProUGUI[_choices.Length];
        var index = 0;
        foreach (var choice in _choices) 
        {
            _choicesText[index] = choice.GetComponentInChildren<TextMeshProUGUI>();
            index++;
        }
    }

    private void Update()
    {
        if (!IsDialoguePlaying) return;

        if (_canContinueToNextLine 
            && _currentStory.currentChoices.Count == 0 
            && InputManager.Instance.GetSubmitPressed())
        {
            ContinueStory();
        }
    }

    public void EnterDialogueMode(TextAsset inkJson)
    {
        _currentStory = new Story(inkJson.text);
        IsDialoguePlaying = true;
        _dialoguePanel.SetActive(IsDialoguePlaying);
        
        // dialogueVariables.StartListening(currentStory);
        // inkExternalFunctions.Bind(currentStory, emoteAnimator);
        //
        // // reset portrait, layout, and speaker
        // displayNameText.text = "???";
        // portraitAnimator.Play("default");
        // layoutAnimator.Play("right");
        ContinueStory();
    }

    private IEnumerator ExitDialogueMode()
    {
        yield return new WaitForSeconds(0.2f);
        
        IsDialoguePlaying = false;
        _dialoguePanel.SetActive(IsDialoguePlaying);
        _dialogueText.text = "";
    }

    private void ContinueStory()
    {
        if (_currentStory.canContinue) 
        {
            // set text for the current dialogue line
            if (displayLineCoroutine != null) 
            {
                StopCoroutine(displayLineCoroutine);
            }
            string nextLine = _currentStory.Continue();
            // handle case where the last line is an external function
            if (nextLine.Equals("") && !_currentStory.canContinue)
            {
                StartCoroutine(ExitDialogueMode());
            }
            // otherwise, handle the normal case for continuing the story
            else 
            {
                // handle tags
                //HandleTags(currentStory.currentTags);
                displayLineCoroutine = StartCoroutine(DisplayLine(nextLine));
            }
        }
        else 
        {
            StartCoroutine(ExitDialogueMode());
        }
        // if (!_currentStory.canContinue)
        // {
        //     StartCoroutine(ExitDialogueMode());
        //     return;
        // }
        //
        // if (displayLineCoroutine != null)
        // {
        //     StopCoroutine(displayLineCoroutine);
        // }
        //
        // var nextLine = _currentStory.Continue();
        //
        // if (string.IsNullOrEmpty(nextLine) && !_currentStory.canContinue)
        // {
        //     StartCoroutine(ExitDialogueMode());
        //     return;
        // }
        //
        // // handle tags
        // //HandleTags(_currentStory.currentTags);
        // displayLineCoroutine = StartCoroutine(DisplayLine(nextLine));
        
    }
    
    private IEnumerator DisplayLine(string line) 
    {
        // set the text to the full line, but set the visible characters to 0
        _dialogueText.text = line;
        _dialogueText.maxVisibleCharacters = 0;
        // hide items while text is typing
        //continueIcon.SetActive(false);
        HideChoices();

        _canContinueToNextLine = false;

        var isAddingRichTextTag = false;

        // display each letter one at a time
        foreach (var letter in line.ToCharArray())
        {
            //if the submit button is pressed, finish up displaying the line right away
            if (InputManager.Instance.GetSubmitPressed()) 
            {
                _dialogueText.maxVisibleCharacters = line.Length;
                break;
            }

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

    private IEnumerator SelectFirstChoice()
    {
        EventSystem.current.SetSelectedGameObject(null);
        yield return new WaitForEndOfFrame();
        EventSystem.current.SetSelectedGameObject(_choices[0].gameObject);
    }

    public void MakeChoice(int choiceIndex)
    {
        if (!_canContinueToNextLine) return;
        
        _currentStory.ChooseChoiceIndex(choiceIndex);
        // NOTE: The below two lines were added to fix a bug after the Youtube video was made
        InputManager.Instance.RegisterSubmitPressed(); // this is specific to my InputManager script
        ContinueStory();
    }
    
    private void HideChoices() 
    {
        foreach (var choiceButton in _choices) 
        {
            choiceButton.SetActive(false);
        }
    }
    
    public Ink.Runtime.Object GetVariableState(string variableName) 
    {
        _dialogueVariables.Variables.TryGetValue(variableName, out var variableValue);
        if (variableValue == null) 
        {
            Debug.LogWarning("Ink Variable was found to be null: " + variableName);
        }
        return variableValue;
    }
    
    // private void InitializeAudioInfoDictionary() 
    // {
    //     audioInfoDictionary = new Dictionary<string, DialogueAudioInfoSO>();
    //     audioInfoDictionary.Add(defaultAudioInfo.id, defaultAudioInfo);
    //     foreach (DialogueAudioInfoSO audioInfo in audioInfos) 
    //     {
    //         audioInfoDictionary.Add(audioInfo.id, audioInfo);
    //     }
    // }
    
    /// <summary>
    /// called anytime the application exits.
    /// Depending on your game, you may want to save variable state in other places.
    /// </summary>
    public void OnApplicationQuit() 
    {
        _dialogueVariables.SaveVariables();
    }
}
