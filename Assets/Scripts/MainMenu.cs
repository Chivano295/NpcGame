using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    
    //gameobject for the credits list and main menu list so you can switch between them
    public GameObject Mainmenu;
    public GameObject Credits;
    //changes the scene based on string
    public void ChangeScene(string scene)
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(scene);
    }
    //quits the game
    public void Quitgame()
    {
        Application.Quit();
    }

    //turns on the credits
    public void CreditsOn()
    {
        Mainmenu.SetActive(false);
        Credits.SetActive(true);
    }
    //turns credits off
    public void CreditsOff()
    {
        Mainmenu.SetActive(true);
        Credits.SetActive(false);
    }

}
