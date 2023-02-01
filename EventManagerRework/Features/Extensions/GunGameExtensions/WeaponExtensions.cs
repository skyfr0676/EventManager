using Exiled.API.Enums;
using Exiled.API.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventManagerRework.Features.Extensions.GunGameExtensions
{
    public static class WeaponExtensions
    {
        public static bool IsWeapon(this ItemType type, bool CheckMicro = true, bool Checkx3 = true)
        {
            if (type.GetFirearmType() == FirearmType.None)
            {
                if (CheckMicro)
                {
                    return type == ItemType.MicroHID;
                }

                return false;
            }
            if (!Checkx3)
            {
                return !(type == ItemType.ParticleDisruptor);
            }
            return true;
        }
    }
}
