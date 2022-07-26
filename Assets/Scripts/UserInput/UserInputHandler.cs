using UnityEngine;

namespace MegameAnimatoins.UserInput {
    public class UserInputHandler : MonoBehaviour {
        public delegate void OnRotate(Vector3 mousePosition);

        public delegate void OnMove(Vector3 direction);

        public event OnRotate OnRotateEvent;

        public event OnMove OnMoveEvent;

        private bool _isLocked;

        private bool CanProcessInput
            => !_isLocked;

        private void Update() {
            if (!CanProcessInput) {
                return;
            }

            GetMovement();
            GetRotation();
        }

        public void SetLock(bool lockState)
            => _isLocked = lockState;

        private void GetMovement() {
            var direction = new Vector3(
                Input.GetAxisRaw(InputConstants.AxisNameHorizontal),
                0f,
                Input.GetAxisRaw(InputConstants.AxisNameVertical)
            );

            OnMoveEvent?.Invoke(direction);
        }

        private void GetRotation()
            => OnRotateEvent?.Invoke(Input.mousePosition);
    }
}
