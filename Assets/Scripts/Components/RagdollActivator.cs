using UnityEngine;

namespace MegameAnimatoins.Components {
    public class RagdollActivator : MonoBehaviour {
        [SerializeField] private Animator animator;
        [SerializeField] private Transform ragdollRoot;
        [SerializeField] private bool ragdoolAwake;

        private Rigidbody[] _rigidbodies;
        private CharacterJoint[] _joints;
        private Collider[] _colliders;

        private void Awake() {
            _rigidbodies = ragdollRoot.GetComponentsInChildren<Rigidbody>();
            _joints = ragdollRoot.GetComponentsInChildren<CharacterJoint>();
            _colliders = ragdollRoot.GetComponentsInChildren<Collider>();
        }

        private void Start() {
            SwitchRagdoll(ragdoolAwake);
        }

        [ContextMenu("EnableRagdoll")]
        public void EnableRagdoll() {
            SwitchRagdoll(true);
        }

        [ContextMenu("EnableAnimator")]
        public void EnableAnimator() {
            SwitchRagdoll(false);
        }

        private void SwitchRagdoll(bool ragdollState) {
            animator.enabled = !ragdollState;
            foreach (var joint in _joints) {
                joint.enableCollision = ragdollState;
            }

            foreach (var modelCollider in _colliders) {
                modelCollider.enabled = ragdollState;
            }

            foreach (var modelRigidbody in _rigidbodies) {
                modelRigidbody.isKinematic = !ragdollState;
                modelRigidbody.useGravity = ragdollState;
                modelRigidbody.detectCollisions = ragdollState;
                modelRigidbody.velocity = ragdollState ? Vector3.zero : modelRigidbody.velocity;
            }
        }
    }
}
