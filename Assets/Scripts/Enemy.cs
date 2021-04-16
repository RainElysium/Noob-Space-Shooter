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
    private int _pathGenerator;
    private bool _respawned = false;
    private float rotateValue = 0;
    [SerializeField]
    float yPosition;

    private void Start()
    {
        _player = GameObject.Find("Player").GetComponent<Player>();

        if (!_player)
            Debug.Log("Player is NULL");

        _onEnemyDeath = GetComponent<Animator>();

        if (!_onEnemyDeath)
            Debug.Log("Animator is NULL");

        _audioSource = GetComponent<AudioSource>();

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
        if (_hackVisual.activeInHierarchy)
            _hackVisual.SetActive(false);

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
                        _isAlive = false;
                        _speed = .5f;
                    }

                    break;
                }

            case "Enemy":
                {
                    Destroy(this.gameObject, 2.8f);
                    Destroy(GetComponent<Collider2D>());
                    _audioSource.Play();
                    _onEnemyDeath.SetTrigger("OnEnemyDeath");
                    _isAlive = false;
                    _speed = .5f;
                    break;
                }

            case "Player_Laser":
                {
                    if (_player)
                        _player.UpdateScore();

                    Destroy(other.gameObject);
                    _audioSource.Play();
                    _onEnemyDeath.SetTrigger("OnEnemyDeath");
                    Destroy(this.gameObject, 2.8f);
                    Destroy(GetComponent<Collider2D>());
                    _enemyThruster.SetActive(false);
                    _isAlive = false;
                    _speed = 0;
                    break;
                }
        }
    }

    public void EnemyHacked()
    {
        _hackVisual.SetActive(true);
        _enemyHacked = true;
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
                        if (xPosition <= 6.50f)
                            rotateValue = 50f;
                        if (xPosition <= 3.10f)
                            rotateValue = 0f;
                        if (xPosition <= 1.00f)
                            rotateValue = -10f;
                        if (xPosition <= -1.10f)
                            rotateValue = 0f;
                        if (xPosition <= -2.90f)
                            rotateValue = -30f;
                        if (xPosition <= -6.25f)
                            rotateValue = 0f;
                    }
                    else
                    {
                        if (xPosition <= 6.50f)
                            rotateValue = -50f;
                        if (xPosition <= 3.10f)
                            rotateValue = 0f;
                        if (xPosition <= 1.00f)
                            rotateValue = 10f;
                        if (xPosition <= -1.10f)
                            rotateValue = 0f;
                        if (xPosition <= -2.90f)
                            rotateValue = 30f;
                        if (xPosition <= -6.25f)
                            rotateValue = 0f;
                    }
                    break;
                }
            case 2:
                {
                    if (yPosition >= 0)
                    {
                        if (xPosition <= 6.90f)
                            rotateValue = -31.6f;
                        if (xPosition <= 4.70f)
                            rotateValue = 0f;
                        if (xPosition <= 1.00f)
                            rotateValue = 52f;
                        if (xPosition <= -1.50f)
                            rotateValue = 0f;
                        if (xPosition <= -7.05f)
                            rotateValue = -31f;
                    }
                    else
                    {
                        if (xPosition <= 1.72f)
                            rotateValue = 28.84f;
                        if (xPosition <= -1.33f)
                            rotateValue = 0f;
                    }
                    break;
                }
            case 3:
                {
                    if (yPosition >= 0)
                    {
                        if (xPosition <= 1f)
                            rotateValue = 35f;
                        if (xPosition <= -3.33f)
                            rotateValue = 0f;
                        if (xPosition <= -6.45f)
                            rotateValue = -47.24f;
                        if (xPosition <= -9.55f)
                            rotateValue = 0f;
                    }
                    else
                    {
                        if (xPosition <= 3.43f)
                            rotateValue = -19.11f;
                        if (xPosition <= 0.45f)
                            rotateValue = -77f;
                        if (xPosition <= 0.14f)
                            rotateValue = -38f;
                        if (xPosition <= -3.44f)
                            rotateValue = 0f;
                    }
                    break;
                }
            case 4:
                {
                    if (yPosition >= 0)
                    {
                        if (xPosition <= 3.31f)
                            rotateValue = 41f;
                        if (xPosition <= -3.23f)
                            rotateValue = 0f;
                        if (xPosition <= -5.31f)
                            rotateValue = -10f;
                        if (xPosition <= -8.28f)
                            rotateValue = 0f;
                    }
                    else
                    {
                        if (xPosition <= 6.50f)
                            rotateValue = Random.Range(-70f, 70f);
                        if (xPosition <= 3.10f)
                            rotateValue = 0f;
                        if (xPosition <= 1.00f)
                            rotateValue = Random.Range(-70f, 70f);
                        if (xPosition <= -1.10f)
                            rotateValue = 0f;
                        if (xPosition <= -2.90f)
                            rotateValue = Random.Range(-70f, 70f);
                        if (xPosition <= -6.25f)
                            rotateValue = 0f;
                    }
                    break;
                }
        }

        transform.eulerAngles = Vector3.forward * rotateValue;
    }

}

