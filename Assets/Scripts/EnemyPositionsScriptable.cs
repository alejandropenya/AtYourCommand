using System;
using UnityEditor;
using UnityEngine;

namespace Assets.Scripts
{
    [CreateAssetMenu(fileName = "EnemyPosition", menuName = "AtYourCommand", order = 0)]
    public class EnemyPositionsScriptable : ScriptableObject
    {
        [SerializeField] private Vector2 position;

        public Vector2 Position
        {
            get => position;
        }
    }
}