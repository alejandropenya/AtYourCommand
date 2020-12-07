using System;
using UnityEngine;

namespace Assets.Scripts
{
    public class PaintOnScreenGizmos : MonoBehaviour
    {
        [SerializeField] private float radius;
        private void OnDrawGizmos()
        {
            Gizmos.DrawSphere(transform.position, radius);
        }
    }
}