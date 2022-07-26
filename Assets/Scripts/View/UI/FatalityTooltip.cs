using MegameAnimatoins.View.Creatures.Player;
using UnityEngine;

namespace MegameAnimatoins.View.UI {
    public class FatalityTooltip : MonoBehaviour {
        [SerializeField] private PlayerView player;
        [SerializeField] private Transform tooltip;

        private void Awake() {
            player.OnEnemyNearbyEvent += ShowUi;
            player.OnEnemyAwayEvent += HideUi;
        }

        private void Start() {
            HideUi();
        }

        private void OnDestroy() {
            player.OnEnemyNearbyEvent -= ShowUi;
            player.OnEnemyAwayEvent -= HideUi;
        }

        private void HideUi() {
            tooltip.gameObject.SetActive(false);
        }

        private void ShowUi() {
            tooltip.gameObject.SetActive(true);
        }
    }
}
