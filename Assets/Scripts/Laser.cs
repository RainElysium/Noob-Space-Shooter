using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Laser : MonoBehaviour
{
    [SerializeField]
    private float _speed = 8.0f;

    [SerializeField]
    private bool _isEnemyLaser = false;

    // Update is called once per frame
    void Update()
    {
        if (_isEnemyLaser)
            MoveLeft();
        else
            MoveRight();
    }

    private void MoveRight()
    {
        transform.Translate(Vector3.right * _speed * Time.deltaTime);

        if (transform.position.x > 10.5f)
        {
            if (transform.parent)
                Destroy(transform.parent.gameObject);

            Destroy(this.gameObject);
        }
    }

    private void MoveLeft()
    {
        transform.Translate(Vector3.left * _speed * Time.deltaTime);

        if (transform.position.x < -13.0f)
        {
            if (transform.parent)
                Destroy(transform.parent.gameObject);

            Destroy(this.gameObject);
        }
    }

    public void ApplyEnemyLaser()
    {
        _isEnemyLaser = true;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Player" && _isEnemyLaser == true)
        {
            Player player = other.GetComponent<Player>();

            if (player)
            {
                player.Damage();
                Destroy(this.gameObject);
            }

        }
    }

}
