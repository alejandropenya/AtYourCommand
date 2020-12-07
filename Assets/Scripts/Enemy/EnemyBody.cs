using System.Collections;
using DG.Tweening;
using UnityEngine;

namespace Assets.Scripts
{
    public abstract class EnemyBody : MonoBehaviour
    {
        public Tween EnemyMove(Vector3 endPosition, float duration)
        {
            return transform.DOMove(endPosition, duration);
        }

        public IEnumerator EnemyAttack(int enemyDMG, PlayerController playerController, Vector3 endPosition, float duration)
        {
            var tween = EnemyMove(endPosition, duration);
            yield return DOVirtual.DelayedCall(duration/2,() => {});
            if (!playerController.IsDefending && !playerController.IsJumping)
            {
                playerController.TakeDamage(enemyDMG);
            }
            yield return tween;
        }
    }
}