using MegameAnimatoins.Core.Dictionares;
using MegameAnimatoins.Core.Extensions;
using MegameAnimatoins.Core.Interfaces;
using MegameAnimatoins.Models.Movement;
using MegameAnimatoins.UserInput;
using MegameAnimatoins.View.Armory;
using MegameAnimatoins.View.Creatures.Mobs;
using UnityEngine;

namespace MegameAnimatoins.View.Creatures.Player {
    [RequireComponent(
        typeof(PlayerWeaponView)
    )]
    public class PlayerView : MonoBehaviour, IPlayerInterface {
        [SerializeField] private Animator animator;
        [SerializeField] private LayerMask groundLayers;
        [SerializeField] private Transform spineNode;
        [SerializeField] private LayerMask enemyLayers;

        [Header("Movement")] [SerializeField] private float pelvisRotateTime = .5f;
        [SerializeField] private float moveSpeed = 1f;
        [SerializeField] private float bodyMaxRotateAngle = 70;

        [Header("Fatality")] [SerializeField] private float fatalityPelvisRotateSpeed = 360f;
        [SerializeField] private float fatalityMoveSpeed = 1f;
        [SerializeField] private float attackDistance = 1.7f;

        public delegate void OnEnemyNearby();

        public delegate void OnEnemyAway();

        public event OnEnemyAway OnEnemyAwayEvent;
        public event OnEnemyNearby OnEnemyNearbyEvent;

        private PlayerWeaponView _weapons;
        private Camera _camera;
        private UserInputHandler _userInput;
        private Vector3 _inputDirection;
        private Vector3 _mousePointPosition;
        private Rigidbody _rigidbody;
        private Enemy _enemyTarget;
        private Enemy _tmpEnemyTarget;

        private PlayerDirectionMovement _directionMovement;
        private PlayerTargetMovement _targetMovement;
        private PlayerChestControl _chestMovement;

        private bool _isFatalityAnimated;

        private void Awake() {
            _camera = Camera.main;
            _userInput = GetComponent<UserInputHandler>();

            _userInput.OnMoveEvent += OnInputMove;
            _userInput.OnRotateEvent += OnInputRotate;
            _userInput.OnFireEvent += OnKillEnemy;

            _rigidbody = GetComponent<Rigidbody>();

            _weapons = GetComponent<PlayerWeaponView>();

            _directionMovement = new PlayerDirectionMovement(
                _camera,
                moveSpeed,
                pelvisRotateTime
            );

            _targetMovement = new PlayerTargetMovement(
                fatalityMoveSpeed,
                fatalityPelvisRotateSpeed,
                attackDistance
            );

            _chestMovement = new PlayerChestControl(
                bodyMaxRotateAngle
            );
        }

        private void Start() {
            _weapons.InitWeapons();
        }

        private void FixedUpdate() {
            var direction = Vector3.zero;
            var deltaTime = Time.deltaTime;
            var currentTransform = transform;

            if (_enemyTarget) {
                var enemyPosition = _enemyTarget.gameObject.transform.position;
                var forwardVector = currentTransform.forward;
                var currentPosition = currentTransform.position;

                _targetMovement.SetTarget(enemyPosition);
                _targetMovement.SetCurrentPosition(currentPosition);

                currentTransform.rotation = _targetMovement.GetAngle(forwardVector, deltaTime);

                if (_targetMovement.OnPosition()) {
                    if (_isFatalityAnimated == false) {
                        _isFatalityAnimated = true;

                        _weapons.SwitchWeapon(true);
                        animator.SetTrigger(AnimatorConstants.FatalityAnimationKey);
                    }
                } else {
                    currentTransform.position = _targetMovement.Move(deltaTime);
                    direction = _targetMovement.Direction;
                }
            } else if (_inputDirection != Vector3.zero) {
                _directionMovement.SetDirection(_inputDirection);
                _directionMovement.SetEulerAngles(transform.eulerAngles);

                _rigidbody.rotation = _directionMovement.GetAngle();
                _rigidbody.MovePosition(_rigidbody.position + _directionMovement.GetDirection(deltaTime));

                direction = _directionMovement.Direction;
            }

            animator.SetFloat(AnimatorConstants.VelocityAnimationKey, direction.magnitude);
        }

        private void LateUpdate() {
            var enemyPosition = _enemyTarget ? _enemyTarget.gameObject.transform.position : Vector3.zero;

            _chestMovement.SetSpinePosition(spineNode.position);

            spineNode.localEulerAngles += _chestMovement.GetEulerAngles(
                transform.forward,
                _enemyTarget ? enemyPosition : _mousePointPosition,
                _enemyTarget == null
            );
        }

        private void OnDestroy() {
            _userInput.OnMoveEvent -= OnInputMove;
            _userInput.OnRotateEvent -= OnInputRotate;
            _userInput.OnFireEvent -= OnKillEnemy;
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

            _mousePointPosition = pointRaycastHit.point;
        }

        private void OnInputMove(Vector3 direction)
            => _inputDirection = direction.normalized;

        private void OnTriggerEnter(Collider other) {
            if (!other.gameObject.IsInLayer(enemyLayers)) {
                return;
            }

            _tmpEnemyTarget = other.gameObject.GetComponent<Enemy>();

            OnEnemyNearbyEvent?.Invoke();
        }

        private void OnTriggerExit(Collider other) {
            if (!other.gameObject.IsInLayer(enemyLayers)) {
                return;
            }

            _tmpEnemyTarget = null;

            OnEnemyAwayEvent?.Invoke();
        }

        public void OnFatalityTrigger()
            => _enemyTarget.Kill();

        public void OnFatalityAnimationEnd() {
            _weapons.SwitchWeapon(false);

            _userInput.SetLock(false);
            _enemyTarget = _tmpEnemyTarget = null;
            _isFatalityAnimated = false;
        }

        private void OnKillEnemy() {
            if (_tmpEnemyTarget == null || _enemyTarget) {
                return;
            }

            _userInput.SetLock(true);
            _inputDirection = Vector3.zero;
            _enemyTarget = _tmpEnemyTarget;

            OnEnemyAwayEvent?.Invoke();
        }
    }
}
