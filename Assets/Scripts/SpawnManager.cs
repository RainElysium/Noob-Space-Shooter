using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    [SerializeField]
    private GameObject _enemyPrefab;
    [SerializeField]
    private GameObject _enemyArtillery;
    [SerializeField]
    private GameObject[] _powerups;
    [SerializeField]
    private GameObject _enemyContainer;
    [SerializeField]
    private UIManager _uiManager;
    [SerializeField]
    private int _randomPowerUp;

    private bool _stopSpawning = false;
    [SerializeField]
    private int _enemyCount = 10;
    [SerializeField]
    private int _waveNumber = 1;
    private int _spawnRate;
    private float _spawnInterval;
    //private float _spawnInterval = 3.0f;
    private float _increasedSpeed = .25f;

    private void Start()
    {
        _uiManager = GameObject.Find("Canvas").GetComponent<UIManager>();
    }

    public void StartSpawning()
    {
        StartCoroutine(SpawnEnemyRoutine());
        StartCoroutine(SpawnPowerupRoutine());
        _uiManager.ShowWaveNumber(1);
    }

    public void Update()
    {
        _spawnRate = _enemyCount / 10;

        if (_spawnRate > _waveNumber)
        {
            _waveNumber = _spawnRate;
            _uiManager.ShowWaveNumber(_waveNumber);
            _increasedSpeed += .25f;

        }

        if (_waveNumber < 2)
        {
            _waveNumber = 1;
            _spawnInterval = 3f;
        }
        else
            _spawnInterval = 2f;
    }

    IEnumerator SpawnEnemyRoutine()
    {

        yield return new WaitForSeconds(_spawnInterval);

        while (!_stopSpawning)
        {
            for (int i = 0; i < _waveNumber; i++)
            {
                yield return new WaitForSeconds(1.5f);
                Vector3 posToSpawn = new Vector3(10.45f, Random.Range(-4.53f, 6.2f), 0);
                GameObject newEnemy = Instantiate(_enemyPrefab, posToSpawn, Quaternion.identity);
                newEnemy.transform.parent = _enemyContainer.transform;

                if (_waveNumber >= 2) // only spawn 1 max in wave 2, unlimited in future waves
                {
                    if (_waveNumber == 2)
                    {
                        int rand = Random.Range(1, 100);
                        if (rand >= 50)
                        {
                            if (_waveNumber == 2)
                            {
                                GameObject findExisting = GameObject.Find("EnemyArtillery");
                                if (!findExisting)
                                    Instantiate(_enemyArtillery, posToSpawn, Quaternion.identity);
                            }
                            else
                                Instantiate(_enemyArtillery, posToSpawn, Quaternion.identity);
                        }
                    }

                }
                yield return new WaitForSeconds(3.0f);
            }
        }
    }

        IEnumerator SpawnPowerupRoutine()
        {
            yield return new WaitForSeconds(Random.Range(3.0f, 5.0f));

            while (!_stopSpawning)
            {
                Vector3 posToSpawn = new Vector3(10.45f, Random.Range(-4.11f, 6.2f), 0);

                _randomPowerUp = Random.Range(1, 101); // Randomized number to add chance to spawn.

                if (_randomPowerUp <= 10) // rare spawn - Hack 10%
                {
                    Instantiate(_powerups[5], posToSpawn, Quaternion.identity);
                }
                else if (_randomPowerUp > 10 && _randomPowerUp <= 25)  // rare spawn - Health 15%
                {
                    Instantiate(_powerups[4], posToSpawn, Quaternion.identity);
                }
                else if (_randomPowerUp > 25 & _randomPowerUp <= 45) // main spawns - Triple Shot 20%
                {
                    Instantiate(_powerups[0], posToSpawn, Quaternion.identity);
                }
                else if (_randomPowerUp > 45 & _randomPowerUp <= 55) // main spawns - Shield 10%
                {
                    Instantiate(_powerups[1], posToSpawn, Quaternion.identity);
                }
                 else if (_randomPowerUp > 55 & _randomPowerUp <= 65) // main spawns - Speed 10%
                {
                    Instantiate(_powerups[2], posToSpawn, Quaternion.identity);
                }
                else if (_randomPowerUp > 65 & _randomPowerUp <= 100) // main spawns - Ammo 35%
                {
                    Instantiate(_powerups[3], posToSpawn, Quaternion.identity);
                }

            if (_randomPowerUp <= 50 && _waveNumber >= 2) // additional rare spawn of damaging & slowing asteroid
                {
                    float zRotation = 0f;

                    if (_randomPowerUp <= 50)
                        posToSpawn = new Vector3(Random.Range(0f, 11.5f), -6f, 0);
                    else
                        posToSpawn = new Vector3(Random.Range(0f, 11.5f), 8f, 0);

                    if (posToSpawn.y > 0)
                        zRotation = Random.Range(15f, 45f);
                    else
                        zRotation = Random.Range(-45f, -15f);

                    Instantiate(_powerups[6], posToSpawn, Quaternion.Euler(0, 0, zRotation));
                }

                yield return new WaitForSeconds(Random.Range(3, 8));
            }
        }
    

    public void OnPlayerDeath()
    {
        _stopSpawning = true;
    }

    public void AddEnemyDeathCount()
    {
        _enemyCount++;
    }

    public int WaveNumber()
    {
        Debug.Log("WaveNumber returned: " + _waveNumber);
        return _waveNumber;
    }

    public float GetIncreasedSpeed()
    {
        return _increasedSpeed;
    }
}
