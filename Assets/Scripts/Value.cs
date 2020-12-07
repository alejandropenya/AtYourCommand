using System;
using UnityEngine;

namespace Assets.Scripts
{
    [CreateAssetMenu(fileName = "Value", menuName = "AtYourCommand/Value", order = 0)]
    public class Value : ScriptableObject
    {
        [SerializeField] private float _value;
        public event Action onModifiedValue;

        public float CurrentValue
        {
            get => _value;
            set
            {
                _value = value;
                OnModifiedValue();
            }
        }
        
        protected virtual void OnModifiedValue()
        {
            onModifiedValue?.Invoke();
        }
    }
}