using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Asteroid : MonoBehaviour
{
    private float _rotateSpeed = 17.0f;
    private float _speed = 2f;
    [SerializeField]
    private GameObject _explosionVisual;
    [SerializeField]
    private SpawnManager _spawnManager;
    [SerializeField]
    private Player _player;

    private int _asteroidHealth = 2;
    // Start is called before the first frame update
    void Start()
    {
        _spawnManager = GameObject.Find("Spawn_Manager").GetComponent<SpawnManager>();
        _player = GameObject.Find("Player").GetComponent<Player>();
    }

    // Update is called once per frame
    void Update()
    {
        if (this.tag == "Starting_Asteroid")
            transform.Rotate(Vector3.forward * _rotateSpeed * Time.deltaTime, Space.Self);

        if (this.tag != "Starting_Asteroid")
            CalculateMovement();

        if (transform.position.x <= -20.0f || transform.position.y >= 8.0f)
            Destroy(this.gameObject);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        switch (other.tag)
        {
            case "Player_Laser":
                {
                    --_asteroidHealth;

                    if (_asteroidHealth <= 0) // Asteroid requires 2 hits to be destroyed
                    {
                        GameObject explode = Instantiate(_explosionVisual, transform.position, Quaternion.identity);
                        Destroy(explode, 2.5f);
                        Destroy(other.gameObject);
                        Destroy(this.gameObject, 0.25f);
                        Destroy(GetComponent<Collider2D>());
                    }

                    if (this.tag == "Starting_Asteroid")
                        _spawnManager.StartSpawning();

                    break;
                }

            case "Player":
                {
                    GameObject explode = Instantiate(_explosionVisual, transform.position, Quaternion.identity);
                    Destroy(explode, 2.5f);
                    Destroy(this.gameObject);
                    Destroy(GetComponent<Collider2D>());
                    _player.Damage();
                    break;
                }
                

        }
    }

    void CalculateMovement()
    {

        //if (transform.position.y > 0)
        //    transform.Rotate(0, 0, -10f);
        //else
        //    transform.Rotate(0, 0, 10f);


        transform.Translate(Vector3.left * _speed * Time.deltaTime);



    }
}
