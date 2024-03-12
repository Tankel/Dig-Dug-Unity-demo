using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public GameObject gameOverUI;
    public GameObject youWinUI;
    public GameObject gameOverUIBg;
    public GameObject youWinUIBg;

    void Start()
    {
        // Inicializar las variables de juego
        gameOverUI.SetActive(false);
        youWinUI.SetActive(false);
        gameOverUIBg.SetActive(false);
        youWinUIBg.SetActive(false);
    }

    void Update()
    {
        // Verificar condiciones de fin de juego
        if (GlobalVariables.lives <= 0)
        {
            Invoke("GameOver",1f);
        }
        else if (GlobalVariables.enemyCount <= 0)
        {
            Invoke("YouWin",1f);
        }
    }

    void GameOver()
    {
        // Activar la interfaz de "Game Over" y pausar el juego
        gameOverUIBg.SetActive(true);
        gameOverUI.SetActive(true);
        Time.timeScale = 0f; // Pausar el juego
        GlobalVariables.isGamePaused = true;
    }

    void YouWin()
    {
        // Activar la interfaz de "You Win" y pausar el juego
        youWinUIBg.SetActive(true);
        youWinUI.SetActive(true);
        Time.timeScale = 0f; // Pausar el juego
        GlobalVariables.isGamePaused = true;
    }
}
