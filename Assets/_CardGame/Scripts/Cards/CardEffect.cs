using UnityEngine;

namespace _CardGame.Scripts.Cards
{
    public abstract class CardEffect : ScriptableObject
    {
        public int duration = 1;
        public abstract void ApplyEffect(GameObject target);
        public virtual void ApplyEffectOverTime(GameObject target){}
    }
}
