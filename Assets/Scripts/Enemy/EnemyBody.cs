using System;
using System.Collections;
using DG.Tweening;
using UnityEngine;

namespace Assets.Scripts
{
    public abstract class EnemyBody : MonoBehaviour
    {
        [SerializeField] protected SpriteRenderer shieldRenderer;

        public void Init()
        {
            if (shieldRenderer != null) shieldRenderer.enabled = false;
        }

        public Tween EnemyMove(EnemyPositionsScriptable endPosition, float duration)
        {
            return transform.DOMove(endPosition.Position, duration).SetEase(Ease.Linear);
        }

        public virtual IEnumerator EnemyAttack(int enemyDMG, PlayerController playerController, EnemyPositionsScriptable endPosition, float duration, Func<bool> onHit)
        {
            var tween = EnemyMove(endPosition, duration);
            yield return DOVirtual.DelayedCall(duration / 2, () => { });
            var cancelled = onHit.Invoke();
            if (cancelled)
            {
                tween.Kill();
                yield break;
            }
            yield return tween;
        }

        public virtual void EnableShield()
        {
            shieldRenderer.enabled = true;
        }

        public virtual void HideShield()
        {
            shieldRenderer.enabled = false;
        }
    }
}