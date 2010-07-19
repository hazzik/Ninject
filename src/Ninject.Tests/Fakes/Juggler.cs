using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ninject.Tests.Fakes
{
    public class Juggler : IWarrior
    {
        private readonly Func<IWeapon> _weaponFactory;

        public Juggler(Func<IWeapon> weaponFactory)
        {
            _weaponFactory = weaponFactory;
        }

        public IWeapon Weapon
        {
            get { return _weaponFactory(); }
        }
    }
}
