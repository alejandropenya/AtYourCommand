using System;
using System.Collections;
using UnityEngine;

namespace Assets.Scripts
{
    public class EnemyCoroutineController : MonoBehaviour
    {
        [SerializeField] private Enemy currentEnemy;
        [SerializeField] private PlayerController playerController;
        [SerializeField] private WaveController waveController;

        private IEnumerator _enemyCoroutine;
        private bool _enemyDead;


        private void OnEnable()
        {
            waveController.onEnemySpawned += SetEnemy;
        }

        private void OnDisable()
        {
            waveController.onEnemySpawned -= SetEnemy;
        }

        private void StartBehaviour()
        {
            if (_enemyCoroutine != null) StopCoroutine(_enemyCoroutine);
            _enemyCoroutine = TestBehaviour();
            StartCoroutine(_enemyCoroutine);
        }

        private void StopBehaviour()
        {
            _enemyCoroutine = null;
        }

        private IEnumerator TestBehaviour()
        {
            
            yield return null;
        }

        private void SetEnemy(Enemy newEnemy)
        {
            //What if there is already an enemy setted
            currentEnemy = newEnemy;
        }
    }
}