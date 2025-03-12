using _CardGame.Scripts.Cards;
using _CardGame.Scripts.Gameplay;
using _CardGame.Scripts.Managers;
using UnityEngine;

[CreateAssetMenu(fileName = "NewHealEffect", menuName = "Card Effects/Heal")]
public class HealEffect : CardEffect
{
    public int healAmount;
    public override void ApplyEffect(GameObject target)
    {
        GameObject correctTarget;

        if (GameManager.Instance.currentState == GameManager.GameState.PlayerTurn)
        {
            correctTarget = GameManager.Instance.player.gameObject; 
        }
        else
        {
            correctTarget = GameManager.Instance.enemy.gameObject; 
        }

        CharacterHealth health = correctTarget.GetComponent<CharacterHealth>();
        if (health != null)
        {
            health.Heal(healAmount);
        }
    }

    public override void ApplyEffectOverTime(GameObject target)
    {
        CharacterHealth health = target.GetComponent<CharacterHealth>();
        if (health != null)
        {
            health.Heal(healAmount);
        }
    }
}
