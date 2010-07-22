using System.Collections.Generic;
using System.Linq;

namespace Ninject.Tests.Fakes
{
    internal class ArmsMaster : IWarrior
    {
        public IWeapon[] Weapons { get; set; }
        private int weaponIndex = 0;
        private IWeapon _favoriteWeapon;

        public ArmsMaster(IEnumerable<IWeapon> weapons)
        {
            Weapons = weapons.ToArray();
        }

        public IWeapon Weapon
        {
            get 
            { 
                weaponIndex = (weaponIndex + 1) % Weapons.Length;
                return Weapons[weaponIndex];
            }
        }

        [Inject]
        public void SetFavorite(IWeapon favoriteWeapon)
        {
            _favoriteWeapon = favoriteWeapon;
        }
    }
}