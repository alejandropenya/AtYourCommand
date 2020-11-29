using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts
{
    public class WaveController
    {
        [SerializeField] private List<Enemy> enemyWaveOrder;

        public event Action<Enemy> onEnemySpawned;

        private void OnEnemySpawned(Enemy currentEnemy)
        {
            onEnemySpawned?.Invoke(currentEnemy);
            
        }
    }
}