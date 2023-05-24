using System;
using System.Collections.Generic;

namespace SanyoniLib.SystemHelper
{
    public static class ArrayHelper
    {
        public static int CalculateLoopedArrayIndex(int arrayLength, int index)
        {
            // 배열의 길이가 0이하라면 처리할 수 없다.
            if (arrayLength <= 0) return -1;

            if (index < 0) return (int)(arrayLength * Math.Ceiling((float)Math.Abs(index) / arrayLength) + index);
            else return index % arrayLength;
        }

        public static bool RemoveAt<T>(ref T[] array, int index)
        {
            if (array.Length == 0
                || index >= array.Length)
                return false;

            T[] outArray = new T[array.Length - 1];

            for (int i = 0; i < index; i++)
                outArray[i] = array[i];

            for (int i = index; i < array.Length - 1; i++)
                outArray[i] = array[i + 1];

            array = outArray;
            return true;
        }
    }
}