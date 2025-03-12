using System.Collections.Generic;
using _CardGame.Scripts.Cards;
using _CardGame.Scripts.Gameplay;
using _CardGame.UI;
using _CardGame.UI.Scripts;
using DG.Tweening;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace _CardGame.Scripts.Managers
{
    public class CardManager : MonoBehaviour
    {
        public static CardManager Instance { get; private set; }
        
        [SerializeField] private Transform playerHandPanel;
        [SerializeField] private Transform enemyHandPanel;
        public Transform playerArea;
        [SerializeField] private Transform enemyArea;
        [SerializeField] private RectTransform playerDeckTransform;
        [SerializeField] private RectTransform enemyDeckTransform;
        [SerializeField] private GameObject cardPrefab;
        
        [SerializeField] private List<GameObject> enemyPlayArea = new List<GameObject>();
        [SerializeField] private List<GameObject> playerPlayArea = new List<GameObject>();
        
        private List<GameObject> enemyHand = new List<GameObject>();
        private List<GameObject> playerHand = new List<GameObject>();

        private const int maxPlayAreaSize = 5;
        private GameObject replacingCard;
        private bool replacingCardIsPlayer;
        
        private void Awake()
        {
            if (Instance == null)
                Instance = this;
            else
                Destroy(gameObject);
        }

        private void Start()
        {
            GameManager.Instance.DrawCards += OnPlayerTurn;
        }
        
        private void OnPlayerTurn()
        {
            DrawCards(1);
        }

        public void DrawCards(int amount)
        {
            for (int i = 0; i < amount; i++)
            {
                CardData drawnCard = DeckManager.Instance.DrawPlayerCard();
                if (drawnCard == null)
                    return;
                AddCardToHand(drawnCard);
            }
        }

        public void DrawCardsForEnemy(int amount)
        {
            for (int i = 0; i < amount; i++)
            {
                CardData drawnCard = DeckManager.Instance.DrawEnemyCard();
                if (drawnCard == null)
                    return;
                AddCardToEnemyHand(drawnCard);
            }
        }

        private void AddCardToEnemyHand(CardData card)
        {
            GameObject cardObj = Instantiate(cardPrefab, enemyDeckTransform);
            
            
            CardController cardController = cardObj.GetComponent<CardController>();
            UICard uiCard = cardController.UICard;
            
            cardObj.gameObject.tag = "Minion";
            uiCard.Initialize(card);
            uiCard.SetEnemyCard();
            
            cardObj.transform.DOMove(enemyHandPanel.position, 1.2f).SetEase(Ease.OutQuad)
                .OnComplete(() =>
                {
                    cardObj.transform.SetParent(enemyHandPanel, false);
                });
            
            enemyHand.Add(cardObj);
            if (!IsCardDamageEffect(cardController))
            {
                cardController.PlayCard();
                uiCard.backgroundImage.SetActive(false);
            }
        }

        private bool IsCardDamageEffect(CardController cardController)
        {
            return cardController.cardData.cardEffect is DamageEffect;
        }

        private void AddCardToHand(CardData card)
        {
            GameObject cardObject = Instantiate(cardPrefab, playerDeckTransform);
            
            
            UICard cardUI = cardObject.GetComponent<UICard>();
            cardUI.Initialize(card);

            cardObject.transform.DOMove(playerHandPanel.position, 1.2f).SetEase(Ease.OutQuad)
                .OnComplete(() =>
                {
                    cardObject.transform.SetParent(playerHandPanel, false);
                    cardUI.backgroundImage.SetActive(false);
                });
            
            
            
            playerHand.Add(cardObject);
        }

        public void RemoveCardFromHand(GameObject card, bool isPlayerHandRemove)
        {
            if (isPlayerHandRemove)
            {
                playerHand.Remove(card);
                playerPlayArea.Remove(card);
            }
            else
            {
                enemyHand.Remove(card);
                enemyPlayArea.Remove(card);
            }

            Destroy(card);
        }

        public void PlaceCardInPlayArea(GameObject cardObj, bool isPlayer)
        {
            Transform playArea = isPlayer ? playerArea : enemyArea;
            List<GameObject> playList = isPlayer ? playerPlayArea : enemyPlayArea;

            if (playList.Count >= maxPlayAreaSize)
            {
                Debug.Log("Choose a card to replace");
                EnableCardReplacement(isPlayer, cardObj);
                return;
            }
            
            cardObj.transform.SetParent(playArea);
            playerPlayArea.Add(cardObj);
        }
        
        public void PlaceCardInEnemyPlayArea(GameObject cardObj)
        {
            cardObj.transform.SetParent(enemyArea);
            enemyPlayArea.Add(cardObj);
        }
        
        public List<GameObject> GetEnemyHand()
        {
            return enemyHand;
        }
        
        private void EnableCardReplacement(bool enable, GameObject newCard = null)
        {
            replacingCard = newCard;
            foreach (GameObject card in playerPlayArea)
            {
                card.GetComponent<UICard>().EnableReplacementMode(enable);
            }
        }

        public void ReplaceCard(GameObject oldCard)
        {
            playerPlayArea.Remove(oldCard);
            Destroy(oldCard);
            
            replacingCard.transform.SetParent(playerArea);
            playerPlayArea.Add(replacingCard);
            EnableCardReplacement(false);
        }
    }
}
