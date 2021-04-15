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

    private float _canFire;
    private float _fireRate;

    private bool _isAlive = true;
    private bool _enemyHacked = false;

    private bool _doOnce = true;
    private int _randomGen;
    private Vector3 _randomVector;

    private void Start()
    {
        _player = GameObject.Find("Player").GetComponent<Player>();

        if (!_player)
            Debug.Log("Player is NULL");

        _onEnemyDeath = GetComponent<Animator>();

        if (!_onEnemyDeath)
            Debug.Log("Animator is NULL");

        _audioSource = GetComponent<AudioSource>();
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
     transform.Translate(Vector3.down * _speed * Time.deltaTime);

     if (transform.position.y <= -6.42f)
     {
        float randomX = Random.Range(-9.3f, 9.3f);
        transform.position = new Vector3(randomX, 6.37f);
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
}