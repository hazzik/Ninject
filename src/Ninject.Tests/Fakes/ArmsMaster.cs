using System.Collections.Generic;
using System.Linq;

namespace Ninject.Tests.Fakes
{
    internal class ArmsMaster : IWarrior
    {
        public IWeapon[] Weapons { get; set; }
        private int weaponIndex = 0;

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
    }
}