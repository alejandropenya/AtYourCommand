using System;
using System.Collections;
using DG.Tweening;

namespace Assets.Scripts.TankyAssaulterEnemy
{
    public class TankyEnemyMind : EnemyMind
    {
        public override void Init(PlayerController newPlayerController)
        {
            base.Init(newPlayerController);
            playerController.DefenseEnabled = true;
            playerController.AttackEnabled = false;
            playerController.JumpEnabled = true;
        }

        protected override IEnumerator Decide()
        {
            var enemyAction = enemyCombo[ComboNumber];
            yield return Wait(enemyAction.WaitingTime);
            switch (enemyAction.EnemyNextAction)
            {
                case EnemyPosibleActions.Attack:
                    yield return Attack(enemyDamage, enemyAction.EndingPosition, enemyAction.DurationTime);
                    break;
                case EnemyPosibleActions.Move:
                    yield return Move(enemyAction.EndingPosition, enemyAction.DurationTime);
                    break;
                case EnemyPosibleActions.InitialPosition:
                    yield return GoActionInitialPosition(enemyAction.DurationTime);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            ComboNumber += 1;
            if (ComboNumber >= enemyCombo.Length) ComboNumber = 0;
        }

        protected override IEnumerator Attack(int enemyDmg, EnemyPositionsScriptable endPosition, float duration)
        {
            playerController.JumpEnabled = true;
            yield return EnemyBody.EnemyAttack(enemyDmg, playerController, endPosition, duration, () =>
            {
                if (!playerController.IsDefending && !playerController.IsJumping)
                {
                    playerController.TakeDamage(base.enemyDamage);
                }
                else
                {
                    if (playerController.IsDefending)
                    {
                        playerController.DefenseEnabled = true;
                        if (CheckParry())
                        {
                            DoParry();
                            return true;
                        }
                    }

                    if (playerController.IsJumping)
                    {
                        playerController.AttackEnabled = true;
                        PushState(Shield());
                        return false;
                    }
                }

                return false;
            });
        }

        protected IEnumerator Shield()
        {
            EnemyBody.EnableShield();
            IsDefending = true;
            yield return DOVirtual.DelayedCall(shieldDuration, () => { });
            EnemyBody.HideShield();
            IsDefending = false;
        }
    }
}