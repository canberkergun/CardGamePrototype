using _CardGame.UI.Scripts;
using DG.Tweening;
using TMPro;
using UnityEngine;

namespace _CardGame.Scripts.Gameplay
{
    public abstract class CharacterHealth : MonoBehaviour
    {
        public int maxHealth = 20;
        private int currentHealth;

        public int maxMana = 2;
        public int currentMana;

        [SerializeField] private GameObject floatingTextPrefab;
        
        public TextMeshProUGUI healthText;
        public TextMeshProUGUI manaText;
        
        protected virtual void Start()
        {
            currentHealth = maxHealth;
            currentMana = maxMana;
            UpdateUI();
        }

        public void TakeDamage(int damage)
        {
            currentHealth -= damage;
            if (currentHealth < 0)
                currentHealth = 0;
        
            UpdateUI();

            PlayHitAnimation(damage, false);

            if (currentHealth == 0)
                OnDeath();
        }

        public void RestoreMana()
        {
            maxMana++;
            currentMana = maxMana;
            UpdateUI();
        }

        public bool SpendMana(int cost)
        {
            if (currentMana < cost)
                return false;
            
            currentMana -= cost;
            UpdateUI();
            return true;
        }

        public void Heal(int amount)
        {
            currentHealth += amount;
            if (currentHealth > maxHealth)
                currentHealth = maxHealth;

            UpdateUI();
            
            PlayHitAnimation(amount, true);
        }

        private void PlayHitAnimation(int amount, bool isHealing)
        {
            SpriteRenderer sprite = GetComponent<SpriteRenderer>();
            if (sprite != null)
            {
                sprite.DOColor(Color.red, 0.2f).OnComplete(() => sprite.DOColor(Color.white, 0.2f));
            }

            if (isHealing)
            {
                transform.DOShakePosition(0.3f, strength: new Vector3(0, 0.2f, 0));
            }
            else
            {
                transform.DOShakePosition(0.3f, strength: new Vector3(0.2f, 0, 0));
            }

            GameObject textObject = Instantiate(floatingTextPrefab, transform.position, Quaternion.identity, transform);
            textObject.GetComponent<FloatingText>().Initialize(amount, isHealing);
        }

        private void UpdateUI()
        {
            healthText.text = $"{currentHealth}";
            manaText.text = $"{currentMana}/{maxMana}";
        }
        protected abstract void OnDeath();
    }
}
