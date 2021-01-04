using System;
using System.Collections;

namespace Assets.Scripts.FastNinjaEnemy
{
    public class FastEnemyMind : EnemyMind
    {
         public override void Init(PlayerController newPlayerController)
        {
            base.Init(newPlayerController);
            playerController.DefenseEnabled = true;
            playerController.AttackEnabled = false;
            playerController.JumpEnabled = false;
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
                case EnemyPosibleActions.Combo:
                    yield return Combo(currentPosition);
                    break;
                case EnemyPosibleActions.Shield:
                    break;
                case EnemyPosibleActions.InitialPosition:
                    yield return GoInitialPosition(enemyAction.DurationTime);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            ComboNumber += 1;
            if (ComboNumber >= enemyCombo.Length) ComboNumber = 0;
        }


        protected override IEnumerator Attack(int enemyDmg, EnemyPositionsScriptable endPosition, float duration)
        {
            yield return EnemyBody.EnemyAttack(enemyDmg, playerController, endPosition, duration, () =>
            {
                if (!playerController.IsDefending && !playerController.IsJumping)
                {
                    playerController.TakeDamage(enemyDamage);
                }
                else
                {
                    if (playerController.IsDefending)
                    {
                        playerController.DefenseEnabled = true;
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
            currentPosition = endPosition;
        }
        
        //TODO: Create a global variable for attacks duration
        private IEnumerator Combo(EnemyPositionsScriptable startingPosition)
        {
            yield return Attack(enemyDamage, GetOppositePosition(currentPosition),0.6f);
            yield return Move(posiblePositions.Find(scriptable => scriptable.name == "Down"),0.6f);
            yield return Attack(enemyDamage, GetOppositePosition(currentPosition),0.6f);
            yield return Move(GetOppositePosition(startingPosition), 0.6f);
            yield return Attack(enemyDamage, GetOppositePosition(currentPosition),0.6f);
        }
    }
}