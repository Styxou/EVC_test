using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


namespace Athena.Prototype
{
    public class Score : MonoBehaviour
    {
        [SerializeField] TextMeshProUGUI text;
        public int score;
        public void addScore()
        {
            score++;
            text.text = score.ToString();
        }
    }
}
