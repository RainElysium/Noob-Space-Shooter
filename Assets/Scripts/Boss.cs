using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss : MonoBehaviour
{
    // 
    private UIManager _uiManager;
    private float _speed = 3f;
    private Vector3 _bossCurrentPosition;
    [SerializeField]
    private List<GameObject> _firingMechanisms = new List<GameObject>();
    [SerializeField]
    private GameObject _artilleryZone, _explosion, _shield;
    private List<Vector3> _artZone = new List<Vector3>();
    private Vector3 _artTargets;
    private Vector3 _positionA, _positionB;
    private bool _atPositionA, _atPositionB;
    private int _phase = 1;
    private bool _canMove;
    private bool _doOnce = true;
    private bool _isDead;
    [SerializeField]
    private List<GameObject> _fires = new List<GameObject>();
    [SerializeField]
    private List<GameObject> _explosionPoints = new List<GameObject>();
    [SerializeField]
    private GameObject _artilleryMechanism;
    private Vector3 target;
    [SerializeField]
    private GameObject _topFire, _bottomFire;

    [SerializeField]
    private int _deadArmsCount = 0;
    private int _coreBossHealth = 10;
    [SerializeField]
    private GameObject _deathExplosion;
    private int laserCount;
    private Player _player;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(FireLasersTimer());

        _player = GameObject.Find("Player").GetComponent<Player>();
        _uiManager = GameObject.Find("Canvas").GetComponent<UIManager>();
        _canMove = true;
        _atPositionA = false;
        _atPositionB = false;
        _isDead = false;

        // phases
        _phase = 1;
        _positionA = new Vector3(5.0f, 4.65f);
        _positionB = new Vector3(5.0f, -2.30f);

    }

    // Update is called once per frame
    void Update()
    {
        if (!_isDead)
        CalculateMovement();
    }

    void CalculateMovement()
    {

        if (transform.position.y >= 4.65f)
        {
            _atPositionA = true;
            _atPositionB = false;
        }

        else if (transform.position.y <= -2.30f)
        {
            _atPositionB = true;
            _atPositionA = false;
        }

        if (CompareTag("Boss"))
        {
            _bossCurrentPosition = transform.position;

            if (_phase == 1) // get into intial position
            {
                transform.position = Vector3.MoveTowards(transform.position, new Vector3(5.0f, 0, 0), _speed * Time.deltaTime);

                if (_bossCurrentPosition.x == 5.0f)
                    ++_phase; // 2
            }

            if (_phase == 2 && _doOnce) // initial firing
            {
                StartCoroutine(ArtilleryFire());
               _canMove = false;
               _doOnce = false;
                ++_phase; // 3
            }

            if (_canMove && _phase == 3) // Movement phases
            {
                transform.position = Vector3.MoveTowards(transform.position, _positionA, _speed * Time.deltaTime);

                if (transform.position.y == 4.65f)
                {
                    _atPositionA = true;
                    _canMove = false;
                    StartCoroutine(ArtilleryFire());
                    ++_phase; // 4
                }
            }

            if (_phase == 4 && _canMove)
            {
               MoveToPoints();
            }
        }

    }

    void FireLasers(int laserCount)
    {
        for (int i = 0; i < laserCount; ++i)
        {
            if (_firingMechanisms[i] != null)
            {
                GameObject laser = Instantiate(_firingMechanisms[i], transform.position, Quaternion.identity, gameObject.transform);
                laser.gameObject.transform.SetParent(null);
                Destroy(laser, 5f);
            }
        }
    }

    IEnumerator FireLasersTimer()
    {
        while (true)
        {
            yield return new WaitForSeconds(5f);
            laserCount = _firingMechanisms.Count;
            FireLasers(laserCount);
        }
    }

    IEnumerator ArtilleryZoneTimerRoutine(GameObject zone)
    {
        yield return new WaitForSeconds(3.75f);

        GameObject _artilleryZone = Instantiate(_artilleryMechanism, transform.position, Quaternion.identity); // Artillery Laser creation
        Destroy(_artilleryZone,1f);

        yield return new WaitForSeconds(1.25f);
        
        Destroy(zone.gameObject, 1f);
        _canMove = true;

    }
    IEnumerator ArtilleryFire()
    {
        _doOnce = false;
        for (int i = 0; i < 6; i++)
        {
            yield return new WaitForSeconds(.25f);

            _artTargets = new Vector3(Random.Range(-10.11f, -2.50f), Random.Range(4.797f, -3.34f), 0);
            GameObject newZone = Instantiate(_artilleryZone, _artTargets, Quaternion.identity); // Drop artillery zone
            if (_artZone.Count > 1)
            {
                newZone.transform.position = new Vector3(newZone.transform.position.x, newZone.transform.position.y + 1.5f, 0);
            }

            _artZone.Add(newZone.transform.position);

            StartCoroutine(ArtilleryZoneTimerRoutine(newZone));
        }
    }

    public Vector3 GetTarget()
    {
        for (int i = 0; i < _artZone.Count; i++)
        {
            target = _artZone[i];
            if (target != null)
            {
                _artZone.RemoveAt(i);
                return target;
            }
        }
        Debug.LogError("ERROR: NO TARGET FOUND");
        return new Vector3(0, 0, 0);
    }

    IEnumerator CheckPositionForMovement()
    {
        yield return new WaitForSeconds(5f);
        _canMove = true;
        _doOnce = true;
    }

    void MoveToPoints()
    {
        if (_atPositionA)
        {
            transform.position = Vector3.MoveTowards(transform.position, _positionB, _speed * Time.deltaTime);

            if (transform.position.y <= -2.30f)
            {
                _canMove = false;
                StartCoroutine(CheckPositionForMovement());
                StartCoroutine(ArtilleryFire());
            }
        }
        else if (_atPositionB)
        {
            transform.position = Vector3.MoveTowards(transform.position, _positionA, _speed * Time.deltaTime);

            if (transform.position.y >= 4.65f)
            {
                _canMove = false;
                StartCoroutine(CheckPositionForMovement());
                StartCoroutine(ArtilleryFire());
            }
        }
    }
    public void DestroyArmCount(int armNumber)
    {
        if (_deadArmsCount != 4)
            _firingMechanisms[armNumber] = null;

        ++_deadArmsCount;

        if (_firingMechanisms[1] == null && _firingMechanisms[3] == null)
            _topFire.SetActive(true);

        if (_firingMechanisms[0] == null && _firingMechanisms[2] == null)
            _bottomFire.SetActive(true);


        if (_deadArmsCount == 4)
            StartCoroutine(ShieldPowerdown());
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player_Laser"))
        {
            Destroy(collision.gameObject);

            if (_deadArmsCount == 4)
            {
                --_coreBossHealth;
            }

            switch (_coreBossHealth)
            {
                case 7:
                    _fires[0].SetActive(true);
                    break;
                case 5:
                    _fires[1].SetActive(true);
                    break;
                case 3:
                    _fires[2].SetActive(true);
                    break;
                case 0:
                    if (_isDead == false)
                    {
                        StartCoroutine(BossDeathRoutine());
                        _isDead = true;
                    }
                    break;      
            }
        }
    }

    IEnumerator ShieldPowerdown()
    {
        if (_shield.activeInHierarchy)
        for (int i = 0; i < 4; i++)
        {
            yield return new WaitForSeconds(.1f);
            _shield.SetActive(false);
            yield return new WaitForSeconds(.1f);
            _shield.SetActive(true);
            yield return new WaitForSeconds(.1f);
            _shield.SetActive(false);
        }
    }

    IEnumerator BossDeathRoutine()
    {
        for (int i = 0; i < _explosionPoints.Count; i++)
        {
            yield return new WaitForSeconds(.5f);
            GameObject explosion = Instantiate(_explosion, _explosionPoints[i].transform.position, Quaternion.identity);
            Destroy(explosion, 2f);

            if (i == 7)
            {
                _deathExplosion.SetActive(true);
                Destroy(this.gameObject, 1f);
                _uiManager.PlayerWon();
            }

        }
    }
    
}
