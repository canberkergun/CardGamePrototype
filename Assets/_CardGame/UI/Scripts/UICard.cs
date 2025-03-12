using System;
using _CardGame.Scripts.Cards;
using _CardGame.Scripts.Gameplay;
using _CardGame.Scripts.Managers;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace _CardGame.UI.Scripts
{
    public class UICard : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerClickHandler
    {
        [SerializeField] private TextMeshProUGUI nameText;
        [SerializeField] private TextMeshProUGUI costText;
        [SerializeField] private TextMeshProUGUI descriptionText;
        [SerializeField] private TextMeshProUGUI healthText;
        [SerializeField] private GameObject lockObject;
        
        private Transform originalParent;
        private Vector3 originalPosition;
        private Tween borderTween;
        
        [SerializeField] private CanvasGroup canvasGroup;
        [SerializeField] private RectTransform rectTransform;
        [SerializeField] private CardController cardController;
        [SerializeField] private Image borderImage;
        
        private bool isDroppedOnPlayArea = false; 
        private bool isPlayerCard = true;
        private bool isReplaceable = false;

        public GameObject backgroundImage;
        

        public void Initialize(CardData data)
        {
            nameText.text = data.name;
            costText.text = $"{data.cost}";
            descriptionText.text = data.description;
            healthText.text = $"{data.health}";
            
            cardController.Initialize(data);
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            if (!isPlayerCard || isDroppedOnPlayArea)
                return;
            
            originalParent = transform.parent;
            originalPosition = rectTransform.anchoredPosition;
            transform.SetParent(transform.root);
            canvasGroup.blocksRaycasts = false;
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (!isPlayerCard || isDroppedOnPlayArea)
                return;
            
            rectTransform.anchoredPosition += eventData.delta;
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            if (!isPlayerCard || isDroppedOnPlayArea)
                return;
            
            canvasGroup.blocksRaycasts = true;
            
            if (IsValidDropArea(eventData))
            {
                MoveCardToPlayArea();
            }
            else
            {
                ResetPosition();
            }
        }

        private bool IsValidDropArea(PointerEventData eventData)
        {
            GameObject dropTarget = eventData.pointerEnter;
            return dropTarget != null && dropTarget.CompareTag("PlayArea");
        }
        
        public void OnPointerClick(PointerEventData eventData)
        {
            if (isReplaceable)
            {
                CardManager.Instance.ReplaceCard(gameObject);
                return;
            }
            if (isDroppedOnPlayArea && GameManager.Instance.currentState == GameManager.GameState.PlayerTurn)
                cardController.PlayCard();   
        }

        private void MoveCardToPlayArea()
        {
            if (!GameManager.Instance.player.SpendMana(cardController.cardData.cost))
            {
                Debug.Log("Not enough Mana!");
                ResetPosition();
                return;
            }

            if (cardController != null)
            {
                if (cardController.cardData.cardEffect is not DamageEffect &&
                    cardController.cardData.cardEffect.duration == 1)
                {
                    cardController.PlayCard();
                    return;
                }
                if (cardController.cardData.cardEffect is DamageEffect &&
                         cardController.cardData.cardEffect.duration > 1)
                {
                    cardController.PlayCard();
                }
            }

            CardManager.Instance.PlaceCardInPlayArea(gameObject, true);
            
            lockObject.SetActive(true);
            cardController.PlaceCard();
            isDroppedOnPlayArea = true;
        }

        public void EnableReplacementMode(bool enable)
        {
            if (borderTween != null)
                borderTween.Kill();

            if (enable)
            {
                borderImage.gameObject.SetActive(true);
                borderTween = borderImage.DOFade(1f, 0.5f).SetLoops(-1, LoopType.Yoyo);
                isReplaceable = true;
            }
            else
            {
                borderTween.Kill();
                borderImage.DOFade(0f, 0.2f).OnComplete(() => borderImage.gameObject.SetActive(false));
                isReplaceable = false;
            }
        }
        
        private void ResetPosition()
        {
            transform.SetParent(originalParent);
            transform.position = originalPosition;
        }

        public void SetEnemyCard()
        {
            canvasGroup.blocksRaycasts = false;
        }

        public void SetCardActive()
        {
            lockObject.SetActive(false);
        }

        public void UpdateHealthText(int currentHealth)
        {
            healthText.text = $"{currentHealth}";
        }
    }
}
