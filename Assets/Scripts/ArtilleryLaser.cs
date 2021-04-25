using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArtilleryLaser : MonoBehaviour
{
    private GameObject _zone;
    private Vector3 _zonePOS;
    private float _speed = 15.0f;
    private bool _doOnce = true;

    [SerializeField]
    private Player _player;

    private Boss _boss;
    private Vector3 _targetPOS;
    [SerializeField]
    private GameObject _explosion;

    // Start is called before the first frame update
    void Start()
    {
        _player = GameObject.Find("Player").GetComponent<Player>();

        if (_player == null)
            Debug.LogError("No Player found!");

        _zonePOS = GameObject.Find("Artillery_Zone(Clone)").transform.position;

        if (CompareTag("Artillery_Shot"))
            _targetPOS = _zonePOS;
        else if (CompareTag("Boss_Artillery"))
        {
            _boss = GameObject.Find("Boss(Clone)").GetComponent<Boss>();
            _targetPOS = _boss.GetTarget();
        }



    }

    // Update is called once per frame
    void Update()
    {
        transform.position = Vector3.MoveTowards(transform.position, _targetPOS, _speed * Time.deltaTime);

        if (_doOnce)
        {
            Destroy(this.gameObject, 3f);
            _doOnce = false;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            _player.Damage();
            GameObject explosionVisual = Instantiate(_explosion, transform.position, Quaternion.identity);
            Destroy(explosionVisual, 2.8f);
            Destroy(this.gameObject);
        }

        if (other.CompareTag("Artillery_Zone"))
        {
            GameObject explosionVisual = Instantiate(_explosion, transform.position, Quaternion.identity);
            Destroy(explosionVisual, 2.8f);
            this.gameObject.SetActive(false); // here to ensure the lasers continue travel
            Destroy(this.gameObject, 1.25f);
            Destroy(other.gameObject);
        }
    }

}
