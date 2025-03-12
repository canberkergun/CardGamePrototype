using TMPro;
using UnityEngine;

namespace _CardGame.Scripts.Managers
{
    public class UIManager : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI turnText;
        private bool isPlayerTurn = false;
        
        [SerializeField] private GameObject gameOverObj;
        [SerializeField] private TextMeshProUGUI gameOverText;
        private void Start()
        {
            GameManager.Instance.OnTurnEnd += UpdateTurnText;
            GameManager.Instance.OnGameOver += GameOver;
        }

        private void UpdateTurnText()
        {
            if (!isPlayerTurn)
            {
                turnText.text = "Enemy Turn";
                isPlayerTurn = true;
            }
            else
            {
                turnText.text = "Player Turn";
                isPlayerTurn = false;
            }
              
        }

        private void GameOver(bool playerWon)
        {
            gameOverObj.SetActive(true);
            gameOverText.text = playerWon ? "Game Over! Player Won" : "Game Over! \nYou Lost";
        }
    }
}
