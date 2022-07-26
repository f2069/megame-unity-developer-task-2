using UnityEngine;

namespace MegameAnimatoins.Models.Movement {
    public class PlayerDirectionMovement {
        private readonly Camera _camera;
        private readonly float _moveSpeed;
        private readonly float _pelvisRotateTime;

        public Vector3 Direction => _direction;

        private Vector3 _direction;
        private Vector3 _eulerAngles;
        private float _turnVelocity;

        public PlayerDirectionMovement(Camera camera, float moveSpeed, float pelvisRotateTime) {
            _camera = camera;
            _moveSpeed = moveSpeed;
            _pelvisRotateTime = pelvisRotateTime;
        }

        public void SetDirection(Vector3 inputDirection)
            => _direction = inputDirection;

        public void SetEulerAngles(Vector3 transformEulerAngles)
            => _eulerAngles = transformEulerAngles;

        public Quaternion GetAngle() {
            var targetAngle = GetDirectionAngle();

            var dampedAngle = Mathf.SmoothDampAngle(
                _eulerAngles.y,
                targetAngle,
                ref _turnVelocity,
                _pelvisRotateTime
            );

            return Quaternion.Euler(0, dampedAngle, 0);
        }

        public Vector3 GetDirection(float deltaTime) {
            var targetAngle = GetDirectionAngle();
            _direction = Quaternion.Euler(0, targetAngle, 0) * Vector3.forward;

            return _moveSpeed * deltaTime * _direction;
        }

        private float GetDirectionAngle()
            => Mathf.Atan2(_direction.x, _direction.z) * Mathf.Rad2Deg + _camera.transform.eulerAngles.y;
    }
}
