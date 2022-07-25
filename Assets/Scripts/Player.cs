using MegameAnimatoins.UserInput;
using UnityEngine;

namespace MegameAnimatoins {
    public class Player : MonoBehaviour {
        [SerializeField] private Animator animator;
        [SerializeField] private Transform activeWeapon;
        [SerializeField] private Transform sword;
        [SerializeField] private float fatalityRadius;
        [SerializeField] private float moveSpeed = 1f;
        [SerializeField] private float rotateTime = .1f;

        private static readonly int VelocityAnimationKey = Animator.StringToHash("velocity");

        private Vector3 _inputDirection;
        private UserInputHandler _userInput;
        private CharacterController _characterController;
        private Camera _camera;

        private float _turnVelocity;

        private void Awake() {
            _camera = Camera.main;
            _characterController = GetComponent<CharacterController>();
            _userInput = GetComponent<UserInputHandler>();

            _userInput.OnMoveEvent += OnInputMove;
            _userInput.OnRotateEvent += OnInputRotate;
        }

        private void Update() {
            var direction = Vector3.zero;

            if (_inputDirection != Vector3.zero) {
                var targetAngle = Mathf.Atan2(_inputDirection.x, _inputDirection.z) * Mathf.Rad2Deg
                                  + _camera.transform.eulerAngles.y;

                var dampedAngle =
                    Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref _turnVelocity, rotateTime);

                transform.rotation = Quaternion.Euler(0, dampedAngle, 0);

                direction = Quaternion.Euler(0, targetAngle, 0) * Vector3.forward;
                _characterController.Move(direction.normalized * moveSpeed * Time.deltaTime);
            }

            animator.SetFloat(VelocityAnimationKey, direction.normalized.magnitude);
        }

        private void OnDestroy() {
            _userInput.OnMoveEvent -= OnInputMove;
            _userInput.OnRotateEvent -= OnInputRotate;
        }

        private void OnInputRotate(Vector3 mouseposition) {
        }

        private void OnInputMove(Vector3 direction) {
            _inputDirection = direction.normalized;
        }
    }
}
