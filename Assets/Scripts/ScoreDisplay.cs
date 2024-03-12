using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ScoreDisplay : MonoBehaviour
{
    private TextMeshProUGUI textMesh;

    private void Start()
    {
        textMesh = GetComponent<TextMeshProUGUI>();
        textMesh.text = "hola";
    }
    private void Update() {
        if (textMesh != null)
        {
            textMesh.text = GlobalVariables.score.ToString();
        }   
        else
        {
            Debug.Log("soy un texto nulo jaja");
        }
    }
}
