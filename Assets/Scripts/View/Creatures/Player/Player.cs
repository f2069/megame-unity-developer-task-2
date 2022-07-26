using MegameAnimatoins.Core.Dictionares;
using MegameAnimatoins.Core.Extensions;
using MegameAnimatoins.UserInput;
using MegameAnimatoins.View.Creatures.Mobs;
using UnityEngine;

namespace MegameAnimatoins.View.Creatures.Player {
    public class Player : MonoBehaviour {
        [SerializeField] private Animator animator;
        [SerializeField] private LayerMask groundLayers;
        [SerializeField] private Transform spineNode;
        [SerializeField] private LayerMask enemyLayers;

        [Header("Weapons")] [SerializeField] private Transform startWeapon;
        [SerializeField] private Transform shootingHandPosition;
        [SerializeField] private Transform sword;

        [SerializeField] private Transform swordHandPosition;

        [Header("Movement")] [SerializeField] private float pelvisRotateTime = .5f;
        [SerializeField] private float moveSpeed = 1f;
        [SerializeField] private float bodyMaxRotateAngle = 70;

        private GameObject _currentWeapon;
        private GameObject _swordWeapon;
        private Vector3 _inputDirection;
        private UserInputHandler _userInput;
        private Camera _camera;
        private Vector3 _lookTarget;
        private Rigidbody _rigidbody;
        private Enemy _enemyTarget;

        private bool _isAnimated;
        private float _turnVelocity;

        private void Awake() {
            _camera = Camera.main;
            _userInput = GetComponent<UserInputHandler>();

            _userInput.OnMoveEvent += OnInputMove;
            _userInput.OnRotateEvent += OnInputRotate;

            _rigidbody = GetComponent<Rigidbody>();
        }

        private void Start() {
            InitWeapons();
        }

        private void FixedUpdate() {
            var direction = Vector3.zero;
            var deltaTime = Time.deltaTime;

            if (_inputDirection != Vector3.zero) {
                var targetAngle = Mathf.Atan2(_inputDirection.x, _inputDirection.z) * Mathf.Rad2Deg
                                  + _camera.transform.eulerAngles.y;

                var dampedAngle = Mathf.SmoothDampAngle(
                    transform.eulerAngles.y,
                    targetAngle,
                    ref _turnVelocity,
                    pelvisRotateTime
                );

                _rigidbody.rotation = Quaternion.Euler(0, dampedAngle, 0);

                direction = Quaternion.Euler(0, targetAngle, 0) * Vector3.forward;

                _rigidbody.MovePosition(_rigidbody.position + direction * moveSpeed * deltaTime);
            }

            animator.SetFloat(AnimatorConstants.VelocityAnimationKey, direction.normalized.magnitude);
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

            signedAngle = signedAngle > 0
                ? Mathf.Min(signedAngle, bodyMaxRotateAngle)
                : Mathf.Max(signedAngle, -bodyMaxRotateAngle);

            spineNode.localEulerAngles += Quaternion.Euler(signedAngle, 0, 0).eulerAngles;
        }

        private void OnDestroy() {
            _userInput.OnMoveEvent -= OnInputMove;
            _userInput.OnRotateEvent -= OnInputRotate;
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

        private void InitWeapons() {
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

        private void KillTarget(Enemy target) {
            if (_isAnimated) {
                return;
            }

            _enemyTarget = target;

            _isAnimated = true;
            _userInput.SetLock(true);
            _inputDirection = Vector3.zero;
            SwitchWeapon(true);

            animator.SetTrigger(AnimatorConstants.FatalityAnimationKey);
        }

        public void OnFatalityTrigger() {
            _enemyTarget.Kill();
        }

        public void OnFatalityEnd() {
            SwitchWeapon(true);

            _userInput.SetLock(false);
            _isAnimated = false;
        }

        private void OnTriggerEnter(Collider other) {
            if (!other.gameObject.IsInLayer(enemyLayers)) {
                return;
            }

            KillTarget(other.gameObject.GetComponent<Enemy>());
        }
    }
}
