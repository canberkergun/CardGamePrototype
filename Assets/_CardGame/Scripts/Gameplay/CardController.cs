using System;
using _CardGame.Scripts.Cards;
using _CardGame.Scripts.Managers;
using _CardGame.UI;
using _CardGame.UI.Scripts;
using UnityEngine;

namespace _CardGame.Scripts.Gameplay
{
    public class CardController : MonoBehaviour
    {
        public CardData cardData {get; private set;}

        public int currentHealth;
        
        private bool isActivated = false;
        private bool isPlayed = false;
        public bool hasBeenUsed = false;
        private bool hasActiveEffect = false;
        public UICard UICard {get; private set; }

        private void Awake()
        {
            UICard = GetComponent<UICard>();
        }

        public void Initialize(CardData data)
        {
            cardData = data;
            currentHealth = data.health;
            isActivated = false;
            isPlayed = false;
            hasBeenUsed = false;
            hasActiveEffect = false;
            
            GameManager.Instance.OnTurnEnd += ActivateCard;
            GameManager.Instance.OnTurnEnd += ResetCardUsage;
        }

        private void ActivateCard()
        {
            if (isPlayed)
            {
                isActivated = true;
                UICard.SetCardActive();
                GameManager.Instance.OnTurnEnd -= ActivateCard;
            }
        }

        public void PlayCard()
        {
            if ((cardData.cardEffect is DamageEffect && !isActivated && cardData.cardEffect.duration == 1) || hasBeenUsed || hasActiveEffect)
                return;

            GameObject target;
            bool isTargetPlayer = GameManager.Instance.currentState == GameManager.GameState.PlayerTurn;

            if (cardData.cardEffect is DamageEffect)
            {
                if (GameManager.Instance.currentState == GameManager.GameState.EnemyTurn)
                {
                    target = GameManager.Instance.player.gameObject;
                    ApplyEffectWithDurationCheck(target, isTargetPlayer);
                }
                else
                {
                    if (cardData.cardEffect.duration > 1)
                    {
                        GameManager.Instance.AddOngoingEffect(cardData.cardEffect, GameManager.Instance.enemy.gameObject, this);
                        hasActiveEffect = true;
                    }
                    else
                    {
                        AttackManager.Instance.StartAttackSelection(this);
                        return;
                    }
                }
            }
            else
            {
                target = isTargetPlayer ? GameManager.Instance.player.gameObject
                    : GameManager.Instance.enemy.gameObject;
                
                ApplyEffectWithDurationCheck(target, isTargetPlayer);
                
            }
            
            hasBeenUsed = true;
        }

        public void PlaceCard()
        {
            isPlayed = true;
        }

        private void ResetCardUsage()
        {
            hasBeenUsed = false;
        }
        
        private void ApplyEffectWithDurationCheck(GameObject usedOnTarget, bool IsTargetPlayer)
        {
            if (cardData.cardEffect.duration > 1)
            {
                hasActiveEffect = true;
                GameManager.Instance.AddOngoingEffect(cardData.cardEffect, usedOnTarget, this);
            }
            else 
            {
                cardData.cardEffect.ApplyEffect(usedOnTarget);
                if(cardData.cardEffect is not DamageEffect)
                    CardManager.Instance.RemoveCardFromHand(gameObject, IsTargetPlayer);
            }
        }

        public void TakeDamage(int damage, GameObject target)
        {
            currentHealth -= damage;
            if (currentHealth <= 0)
            {
                if (target.CompareTag("Player"))
                    CardManager.Instance.RemoveCardFromHand(gameObject, true);
                else
                    CardManager.Instance.RemoveCardFromHand(gameObject, false);
                Destroy(gameObject);
            }
                
            
            UICard.UpdateHealthText(currentHealth);
        }
        
        private void OnDestroy()
        {
            if (GameManager.Instance != null)
            {
                GameManager.Instance.OnTurnEnd -= ActivateCard;
                GameManager.Instance.OnTurnEnd -= ResetCardUsage;
            }
        }
    }
}
