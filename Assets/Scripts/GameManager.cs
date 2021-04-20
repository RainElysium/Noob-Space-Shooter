using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [SerializeField]
    private bool _isGameOver;

    int _activeScene;

    private void Update()
    {
        _activeScene = SceneManager.GetActiveScene().buildIndex;

        if (Input.GetKeyDown(KeyCode.R) && _isGameOver)
            SceneManager.LoadScene(1); // Current game scene

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (_activeScene != 0)
                SceneManager.LoadScene(0);
            else
                Application.Quit();
        }
    }

    public void GameOver()
    {
        _isGameOver = true;
    }

}
