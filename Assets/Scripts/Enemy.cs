using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField]
    private float _enemySpeed = 2f;

    private Player _player;
    [SerializeField]
    private GameObject _laserPrefab;
    private Animator _anim;

    private AudioSource _audioSource;
   
    private float _fireRate = 3.0f;
    private float _nextFire = -1f;

    // Start is called before the first frame update
    void Start()
    {
        _player = GameObject.Find("Player").GetComponent<Player>();

        if (_player == null)
        {
            Debug.LogError("Player is NULL");
        }

        _anim = GetComponent<Animator>();

        if (_anim == null)
        {
            Debug.LogError("Animator is NULL");
        }

        _audioSource = GetComponent<AudioSource>();

        if (_audioSource == null)
        {
            Debug.LogError("AudioSource in Enemy is NULL");
        }
        
    }

    // Update is called once per frame
    void Update()
    {
        CalculateMovement();

        if (Time.time > _nextFire)
        {
            _fireRate = Random.Range(3f, 7f);
            _nextFire = _fireRate + Time.time;
            GameObject enemyLaser = Instantiate(_laserPrefab, transform.position, Quaternion.identity);
            Laser[] lasers = enemyLaser.GetComponentsInChildren<Laser>();

            for (int i = 0; i < lasers.Length; i++)
            {
                lasers[i].AssignEnemyLaser();
            }
        }
    }

    private void CalculateMovement()
    {
        transform.Translate(Vector3.down * _enemySpeed * Time.deltaTime);

        if (transform.position.y < -6f)
        {
            float randomX = Random.Range(-8f, 8f);
            transform.position = new Vector3(randomX, 6, 0);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {

        if (other.tag == "Player")
        {
            Player player = other.transform.GetComponent<Player>();
            if (player != null)
            {
                player.Damage();
            }

            _anim.SetTrigger("OnEnemyDeath");
            _enemySpeed = 0.0f;
            _audioSource.Play();
            
            Destroy(this.gameObject, 1.5f);
            

        }

        if (other.tag == "Laser")
        {
            Destroy(other.gameObject);

            if (_player != null)
            {
                {
                    _player.AddScore(10);
                }

                _anim.SetTrigger("OnEnemyDeath");
                _enemySpeed = 0.0f;
                _audioSource.Play();

                Destroy(GetComponent<Collider2D>());
                Destroy(this.gameObject, 1.5f);
               
            }
        }
    }

}
           