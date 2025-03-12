using System.Collections.Generic;
using _CardGame.Scripts.Cards;
using UnityEngine;
using UnityEngine.Serialization;

namespace _CardGame.Scripts.Managers
{
    public class DeckManager : MonoBehaviour
    {
        public static DeckManager Instance { get; private set;}

        [SerializeField] private int deckSize;
        
        public CardDatabase playerCardDatabase;
        public CardDatabase enemyCardDatabase;
        
        [SerializeField] private List<CardData> playerDeck = new List<CardData>();
        [SerializeField] private List<CardData> enemyDeck = new List<CardData>();
        private void Awake()
        {
            if (Instance == null)
                Instance = this;
            else
                Destroy(gameObject);
        }

        private void Start()
        {
            InitializeDecks();
            ShuffleDecks();
            StartGame();
        }
        
        private void InitializeDecks()
        {
            playerDeck.Clear();
            enemyDeck.Clear();

            playerDeck = GenerateDeck(playerCardDatabase);
            enemyDeck = GenerateDeck(enemyCardDatabase);
        }

        private List<CardData> GenerateDeck(CardDatabase cardDatabase)
        {
                List<CardData> cardPool = new List<CardData>();

                foreach (var card in cardDatabase.allCards)
                {
                    for (int i = 0; i < (int) card.rarity; i++)
                    {
                        cardPool.Add(card);
                    }
                }
                
                List<CardData> deck = new List<CardData>();
                while (deck.Count < deckSize)
                {
                    int randomIndex = Random.Range(0, cardPool.Count);
                    deck.Add(cardPool[randomIndex]);
                }
                
                return deck;
        }
        
        private void ShuffleDecks()
        {
            Shuffle(playerDeck);
            Shuffle(enemyDeck);
        }

        private void StartGame()
        {
            CardManager.Instance.DrawCards(2);
        }

        private void Shuffle(List<CardData> deck)
        {
            for (int i = 0; i < deck.Count; i++)
            {
                int randomIndex = Random.Range(i, deck.Count);
                (deck[i], deck[randomIndex]) = (deck[randomIndex], deck[i]);
            }
        }
        
        public CardData DrawPlayerCard()
        {
            if (playerDeck.Count == 0)
            {
                Debug.Log("player deck is empty! Lost 1 dmg");
                GameManager.Instance.player.TakeDamage(1);
                return null;
            } 
            CardData drawnCard = playerDeck[0];
            playerDeck.RemoveAt(0);
            return drawnCard;
        }

        public CardData DrawEnemyCard()
        {
            if (enemyDeck.Count == 0)
            {
                Debug.Log("enemy deck is empty! Lost 1 dmg");
                GameManager.Instance.enemy.TakeDamage(1);
                return null;
            }
            CardData drawnCard = enemyDeck[0];
            enemyDeck.RemoveAt(0);
            return drawnCard;
        }
    }
}
