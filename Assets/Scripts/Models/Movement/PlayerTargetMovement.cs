using UnityEngine;

namespace MegameAnimatoins.Models.Movement {
    public class PlayerTargetMovement {
        private readonly float _moveSpeed;
        private readonly float _rtateSpeed;
        private readonly float _attackDistance;

        public Vector3 Direction => _direction;

        private Vector3 _enemyPosition;
        private Vector3 _direction;
        private Vector3 _lastMovePosition;

        public PlayerTargetMovement(float moveSpeed, float rtateSpeed, float attackDistance) {
            _moveSpeed = moveSpeed;
            _rtateSpeed = rtateSpeed;
            _attackDistance = attackDistance;
        }

        public void SetTarget(Vector3 enemyPosition)
            => _enemyPosition = enemyPosition;

        public Quaternion GetAngle(Vector3 forwardVector, float deltaTime) {
            _direction = Vector3.RotateTowards(
                forwardVector,
                _enemyPosition - _lastMovePosition,
                _rtateSpeed * deltaTime * Mathf.Deg2Rad,
                0f
            );

            return Quaternion.LookRotation(_direction);
        }

        public Vector3 Move(float deltaTime)
            => _lastMovePosition = Vector3.MoveTowards(_lastMovePosition, _enemyPosition, _moveSpeed * deltaTime);

        public bool OnPosition()
            => Vector3.Distance(_enemyPosition, _lastMovePosition) <= _attackDistance;

        public void SetCurrentPosition(Vector3 currentPosition)
            => _lastMovePosition = currentPosition;
    }
}
