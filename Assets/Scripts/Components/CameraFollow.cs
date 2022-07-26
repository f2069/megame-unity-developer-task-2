using UnityEngine;

namespace MegameAnimatoins.Components {
    public class CameraFollow : MonoBehaviour {
        [SerializeField] private Transform target;
        [SerializeField] private Vector3 offset;

        private void LateUpdate() {
            transform.position = target.position + offset;
            transform.LookAt(target);
        }
    }
}