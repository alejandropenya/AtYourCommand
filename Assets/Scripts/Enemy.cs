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
        [SerializeField] private int[] timePattern;
        [SerializeField] private EnemyPositionsScriptable[] positionsPattern;
        
        private int _movementNumber;
        private int _currentHealth;
        private bool _isDead;

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
            if (_isDead) return;
            _currentHealth -= playerDmg;
            if (_currentHealth <= 0) _isDead = true;
        }

        private void EnemyAttack(Vector3 endPosition, float duration)
        {
            transform.DOMove(endPosition, duration);
            DOVirtual.DelayedCall(duration/2,() =>
            {
                if (!playerController.IsDefending && !playerController.IsJumping)
                {
                    playerController.TakeDamage(enemyDMG);
                }
            });
        }
    }
}