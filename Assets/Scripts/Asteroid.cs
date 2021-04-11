using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Asteroid : MonoBehaviour
{
    [SerializeField]
    private float _rotateSpeed = 17.0f;
    [SerializeField]
    private GameObject _explosionVisual;
    [SerializeField]
    private SpawnManager _spawnManager;
    // Start is called before the first frame update
    void Start()
    {
        _spawnManager = GameObject.Find("Spawn_Manager").GetComponent<SpawnManager>();
    }

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(Vector3.forward * _rotateSpeed * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        switch (other.tag)
        {
            case "Laser":
                {
                    GameObject explode = Instantiate(_explosionVisual, transform.position, Quaternion.identity);
                    Destroy(explode, 2.5f);
                    _spawnManager.StartSpawning();
                    Destroy(other.gameObject);
                    Destroy(this.gameObject, 0.25f);
                    Destroy(GetComponent<Collider2D>());
                    break;
                }
        }
    }
}
