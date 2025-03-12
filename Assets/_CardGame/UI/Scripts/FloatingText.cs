using System;
using DG.Tweening;
using TMPro;
using UnityEngine;

namespace _CardGame.UI.Scripts
{
    public class FloatingText : MonoBehaviour
    {
        private TextMeshProUGUI text;

        private void Awake()
        {
            text = GetComponent<TextMeshProUGUI>();
        }

        public void Initialize(int amount, bool isHealing)
        {
            text.text = isHealing ? $"+{amount}" : $"-{amount}";
            text.color = isHealing ? Color.green : Color.red;

            transform.DOScale(1.2f, 0.1f).OnComplete(() => transform.DOScale(1f, 0.1f));
            transform.DOMoveY(transform.position.y + 1f, 0.5f).SetEase(Ease.OutQuad)
                .OnComplete(() => Destroy(gameObject));
        }
    }
}
