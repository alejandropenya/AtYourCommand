using System;
using System.Collections;
using DG.Tweening;
using UnityEngine;

namespace Assets.Scripts
{
    public class BasicEnemyMind : EnemyMind
    {
        protected override void OnEnable()
        {
            DOVirtual.DelayedCall(0.1f, () =>
            {
                base.OnEnable();
                playerController.DefenseEnabled = true;
                playerController.AttackEnabled = false;
                playerController.JumpEnabled = false;
            });
        }

        protected override IEnumerator Decide()
        {
            var enemyAction = enemyCombo[_comboNumber];
            yield return Wait(enemyAction.WaitingTime);
            switch (enemyAction.EnemyNextAction)
            {
                case EnemyPosibleActions.Attack:
                    yield return Attack(enemyDMG, enemyAction.EndingPosition, enemyAction.DurationTime);
                    break;
                case EnemyPosibleActions.Move:
                    yield return Move(enemyAction.EndingPosition, enemyAction.DurationTime);
                    break;
                case EnemyPosibleActions.Shield:
                    break;
                case EnemyPosibleActions.InitialPosition:
                    yield return GoInitialPosition(enemyAction.DurationTime);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            _comboNumber += 1;
            if (_comboNumber >= enemyCombo.Length) _comboNumber = 0;
        }

        protected override IEnumerator Attack(int enemyDamage, EnemyPositionsScriptable endPosition, float duration)
        {
            yield return _enemyBody.EnemyAttack(enemyDamage, playerController, endPosition, duration, () =>
            {
                if (!playerController.IsDefending && !playerController.IsJumping)
                {
                    playerController.TakeDamage(enemyDMG);
                }
                else
                {
                    if (playerController.IsDefending)
                    {
                        playerController.AttackEnabled = true;
                        if (CheckParry())
                        {
                            DoParry();
                            return true;
                        }
                    }
                }
                return false;
            });
        }
    }
}