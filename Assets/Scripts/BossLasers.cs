using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossLasers : MonoBehaviour
{
    private Player _player;
    private float _speed = 8f;
    void Start()
    {
        _player = GameObject.Find("Player").GetComponent<Player>();
    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate(Vector3.left * _speed * Time.deltaTime);

        if (transform.position.x <= -13.0f)
            Destroy(this.gameObject);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Destroy(this.gameObject);
            _player.Damage();
        }
    }
}
