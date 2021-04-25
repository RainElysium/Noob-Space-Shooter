using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossArms : MonoBehaviour
{
    private int _health = 5;
    [SerializeField]
    private GameObject _fire, _explosion;
    private Boss _boss;

    // Start is called before the first frame update
    void Start()
    {
        _boss = GameObject.Find("Boss(Clone)").GetComponent<Boss>();
    }

    // Update is called once per frame
    void Update()
    {
        if (_health == 2)
        {
            _fire.SetActive(true);

        }
    }
    void Damage()
    {
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player_Laser"))
        {
            --_health;
            Destroy(collision.gameObject);
        }

        if (_health == 0)
        {
            GameObject explosionVisual = Instantiate(_explosion, _fire.transform.position, Quaternion.identity);
            _fire.SetActive(false);
            Destroy(this.gameObject);
            Destroy(explosionVisual.gameObject, 2.5f);

            switch (tag)
            {
                case "Boss_Inner_Bottom":
                    _boss.DestroyArmCount(0);
                    break;
                case "Boss_Inner_Top":
                    _boss.DestroyArmCount(1);
                    break;
                case "Boss_Outter_Bottom":
                    _boss.DestroyArmCount(2);
                    break;
                case "Boss_Outter_Top":
                    _boss.DestroyArmCount(3);
                    break;
            }
        }
    }

}
