using _CardGame.Scripts.Gameplay;
using _CardGame.Scripts.Managers;
using _CardGame.UI.Scripts;
using DG.Tweening;
using Unity.Mathematics;
using UnityEngine;

namespace _CardGame.Scripts.Cards
{
    [CreateAssetMenu(fileName = "NewDamageEffect", menuName = "Card Effects/Damage")]
    public class DamageEffect : CardEffect
    {
        public int damageAmount;

        public override void ApplyEffect(GameObject target)
        {
            GameObject attacker = AttackManager.Instance.GetAttackingCard();

            if (attacker == null)
            {
                DirectDamage(target);
                return;
            }

            AttackAnimation(attacker, target, () => DirectDamage(target));
        }
        
        
        /// <summary>
        /// Apply damage to target.
        /// Target can be either enemy card or directly the enemy
        /// </summary>
        /// <param name="target"></param>
        private void DirectDamage(GameObject target)
        {
            if (target.TryGetComponent(out CardController cardController))
                cardController.TakeDamage(damageAmount, target);
            else if (target.TryGetComponent(out CharacterHealth health))
                health.TakeDamage(damageAmount);
        }
        
        /// <summary>
        /// Using for effects that applies damage per turn
        /// for instance deal 2 damage for 3 rounds
        /// </summary>
        /// <param name="target"></param>
        public override void ApplyEffectOverTime(GameObject target)
        {
            CharacterHealth health = target.GetComponent<CharacterHealth>();
            
            if (health != null)
                health.TakeDamage(damageAmount);
        }
        
        private void AttackAnimation(GameObject attacker, GameObject target, System.Action onHit)
        {
            GameObject projectile = Instantiate(AttackManager.Instance.projectilePrefab, attacker.transform.position
                , Quaternion.identity, AttackManager.Instance.canvas.transform);
            
            projectile.transform.DOMove(target.transform.position, 0.5f).SetEase(Ease.InQuad)
                .OnComplete(() =>
                {
                    onHit.Invoke(); 
                    Destroy(projectile);
                    PlayHitEffect(target);
                });
        }
        
        private void PlayHitEffect(GameObject target)
        {
            SpriteRenderer sprite = target.GetComponent<SpriteRenderer>();
            if (sprite != null)
            {
                sprite.DOColor(Color.red, 0.2f).OnComplete(() => sprite.DOColor(Color.white, 0.2f));
            }

            target.transform.DOShakePosition(0.3f, strength: new Vector3(0.2f, 0, 0));

            GameObject textObj = Instantiate(AttackManager.Instance.floatingTextPrefab, target.transform.position,
                quaternion.identity, target.transform);
            
            textObj.GetComponent<FloatingText>().Initialize(damageAmount, false);
        }
    }
}
