using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MiBandNaramek.Constants
{
    public static class ActivityConstant
    {
        public static string GetConstantNameById(int Id)
        {
            switch (Id)
            {
                case -1:
                    return "Neměřeno";
                case 0x00000001:
                    return "TYPE_ACTIVITY";
                case 0x00000002:
                    return "TYPE_LIGHT_SLEEP";
                case 0x00000004:
                    return "TYPE_DEEP_SLEEP";
                case 0x00000008:
                    return "TYPE_NOT_WORN";
                case 0x00000010:
                    return "TYPE_RUNNING";
                case 0x00000020:
                    return "TYPE_WALKING";
                case 0x00000040:
                    return "TYPE_SWIMMING";
                case 0x00000080:
                    return "TYPE_CYCLING";
                case 0x00000100:
                    return "TYPE_TREADMILL";
                case 0x00000200:
                    return "TYPE_EXERCISE";
                case 0x00000400:
                    return "TYPE_SWIMMING_OPENWATER";
                case 0x00000800:
                    return "TYPE_INDOOR_CYCLING";
                case 0x00001000:
                    return "TYPE_ELLIPTICAL_TRAINER";
                case 0x00002000:
                    return "TYPE_JUMP_ROPING";
                case 0x00004000:
                    return "TYPE_YOGA";
                case 0x00008000:
                    return "TYPE_SOCCER";
                case 0x00010000:
                    return "TYPE_ROWING_MACHINE";
                case 0x00020000:
                    return "TYPE_CRICKET";
                case 0x00040000:
                    return "TYPE_BASKETBALL";
                case 0x00080000:
                    return "TYPE_PINGPONG";
                case 0x00100000:
                    return "TYPE_BADMINTON";
                case 0x00200000:
                    return "TYPE_STRENGTH_TRAINING";
                case 0x00000000:
                default:
                    return $"Neznáná aktivita {Id}";
            }
        }
    }
}
