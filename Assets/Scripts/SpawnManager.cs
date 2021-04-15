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
    private bool _stopSpawning = false;

    // Start is called before the first frame update
    void Start()
    {
    }

    public void StartSpawning()
    {
        StartCoroutine(SpawnEnemyRoutine());
        StartCoroutine(SpawnPowerupRoutine());
    }

    IEnumerator SpawnEnemyRoutine()
    {
        yield return new WaitForSeconds(3.0f);

        while (!_stopSpawning)
        {
            Vector3 posToSpawn = new Vector3(Random.Range(-11.17f, 8.0f), 7.39f);
            GameObject newEnemy = Instantiate(_enemyPrefab, posToSpawn, Quaternion.identity);
            newEnemy.transform.parent = _enemyContainer.transform;

            yield return new WaitForSeconds(5.0f);
        }
    }

    IEnumerator SpawnPowerupRoutine()
    {
        yield return new WaitForSeconds(3.0f);

        while (!_stopSpawning)
        {
            Vector3 posToSpawn = new Vector3(Random.Range(-11.17f, 8.0f), 6.74f);

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
}
