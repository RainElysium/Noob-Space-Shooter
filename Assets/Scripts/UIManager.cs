using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [SerializeField]
    private Text _scoreText;
    [SerializeField]
    private Sprite[] _liveSprites;
    [SerializeField]
    private Image _LivesImg;
    [SerializeField]
    private Text _GameOver;
    [SerializeField]
    public Text _RestartR;
    public int _score;

    private GameManager _gameManager;

    // Start is called before the first frame update
    void Start()
    {

        _scoreText.text = "Score: " + 0;
        _GameOver.gameObject.SetActive(false);
        _gameManager = GameObject.Find("Game_Manager").GetComponent<GameManager>();

        if (!_gameManager)
            Debug.LogError("GameManager is NULL.");
    }

    // Update is called once per frame
    public void UpdateScore(int score)
    {
        _scoreText.text = "Score: " + score;
    }

    public void UpdateLives(int currentLives)
    {
        _LivesImg.sprite = _liveSprites[currentLives];

        if (currentLives < 1)
        {
            GameOverSequence();
        }

    }

    void GameOverSequence()
    {
        StartCoroutine(GameOverFlickerRoutine());
        _RestartR.gameObject.SetActive(true);
        _gameManager.GameOver();


    }

    IEnumerator GameOverFlickerRoutine()
    {
        while (true)
        {
            _GameOver.gameObject.SetActive(true);
            yield return new WaitForSeconds(0.5f);
            _GameOver.gameObject.SetActive(false);
            yield return new WaitForSeconds(0.5f);
        }
    }


}