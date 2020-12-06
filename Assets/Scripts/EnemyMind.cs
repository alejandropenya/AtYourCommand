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
        [SerializeField] private PlayerController playerController;
        [SerializeField] private int maxHealth;
        [SerializeField] private int enemyDMG;
        [SerializeField] private int[] timePattern;
        [SerializeField] private EnemyPositionsScriptable[] positionsPattern;

        private IEnumerator _enemyCoroutine;
        private EnemyBody _enemyBody;
        private Vector3 _initialPosition;
        private bool _enemyDead;
        private int _currentHealth;
        private List<IEnumerator> _stackState;
        private int _movementNumber;

        private void OnEnable()
        {
            playerController.onPlayerAttack += OnDamaged;
            _enemyBody = GetComponent<EnemyBody>();
            _initialPosition = transform.position;
            _enemyDead = false;
        }

        private void OnDisable()
        {
            playerController.onPlayerAttack -= OnDamaged;
        }

        private void Update()
        {
            if (_stackState == null) _stackState = new List<IEnumerator>();
            if (_stackState.Any()) _stackState.Add(Decide());
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
            if (_currentHealth <= 0)
            {
                _enemyDead = true;
                _stackState.Clear();
                PushState(Die());
            }
        }

        protected virtual IEnumerator Die()
        {
            Destroy(gameObject);
            yield break;
        }

        protected virtual IEnumerator GoInitialPosition(float duration)
        {
            yield return _enemyBody.EnemyMove(_initialPosition, duration);
        }

        protected virtual IEnumerator Attack(int enemyDamage, Vector3 endPosition, float duration)
        {
            yield return _enemyBody.EnemyAttack(enemyDamage, playerController, endPosition, duration);
        }
    }
}