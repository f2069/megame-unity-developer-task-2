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
        [SerializeField] private Transform swordHandPosition;
        [SerializeField] private float fatalityRadius;
        [SerializeField] private float moveSpeed = 1f;
        [SerializeField] private float rotateTime = .1f;

        [SerializeField] private Enemy target;

        private static readonly int VelocityAnimationKey = Animator.StringToHash("velocity");
        private static readonly int FatalityAnimationKey = Animator.StringToHash("is-fatality");

        private GameObject _currentWeapon;
        private GameObject _swordWeapon;
        private Vector3 _inputDirection;
        private UserInputHandler _userInput;
        private CharacterController _characterController;
        private Camera _camera;
        private Vector3 _lookTarget;

        private float _turnVelocity;

        private void Awake() {
            _camera = Camera.main;
            _characterController = GetComponent<CharacterController>();
            _userInput = GetComponent<UserInputHandler>();

            _userInput.OnMoveEvent += OnInputMove;
            _userInput.OnRotateEvent += OnInputRotate;
            _userInput.OnFireEvent += OnInputFire;
        }

        private bool _playerOnGround = true;

        private void Start() {
            InitWeapon();
        }

        private void Update() {
            var direction = Vector3.zero;
            var deltaTime = Time.deltaTime;

            if (_inputDirection != Vector3.zero) {
                var targetAngle = Mathf.Atan2(_inputDirection.x, _inputDirection.z) * Mathf.Rad2Deg
                                  + _camera.transform.eulerAngles.y;

                var dampedAngle =
                    Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref _turnVelocity, rotateTime);

                transform.rotation = Quaternion.Euler(0, dampedAngle, 0);

                direction = Quaternion.Euler(0, targetAngle, 0) * Vector3.forward;
            }

            direction = _playerOnGround ? direction : direction + Vector3.down;

            if (direction != Vector3.zero) {
                _characterController.Move(direction.normalized * moveSpeed * deltaTime);
                _playerOnGround = _characterController.isGrounded;
            }

            animator.SetFloat(VelocityAnimationKey, direction.normalized.magnitude);
        }

        private void LateUpdate() {
            var spineNodePosition = spineNode.position;

            var lookTarget = _lookTarget;
            lookTarget.y = spineNodePosition.y;

            var signedAngle = Vector3.SignedAngle(
                lookTarget - spineNodePosition,
                transform.forward,
                Vector3.up
            );

            spineNode.localEulerAngles += Quaternion.Euler(signedAngle, 0, 0).eulerAngles;
        }

        private void OnDestroy() {
            _userInput.OnMoveEvent -= OnInputMove;
            _userInput.OnRotateEvent -= OnInputRotate;
            _userInput.OnFireEvent -= OnInputFire;
        }

        private void OnInputRotate(Vector3 mouseposition) {
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

            _lookTarget = pointRaycastHit.point;
        }

        private void OnInputMove(Vector3 direction)
            => _inputDirection = direction.normalized;

        private void InitWeapon() {
            _currentWeapon = Instantiate(
                startWeapon.gameObject,
                shootingHandPosition
            );

            _swordWeapon = Instantiate(
                sword.gameObject,
                swordHandPosition
            );
            _swordWeapon.SetActive(false);
        }

        private void SwitchWeapon(bool isSword) {
            _currentWeapon.SetActive(!isSword);
            _swordWeapon.SetActive(isSword);
        }

        private bool _isAnimated;

        private void OnInputFire() {
            if (_isAnimated) {
                return;
            }

            _isAnimated = true;
            _userInput.SetLock(true);
            _inputDirection = Vector3.zero;
            SwitchWeapon(true);

            animator.SetTrigger(FatalityAnimationKey);
        }

        public void OnFatalityTrigger() {
            target.Kill();
        }

        public void OnFatalityEnd() {
            SwitchWeapon(true);

            _userInput.SetLock(false);
            _isAnimated = false;
        }
    }
}
