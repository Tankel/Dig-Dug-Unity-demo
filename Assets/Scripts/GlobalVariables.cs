using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobalVariables : MonoBehaviour
{
    public static int score = 0;
    public static int lives = 3;
    public static int enemyCount = 4;
    public static bool isGamePaused;

    // Métodos para modificar las variables globales
    public static void IncreaseScore(int points) {
        score += points;
    }

    public static void DecreaseLives() {
        lives--;
    }
    public static void DecreaseEnemyCount() {
        Debug.Log("quitamos una vida");
        enemyCount--;
    }
    public static void ResetGame() {
        score = 0;
        lives = 3;
        isGamePaused = false;
        // Puedes agregar más reinicios de variables aquí si es necesario
    }
}
