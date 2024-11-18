using UnityEngine;

namespace Platformer
{
    public class GroundChecker: MonoBehaviour
    {
        [SerializeField] float groundDistance = 0.08f;
        [SerializeField] LayerMask groundLayers;

        public bool IsGround { get; private set; }
        private void Update()
        {
            IsGround = Physics.CheckSphere(transform.position, groundDistance, groundLayers);
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.DrawSphere(transform.position, groundDistance);
        }
    }
}
