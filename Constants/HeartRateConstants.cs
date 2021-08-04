using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MiBandNaramek.Constants
{
    public static class HeartRateConstants
    {
        public static int HighHeartRate = 90;
        public static int MediumHeartRate = 60;
        public static int LowHeartRate = 0;
    }

    public static class ActivityKindConstants
    {
        public static string GetConstantNameById(int Id)
        {
            switch(Id)
            {
                case 0x01:
                    return "WALK";
                case 0x06:
                    return "charging";
                case 0x11:
                    return "WALK 1";
                case 0x12:
                    return "RUN 1";
                case 0x19:
                    return "GO TO BED";
                case 0x1c:
                    return "GET UP 1";
                case 0x21:
                    return "WALK UP";
                case 0x22:
                    return "RUN 2, UP";
                case 0x31:
                    return "WALK 3, DOWN";
                case 0x32:
                    return "RUN 3, DOWN";
                case 0x42:
                    return "RUN 4";
                case 0x51:
                    return "WALK 5";
                case 0x52:
                    return "RUN 5";
                case 0x53:
                    return "NOT WORN 5";
                case 0x5c:
                    return "GET UP SIT";
                case 0x62:
                    return "RUN 6";
                case 0x63:
                    return "NOT WORN 6";
                case 0x6c:
                    return "GET UP 6";
                case 0x73:
                    return "NOT WORN 7";
                case 0x7c:
                    return "GET UP SLP";
                case 0x41:
                    return "UNKNOWN";
                case 0x13:
                    return "UNKNOWN";
                case 0x79:
                    return "SLP MOVE 9";
                case 0x5a:
                    return "SLOW RUN";
                case 0x59:
                    return "SIT MOVE 9";
                case 0x10:
                    return "SLOW WALK";
                case 0x50:
                    return "SIT";
                case 0x60:
                    return "STAND";
                case 0x70:
                case 240:
                    return "SLEEP";
                case 0x1a:
                    return "BACK TO BED";
                case 0x6a:
                    return "STAND ACT.";
                case 243:
                    return "NÁRAMEK NENOŠEN";
                default:
                    return $"Neznámá aktivita id {Id}";
            }
        }
    }
}
