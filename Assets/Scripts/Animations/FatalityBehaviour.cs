using MegameAnimatoins.View.Creatures.Player;
using UnityEngine;

namespace MegameAnimatoins.Animations {
    public class FatalityBehaviour : StateMachineBehaviour {
        [SerializeField] [Range(0f, 1f)] private float triggerTime = .5f;

        private Player _player;
        private bool _isTriggered;

        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
            _player = animator.GetComponentInParent<Player>();
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
            _player.OnFatalityTrigger();
        }

        public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
            _player.OnFatalityEnd();
        }
    }
}
