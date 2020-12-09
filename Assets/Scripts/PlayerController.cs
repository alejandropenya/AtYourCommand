using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Assets.Scripts
{
    public class PlayerController : MonoBehaviour
    {
        [SerializeField] private float jumpForce;
        [SerializeField] private int numberOfJumps = 1;
        [SerializeField] private float jumpDuration;
        [SerializeField] private float jumpCoolDown;
        [SerializeField] private AnimationCurve jumpEase;
        [SerializeField] private SpriteRenderer shieldRenderer;
        [SerializeField] private float shieldDuration;
        [SerializeField] private float defenseCooldown;
        [SerializeField] private int playerDMG;
        [SerializeField] private FluctuableValue healthValue;

        public event Action<int> onPlayerAttacks;
        public event Action onPlayerParries;

        private Vector3 _initialPosition;
        private CommandControls _commandControls;

        private bool _isAttacking;
        private bool _attackEnabled;
        public bool CanAttack => !_isAttacking && _attackEnabled;


        public bool IsAttacking
        {
            set => _isAttacking = value;
        }

        public bool AttackEnabled
        {
            set => _attackEnabled = value;
        }

        private bool _isDefending;
        private bool _defenseEnabled;
        public bool CanDefend => !_isDefending && _defenseEnabled;


        public bool IsDefending
        {
            get => _isDefending;
            set => _isDefending = value;
        }

        public bool DefenseEnabled
        {
            set => _defenseEnabled = value;
        }

        private bool _isJumping;
        private bool _jumpEnabled;

        public bool CanJump => !_isJumping && _jumpEnabled;

        public bool IsJumping
        {
            get => _isJumping;
            set => _isJumping = value;
        }

        public bool JumpEnabled
        {
            set => _jumpEnabled = value;
        }

        private void Start()
        {
            _commandControls = new CommandControls();
            _commandControls.Gameplay.Enable();
            _commandControls.Gameplay.Attack.started += PlayerAttack;
            _commandControls.Gameplay.Defend.started += PlayerDefend;
            _commandControls.Gameplay.Jump.started += PlayerJump;
            _initialPosition = transform.position;
            healthValue.ResetValue();
            _isAttacking = false;
            _isDefending = false;
            _isJumping = false;
            _attackEnabled = false;
            _defenseEnabled = false;
            _jumpEnabled = false;
            shieldRenderer.enabled = false;
        }

        private void PlayerAttack(InputAction.CallbackContext ctx)
        {
            if (!CanAttack) return;
            OnPlayerAttack(playerDMG);
            _attackEnabled = false;
        }

        private void PlayerDefend(InputAction.CallbackContext obj)
        {
            if (!CanDefend) return;
            shieldRenderer.enabled = true;
            _defenseEnabled = false;
            _isDefending = true;
            DOVirtual.DelayedCall(shieldDuration, () =>
            {
                shieldRenderer.enabled = false;
                _isDefending = false;
                DOVirtual.DelayedCall(defenseCooldown, () => _defenseEnabled = true);
            });
        }

        private void PlayerJump(InputAction.CallbackContext obj)
        {
            if (!CanJump) return;
            _jumpEnabled = false;
            transform.DOJump(_initialPosition, jumpForce, numberOfJumps, jumpDuration).OnStart(() => { _isJumping = true; })
                .OnComplete(
                    () =>
                    {
                        _isJumping = false;
                        DOVirtual.DelayedCall(jumpCoolDown, () => _jumpEnabled = true);
                    }).SetEase(jumpEase);
        }

        private void OnPlayerAttack(int playerDmg)
        {
            onPlayerAttacks?.Invoke(playerDmg);
        }

        public void TakeDamage(int enemyDmg)
        {
            healthValue.CurrentValue -= enemyDmg;
        }
    }
}