using UnityEngine;

namespace _CardGame.Scripts.Cards
{
    [CreateAssetMenu(fileName = "NewCard", menuName = "Card")]
    public class CardData : ScriptableObject
    {
        public int cost;
        public int health;
        public string description;
        public CardEffect cardEffect;
        public Rarity rarity;
    }
    public enum Rarity {Common = 3, Rare = 2, Epic = 1}
}