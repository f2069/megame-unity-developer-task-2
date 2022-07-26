using UnityEngine;

namespace MegameAnimatoins.View.Armory {
    public class PlayerWeaponView : MonoBehaviour {
        [Header("Weapons")] [SerializeField] private Transform startWeapon;
        [SerializeField] private Transform startHandPosition;
        [SerializeField] private Transform sword;
        [SerializeField] private Transform swordHandPosition;

        private GameObject _currentWeapon;
        private GameObject _swordWeapon;

        public void InitWeapons() {
            _currentWeapon = Instantiate(
                startWeapon.gameObject,
                startHandPosition
            );

            _swordWeapon = Instantiate(
                sword.gameObject,
                swordHandPosition
            );
            _swordWeapon.SetActive(false);
        }

        public void SwitchWeapon(bool isSword) {
            _currentWeapon.SetActive(!isSword);
            _swordWeapon.SetActive(isSword);
        }
    }
}