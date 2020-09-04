using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [SerializeField]
    private GameObject _controlsPanel;
    [SerializeField]
    private GameObject _mainMenuPanel;
   public void LoadSinglePlayer()
    {
        SceneManager.LoadScene(1);
    }    

    public void LoadCoOpMode()
    {
        SceneManager.LoadScene(2);
    }

    public void ControlsButton()
    {
        _controlsPanel.SetActive(true);
        _mainMenuPanel.SetActive(false);
    }

    public void BackButton()
    {
        _controlsPanel.SetActive(false);
        _mainMenuPanel.SetActive(true);
    }
}
