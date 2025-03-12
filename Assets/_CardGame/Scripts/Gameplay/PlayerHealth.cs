using _CardGame.Scripts.Managers;

namespace _CardGame.Scripts.Gameplay
{
    public class PlayerHealth : CharacterHealth
    {
        protected override void OnDeath()
        {
            GameManager.Instance.GameOver(false);
        }
    
    
    }
}
