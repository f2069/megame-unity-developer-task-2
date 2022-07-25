using MegameAnimatoins.UserInput;
using UnityEngine;

namespace MegameAnimatoins {
    public class Player : MonoBehaviour {
        [SerializeField] private Animator animator;
        [SerializeField] private LayerMask groundLayers;
        [SerializeField] private Transform spineNode;
        [SerializeField] private Transform startWeapon;
        [SerializeField] private Transform shootingHandPosition;
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
        private Rigidbody _rigidbody;

        private void Awake() {
            _camera = Camera.main;
            _characterController = GetComponent<CharacterController>();
            _userInput = GetComponent<UserInputHandler>();
            _rigidbody = GetComponent<Rigidbody>();

            _userInput.OnMoveEvent += OnInputMove;
            _userInput.OnRotateEvent += OnInputRotate;
        }

        private void Start() {
            InitWeapon();
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

        private void LateUpdate() {
            var lookTarget = _lookTarget;
            lookTarget.y = spineNode.position.y;

            // Debug.DrawRay(
            //     spineNode.position,
            //     lookTarget - spineNode.position,
            //     Color.red
            // );

            var signedAngle = Vector3.SignedAngle(
                lookTarget - spineNode.position,
                transform.forward,
                Vector3.up
            );

            spineNode.localEulerAngles += Quaternion.Euler(signedAngle, 0, 0).eulerAngles;
        }

        private void OnDestroy() {
            _userInput.OnMoveEvent -= OnInputMove;
            _userInput.OnRotateEvent -= OnInputRotate;
        }

        private Vector3 _lookTarget;

        private void OnInputRotate(Vector3 mouseposition) {
            // var cameraPosition = _camera.transform.position;
            var maxDistanceRay = 100f;
            var screenPointRay = _camera.ScreenPointToRay(
                new Vector3(
                    mouseposition.x,
                    mouseposition.y,
                    maxDistanceRay
                )
            );

            if (!Physics.Raycast(screenPointRay, out var pointRaycastHit, maxDistanceRay, groundLayers)) {
                return;
            }

            // var targetPosition = pointRaycastHit.point - cameraPosition;
            // Debug.DrawRay(
            //     cameraPosition,
            //     targetPosition,
            //     Color.yellow
            // );

            _lookTarget = pointRaycastHit.point;
        }

        private void OnInputMove(Vector3 direction)
            => _inputDirection = direction.normalized;

        private void InitWeapon() {
            Instantiate(
                startWeapon.gameObject,
                shootingHandPosition
            );
        }
    }
}
