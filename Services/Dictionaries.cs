﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MRTest.Services
{
    public static class Dictionaries
    {
        public static readonly Dictionary<double, int> MyDict = new Dictionary<double, int>
        {
            {0.0, 0}, {0.1, 0}, {0.2, 0}, {0.3, 0}, {0.4, 1}, {0.5, 1}, {0.6, 1},
            {0.7, 2}, {0.8, 2}, {0.9, 2}, {1.0, 3}, {1.1, 3}, {1.2, 3}, {1.3, 4},
            {1.4, 4}, {1.5, 4}, {1.6, 5}, {1.7, 5}, {1.8, 5}, {1.9, 6}, {2.0, 6},
            {2.1, 6}, {2.2, 7}, {2.3, 7}, {2.4, 7}, {2.5, 8}, {2.6, 9}, {2.7, 9},
            {2.8, 9}, {2.9, 9}, {3.0, 9}, {3.1, 9}
        };

        public static readonly Dictionary<double, int> MyDictIndex = new Dictionary<double, int>
        {
            {0.0, 9}, {0.1, 9}, {0.2, 9}, {0.3, 9}, {0.4, 8}, {0.5, 8}, {0.6, 8},
            {0.7, 7}, {0.8, 7}, {0.9, 7}, {1.0, 6}, {1.1, 6}, {1.2, 6}, {1.3, 5},
            {1.4, 5}, {1.5, 5}, {1.6, 4}, {1.7, 4}, {1.8, 4}, {1.9, 3}, {2.0, 3},
            {2.1, 3}, {2.2, 2}, {2.3, 2}, {2.4, 2}, {2.5, 1}, {2.6, 0}, {2.7, 0},
            {2.8, 0}, {2.9, 0}, {3.0, 0}, {3.1, 0}
        };

        public static readonly Dictionary<double, int> MyDictThumb = new Dictionary<double, int>
        {
            {0.0, 0}, {0.1, 0}, {0.2, 1}, {0.3, 2}, {0.4, 3}, {0.5, 4}, {0.6, 5},
            {0.7, 6}, {0.8, 7}, {0.9, 8}, {1.0, 9}, {1.1, 9}, {1.2, 9}
        };
    }

}