using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventManager.Extensions
{
    public static class ItemExtensions
    {
        public static bool IsWeapon(this ItemType type, bool checkMicro = true, bool checkParticle = true)
        {
            switch (type)
            {
                case ItemType.GunCOM15:
                case ItemType.GunE11SR:
                case ItemType.GunCrossvec:
                case ItemType.GunFSP9:
                case ItemType.GunLogicer:
                case ItemType.GunCOM18:
                case ItemType.GunRevolver:
                case ItemType.GunAK:
                case ItemType.GunShotgun:
                    return true;
                case ItemType.ParticleDisruptor:
                    if (checkParticle)
                    {
                        return true;
                    }
                    break;
                case ItemType.MicroHID:
                    if (checkMicro)
                    {
                        return true;
                    }
                    break;
            }
            return false;
        }
    }
}
