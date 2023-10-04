using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Mail;

using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField]
    private float _speed = 5f;
    private float _speedMultiplier = 2;
    [SerializeField]
    private GameObject _laserPrefabs;
    [SerializeField]
    private GameObject _Triple_ShotPrefabs;
    [SerializeField]
    private GameObject _thrusterBar;
    
    
    private float _fireRate = 0.15f;
    private float _nextFire = -1f;
    [SerializeField]
    private int _ammoCount = 15;
    [SerializeField]
    private AudioClip _laserSoundClip;
    private AudioSource _audioSource;
    [SerializeField]
    private float _thrusterSpeed = 2f;
    private float _thrusterPower = 100f;
    private float _thrusterUsage = 1f;
    
    private float _thrusterWait = 3.0f;
    
    [SerializeField]
    private int _lives = 3;
    private SpawnManager _spawnManager;

    private bool _isTripleShotActive = false;
    private bool _isShieldsActive = false;
    private bool _isSpeedBoostActive = false;
    private bool _isMultiShotActive = false;
    private bool _isThrusterActive = false;

    [SerializeField]
    private GameObject _shieldVisualizer;
    [SerializeField]
    private int _shieldStrength = 0;
    [SerializeField]
    private Color[] _shieldColor;
   
    private SpriteRenderer _isShieldVisualizerSpriteRenderer;

    [SerializeField]
    private GameObject _leftEngine, _rightEngine;
    

    [SerializeField]
    private int _score;

    [SerializeField]
    private UIManager _uiManager;
    [SerializeField]
    private Main_Camera _mainCamera;

    // Start is called before the first frame update
    void Start()
    {
        // take current position = new position (0, 0, 0)
        transform.position = new Vector3(0, 0, 0);
        _spawnManager = GameObject.Find("Spawn_Manager").GetComponent<SpawnManager>();
        _uiManager = GameObject.Find("UI_Manager").GetComponent<UIManager>();
        _audioSource = GetComponent<AudioSource>();
        _isShieldVisualizerSpriteRenderer = GetComponent<SpriteRenderer>();
        _mainCamera = GameObject.Find("Main Camera").GetComponent<Main_Camera>();  
        
        if (_spawnManager == null)
        {
            Debug.LogError("Spawn Manager is null");
        }

        if (_uiManager == null)
        {
            Debug.LogError("UI Manager is NULL");
        }

        if (_audioSource == null)
        {
            Debug.LogError("AudioSource on Player is NULL");
        }
        else
        {
            _audioSource.clip = _laserSoundClip;
        }

        if (_mainCamera == null)
        {
            Debug.LogError("Main Camera is NULL");
        }

    }

    // Update is called once per frame
    void Update()
    {
        CalculateMovement();

        if (Input.GetKeyDown(KeyCode.Space) && Time.time > _nextFire)
        {
            FireLaser();
        }

        //press Left Shift to increase the speed
        //release Left Shift to go back to normal speed
                              
    }

    void CalculateMovement()
    {
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        Vector3 direction = new Vector3(horizontalInput, verticalInput, 0);
        // new Vector3(1, 0, 0)
        transform.Translate(direction * _speed * Time.deltaTime);

        if (transform.position.y >= 6)
        {
            transform.position = new Vector3(transform.position.x, 6, 0);
        }
        else if (transform.position.y <= -6)
        {
            transform.position = new Vector3(transform.position.x, -6, 0);
        }

        if (transform.position.x > 10)
        {
            transform.position = new Vector3(-10, transform.position.y, 0);
        }
        else if (transform.position.x < -10)
        {
            transform.position = new Vector3(10, transform.position.y, 0);
        }

        if (Input.GetKey(KeyCode.LeftShift))
        {
            transform.Translate(direction * _speed * _thrusterSpeed * Time.deltaTime);
            StartCoroutine(ThrusterDeplete());
            
        }
        else
        {
            transform.Translate(direction * _speed * Time.deltaTime);
            StartCoroutine(ThrusterReplen());
        }
    }

    void FireLaser()
    {
        _nextFire = Time.time + _fireRate;

        if (_isTripleShotActive == true && _ammoCount > 0)
        {
            Instantiate(_Triple_ShotPrefabs, transform.position, Quaternion.identity);
            _ammoCount = _ammoCount - 1;
            UpdatePlayerAmmo(_ammoCount);
        }

        else if(_isMultiShotActive == true && _ammoCount > 0)
        {
            Instantiate(_laserPrefabs, transform.position + new Vector3(0, 1.05f, 0), Quaternion.identity);
            Instantiate(_laserPrefabs, transform.position + new Vector3(0, 1.05f, 0), Quaternion.Euler(0, 0, 45));
            Instantiate(_laserPrefabs, transform.position + new Vector3(0, 1.05f, 0), Quaternion.Euler(0, 0, 90));
            Instantiate(_laserPrefabs, transform.position + new Vector3(0, 1.05f, 0), Quaternion.Euler(0, 0, -45));
            Instantiate(_laserPrefabs, transform.position + new Vector3(0, 1.05f, 0), Quaternion.Euler(0, 0, -90));
            _ammoCount = _ammoCount - 1;
            UpdatePlayerAmmo(_ammoCount);
        }
        
        else if (_ammoCount > 0)
        {
            Instantiate(_laserPrefabs, transform.position + new Vector3(0, 1.05f, 0), Quaternion.identity);
            _ammoCount = _ammoCount - 1;
            UpdatePlayerAmmo(_ammoCount);
        }

        if (_ammoCount > 0)
        {
            _audioSource.Play();
        }
        else 
        {
            _uiManager.AmmoOut();
        }
                      
    }

    public void Damage()
    {
        if (_isShieldsActive == true)
        {
            if (_shieldStrength >= 1)
            {
                _shieldStrength = _shieldStrength - 1;
                UpdateShieldsColor();
                _mainCamera.CameraShake();
                return;
            }
            if (_shieldStrength == 0)
            {
                _isShieldsActive = false;
                _shieldVisualizer.SetActive(false);
                return;
            }
               
        }

        _lives = _lives - 1;
        _mainCamera.CameraShake();

        if (_lives == 2)
        {
            _leftEngine.SetActive(true);
        }
        else if (_lives == 1)
        {
            _rightEngine.SetActive(true);
        }

        _uiManager.UpdateLives(_lives);

        if (_lives < 1)
        {
            _spawnManager.OnPlayerDeath();
            Destroy(this.gameObject);
       }
    }

    public void TripleShotActive()
    {
       _isTripleShotActive = true;
        StartCoroutine(TripleShotPowerDown());
    }

    IEnumerator TripleShotPowerDown()
    {
        yield return new WaitForSeconds(5.0f);
       _isTripleShotActive = false;
    }

    public void MultiShotActive()
    {
       _isMultiShotActive = true;
       StartCoroutine(MultishotPowerDown());
    }

    IEnumerator MultishotPowerDown()
    {
        yield return new WaitForSeconds(5.0f);
        _isMultiShotActive = false;
    }

    public void SpeedBoostActive()
    {
        _isSpeedBoostActive = true;
        _speed *= _thrusterSpeed;
        StartCoroutine(SpeedBoostPowerDownRoutine());
    }

    IEnumerator SpeedBoostPowerDownRoutine()
    {
        yield return new WaitForSeconds(5.0f);
        _speed *= _speedMultiplier;
        _isSpeedBoostActive = false;
    }

    public void ShieldsActive() 
    {
        _isShieldsActive = true;
        _shieldVisualizer.SetActive(true);
        _shieldStrength = 3;
        _isShieldVisualizerSpriteRenderer = _shieldVisualizer.GetComponent<SpriteRenderer>();
    }

    public void UpdateShieldsColor()
    {
        if (_shieldStrength > 0)
        {
            _isShieldVisualizerSpriteRenderer.color = _shieldColor[_shieldStrength];
        }
    }

    public void AddScore(int points)
    {
        _score += points;
        _uiManager.UpdateScore(_score);
    }

    public void AmmoPowerUpActive()
    {
        _ammoCount = _ammoCount + 15;
        UpdatePlayerAmmo(_ammoCount);
        StopCoroutine(_uiManager.AmmoOutFlickerRoutine());
    }

    public void UpdatePlayerAmmo(int ammo)
    {
        _uiManager.UpdateAmmo(ammo);
        _uiManager.AmmoReceived();
    }

    public void HealthPowerUpActive()
    {
       if (_lives < 3)
        {
            _lives = _lives + 1;
            _uiManager.UpdateLives(_lives);

            if (_lives >= 2)
            {
                _leftEngine.SetActive(false);
                _rightEngine.SetActive(false);
            }

            else if (_lives > 1)
            {
                _leftEngine.SetActive(false);
                _rightEngine.SetActive(true);
            }

            else if (_lives == 1)
            {
                _leftEngine.SetActive(true) ;
            }
        }
    }

    IEnumerator ThrusterDeplete()
    {
        _isThrusterActive = true;

        while (Input.GetKey(KeyCode.LeftShift) && _thrusterPower > 0) 
        {
            yield return null;
            _thrusterPower = _thrusterPower - _thrusterUsage * Time.deltaTime;
            _uiManager.ThrusterBar(_thrusterPower);
        }
    }

    IEnumerator ThrusterReplen()
    {
        _isThrusterActive = false;

        if (_thrusterPower < 100 && _isThrusterActive == false)
        {
            yield return new WaitForSeconds(_thrusterWait);
        }

        while (_thrusterPower < 100 && _isThrusterActive == false)
        {
            yield return null;
            _thrusterPower = _thrusterPower + _thrusterUsage * Time.deltaTime;
            _uiManager.ThrusterBar(_thrusterPower);
        }
    }

}

       