using Microsoft.Xna.Framework;
using System;

namespace Utilities {
    public static class Util {
        public static int Clamp(int mainVal, int range1, int range2) {
            int min = range1;
            int max = range2;
            if (range1 > range2) {
                min = range2;
                max = range1;
            }
            if(mainVal < min)
                return min;
            else if(mainVal > max)
                return max;
            return mainVal;
        }// end Clamp(): int

        public static float Clamp(float mainVal, float range1, float range2) {
            float min = range1;
            float max = range2;
            if (range1 > range2) {
                min = range2;
                max = range1;
            }
            if(mainVal < min)
                return min;
            else if(mainVal > max)
                return max;
            return mainVal;
        }// end Clamp(): float

    }// end Util class

}// end Utilities namespace