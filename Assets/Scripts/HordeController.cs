using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;

namespace Assets.Scripts
{
    public class HordeController : MonoBehaviour
    {
        [SerializeField] private List<GameObject> enemyEditorOrder;
        [SerializeField] private PlayerController playerController;

        private List<GameObject> _enemyOrder;
        private GameObject _currentEnemy;

        private void Start()
        {
            _enemyOrder = new List<GameObject>();
            foreach (var objectInList in enemyEditorOrder)
            {
                var currentObject = objectInList.gameObject;
                if (currentObject.GetComponent<EnemyMind>() != null) _enemyOrder.Add(currentObject);
            }
            _currentEnemy = _enemyOrder.First();
            InstantiateNextEnemy();
        }

        private void InstantiateNextEnemy()
        {
            if (_enemyOrder.Count == 0) return;
            _currentEnemy = Instantiate(_enemyOrder.First());
            var currentEnemyMind = _currentEnemy.GetComponent<EnemyMind>();
            currentEnemyMind.onEnemyDies += RemoveEnemy;
            currentEnemyMind.Init(playerController);
        }

        private void RemoveEnemy()
        {
            _currentEnemy.GetComponent<EnemyMind>().onEnemyDies -= RemoveEnemy;
            _currentEnemy = null;
            _enemyOrder.RemoveAt(0);
            DOVirtual.DelayedCall(4, InstantiateNextEnemy);
        }
    }
}