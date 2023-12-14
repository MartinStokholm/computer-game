using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenuUI : MonoBehaviour
{
    [SerializeField]private GameObject TutorialMenu;
    [SerializeField]private GameObject ReturnToMainMenu;
    [SerializeField]private GameObject OptionsMenu;
    [SerializeField]private GameObject QuitMenu;
    //[SerializeField]private GameObject Cursor;
    
    // Start is called before the first frame update
    void Start()
    {
        gameObject.SetActive(false);
        HideSubMenu();
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
