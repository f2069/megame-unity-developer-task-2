using System.Collections;
using MegameAnimatoins.Components;
using UnityEngine;

namespace MegameAnimatoins {
    public class Enemy : MonoBehaviour {
        private RagdollActivator _ragdollActivator;

        private void Awake() {
            _ragdollActivator = GetComponent<RagdollActivator>();
        }

        public void Kill() {
            _ragdollActivator.EnableRagdoll();

            // @todo destroy

            StartCoroutine(Respawn());
        }

        private IEnumerator Respawn() {
            // @todo
            yield return new WaitForSeconds(3);

            _ragdollActivator.EnableAnimator();
        }
    }
}
