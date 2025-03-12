using System;
using System.Collections.Generic;
using _CardGame.Scripts.Cards;
using _CardGame.Scripts.Gameplay;
using TMPro;
using UnityEditor.Build.Content;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using Random = UnityEngine.Random;

namespace _CardGame.Scripts.Managers
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }
        public event Action OnTurnEnd;
        public event Action DrawCards;
        public event Action<bool> OnGameOver;
        public enum GameState {PlayerTurn, EnemyTurn, GameOver}
        public GameState currentState;

        public PlayerHealth player;
        public EnemyHealth enemy;
        
        public EnemyController enemyController;
        
        private List<(CardEffect, GameObject, CardController, int, bool)> activeEffects = new List<(CardEffect, GameObject, CardController, int, bool)>();

        private void Awake()
        {
            if (Instance == null)
                Instance = this;
            else
                Destroy(gameObject);
        }
        
        private void Start()
        {
            StartGame();
        }

        private void StartGame()
        {
            currentState = GameState.PlayerTurn;
        }

        public void EndTurn()
        {
            currentState = (currentState == GameState.PlayerTurn) ? GameState.EnemyTurn : GameState.PlayerTurn;

            OnTurnEnd?.Invoke();
            
            ApplyOngoingEffects();
            
            if (currentState == GameState.PlayerTurn)
            {
                player.RestoreMana();
                DrawCards?.Invoke();
            }
            else if (currentState == GameState.EnemyTurn)
            {
                enemy.RestoreMana();
                Invoke(nameof(EnemyTurn), 1.5f);
            }
        }

        public void AddOngoingEffect(CardEffect effect, GameObject target, CardController cardController)
        {
            bool isPlayerEffect = currentState == GameState.PlayerTurn;
            activeEffects.Add((effect, target, cardController, effect.duration, isPlayerEffect));
            Debug.Log($"added {effect.name} for {effect.duration} turns");
        }

        private void ApplyOngoingEffects()
        {
            for (int i = 0; i < activeEffects.Count; i++)
            {
                (CardEffect effect, GameObject target, CardController card, int remainingTurns, bool isPlayerEffect) = activeEffects[i];

                if ((isPlayerEffect && currentState == GameState.PlayerTurn) || (!isPlayerEffect && currentState == GameState.EnemyTurn))
                {
                    effect.ApplyEffectOverTime(target);
                    remainingTurns--;

                    if (remainingTurns <= 0)
                    {
                        activeEffects.RemoveAt(i);
                        CardManager.Instance.RemoveCardFromHand(card.gameObject, card.transform.parent == CardManager.Instance.playerArea);
                    }
                    else
                    {
                        activeEffects[i] = (effect, target, card, remainingTurns, isPlayerEffect);
                    }
                }
            }
        }

        public void GameOver(bool isPlayerWon)
        {
            OnGameOver?.Invoke(isPlayerWon);
        }

        private void EnemyTurn()
        {
            if (enemyController != null)
            {
                enemyController.TakeTurn();
            }
        }
    }
}
