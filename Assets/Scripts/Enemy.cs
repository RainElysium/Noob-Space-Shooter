using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField]
    private float _speed = 3f;

    private Player _player;

    private Animator _onEnemyDeath;
    private AudioSource _audioSource;

    [SerializeField]
    private GameObject _laserPrefab;
    [SerializeField]
    private GameObject _hackVisual, _rammingSpeedVisual;
    [SerializeField]
    private GameObject[] _enemyThruster;
    [SerializeField]
    private GameObject _explosionVisual;
    [SerializeField]
    private GameObject _artilleryZoneVisual;

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
    private float _speedMultiplier;

    [SerializeField]
    private int _pathGenerator;
    [SerializeField]
    private GameObject[] _shieldVisual;
    [SerializeField]
    private int _shieldCharges = 2;
    private bool _isShieldActive = false;
    private bool _ram = false;
    private int _avoidsLeft;
    private bool _allowedToFire = true;
    private bool _beginAvoid;

    Vector3 newPos;

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

        _speedMultiplier = _spawnManager.GetIncreasedSpeed(); // check for increased speed at each spawn
        _speed *= _speedMultiplier;
        _shieldCharges = _shieldVisual.Length;
        _avoidsLeft = Random.Range(1, 4); // avoids for Enemy Avoider
        Debug.Log("Avoids started with: " + _avoidsLeft);


        if (CompareTag("Enemy"))
        {
            int rand = Random.Range(1, 101);

            if (rand <= 25) // 25% chance to spawn enemy shield
            {
                _shieldVisual[0].SetActive(true);
                _isShieldActive = true;
            }
        }
        else if (CompareTag("Enemy_Artillery"))
            _isShieldActive = true;

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
        else if (!CompareTag("Enemy_Artillery"))
        {
            CalculateMovement();

            if (Time.time > _canFire && _isAlive && _allowedToFire) // firing
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

        float distanceCheck = Vector3.Distance(_player.transform.position, transform.position);

        if (distanceCheck <= 3.25f && !_ram) // check distance between player & enemy
        {
            _ram = true;
            _speed *= 2;
        }

        if (_ram)
        {
            transform.position = Vector3.MoveTowards(transform.position, _player.transform.position, Time.deltaTime);
            transform.right -= (_player.transform.position - transform.position);
            
            if (_rammingSpeedVisual != null)
            _rammingSpeedVisual.SetActive(true);
            _allowedToFire = false;
        }

        if (_beginAvoid)
            AvoidMovement(newPos);
    }
    private void CalculateMovement()
    {

        transform.Translate(Vector3.left * _speed * Time.deltaTime);

        if (_avoidsLeft >= 1)
            AvoiderLaserCheck(); // If avoider, check distance and evade mechanics.

        PathChange();

        if (transform.position.x <= -13.0f || transform.position.y >= 8.14f || transform.position.y <= -6.0f)
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
                        _player.UpdateScore();
                        _audioSource.Play();

                        if (CompareTag("Enemy"))
                            _onEnemyDeath.SetTrigger("OnEnemyDeath");
                        else
                            ExplosionVisual();

                        other.transform.GetComponent<Player>().Damage();
                        Destroy(this.gameObject, 2.8f);
                        Destroy(GetComponent<Collider2D>());
                        Destroy(_rammingSpeedVisual.gameObject);
                        _hackVisual.SetActive(false);

                        if (!CompareTag("Enemy"))
                            _enemyThruster[1].SetActive(false);
                        
                        _enemyThruster[0].SetActive(false);
                        _isAlive = false;
                        _speed = .5f;
                        _spawnManager.AddEnemyDeathCount();


                    }

                    break;
                }

            case "Enemy":
                {
                    Enemy enemy = other.GetComponentInParent<Enemy>();

                    if (enemy.IsEnemyHacked() || _enemyHacked)
                    {
                        _player.UpdateScore();
                        Destroy(this.gameObject, 2.8f);
                        Destroy(GetComponent<Collider2D>());
                        Destroy(_rammingSpeedVisual.gameObject);
                        _hackVisual.SetActive(false);
                        _audioSource.Play();

                        if (CompareTag("Enemy"))
                            _onEnemyDeath.SetTrigger("OnEnemyDeath");
                        else
                            ExplosionVisual();

                        if (!CompareTag("Enemy"))
                            _enemyThruster[1].SetActive(false);

                        _enemyThruster[0].SetActive(false);
                        _isAlive = false;
                        _speed = .5f;
                        _spawnManager.AddEnemyDeathCount();

                    }
                    break;
                }

            case "Player_Laser":
                {
                    LaserDamage(other);
                    break; 
                }
            
            }
    }

    public void EnemyHacked()
    {
        _hackVisual.SetActive(true);
        _enemyHacked = true;

        StartCoroutine(SelfDestructRoutine());

    }

    private bool IsEnemyHacked()
    {
        if (_enemyHacked)
            return true;
        else return false;
    }

    void PathChange()
    {
        float xPosition = transform.position.x;

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

    IEnumerator SelfDestructRoutine()
    {
        yield return new WaitForSeconds(2f);

        Destroy(this.gameObject, 2.8f);
        Destroy(GetComponent<Collider2D>());
        _hackVisual.SetActive(false);
        _audioSource.Play();
        _onEnemyDeath.SetTrigger("OnEnemyDeath");
        _enemyThruster[0].SetActive(false);
        _isAlive = false;
        _speed = .5f;
        _spawnManager.AddEnemyDeathCount();
    }

    void DamageEnemyShields()
    {
        if (_isShieldActive)
        {
            --_shieldCharges;

            if (CompareTag("Enemy_Artillery"))
            {
                _shieldVisual[1].SetActive(false);
            }

            if (_shieldCharges <= 0)
            {
                _isShieldActive = false;
                _shieldVisual[0].SetActive(false);
            }
        }
    }

    void LaserDamage(Collider2D other)
        {

        GameObject laserGob = other.gameObject;

        _player.RemoveLaserFromList(laserGob);
        
        if (_isShieldActive)
            {
                DamageEnemyShields();
                Destroy(other.gameObject);
            }
            else
            {
                if (_player)
                    _player.UpdateScore();
                if (_rammingSpeedVisual.activeInHierarchy)
                    Destroy(_rammingSpeedVisual.gameObject);

                Destroy(other.gameObject);

                if (_hackVisual.activeInHierarchy)
                    _hackVisual.SetActive(false);

                _audioSource.Play();

            if (CompareTag("Enemy"))
                _onEnemyDeath.SetTrigger("OnEnemyDeath");
            else
                ExplosionVisual();

                Destroy(this.gameObject, 1f);
                Destroy(GetComponent<Collider2D>());
                _enemyThruster[0].SetActive(false);

                if (!CompareTag("Enemy"))
                    _enemyThruster[1].SetActive(false);

                _isAlive = false;
                _speed = 0;
                _spawnManager.AddEnemyDeathCount();

            if (CompareTag("Enemy_Artillery"))
            {
                GameObject artzone = GameObject.Find("Artillery_Zone(Clone)");
                Destroy(artzone.gameObject);
            }
            }
        }    

    private void AvoiderLaserCheck()
    {
       List<GameObject> _playerLasersActive = _player.GetPlayerLaserList();

        if (CompareTag("Enemy_Avoider") && _playerLasersActive.Count > 0 && _avoidsLeft > 0)
        {
            for (int i = 0; i < _playerLasersActive.Count; i++)
            {

                float distance = Vector3.Distance(_playerLasersActive[i].transform.position, transform.position);
                
                if (distance <= 3.25f)
                {
                    if (_playerLasersActive[i].transform.position.y >= transform.position.y)
                        newPos = new Vector3(transform.position.x, transform.position.y - 3f);
                    else
                        newPos = new Vector3(transform.position.x, transform.position.y + 3f);

                    _player.RemoveLaserFromList(_playerLasersActive[i]);
                    --_avoidsLeft;
                    _beginAvoid = true;
                }

            }
        }
    }

    private void AvoidMovement(Vector3 avoidTarget)
    {
        transform.position = Vector3.Lerp(transform.position, avoidTarget, Time.deltaTime * _speed);
    }

    void ExplosionVisual()
    {
        GameObject explosion = Instantiate(_explosionVisual, transform.position, Quaternion.identity);
        Destroy(explosion.gameObject, 2.8f); // destroy after 2.8s
    }
}

