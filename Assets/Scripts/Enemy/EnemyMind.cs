using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DG.Tweening;
using UnityEngine;

namespace Assets.Scripts
{
    public abstract class EnemyMind : MonoBehaviour
    {
        [SerializeField] protected PlayerController playerController;
        [SerializeField] private int maxHealth;
        [SerializeField] protected int enemyDMG;
        [SerializeField] protected EnemyAction[] enemyCombo;
        [SerializeField] protected EnemyPositionsScriptable initialPosition;

        protected EnemyBody _enemyBody;
        private bool _enemyDead;
        private int _currentHealth;
        private List<IEnumerator> _stackState;
        protected int _comboNumber;

        protected virtual void OnEnable()
        {
            playerController.onPlayerAttacks += OnDamaged;
            _currentHealth = maxHealth;
            transform.position = initialPosition.Position;
            _enemyBody = GetComponent<EnemyBody>();
            _enemyDead = false;

            //Pattern preparation
            if (enemyCombo == null) enemyCombo = new EnemyAction[6];
            _comboNumber = 0;
        }

        private void OnDisable()
        {
            playerController.onPlayerAttacks -= OnDamaged;
        }

        private void Update()
        {
            if (_stackState == null) _stackState = new List<IEnumerator>();
            if (!_stackState.Any()) _stackState.Add(Decide());
            var currentState = _stackState.Last();
            var endedAction = !currentState.MoveNext();
            var currentYieldedObject = currentState.Current;
            if (currentYieldedObject is IEnumerator enumerator) _stackState.Add(enumerator);
            if (currentYieldedObject is Tween tween) _stackState.Add(WaitTween(tween));
            if (endedAction) _stackState.Remove(currentState);
        }

        private IEnumerator WaitTween(Tween tween)
        {
            var completed = false;
            while (!completed)
            {
                if (!tween.IsActive()) completed = true;
                yield return null;
            }
        }

        protected abstract IEnumerator Decide();

        protected void PushState(IEnumerator enumerator)
        {
            _stackState.Add(enumerator);
        }

        private void OnDamaged(int playerDmg)
        {
            _currentHealth -= playerDmg;
            if (_currentHealth > 0) return;
            _enemyDead = true;
            ClearStackState();
            PushState(Die());
        }

        protected virtual IEnumerator Die()
        {
            Destroy(gameObject);
            yield break;
        }

        protected virtual IEnumerator GoInitialPosition(float duration)
        {
            yield return _enemyBody.EnemyMove(initialPosition, duration);
        }

        protected virtual IEnumerator Attack(int enemyDamage, EnemyPositionsScriptable endPosition, float duration)
        {
            yield return _enemyBody.EnemyAttack(enemyDamage, playerController, endPosition, duration, () =>
            {
                if (!playerController.IsDefending && !playerController.IsJumping)
                {
                    playerController.TakeDamage(enemyDMG);
                }

                return false;
            });
        }

        protected virtual IEnumerator Move(EnemyPositionsScriptable endPosition, float duration)
        {
            yield return _enemyBody.EnemyMove(endPosition, duration);
        }

        protected IEnumerator Wait(float waitingTime)
        {
            yield return DOVirtual.DelayedCall(waitingTime, () => { });
        }

        protected void ClearStackState()
        {
            _stackState.Clear();
        }

        protected virtual bool CheckParry()
        {
            return playerController.TimeAfterShield < playerController.ParryTimeWindow;
        }

        protected virtual void DoParry()
        {
            ClearStackState();
            PushState(GoInitialPosition(1));
        }
    }
}