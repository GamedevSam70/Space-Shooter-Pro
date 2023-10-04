using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
    [SerializeField]
    private Text _scoreText;
    [SerializeField]
    private Image _livesImg;
    [SerializeField]
    private Sprite[] _livesSprites;
    [SerializeField]
    private Text _ammoLeft;
    [SerializeField]
    private Text _ammoOut;
    [SerializeField]
    private Slider _thrusterBar;
 
    [SerializeField]
    private Text _gameOverText;

    [SerializeField]
    private Text _restartText;

    private GameManager _gameManager;
    private int _ammoCount;
    private bool _ammoIsOut;

    // Start is called before the first frame update
    void Start()
    {
        _scoreText.text = "Score: " + 0;
        _ammoLeft.text = "Ammo: " + 15;
        _gameOverText.gameObject.SetActive(false);
        _restartText.gameObject.SetActive(false);
        _gameManager = GameObject.Find("Game_Manager").GetComponent<GameManager>();
        
       if (_gameManager == null)
        {
            Debug.LogError("GameManager is NULL");
        }
    }

    // Update is called once per frame
    
    public void UpdateScore(int playerScore)
    {
        _scoreText.text = "Score: " + playerScore.ToString();
    }

    public void UpdateLives(int currentLives)
    {
        if (currentLives > 0)
        {
            _livesImg.sprite = _livesSprites[currentLives];
        }
        
        if(currentLives == 0)
        {
           _GameOverSequence();
        }
    }

    public void UpdateAmmo(int ammo)
    {
       _ammoLeft.text = "Ammo: " + ammo.ToString();
    }

    void _GameOverSequence()
    {
        _gameManager.GameOver();
        _gameOverText.gameObject.SetActive(true);
        _restartText.gameObject.SetActive(true);
        StartCoroutine(GameOverFlickerRoutine());
      
    }

    IEnumerator GameOverFlickerRoutine()
    {
        while(true)
        {
            _gameOverText.gameObject.SetActive(false);
            yield return new WaitForSeconds(0.5f);
            _gameOverText.gameObject.SetActive(true);
            yield return new WaitForSeconds(0.5f);
        }
       
    }

    public void AmmoOut()
    {
        _ammoIsOut = true;
        StartCoroutine(AmmoOutFlickerRoutine());
    }

    public IEnumerator AmmoOutFlickerRoutine()
    {
        while(_ammoIsOut == true)
        {
            _ammoOut.gameObject.SetActive(true);
            yield return new WaitForSeconds(0.5f);
            _ammoOut.gameObject.SetActive(false);
            yield return new WaitForSeconds(0.5f);
        }
    }

    public void AmmoReceived()
    {
        _ammoIsOut = false; 
    }

    public void ThrusterBar(float thrusterPower)
    {
        _thrusterBar.value = thrusterPower;
    }

    
}
