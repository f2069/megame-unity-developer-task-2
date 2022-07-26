using UnityEngine;

namespace MegameAnimatoins.Models.Movement {
    public class PlayerChestControl {
        private readonly float _bodyMaxRotateAngle;
        private Vector3 _spineNodePosition;

        public PlayerChestControl(float bodyMaxRotateAngle) {
            _bodyMaxRotateAngle = bodyMaxRotateAngle;
        }

        public void SetSpinePosition(Vector3 spineNodePosition)
            => _spineNodePosition = spineNodePosition;

        public Vector3 GetEulerAngles(Vector3 forwardVector, Vector3 targetPosition, bool restrictRotate) {
            var lookTarget = targetPosition;
            lookTarget.y = _spineNodePosition.y;

            var signedAngle = Vector3.SignedAngle(
                lookTarget - _spineNodePosition,
                forwardVector,
                Vector3.up
            );

            if (restrictRotate) {
                signedAngle = signedAngle > 0
                    ? Mathf.Min(signedAngle, _bodyMaxRotateAngle)
                    : Mathf.Max(signedAngle, _bodyMaxRotateAngle * -1);
            }

            return Quaternion.Euler(signedAngle, 0, 0).eulerAngles;
        }
    }
}
