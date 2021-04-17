using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    private float _speed = 2f;

    private Player _player;

    private Animator _onEnemyDeath;
    private AudioSource _audioSource;

    [SerializeField]
    private GameObject _laserPrefab;
    [SerializeField]
    private GameObject _hackVisual;
    [SerializeField]
    private GameObject _enemyThruster;

    private float _canFire;
    private float _fireRate;

    private bool _isAlive = true;
    private bool _enemyHacked = false;

    private bool _doOnce = true;
    private int _randomGen;
    private Vector3 _randomVector;
    private float _rotateValue = 0;
    float yPosition;
    private SpawnManager _spawnManager;

    [SerializeField]
    private int _pathGenerator;

    private void Start()
    {
        _player = GameObject.Find("Player").GetComponent<Player>();

        if (!_player)
            Debug.Log("Player is NULL");

        _onEnemyDeath = GetComponent<Animator>();

        if (!_onEnemyDeath)
            Debug.Log("Animator is NULL");

        _audioSource = GetComponent<AudioSource>();
        _spawnManager = GameObject.Find("Spawn_Manager").GetComponent<SpawnManager>();
        _pathGenerator = Random.Range(1, 4); // random path generator
        yPosition = transform.position.y; // get initial spawn location and save it for pathing purposes
    }

    void Update()
    {
        if (_enemyHacked)
        {
            if (_doOnce)
            {
                _randomGen = Random.Range(1, 4);

                switch (_randomGen)
                {
                    case 1:
                        _randomVector = Vector3.left;
                        break;
                    case 2:
                        _randomVector = Vector3.right;
                        break;
                    case 3:
                        _randomVector = Vector3.up;
                        break;
                }

                _doOnce = false;
            }

            transform.Translate(_randomVector * (_speed * 2) * Time.deltaTime);
            transform.Rotate(Vector3.forward * _speed * Time.deltaTime);
        }
        else
        {
            CalculateMovement();

            if (Time.time > _canFire && _isAlive) // firing
            {
                _fireRate = Random.Range(2f, 5f);
                _canFire = Time.time + _fireRate;

                GameObject enemyLaser = Instantiate(_laserPrefab, transform.position, Quaternion.identity);
                Laser[] lasers = enemyLaser.GetComponentsInChildren<Laser>();

                for (int i = 0; i < lasers.Length; i++)
                {
                    lasers[i].ApplyEnemyLaser();
                }

            }
        }


    }
    private void CalculateMovement()
    {

        transform.Translate(Vector3.left * _speed * Time.deltaTime);
        
        PathChange();

        if (transform.position.x <= -13.0f || transform.position.y >= 7.35f || transform.position.y <= -6.0f)
        {
            float randomY = Random.Range(-4.53f, 6.2f);
            transform.position = new Vector3(10.45f, randomY);
            yPosition = transform.position.y; // get initial spawn location and save it for pathing purposes
            _pathGenerator = Random.Range(1, 4);
        }

    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        switch (other.tag)
        {
            case "Player":
                {
                    if (_player)
                    {
                        _audioSource.Play();
                        _onEnemyDeath.SetTrigger("OnEnemyDeath");
                        other.transform.GetComponent<Player>().Damage();
                        Destroy(this.gameObject, 2.8f);
                        Destroy(GetComponent<Collider2D>());
                        _hackVisual.SetActive(false);
                        _enemyThruster.SetActive(false);
                        _isAlive = false;
                        _speed = .5f;
                        _spawnManager.AddEnemyDeathCount();
                        
                    }

                    break;
                }

            case "Enemy":
                {
                    Enemy enemy = other.GetComponentInParent<Enemy>();

                    if (enemy.isEnemyHacked(enemy) || _enemyHacked)
                    {
                        Destroy(this.gameObject, 2.8f);
                        Destroy(GetComponent<Collider2D>());
                        _hackVisual.SetActive(false);
                        _audioSource.Play();
                        _onEnemyDeath.SetTrigger("OnEnemyDeath");
                        _enemyThruster.SetActive(false);
                        _isAlive = false;
                        _speed = .5f;
                        _spawnManager.AddEnemyDeathCount();
                    }
                    break;
                }

            case "Player_Laser":
                {
                    if (_player)
                        _player.UpdateScore();

                    Destroy(other.gameObject);
                    _hackVisual.SetActive(false);
                    _audioSource.Play();
                    _onEnemyDeath.SetTrigger("OnEnemyDeath");
                    Destroy(this.gameObject, 2.8f);
                    Destroy(GetComponent<Collider2D>());
                    _enemyThruster.SetActive(false);
                    _isAlive = false;
                    _speed = 0;
                    _spawnManager.AddEnemyDeathCount();
                    break;
                }
        }
    }

    public void EnemyHacked()
    {
        _hackVisual.SetActive(true);
        _enemyHacked = true;
    }

    private bool isEnemyHacked(Enemy target)
    {
        if (target._enemyHacked)
            return true;
        else return false;
    }

    void PathChange()
    {
        float xPosition = transform.position.x;

        //_pathGenerator = 1;

        switch (_pathGenerator) // different paths that can be called
        {
            case 1:
                {
                    if (yPosition >= 0)
                    {
                        if (xPosition <= 7.50f)
                            _rotateValue = 30f;
                        if (xPosition <= 3.10f)
                            _rotateValue = 0f;
                        if (xPosition <= 1.50f)
                            _rotateValue = -10f;
                        if (xPosition <= -1.50f)
                            _rotateValue = 0f;
                        if (xPosition <= -2.75f)
                            _rotateValue = -30f;
                        if (xPosition <= -7.25f)
                            _rotateValue = 0f;
                    }
                    else
                    {
                        if (xPosition <= 5.50f)
                            _rotateValue = -50f;
                        if (xPosition <= 2.50f)
                            _rotateValue = 0f;
                        if (xPosition <= 1.25f)
                            _rotateValue = 10f;
                        if (xPosition <= -1.10f)
                            _rotateValue = 0f;
                        if (xPosition <= -3.90f)
                            _rotateValue = 30f;
                        if (xPosition <= -5.25f)
                            _rotateValue = 0f;
                    }
                    break;
                }
            case 2:
                {
                    if (yPosition >= 0)
                    {
                        if (xPosition <= 4.90f)
                            _rotateValue = -31.6f;
                        if (xPosition <= 3.70f)
                            _rotateValue = 0f;
                        if (xPosition <= 2.00f)
                            _rotateValue = 52f;
                        if (xPosition <= -2.50f)
                            _rotateValue = 0f;
                        if (xPosition <= -6.50f)
                            _rotateValue = -31f;
                    }
                    else
                    {
                        if (xPosition <= 2.72f)
                            _rotateValue = 28.84f;
                        if (xPosition <= -2.33f)
                            _rotateValue = 0f;
                    }
                    break;
                }
            case 3:
                {
                    if (yPosition >= 0)
                    {
                        if (xPosition <= 1f)
                            _rotateValue = 35f;
                        if (xPosition <= -3.33f)
                            _rotateValue = 0f;
                        if (xPosition <= -6.45f)
                            _rotateValue = -47.24f;
                        if (xPosition <= -9.55f)
                            _rotateValue = 0f;
                    }
                    else
                    {
                        if (xPosition <= 3.43f)
                            _rotateValue = -19.11f;
                        if (xPosition <= 0.45f)
                            _rotateValue = -77f;
                        if (xPosition <= 0.14f)
                            _rotateValue = -38f;
                        if (xPosition <= -3.44f)
                            _rotateValue = 0f;
                    }
                    break;
                }
            case 4:
                {
                    if (yPosition >= 0)
                    {
                        if (xPosition <= 3.31f)
                            _rotateValue = 41f;
                        if (xPosition <= -3.23f)
                            _rotateValue = 0f;
                        if (xPosition <= -5.31f)
                            _rotateValue = -10f;
                        if (xPosition <= -8.28f)
                            _rotateValue = 0f;
                    }
                    else
                    {
                        if (xPosition <= 6.50f)
                            _rotateValue = Random.Range(-70f, 70f);
                        if (xPosition <= 3.10f)
                            _rotateValue = 0f;
                        if (xPosition <= 1.00f)
                            _rotateValue = Random.Range(-70f, 70f);
                        if (xPosition <= -1.10f)
                            _rotateValue = 0f;
                        if (xPosition <= -2.90f)
                            _rotateValue = Random.Range(-70f, 70f);
                        if (xPosition <= -6.25f)
                            _rotateValue = 0f;
                    }
                    break;
                }
        }

        transform.eulerAngles = Vector3.forward * _rotateValue;
    }

}

