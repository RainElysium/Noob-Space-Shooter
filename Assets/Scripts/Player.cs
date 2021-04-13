using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField]
    private float _speed = 5f;
    [SerializeField]
    private float _thrusterMultiplier = 1.25f;
    [SerializeField]
    private GameObject _laserPrefab;
    [SerializeField]
    private GameObject _tripleShot;
    [SerializeField]
    private GameObject[] _shieldVisual;
    [SerializeField]
    private float _fireRate = 0.2f;
    [SerializeField]
    private int _lives = 3;
    [SerializeField]
    private GameObject _leftEngine, _rightEngine;

    [SerializeField]
    private AudioClip _laserSoundClip;
    [SerializeField]
    private AudioClip _outOfAmmoClip;
    [SerializeField]
    private GameObject _explosionVisual;
    private AudioSource _audioSource;

    private SpawnManager _spawnManager;

    private bool _isTripleShotActive = false;
    private bool _isShieldActive = false;
    private int _shieldCharges = 3;
    private int _ammoCount = 15;

    private UIManager _uiManager;
    private int _score;
    private float _canFire = -1f;

    void Start()
    {
        transform.position = new Vector3(0.15f, -3.99f, 0);
        _spawnManager = GameObject.Find("Spawn_Manager").GetComponent<SpawnManager>();
        _uiManager = GameObject.Find("Canvas").GetComponent<UIManager>();
        _audioSource = GetComponent<AudioSource>();

        if (!_audioSource)
            Debug.LogError("Audiosource in Player is NULL");

        if (!_spawnManager)
            Debug.LogError("Spawn Manager is NULL");
    }

    // Update is called once per frame
    void Update()
    {
        CalculateMovement();

        if (Input.GetKeyDown(KeyCode.Space) && Time.time > _canFire)
            FireLaser();

        if (Input.GetKeyDown(KeyCode.LeftShift)) // Thrusters engage!
            _speed *= _thrusterMultiplier;

        if (Input.GetKeyUp(KeyCode.LeftShift)) // Thrusters disengage!
            _speed /= _thrusterMultiplier;

    }

    void CalculateMovement()
    {
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        Vector3 direction = new Vector3(horizontalInput, verticalInput, 0);

        transform.Translate(direction * _speed * Time.deltaTime);

        transform.position = new Vector3(transform.position.x, Mathf.Clamp(transform.position.y, -3.8f, 0), 0);

        if (transform.position.x <= -11.28f)
            transform.position = new Vector3(11.33f, transform.position.y, 0);
        else if (transform.position.x >= 11.33f)
            transform.position = new Vector3(-11.28f, transform.position.y, 0);
    }

    void FireLaser()
    {
        if (_ammoCount < 1)
        {
            _audioSource.clip = _outOfAmmoClip;
            _audioSource.Play();
            return;
        }

        if (_isTripleShotActive)
        {
            Instantiate(_tripleShot, transform.position, Quaternion.identity);
        }
        else
            Instantiate(_laserPrefab, transform.position + new Vector3(0, 1.032f, 0), Quaternion.identity);

        _canFire = Time.time + _fireRate;
        _audioSource.clip = _laserSoundClip;
        _audioSource.Play();
        --_ammoCount;
        _uiManager.UpdateAmmo(_ammoCount);


    }

    public void Damage()
    {
        if (_isShieldActive)
        {
            --_shieldCharges;

            switch (_shieldCharges)
            {
                case 3:
                    _shieldVisual[2].SetActive(true);
                    break;
                case 2:
                    _shieldVisual[2].SetActive(false);
                    _shieldVisual[1].SetActive(true);
                    break;
                case 1:
                    _shieldVisual[1].SetActive(false);
                    _shieldVisual[0].SetActive(true);
                    break;
                case 0:
                    _shieldVisual[0].SetActive(false);
                    _isShieldActive = false;
                    _shieldCharges = 3;
                    break;
            }
            return;
        }

        int random = Random.Range(1, 3);

        switch (random)
        {
            case 1:
                if (!_leftEngine.activeInHierarchy)
                {
                    _leftEngine.SetActive(true);
                    break;
                }
                else
                    _rightEngine.SetActive(true);
                break;
            case 2:
                if (!_rightEngine.activeInHierarchy)
                {
                    _rightEngine.SetActive(true);
                    break;
                }
                else
                    _leftEngine.SetActive(true);
                break;
        }

        --_lives; // remove health

        _uiManager.UpdateLives(_lives);

        if (_lives < 1) // dead
        {
            Destroy(this.gameObject);
            _spawnManager.OnPlayerDeath();
            GameObject explode = Instantiate(_explosionVisual, transform.position, Quaternion.identity);
            Destroy(explode, 2.5f);
        }
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
        _speed = 8.5f;
        StartCoroutine(SpeedBoostPowerDownRoutine());
    }

    IEnumerator SpeedBoostPowerDownRoutine()
    {
        yield return new WaitForSeconds(5.0f);
        _speed = 5f;
    }

    public void ShieldBoostActive()
    {
        _shieldVisual[2].SetActive(true);
        _shieldCharges = 3;
        _isShieldActive = true;
    }

    public void UpdateScore()
    {
        _score += 10;
        _uiManager.UpdateScore(_score);
    }

    public void AmmoPowerup()
    {
        _ammoCount = 15;
        _uiManager.UpdateAmmo(_ammoCount);
    }
}


