using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;

namespace Assets.Scripts
{
    public abstract class EnemyMind : MonoBehaviour
    {
        [SerializeField] protected PlayerController playerController;
        [SerializeField] private int maxHealth;
        [SerializeField] protected int enemyDamage;
        [SerializeField] private float playerParryTimeWindow;
        [SerializeField] protected float shieldDuration;
        [SerializeField] protected EnemyAction[] enemyCombo;
        [SerializeField] protected EnemyPositionsScriptable initialPosition;
        [SerializeField] protected List<EnemyPositionsScriptable> posiblePositions;

        private bool _enemyDead;
        private int _currentHealth;
        private List<IEnumerator> _stackState;
        protected EnemyBody EnemyBody;
        protected int ComboNumber;
        protected bool IsDefending;
        protected EnemyPositionsScriptable lastPosition;

        public event Action onEnemyDies;

        private void OnDisable()
        {
            playerController.onPlayerAttacks -= OnDamaged;
        }

        private void Update()
        {
            if (_stackState == null) _stackState = new List<IEnumerator>();
            if (!_stackState.Any()) _stackState.Add(Decide());
            var currentState = _stackState.Last();
            var stateCount = _stackState.Count;
            var endedAction = !currentState.MoveNext();
            if (stateCount < _stackState.Count)
            {
                var currentYieldedObject = currentState.Current;
                if (currentYieldedObject is IEnumerator enumerator) _stackState.Insert(stateCount, enumerator);
                if (currentYieldedObject is Tween tween) _stackState.Insert(stateCount, WaitTween(tween));
            }
            else
            {
                var currentYieldedObject = currentState.Current;
                if (currentYieldedObject is IEnumerator enumerator) _stackState.Add(enumerator);
                if (currentYieldedObject is Tween tween) _stackState.Add(WaitTween(tween));
            }

            if (endedAction) _stackState.Remove(currentState);
        }

        public virtual void Init(PlayerController newPlayerController)
        {
            _currentHealth = maxHealth;
            transform.position = initialPosition.Position;
            lastPosition = initialPosition;
            playerController = newPlayerController;
            EnemyBody = GetComponent<EnemyBody>();
            _enemyDead = false;
            playerController.onPlayerAttacks += OnDamaged;
            IsDefending = false;
            EnemyBody.Init();

            //Pattern preparation
            if (enemyCombo == null) enemyCombo = new EnemyAction[6];
            ComboNumber = 0;
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
            if (IsDefending) return;
            _currentHealth -= playerDmg;
            if (_currentHealth > 0) return;
            _enemyDead = true;
            ClearStackState();
            PushState(Die());
        }

        protected virtual IEnumerator Die()
        {
            OnEnemyDies();
            Destroy(gameObject);
            yield break;
        }

        protected virtual IEnumerator GoActionInitialPosition(float duration)
        {
            yield return Move(lastPosition, duration);
        }

        protected virtual IEnumerator Attack(int enemyDmg, EnemyPositionsScriptable endPosition, float duration)
        {
            yield return EnemyBody.EnemyAttack(enemyDmg, playerController, endPosition, duration, () =>
            {
                if (!playerController.IsDefending && !playerController.IsJumping)
                {
                    playerController.TakeDamage(this.enemyDamage);
                }

                return false;
            });
            lastPosition = endPosition;
        }

        protected virtual IEnumerator Move(EnemyPositionsScriptable endPosition, float duration)
        {
            yield return EnemyBody.EnemyMove(endPosition, duration);
            lastPosition = endPosition;
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
            return playerController.TimeAfterShield < playerParryTimeWindow;
        }

        protected virtual void DoParry()
        {
            playerController.AttackEnabled = true;
            ClearStackState();
            ComboNumber = 0;
            PushState(GoActionInitialPosition(1));
        }

        protected virtual void OnEnemyDies()
        {
            onEnemyDies?.Invoke();
        }

        protected EnemyPositionsScriptable GetOppositePosition(EnemyPositionsScriptable actualPosition)
        {
            var position = actualPosition;
            var maxDistance = 0f;
            foreach (var pos in posiblePositions)
            {
                var distance = Vector3.Distance(actualPosition.Position, pos.Position);
                if (distance > maxDistance)
                {
                    maxDistance = distance;
                    position = pos;
                }
            }

            return position;
        }
    }
}