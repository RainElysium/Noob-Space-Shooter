using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Artillery : MonoBehaviour
{
    private Player _player;
    private float _speed = 5f;
    private float _canFire;
    private float _fireRate = 5f;

    [SerializeField]
    private GameObject _artilleryZone, _artilleryShot;
    private GameObject _artZone;
    [SerializeField]
    private GameObject _explosion;
    // Start is called before the first frame update
    void Start()
    {
        _player = GameObject.Find("Player").GetComponent<Player>();
    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate(Vector3.left * _speed * Time.deltaTime);

        if (transform.position.x <= 6.0f)
            transform.position = new Vector3(6.0f, transform.position.y, 0);

        if (Time.time > _canFire) // firing
        {
            ArtilleryFire();
            _fireRate = Random.Range(5f, 10f);
            _canFire = Time.time + _fireRate;
        }
    }

    void ArtilleryFire()
    {
        Vector3 playerPOS = _player.transform.position;

        _artZone = Instantiate(_artilleryZone, playerPOS, Quaternion.identity); // Drop artillery zone
        StartCoroutine(ArtilleryZoneTimerRoutine(_artZone));

    }

    IEnumerator ArtilleryZoneTimerRoutine(GameObject zone)
    {
        yield return new WaitForSeconds(3f);
        Vector3 laserPos = new Vector3(-1.97f, 0, 0);
        GameObject artilleryShot = Instantiate(_artilleryShot, transform.position + laserPos, Quaternion.identity); // Laser creation
        yield return new WaitForSeconds(1.25f);
        GameObject explosion = Instantiate(_explosion, zone.transform.position, Quaternion.identity);
        Destroy(zone.gameObject);
        yield return new WaitForSeconds(2f);
        Destroy(explosion.gameObject);
    }
}
