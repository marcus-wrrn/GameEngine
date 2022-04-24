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

        public static bool IsInArray<T>(T[] array, T value) {
            foreach(T arrayVal in array) {
                if(arrayVal.Equals(value))
                    return true;
            }
            return false;
        }// end IsInArray()

    }// end Util class

    public class DoubleBool<T> where T: Enum {
        public bool IsTrue{ get; private set; }
        public bool IsFalse{ get; private set; }
        // Gets what the Enum Value would be if it was True
        public T TrueVal{ get; private set; }
        // Gets what the Enum Value would be if it was False
        public T FalseVal{ get; private set; }
        // Gets the current Enum Value 
        public T CurrentVal{ get { return GetCurrentValue(); } }

        public DoubleBool(T trueState, T falseState ) {
            SetVal(true);
            TrueVal = trueState;
            FalseVal = falseState;
        }// end DoubleBool constructor

        private T GetCurrentValue() {
            if(IsTrue)
                return TrueVal;
            return FalseVal;
        }// end GetCurrentValue()

        public void SetVal(bool val) {
            IsTrue = val;
            IsFalse = !IsTrue;
        }// end SetVal()

        public void Swap() {
            SetVal(!IsTrue);
        }// end SwapVal()
        
    }// end DoubleBool class

}// end Utilities namespace