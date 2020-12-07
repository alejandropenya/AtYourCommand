using System;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts
{
    public class IntFluctuableDisplayer : MonoBehaviour
    {
        [SerializeField] private Image fill;
        [SerializeField] private FluctuableValue fluctuableValue;

        private void OnEnable()
        {
            if (fluctuableValue) fluctuableValue.onModifiedValue += ModifyDisplayer;
        }

        private void OnDisable()
        {
            if (fluctuableValue) fluctuableValue.onModifiedValue -= ModifyDisplayer;
        }

        private void Start()
        {
            
        }

        private void ModifyDisplayer()
        {
            fill.fillAmount = fluctuableValue.CurrentValue / fluctuableValue.MAXValue;
        }
    }
}