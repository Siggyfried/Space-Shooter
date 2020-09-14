using System.Collections;
using System.Collections.Generic;
using System.Transactions;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.SocialPlatforms.Impl;
using System.Runtime.CompilerServices;

public class UIManager : MonoBehaviour
{
    [SerializeField]
    private Text _scoreText, _bestText, _ammoText;
    [SerializeField]
    private Image _LivesImg;
    [SerializeField]
    private Sprite[] _liveSprites;
    [SerializeField]
    private Text _gameOverText;
    [SerializeField]
    private Text _restartText;

    private GameManager _gameManager;
    private Player _player;

    public int bestScore;

    public Slider _thrusterBar;
    private float _maxThruster = 100f;

    void Start()
    {
        _scoreText.text = "Score: " + 0;
        _gameOverText.gameObject.SetActive(false);
        _gameManager = GameObject.Find("Game_Manager").GetComponent<GameManager>();

        if (_gameManager.isCoopMode == false)
        {
            _player = GameObject.Find("Player").GetComponent<Player>();
        }
        else if (_gameManager.isCoopMode == true)
        {
            _player = GameObject.Find("Player_1").GetComponent<Player>();
        }

        if (_gameManager == null)
        {
            Debug.LogError("game manger is null");
        }

        bestScore = PlayerPrefs.GetInt("HighScore", 0);
        _bestText.text = "Best: " + bestScore;

        _ammoText.text = "Ammo: " + 0;

        _thrusterBar.maxValue = _maxThruster;
        _thrusterBar.value = _maxThruster;
    }

    public void UpdateScore(int playerScore)
    {
        _scoreText.text = "Score: " + playerScore.ToString();
    }
  
    public void UpdateBestScore (int bestScore)
    {
        _bestText.text = "Best: " + bestScore.ToString();
    }
    public void UpdateLives(int currentLives)
    {
        _LivesImg.sprite = _liveSprites[currentLives];

        if (currentLives <= 0)
        {
            GameOverSequence();
        }
    }

    public void UpdateAmmoText(int currentAmmo, int maxAmmo)
    {
        _ammoText.text = "Ammo: " + currentAmmo.ToString() + " / " + maxAmmo.ToString();
        if (currentAmmo <= 0)
        {
            _ammoText.color = Color.red;
        }
        else 
        {
            _ammoText.color = Color.white;
        }
    }

    void GameOverSequence()
    {
        _gameManager.GameOver();
        _gameOverText.gameObject.SetActive(true);
        _restartText.gameObject.SetActive(true);
        StartCoroutine(GameOverFlickerRoutine());
        _player.CheckForBestScore();
    }

    IEnumerator GameOverFlickerRoutine()
    {
        while(true)
        {
            _gameOverText.text = "GAME OVER";
            yield return new WaitForSeconds(0.5f);
            _gameOverText.text = "";
            yield return new WaitForSeconds(0.5f);
        }
    }

    public void ResumePlay()
    {
        _gameManager.ResumeGame();
    }
   
    public void BackToMainMenu()
    {
        SceneManager.LoadScene(0);
    }

    
    public void UpdateThruster(float thruster)
    {
        _thrusterBar.value = thruster;
    }

}

