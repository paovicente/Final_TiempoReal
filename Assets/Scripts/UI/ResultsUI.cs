using TMPro;
using UnityEngine;

public class ResultsUI : MonoBehaviour
{
    [SerializeField] private TMP_Text resultText;

    private void Start()
    {
        string result = PlayerPrefs.GetString("GameResult", "ResultScene");

        resultText.text = result;
    }
}