using System;
using DG.Tweening;
using UnityEngine;

namespace Assets.Scripts
{
    public class Enemy : MonoBehaviour
    {
        [SerializeField] private int maxHealth;
        [SerializeField] private PlayerController playerController;
        [SerializeField] private int enemyDMG;
        private int _currentHealth;

        private void Start()
        {
            _currentHealth = maxHealth;
        }

        private void OnEnable()
        {
            playerController.onPlayerAttack += OnDamaged;
        }

        private void OnDisable()
        {
            playerController.onPlayerAttack -= OnDamaged;
        }

        private void OnDamaged(int playerDmg)
        {
            _currentHealth -= playerDmg;
            if (_currentHealth <= 0) Destroy(gameObject);
        }

        private void EnemyAttack(Vector3 endPosition, float duration, int enemyDmg)
        {
            transform.DOMove(endPosition, duration);
            DOVirtual.DelayedCall(duration/2,() =>
            {
                if (!playerController.IsDefending && !playerController.IsJumping)
                {
                    playerController.TakeDamage(enemyDmg);
                }
            });
        }
    }
}