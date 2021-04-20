using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArtilleryLaser : MonoBehaviour
{
    private GameObject _zone;
    private Vector3 _zonePOS;
    private float _speed = 10.0f;
    private Player _player;
    // Start is called before the first frame update
    void Start()
    {
        _zone = GameObject.Find("Artillery_Zone(Clone)");
        _zonePOS = _zone.transform.position;

        if (_zone == null)
            Debug.LogError("No Zone found!");

        _player = GameObject.Find("Player").GetComponent<Player>();
    }

    // Update is called once per frame
    void Update()
    {
       transform.position = Vector3.MoveTowards(transform.position, _zonePOS, _speed * Time.deltaTime);

        if (transform.position == _zonePOS)
            Destroy(this.gameObject);

    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Player")
        {
            _player.Damage();
        }
    }

}
