using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    [SerializeField]
    private GameObject _enemyPrefab;
    [SerializeField]
    private GameObject[] _powerups;
    [SerializeField]
    private GameObject _enemyContainer;
    [SerializeField]
    private UIManager _uiManager;

    private bool _stopSpawning = false;
    [SerializeField]
    private int _enemyCount = 10;
    [SerializeField]
    private int _waveNumber = 1;
    private int _spawnRate;
    private float _spawnInterval = 3.0f;
    private Enemy _enemy;
    private float _increasedSpeed = 1f;

    private void Start()
    {
        _uiManager = GameObject.Find("Canvas").GetComponent<UIManager>();
        _enemy = GameObject.Find("Enemy").GetComponent<Enemy>();
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
            ++_increasedSpeed;

        }
        if (_enemyCount % 10 == 0)
            //_uiManager.WaveDisplay();

        if (_waveNumber < 2)
        {
            _waveNumber = 1;
            _spawnInterval = 1.5f;
        }
        else
            _spawnInterval = 3.0f;
    }

    IEnumerator SpawnEnemyRoutine()
    { 

        yield return new WaitForSeconds(3.0f);

        while (!_stopSpawning)
        {
            for (int i = 0; i < _waveNumber; i++)
            {
                yield return new WaitForSeconds(1.5f);
                Vector3 posToSpawn = new Vector3(10.45f, Random.Range(-4.53f, 6.2f), 0);
                GameObject newEnemy = Instantiate(_enemyPrefab, posToSpawn, Quaternion.identity);
                newEnemy.transform.parent = _enemyContainer.transform;

            }
            yield return new WaitForSeconds(3.0f);
        }
    }

    IEnumerator SpawnPowerupRoutine()
    {
        yield return new WaitForSeconds(Random.Range(3.0f, 5.0f));

        while (!_stopSpawning)
        {
            Vector3 posToSpawn = new Vector3(10.45f, Random.Range(-4.11f, 6.2f), 0);

            int randomPowerUp = Random.Range(1, 101); // Randomized number to add chance to spawn.

            if (randomPowerUp <= 25) // rare spawn - Hack 25%
            {
                Instantiate(_powerups[5], posToSpawn, Quaternion.identity);
            }
            else if (randomPowerUp > 25 && randomPowerUp <= 55)  // rare spawn - Health 30%
            {
                Instantiate(_powerups[4], posToSpawn, Quaternion.identity);
            }
            else if (randomPowerUp > 55  && randomPowerUp <= 100) // main spawns - 45%
            {
                Instantiate(_powerups[Random.Range(1, 4)], posToSpawn, Quaternion.identity);
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
