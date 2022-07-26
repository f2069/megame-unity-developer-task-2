using System.Collections;
using MegameAnimatoins.Components;
using UnityEngine;

namespace MegameAnimatoins.View.Creatures.Mobs {
    [RequireComponent(
        typeof(RagdollActivator),
        typeof(Collider)
    )]
    public class Enemy : MonoBehaviour {
        [SerializeField] private float respawnRange = 15f;
        [SerializeField] private float respawnDelay = 5f;

        private RagdollActivator _ragdollActivator;
        private Collider _selfCollider;
        private Coroutine _coroutine;

        private void Awake() {
            _ragdollActivator = GetComponent<RagdollActivator>();
            _selfCollider = GetComponent<Collider>();
        }

        public void Kill() {
            _selfCollider.enabled = false;
            _ragdollActivator.EnableRagdoll();

            _coroutine = StartCoroutine(Respawn());
        }

        private IEnumerator Respawn() {
            yield return new WaitForSeconds(respawnDelay);

            transform.position = new Vector3(
                Random.Range(-respawnRange, respawnRange),
                0f,
                Random.Range(-respawnRange, respawnRange)
            );

            _ragdollActivator.EnableAnimator();
            _selfCollider.enabled = true;
        }

        private void OnDestroy() {
            if (_coroutine == null) {
                return;
            }

            StopCoroutine(_coroutine);
            _coroutine = null;
        }
    }
}
