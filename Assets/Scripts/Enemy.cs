using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField]
    private float _speed = 4.0f;

    [SerializeField]
    private GameObject _laserPrefab;
    [SerializeField]
    private float _fireRate = 3.0f;
    private float _canFire = -1f;

    private GameManager _gameManager;
    private Player _player;
    private Animator _anim;
    private AudioSource _audioSource;

    private bool _isDead;
    void Start()
    {
        _gameManager = GameObject.Find("Game_Manager").GetComponent<GameManager>();

        if (_gameManager.isCoopMode == false)
        {
            _player = GameObject.Find("Player").GetComponent<Player>();
        }

        else if (_gameManager.isCoopMode == true)
        {
            _player = GameObject.Find("Player_1").GetComponent<Player>();
        }
        _audioSource = GetComponent<AudioSource>();
        _anim = GetComponent<Animator>();

        if (_anim == null)
        {
            Debug.LogError("Animator is NULL");
        }
    }
    void Update()
    {
        CalculateMovement();

        if (!_isDead)
        {
            Shoot();
        }
    }

    void CalculateMovement()
    {
        transform.Translate(Vector3.down * _speed * Time.deltaTime);

        if (transform.position.y <= -5f)
        {
            float randomX = Random.Range(-8f, 8f);
            transform.position = new Vector3(randomX, 7f, 0);
        }
    }

    void Shoot()
    {
        if (Time.time > _canFire)
        {
            _fireRate = Random.Range(3f, 7f);
            _canFire = Time.time + _fireRate;
            GameObject enemyLaser = Instantiate(_laserPrefab, transform.position, Quaternion.identity);
            Laser[] lasers = enemyLaser.GetComponentsInChildren<Laser>();

            for (int i = 0; i < lasers.Length; i++)
            {
                lasers[i].AssignEnemyLaser();
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Player"))
            {
                Player player = other.transform.GetComponent<Player>();
                if (player != null)
                {
                    player.Damage();
                }
                _anim.SetTrigger("OnEnemyDeath");
                _speed = 0;
                _isDead = true;
                _audioSource.Play();
                Destroy(this.gameObject, 2.8f);
                Destroy(GetComponent<Collider2D>());
            }
            
            if (other.CompareTag("Laser"))
            {
                Destroy(other.gameObject);
            if (_player != null)
            {
                _player.AddScore(10);
            }
                _anim.SetTrigger("OnEnemyDeath");
                _speed = 0;
                _isDead = true;
                _audioSource.Play();
                Destroy(GetComponent<Collider2D>());
                Destroy(this.gameObject, 2.8f);            
            }         
       }
}
