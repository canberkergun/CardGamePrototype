using System;
using _CardGame.Scripts.Gameplay;
using _CardGame.UI;
using UnityEngine;

namespace _CardGame.Scripts.Managers
{
    public class AttackManager : MonoBehaviour
    {
        public static AttackManager Instance { get; private set; }
        
        public GameObject projectilePrefab;
        public GameObject floatingTextPrefab;

        private LineRenderer attackLine;
        private CardController selectedCard;
        private bool isAttacking = false;

        public Canvas canvas;

        private void Awake()
        {
            if (Instance == null)
                Instance = this;
            else
                Destroy(gameObject);

            attackLine = GetComponent<LineRenderer>();
            attackLine.positionCount = 2;
            attackLine.enabled = false;
        }

        private void Start()
        {
            attackLine.sortingOrder = 10;
        }

        public void StartAttackSelection(CardController card)
        {
            if (isAttacking) return;
            isAttacking = true;
            selectedCard = card;
            
            attackLine.enabled = true;
            attackLine.positionCount = 2;
            
            Vector3 startPos = selectedCard.transform.position;
            
            attackLine.SetPosition(0, startPos);
            attackLine.SetPosition(1, startPos);

        }

        private void Update()
        {
            if (!isAttacking)
                return;

            Vector3 mousePos = Input.mousePosition;
            mousePos.z = Camera.main.nearClipPlane + 10f; 

            Vector3 worldMousePos = Camera.main.ScreenToWorldPoint(mousePos);
            worldMousePos.z = 0; 

            attackLine.SetPosition(1, worldMousePos);

            if (Input.GetMouseButtonDown(0)) 
            {
                TryAttack();
            }
        }


        private void TryAttack()
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                if (hit.collider.transform.parent.CompareTag("Minion"))
                {
                    ApplyAttack(hit.collider.transform.parent.gameObject);
                }
                else if (hit.collider.CompareTag("Enemy"))
                {
                    ApplyAttack(hit.collider.gameObject);
                }
            }

            ResetAttack();
        }

        private void ApplyAttack(GameObject enemy)
        {
            if (selectedCard.cardData.cardEffect.duration > 1)
            {
                GameManager.Instance.AddOngoingEffect(selectedCard.cardData.cardEffect, enemy, selectedCard);
            }
            else
            {
                selectedCard.cardData.cardEffect.ApplyEffect(enemy);
                selectedCard.hasBeenUsed = true;
            }
        }

        public GameObject GetAttackingCard()
        {
            return selectedCard?.gameObject;
        }
        
        private void ResetAttack()
        {
            isAttacking = false;
            attackLine.enabled = false;
            selectedCard = null;
        }
    }
}
