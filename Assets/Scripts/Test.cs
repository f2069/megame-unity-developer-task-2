using UnityEngine;

namespace MegameAnimatoins {
    public class Test : MonoBehaviour {
        [SerializeField] private Transform targetBone;
        [SerializeField] private Vector3 rotation;

        public void LateUpdate() {
            targetBone.localEulerAngles = rotation;
        }
    }
}
