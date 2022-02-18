using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    
    public GameObject Mainmenu;
    public GameObject Credits;
    public void ChangeScene(string scene)
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(scene);
    }
    public void Quitgame()
    {
        Application.Quit();
    }

    public void CreditsOn()
    {
        Mainmenu.SetActive(false);
        Credits.SetActive(true);
    }
    public void CreditsOff()
    {
        Mainmenu.SetActive(true);
        Credits.SetActive(false);
    }

}
