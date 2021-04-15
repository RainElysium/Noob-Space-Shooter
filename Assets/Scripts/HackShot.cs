using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HackShot : MonoBehaviour
{
    private GameObject _target = null;

    [SerializeField]
    private float _speed = 2f;

    // Update is called once per frame
    void Update()
    {
        MoveRight();

        transform.position = Vector3.MoveTowards(this.transform.position, _target.transform.position, 5f * Time.deltaTime);

        if (transform.position == _target.transform.position)
        {
            Destroy(this.gameObject);
            _target.GetComponent<Enemy>().EnemyHacked();
        }

        if (transform.position.y > 8.0f || transform.position.y < -6.0f)
            Destroy(this.gameObject);
    }

    void MoveRight()
    {
        transform.Translate(Vector3.right * _speed * Time.deltaTime);

        if (transform.position.x > 10.5f)
            Destroy(this.gameObject);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (_target && other.gameObject != _target)
            return;

        if (other.CompareTag("Enemy"))
        {
            _target = other.gameObject;
        }
    }
}
