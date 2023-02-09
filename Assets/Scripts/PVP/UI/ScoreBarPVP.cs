using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace WE.UI.PVP
{
    public class ScoreBarPVP : MonoBehaviour
    {
        [SerializeField]
        private TextMeshProUGUI lbCurrentScore, lbOpponentsScore;
 
        private void Start()
        {
            lbCurrentScore.text = "0";
            lbOpponentsScore.text = "0";
        }

        public void UpdateScore(int currentScore, int opponentsScore)
        {
            int totalScore = currentScore + opponentsScore;
            if (totalScore > 0)
            {
                lbCurrentScore.text = $"{currentScore}";
                lbOpponentsScore.text = $"{opponentsScore}";
            } else
            {
                lbCurrentScore.text = $"{0}";
                lbOpponentsScore.text = $"{0}";
            }
        }
    }
}