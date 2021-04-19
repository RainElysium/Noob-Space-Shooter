using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Powerup : MonoBehaviour
{

    [SerializeField]
    private float _speed = 3.0f;
    [SerializeField] // 0 = Triple Shot 1 = Speed 2 = Shields
    private int _powerupID;
    [SerializeField]
    private AudioClip _powerupClip;


    // Update is called once per frame
    void Update()
    {
        transform.Translate(Vector3.left * _speed * Time.deltaTime);

        if (transform.position.x <= -13.0f)
        {
            Destroy(this.gameObject);
        }

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            Player player = collision.transform.GetComponent<Player>();

            AudioSource.PlayClipAtPoint(_powerupClip, transform.position);
            if (player)
                switch (_powerupID)
                {
                    case 0: // triple shot
                        player.TripleShotActive();
                        break;
                    case 1: // Speed
                        player.SpeedBoostActive();
                        break;
                    case 2: // Shields
                        player.ShieldBoostActive();
                        break;
                    case 3: // Ammo
                        player.AmmoPowerup();
                        break;
                    case 4: // Health
                        player.HealthPowerup();
                        break;
                    case 5: // Hack
                        player.HackPowerup();
                        break;
                    case 6: // Asteroid Impact  
                        player.AsteroidImpact();
                        break;
                    default:
                        Debug.Log("Default value");
                        break;
                }

            Destroy(this.gameObject);
        }
    }
}
