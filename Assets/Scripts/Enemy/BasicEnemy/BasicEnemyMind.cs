using System.Collections;
using UnityEngine;

namespace Assets.Scripts
{
    public class BasicEnemyMind : EnemyMind
    {
        [SerializeField] private float attackDuration;
        
        protected override IEnumerator Decide()
        {
            var attackDelay = timePattern[_movementNumber];
            yield return Wait(attackDelay);
            yield return Attack(enemyDMG, positionsPattern[_movementNumber].Position, attackDuration);
            _movementNumber += 1;
        }
    }
}