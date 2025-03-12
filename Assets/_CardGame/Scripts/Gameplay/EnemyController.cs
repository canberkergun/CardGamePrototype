using System.Collections;
using System.Collections.Generic;
using _CardGame.Scripts.Cards;
using _CardGame.Scripts.Managers;
using _CardGame.UI.Scripts;
using Unity.VisualScripting;
using UnityEngine;

namespace _CardGame.Scripts.Gameplay
{
    public class EnemyController : MonoBehaviour
    {
        private List<GameObject> enemyPlayArea = new List<GameObject>();

        public void TakeTurn()
        {
            StartCoroutine(EnemyTurn());
        }

        private IEnumerator EnemyTurn()
        {
            CardManager.Instance.DrawCardsForEnemy(1);
            
            yield return new WaitForSeconds(1.5f);
            PlaceCardsInPlayArea();
            
            yield return new WaitForSeconds(1f);
            ActivateEnemyCards();

            yield return new WaitForSeconds(1.5f);
            GameManager.Instance.EndTurn();
        }

        private void PlaceCardsInPlayArea()
        {
            List<GameObject> enemyHand = CardManager.Instance.GetEnemyHand();
            
            if (enemyHand.Count == 0)
            {
                return;
            }
            
            int randomIndex = Random.Range(0, enemyHand.Count);

            for (int i = 0; i < randomIndex + 1; i++)
            {
                GameObject cardObject = enemyHand[randomIndex];
                CardController cardController = cardObject.GetComponent<CardController>();

                if (!GameManager.Instance.enemy.SpendMana(cardController.cardData.cost))
                {
                    Debug.Log("Enemy does not have enough mna");
                    return;
                }
                
                if (cardController != null)
                {
                    if (cardController.cardData.cardEffect is not DamageEffect)
                    {
                        cardController.PlayCard();
                    }
                    else
                    {
                        CardManager.Instance.PlaceCardInEnemyPlayArea(enemyHand[randomIndex]);
                        enemyPlayArea.Add(enemyHand[randomIndex]);
                        cardController.PlaceCard();
                    }
                }
            }
        }

        private void ActivateEnemyCards()
        {
            if (enemyPlayArea.Count == 0)
                return;

            foreach (GameObject cardObject in enemyPlayArea)
            {
                if (cardObject != null)
                {
                    CardController card = cardObject.GetComponent<CardController>();
                    if (card != null)
                    {
                        card.PlayCard();
                        card.GetComponent<UICard>().backgroundImage.SetActive(false);
                    }
                }
            }
        }
    }
}
