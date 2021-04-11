using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    private float _speed = 2;

    private Player _player;

    private Animator _onEnemyDeath;
    private AudioSource _audioSource;

    [SerializeField]
    private GameObject _laserPrefab;

    private float _canFire;
    private float _fireRate;

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
        CalculateMovement();

        if (Time.time > _canFire) // firing
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
                        _speed = .5f;
                    }

                    break;
                }

            case "Laser":
                {
                    if (other.tag == "Enemy")
                        break;

                    Destroy(other.gameObject);
                    _audioSource.Play();
                    if (_player)
                    {
                        _player.UpdateScore();
                    }

                    _onEnemyDeath.SetTrigger("OnEnemyDeath");
                    Destroy(this.gameObject, 2.5f);
                    Destroy(GetComponent<Collider2D>());

                    _speed = 0;
                    break;
                }
        }
    }
}