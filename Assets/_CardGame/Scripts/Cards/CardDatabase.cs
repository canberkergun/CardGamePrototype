using UnityEngine;

namespace _CardGame.Scripts.Cards
{
    [CreateAssetMenu(fileName = "CardDatabase", menuName = "Database/Card Database")]
    public class CardDatabase : ScriptableObject
    {
        public CardData[] allCards;
    }
}
