using DG.Tweening;
using UnityEngine;
using Image = UnityEngine.UI.Image;

namespace Assets.Scripts
{
    public class UISkillsController : MonoBehaviour
    {
        [SerializeField] private PlayerController playerController;
        [SerializeField] private Image attackImage;
        [SerializeField] private Image defenseImage;
        [SerializeField] private Image jumpImage;
        [SerializeField] [Range(0, 0.9f)] private float minFadeValue;
        [SerializeField] private float fadeDuration;

        private bool _attackButtonEnabled;
        private bool _defenseButtonEnabled;
        private bool _jumpButtonEnabled;

        private void Start()
        {
            _attackButtonEnabled = false;
            attackImage.DOFade(minFadeValue, 0);
            _defenseButtonEnabled = false;
            defenseImage.DOFade(minFadeValue, 0);
            _jumpButtonEnabled = false;
            jumpImage.DOFade(minFadeValue, 0);
        }

        private void Update()
        {
            UpdateAttackButton();
            UpdateDefenseButton();
            UpdateJumpButton();
        }

        private void UpdateAttackButton()
        {
            if (_attackButtonEnabled && !playerController.CanAttack)
            {
                attackImage.DOFade(minFadeValue, fadeDuration);
                _attackButtonEnabled = false;
                return;
            }

            if (!_attackButtonEnabled && playerController.CanAttack)
            {
                attackImage.DOFade(1, fadeDuration);
                _attackButtonEnabled = true;
            }
        }

        private void UpdateDefenseButton()
        {
            if (_defenseButtonEnabled && !playerController.CanDefend)
            {
                defenseImage.DOFade(minFadeValue, fadeDuration);
                _defenseButtonEnabled = false;
                return;
            }

            if (!_defenseButtonEnabled && playerController.CanDefend)
            {
                defenseImage.DOFade(1, fadeDuration);
                _defenseButtonEnabled = true;
            }
        }

        private void UpdateJumpButton()
        {
            if (_jumpButtonEnabled && !playerController.CanJump)
            {
                jumpImage.DOFade(minFadeValue, fadeDuration);
                _jumpButtonEnabled = false;
                return;
            }

            if (!_jumpButtonEnabled && playerController.CanJump)
            {
                jumpImage.DOFade(1, fadeDuration);
                _jumpButtonEnabled = true;
            }
        }
    }
}