using _CardGame.Scripts.Managers;

namespace _CardGame.Scripts.Gameplay
{
    public class EnemyHealth : CharacterHealth
    {
        protected override void OnDeath()
        {
            GameManager.Instance.GameOver(true);
        }
    }
}
