using System;
using UnityEngine;

namespace Assets.Scripts
{
    [CreateAssetMenu(fileName = "FluctuableValue", menuName = "AtYourCommand/FluctuableValue", order = 0)]
    public class FluctuableValue : ScriptableObject
    {
        [SerializeField] private float maxValue;
        [SerializeField] private float minValue;
        [SerializeField] private Value _currentValue;
        
        public event Action onModifiedValue;

        public float MAXValue
        {
            get => maxValue;
            set => maxValue = value;
        }

        public float MINValue
        {
            get => minValue;
            set => minValue = value;
        }

        public float CurrentValue
        {
            get => _currentValue.CurrentValue;
            set
            {
                _currentValue.CurrentValue = Mathf.Clamp(value, minValue, maxValue);
                OnModifiedValue();
            }
        }

        protected virtual void OnModifiedValue()
        {
            onModifiedValue?.Invoke();
        }

        public void ResetValue()
        {
            _currentValue.CurrentValue = maxValue;
        }
    }
}