using System;
using BasicTools.ButtonInspector;
using UnityEditor;
using UnityEngine;

namespace Assets.Scripts
{
    [CreateAssetMenu(fileName = "EnemyPosition", menuName = "AtYourCommand/EnemyPosition", order = 0)]
    public class EnemyPositionsScriptable : ScriptableObject
    {
        [SerializeField] private Vector3 position;

        public Vector3 Position
        {
            get => position;
        }
    }
}