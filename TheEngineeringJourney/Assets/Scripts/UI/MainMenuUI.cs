using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuUI : MonoBehaviour
{
    [SerializeField]private GameObject StartGameButton;
    //[SerializeField]private GameObject LoadMenu;
    [SerializeField]private GameObject ReturnToMainMenu;
    [SerializeField]private GameObject QuitMenu;
    [SerializeField]private GameObject TutorialMenu;
    [SerializeField]private GameObject OptionsMenu;
    
    private void Start()
    {
        MusicManager.Instance.PlayMusic(GameResources.Instance.MainMenuMusic, 0f, 2f);
        HideSubMenu();
    }

    public void StartGame()
    {
        gameObject.SetActive(false);
        //SceneManager.LoadScene("MainGameScene");
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
}
