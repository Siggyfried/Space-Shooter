using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField]
    private float _speed = 3.5f;
    private float _speedMultipler = 2.0f;
    private float _thrusterSpeed = 5.0f;
    [SerializeField]
    private GameObject _laserPrefab;
    [SerializeField]
    private GameObject _tripleShotPrefab;
    [SerializeField]
    private GameObject _spreadShotPrefab;
    [SerializeField]
    private float _fireRate = 0.5f;
    private float _canFire = -1f;
    [SerializeField]
    private int _lives = 3;
    private int _maxLives = 3;
    [SerializeField]
    private int _shieldLives = 0;
    private int _maxShield = 3;
    private int _maxAmmo = 15;
    [SerializeField]
    private int _curAmmo;

    private SpawnManager _spawnManager;

    [SerializeField]
    private bool _isSpreadShotActive = false;
    private bool _isTripleShotActive = false;
    private bool _isSpeedBoostActive = false;
    private bool _isShieldActive = false;
    public bool _isPlayerOne = false;
    public bool _isPlayerTwo = false;

    [SerializeField]
    private GameObject _shieldVisualizer;
    [SerializeField]
    private GameObject _shieldVisualizerMed;
    [SerializeField]
    private GameObject _shieldVisualizerHigh;
    [SerializeField]
    private GameObject _rightEngine;
    [SerializeField]
    private GameObject _leftEngine;

    [SerializeField]
    public int _score;
    public int bestScore;


    private UIManager _uiManager;
    private GameManager _gameManager;

    [SerializeField]
    private AudioClip _laserSoundClip;
    [SerializeField]
    private AudioSource _audioSource;

    void Start()
    {      
        _gameManager = GameObject.Find("Game_Manager").GetComponent<GameManager>();
        
        _spawnManager = GameObject.Find("Spawn_Manager").GetComponent<SpawnManager>();
        _uiManager = GameObject.Find("Canvas").GetComponent<UIManager>();
        _audioSource = GetComponent<AudioSource>();

        if (_audioSource == null)
        {
            Debug.LogError("Audio Source on the player is Null.");
        }
        else
        {
            _audioSource.clip = _laserSoundClip;
        }

        if (_gameManager.isCoopMode == false)
        {
            transform.position = new Vector3(0, 0, 0);
        }

        _curAmmo = _maxAmmo;
    }

    void Update()
    {
        CheckAmmo();
        

        if (_isPlayerOne == true)
        {
            CalculateMovement();

            if (Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButton(0))
            {
                FireLaser();
            }                  
        }

        if (_isPlayerTwo == true)
        {
            PlayerTwoMovement();
            if (Input.GetKeyDown(KeyCode.KeypadEnter))
            {
                FireLaser();
            }
        }
    }
    void CalculateMovement()
    {
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        transform.Translate(Vector3.right * horizontalInput * _speed * Time.deltaTime);
        transform.Translate(Vector3.up * verticalInput * _speed * Time.deltaTime);

        Vector3 direction = new Vector3(horizontalInput, verticalInput, 0);

        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            transform.Translate(direction * (_speed += _thrusterSpeed) * Time.deltaTime);
        }
        if (Input.GetKeyUp(KeyCode.LeftShift))
        {
            transform.Translate(direction * (_speed -= _thrusterSpeed) * Time.deltaTime);
        }
        else
        {
            transform.Translate(direction * _speed * Time.deltaTime);
        }

        transform.position = new Vector3(transform.position.x, Mathf.Clamp(transform.position.y, -3.8f, 0), 0);

        if (transform.position.x >= 11.3f)
        {
            transform.position = new Vector3(-11.3f, transform.position.y, 0);
        }
        else if (transform.position.x <= -11.3f)
        {
            transform.position = new Vector3(11.3f, transform.position.y, 0);
        }
    }

    void PlayerTwoMovement()
    {
        if (Input.GetKey(KeyCode.Keypad8))
        {
            transform.Translate(Vector3.up * _speed * Time.deltaTime);
        }

       if (Input.GetKey(KeyCode.Keypad6))
        {
            transform.Translate(Vector3.right * _speed * Time.deltaTime);
        }
        if (Input.GetKey(KeyCode.Keypad2))
        {
            transform.Translate(Vector3.down * _speed * Time.deltaTime);
        }

        if (Input.GetKey(KeyCode.Keypad4))
        {
            transform.Translate(Vector3.left * _speed * Time.deltaTime);
        }

        transform.position = new Vector3(transform.position.x, Mathf.Clamp(transform.position.y, -3.8f, 0), 0);

        if (transform.position.x >= 11.3f)
        {
            transform.position = new Vector3(-11.3f, transform.position.y, 0);
        }
        else if (transform.position.x <= -11.3f)
        {
            transform.position = new Vector3(11.3f, transform.position.y, 0);
        }
    }

    void FireLaser()
    {
        if  ((Time.time > _canFire) && (_curAmmo > 0))
        {
            _audioSource.Play();
            if (_isTripleShotActive == true)
            {
                Instantiate(_tripleShotPrefab, transform.position, Quaternion.identity);
            }
            if (_isSpreadShotActive == true)
            {
                Instantiate(_spreadShotPrefab, transform.position, Quaternion.identity);
            }
            else
            {
                Instantiate(_laserPrefab, transform.position + new Vector3(0, 1.05f, 0), Quaternion.identity);
            }
            _canFire = Time.time + _fireRate;
            _curAmmo--;
            CheckAmmo();
        }
        // else if no ammo play sound
    }

    public void Damage()
    {
        if (_isShieldActive == true)
        {
            _shieldLives--;

            if (_shieldLives == 2)
            {
                _shieldVisualizerHigh.SetActive(false);
            }

            else if (_shieldLives == 1)
            {
                _shieldVisualizerMed.SetActive(false);
            }

            if (_shieldLives < 1)
            {
                _shieldVisualizer.SetActive(false);
            }
            return;
        }
        
        _lives--;
        ShipDamage();
        _uiManager.UpdateLives(_lives);
        
    }

    public void TripleShotActive()
    {
        _isTripleShotActive = true;
        StartCoroutine(TripleShotPowerDownRoutine());
    }

    IEnumerator TripleShotPowerDownRoutine()
    {
        yield return new WaitForSeconds(5.0f);
        _isTripleShotActive = false;
    }

    public void SpeedBoostActive()
    {
        _isSpeedBoostActive = true;
        _speed *= _speedMultipler;
        StartCoroutine(SpeedBoostPowerDownRoutine());
    }

    IEnumerator SpeedBoostPowerDownRoutine()
    {
        yield return new WaitForSeconds(5.0f);
        _isSpeedBoostActive = false;
        _speed /= _speedMultipler;
    }

    public void ShieldActive()
    {       
            _shieldLives = _maxShield;
            _isShieldActive = true;
            _shieldVisualizer.SetActive(true);
            _shieldVisualizerMed.SetActive(true);
            _shieldVisualizerHigh.SetActive(true);
    }

    public void SpreadShotActive()
    {
        _isSpreadShotActive = true;
        StartCoroutine(SpreadShotPowerDownRoutine());
    }

    IEnumerator SpreadShotPowerDownRoutine()
    {
        yield return new WaitForSeconds(5.0f);
        _isSpreadShotActive = false;
    }

    public void AddScore(int points)
    {
        _score += 10;
        _uiManager.UpdateScore(_score);
    }

    public void CheckForBestScore()
    {

        if (_score > bestScore)
        {
            bestScore = _score;
            PlayerPrefs.SetInt("HighScore", bestScore);
            _uiManager.UpdateBestScore(bestScore);
        }
    }

   public void CheckAmmo()
    {
        _uiManager.UpdateAmmoText(_curAmmo);
    }

    public void AmmoPowerup()
    {
        _curAmmo = _maxAmmo;
    }

    public void HealthPowerup()
    {
        if (_lives < _maxLives)
        {
            _lives++;
            _uiManager.UpdateLives(_lives);
            ShipDamage();
        }
    }

    public void ShipDamage()
    {
        if (_lives == 3)
        {
            _leftEngine.SetActive(false);
            _rightEngine.SetActive(false);
        }
        if (_lives == 2)
        {   
            _leftEngine.SetActive(true);
            _rightEngine.SetActive(false);
        }
        else if (_lives == 1)
        {     
            _rightEngine.SetActive(true);
            _leftEngine.SetActive(true);
        }
        if (_lives < 1)
        {
            _spawnManager.OnPlayerDeath();
            Destroy(this.gameObject);
        }
    }
}
