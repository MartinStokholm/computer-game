using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenuUI : MonoBehaviour
{
    [SerializeField]private GameObject TutorialMenu;
    [SerializeField]private GameObject ReturnToMainMenu;
    [SerializeField]private GameObject OptionsMenu;
    [SerializeField]private GameObject QuitMenu;
    #region Tooltip
    [Tooltip("Populate with the music volume level")]
    #endregion Tooltip
    [SerializeField] private TextMeshProUGUI musicLevelText;
    #region Tooltip
    [Tooltip("Populate with the sounds volume level")]
    #endregion Tooltip
    [SerializeField] private TextMeshProUGUI soundsLevelText;
    //[SerializeField]private GameObject Cursor;
    
    // Start is called before the first frame update
    void Start()
    {
        gameObject.SetActive(false);
        HideSubMenu();
    }
    
    private void OnEnable()
    {
        Time.timeScale = 0f;
        
        StartCoroutine(InitializeUI());
    }
    
    private void OnDisable()
    {
        Time.timeScale = 1f;
    }
    
    /// <summary>
    /// Initialize the UI text
    /// </summary>
    private IEnumerator InitializeUI()
    {
        // Wait a frame to ensure the previous music and sound levels have been set
        yield return null;

        // Initialise UI text
        soundsLevelText.SetText(SoundEffectManager.Instance.soundsVolume.ToString());
        musicLevelText.SetText(MusicManager.Instance.musicVolume.ToString());
    }
    
    public void IncreaseMusicVolume()
    {
        MusicManager.Instance.IncreaseMusicVolume();
        musicLevelText.SetText(MusicManager.Instance.musicVolume.ToString());
    }

    /// <summary>
    /// Decrease music volume - linked to from music volume decrease button in UI
    /// </summary>
    public void DecreaseMusicVolume()
    {
        MusicManager.Instance.DecreaseMusicVolume();
        musicLevelText.SetText(MusicManager.Instance.musicVolume.ToString());
    }
    
    /// <summary>
    /// Increase sounds volume - linked to from sounds volume increase button in UI
    /// </summary>
    public void IncreaseSoundsVolume()
    {
        SoundEffectManager.Instance.IncreaseSoundsVolume();
        soundsLevelText.SetText(SoundEffectManager.Instance.soundsVolume.ToString());
    }

    /// <summary>
    /// Decrease sounds volume - linked to from sounds volume decrease button in UI
    /// </summary>
    public void DecreaseSoundsVolume()
    {
        SoundEffectManager.Instance.DecreaseSoundsVolume();
        soundsLevelText.SetText(SoundEffectManager.Instance.soundsVolume.ToString());
    }

    public void TutorialMenuButton()
    {
        TutorialMenu.SetActive(true);
        ShowSubMenu();
    }
    
    public void BackToPauseMenuButton()
    {
        HideSubMenu();
        ViewDefaultMenu();
    }
    
    public void OptionsMenuButton()
    {
        OptionsMenu.SetActive(true);
        ShowSubMenu();
    }

    public void QuitMenuButton()
    {
        QuitMenu.SetActive(true);
        ShowSubMenu();
    }

    public void QuitGameButton() => Application.Quit();
    
    private void  ViewDefaultMenu()
    {
        ReturnToMainMenu.SetActive(true);
    }

    private void ShowSubMenu()
    {
        ReturnToMainMenu.SetActive(false);
    }
    
    private void HideSubMenu()
    {
        TutorialMenu.SetActive(false);
        OptionsMenu.SetActive(false);
        QuitMenu.SetActive(false);
    }

    // private void LoadCursor()
    // {
    //     const string fileName = "Assets/Textures/Cursor/MenuCursor.png";
    //     var rawData = System.IO.File.ReadAllBytes(fileName);
    //     var cursor = new Texture2D(2, 2);;
    //     cursor.LoadImage(rawData);
    //     Cursor.SetCursor(cursor, Vector2.zero, CursorMode.Auto);
    // }
}
