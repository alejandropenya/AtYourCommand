using System;
using UnityEngine;

namespace Assets.Scripts
{
    [Serializable]
    public class EnemyAction
    {
        [SerializeField] private float waitingTime;
        [SerializeField] private float durationTime;
        [SerializeField] private EnemyPositionsScriptable endingPosition;
        [SerializeField] private EnemyPosibleActions enemyNextAction;

        public float WaitingTime => waitingTime;

        public float DurationTime => durationTime;

        public EnemyPositionsScriptable EndingPosition => endingPosition;

        public EnemyPosibleActions EnemyNextAction => enemyNextAction;
    }
    
    public enum EnemyPosibleActions
    {
        Attack,
        Move,
        Shield,
        InitialPosition,
    }
}