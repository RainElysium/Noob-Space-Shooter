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
    private GameObject _hackShot;
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
    private AudioClip _thrusterClip;
    [SerializeField]
    private GameObject _explosionVisual;
    [SerializeField]
    private GameObject _leftHack, _rightHack;
    [SerializeField]
    private GameObject _thrusterVisual;

    private AudioSource _audioSource;
    private AudioSource _thrusterAudioSource;
    private SpawnManager _spawnManager;
    private UIManager _uiManager;

    private bool _isTripleShotActive = false;
    private bool _isShieldActive = false;
    private bool _isHackShotActive = false;
    private bool _isThrustersActive = false;

    private int _hackShotCount;
    private int _shieldCharges = 3;
    private int _ammoCount = 15;

    private int _score;
    private float _canFire = -1f;

    void Start()
    {
        transform.position = new Vector3(-9.59f, 0.39f, 0);
        _spawnManager = GameObject.Find("Spawn_Manager").GetComponent<SpawnManager>();
        _uiManager = GameObject.Find("Canvas").GetComponent<UIManager>();
        _audioSource = GetComponent<AudioSource>();
        _thrusterAudioSource = _thrusterVisual.gameObject.GetComponent<AudioSource>();

        if (!_audioSource)
            Debug.LogError("Audiosource in Player is NULL");

        if (!_spawnManager)
            Debug.LogError("Spawn Manager is NULL");

        if (!_thrusterAudioSource)
            Debug.LogError("Thruster Audiosource is NULL");
    }

    // Update is called once per frame
    void Update()
    {
        CalculateMovement();

        if (Input.GetKeyDown(KeyCode.Space) && Time.time > _canFire)
            FireLaser();

        if (Input.GetKeyDown(KeyCode.LeftShift) && !_isThrustersActive) // Thrusters engage!
        {
            _speed *= _thrusterMultiplier;
            _thrusterVisual.transform.localScale = new Vector3(0.15f, 0.35f, 0);
            _isThrustersActive = true;
            _thrusterAudioSource.clip = _thrusterClip;
            _thrusterAudioSource.Play();
            _uiManager.ThrusterCooldown();
        }

        if (Input.GetKeyUp(KeyCode.LeftShift) && _isThrustersActive) // Thrusters disengage!
        {
            _speed = 5f;
        }
    }

    void CalculateMovement()
    {
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        Vector3 direction = new Vector3(verticalInput, horizontalInput, 0);

        transform.Translate(direction * _speed * Time.deltaTime);

        transform.position = new Vector3(transform.position.x, Mathf.Clamp(transform.position.y, -4.64f, 6.20f), 0);

        if (transform.position.x <= -11.27f)
            transform.position = new Vector3(-11.27f, transform.position.y, 0);

        if  (transform.position.x >= -1.42f)
            transform.position = new Vector3(-1.42f, transform.position.y, 0);

    }

    void FireLaser()
    {
        if (_ammoCount < 1 && !_isHackShotActive)
        {
            _audioSource.clip = _outOfAmmoClip;
            _audioSource.Play();
            return;
        }

        if (_isTripleShotActive && !_isHackShotActive) // FIRE TRIPLE SHOTS!
        {
            Instantiate(_tripleShot, transform.position, Quaternion.identity);
        }

        if (!_isHackShotActive && !_isTripleShotActive) // FIRE REGULAR LASERS!
        {
            Instantiate(_laserPrefab, transform.position + new Vector3(0.853f, 0, 0), Quaternion.identity);
            --_ammoCount;
            _uiManager.UpdateAmmo(_ammoCount);
        }

        if (_isHackShotActive && !_isTripleShotActive && _hackShotCount > 0) // FIRE HACKS!
        {
            Instantiate(_hackShot, transform.position + new Vector3(0, 1.05f, 0), Quaternion.identity);
            --_hackShotCount;

            if (_leftHack.activeInHierarchy) // disable hack visuals
                _leftHack.SetActive(false);
            else if (_rightHack.activeInHierarchy)
                _rightHack.SetActive(false);

            if (_hackShotCount <= 0)
                _isHackShotActive = false;
        }

            _canFire = Time.time + _fireRate;
            _audioSource.clip = _laserSoundClip;
            _audioSource.Play();


    }

    public void Damage()
    {
        _uiManager.CameraShake();

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

    public void HealthPowerup()
    {
        if (_lives < 3 && _lives != 0)
        {
            ++_lives;

            if (_leftEngine.activeInHierarchy)
            {
                _leftEngine.SetActive(false);
            }
            else if (_rightEngine.activeInHierarchy)
            {
                _rightEngine.SetActive(false);
            }

            _uiManager.UpdateLives(_lives);
        }
    }
    public void HackPowerup()
    {
        _isHackShotActive = true;
        _hackShotCount = 2;
        HackCooldownRoutine();

        _leftHack.SetActive(true);
        _rightHack.SetActive(true);
    }

    IEnumerator HackCooldownRoutine()
    {
        yield return new WaitForSeconds(5.0f);
        _isHackShotActive = false;
    }

    public void ReleaseThrusters()
    {
        _isThrustersActive = false;
    }

    public void PowerDownThrusters()
    {
        _speed = 5f;
        _thrusterVisual.transform.localScale = new Vector3(0.15f, 0.15f, 0);
    }

    public void AsteroidImpact()
    {
        Damage();
        _speed /= 2; // half speed
        StartCoroutine(SpeedBoostPowerDownRoutine()); // restore speed after 5s
    }
}


