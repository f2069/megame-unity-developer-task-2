using MegameAnimatoins.Core.Interfaces;
using UnityEngine;

namespace MegameAnimatoins.Animations {
    public class FatalityBehaviour : StateMachineBehaviour {
        [SerializeField] [Range(0f, 1f)] private float triggerTime = .5f;

        private IPlayerInterface _playerView;
        private bool _isTriggered;

        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
            _playerView = animator.GetComponentInParent<IPlayerInterface>();
            _isTriggered = false;
        }

        public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
            if (_isTriggered) {
                return;
            }

            var t = stateInfo.normalizedTime;
            t -= (int) t;

            if (t < triggerTime) {
                return;
            }

            _isTriggered = true;
            _playerView.OnFatalityTrigger();
        }

        public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
            => _playerView.OnFatalityAnimationEnd();
    }
}
