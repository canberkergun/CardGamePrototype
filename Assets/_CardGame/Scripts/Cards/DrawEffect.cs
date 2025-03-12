using _CardGame.Scripts.Managers;
using UnityEngine;

namespace _CardGame.Scripts.Cards
{
    [CreateAssetMenu(fileName = "NewDrawEffect", menuName = "Card Effects/Draw")]
    public class DrawEffect : CardEffect
    {
        public int drawAmount;
        
        public override void ApplyEffect(GameObject target)
        {
            if (GameManager.Instance.currentState == GameManager.GameState.PlayerTurn)
            {
                CardManager.Instance.DrawCards(drawAmount);
            }
            else
            {
                CardManager.Instance.DrawCardsForEnemy(drawAmount);
            }
        }
    }
}
