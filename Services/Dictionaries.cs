using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MRTest.Services
{
    public static class Dictionaries
    {
        public static int MaxIndex = 170;
        public static int MaxMiddle = 0;
        public static int MaxRing = 170;
        public static int MaxPinky = 0;

        public static int MinIndex = 0;
        public static int MinMiddle = 170;
        public static int MinRing = 0;
        public static int MinPinky = 170;


        public static int MaxIndexRight = 0;
        public static int MaxMiddleRight = 170;
        public static int MaxRingRight = 0;
        public static int MaxPinkyRight = 170;

        public static int MinIndexRight = 170;
        public static int MinMiddleRight = 0;
        public static int MinRingRight = 170;
        public static int MinPinkyRight = 0;


        public static  Dictionary<double, int> DictIndex = new(); // no reverse
        public static  Dictionary<double, int> DictMiddle = new(); // reverse
        public static  Dictionary<double, int> DictRing = new(); // no reverse
        public static  Dictionary<double, int> DictPinky = new(); // reverse


        public static Dictionary<double, int> DictIndexRight = new(); // reverse
        public static Dictionary<double, int> DictMiddleRight = new(); //no reverse
        public static Dictionary<double, int> DictRingRight = new(); // reverse
        public static Dictionary<double, int> DictPinkyRight = new(); //no reverse


        public static Dictionary<double, int> MyDict = new Dictionary<double, int>
         {

              {0.0, 0}, {0.1, 2}, {0.2, 4}, {0.3, 8}, {0.4, 12}, {0.5, 14}, {0.6, 16},
             {0.7, 18}, {0.8, 20}, {0.9, 22}, {1.0, 24}, {1.1, 26}, {1.2, 28}, {1.3, 30},
             {1.4, 32}, {1.5, 34}, {1.6, 36}, {1.7, 38}, {1.8, 40}, {1.9, 42}, {2.0, 44},
             {2.1, 46}, {2.2, 48}, {2.3, 50}, {2.4, 52}, {2.5, 54}, {2.6, 56}, {2.7, 58},
             {2.8, 60}, {2.9, 62}, {3.0, 68}

         };
        public static Dictionary<double, int> MyDictReverse = new Dictionary<double, int>
         {
             {0.0, 68}, {0.1, 64}, {0.2, 62}, {0.3, 60}, {0.4, 58}, {0.5, 56}, {0.6, 54},
             {0.7, 52}, {0.8, 50}, {0.9, 48}, {1.0, 46}, {1.1, 44}, {1.2, 42}, {1.3, 40},
             {1.4, 38}, {1.5, 36}, {1.6, 34}, {1.7, 32}, {1.8, 30}, {1.9, 28}, {2.0, 26},
             {2.1, 24}, {2.2, 22}, {2.3, 20}, {2.4, 18}, {2.5, 16}, {2.6, 14}, {2.7, 12},
             {2.8, 10}, {2.9, 8}, {3.0, 2}
         };

        public static Dictionary<double, int> RedistributeValues(Dictionary<double, int> dict, int minValue, int maxValue, bool reverse = false)
        {
            if (dict.Count == 0) return new Dictionary<double, int>();

            int oldMin = dict.Values.Min();
            int oldMax = dict.Values.Max();

            if (oldMin == oldMax)
            {
                return dict.ToDictionary(kvp => kvp.Key, kvp => minValue);
            }

            Dictionary<double, int> newDict = new Dictionary<double, int>();

            foreach (var kvp in dict)
            {
                double key = kvp.Key;
                int oldValue = kvp.Value;

                // Реверсируем значение относительно старого диапазона, если нужно
                if (reverse)
                {
                    oldValue = oldMax - (oldValue - oldMin);
                }

                // Преобразуем значение в новый диапазон
                int newValue = (int)Math.Round(((double)(oldValue - oldMin) / (oldMax - oldMin)) * (maxValue - minValue) + minValue);

                newDict[key] = newValue;
            }

            return newDict;
        }


    }

   

}
