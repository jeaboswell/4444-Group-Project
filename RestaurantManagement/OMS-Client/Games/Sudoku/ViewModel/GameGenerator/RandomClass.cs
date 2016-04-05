//
// Copyright (c) 2014 Han Hung
// 
// This program is free software; it is distributed under the terms
// of the GNU General Public License v3 as published by the Free
// Software Foundation.
//
// http://www.gnu.org/licenses/gpl-3.0.html
// 

using System;
using System.Diagnostics;

namespace OMS.Games.Sudoku.ViewModel.GameGenerator
{
    internal class RandomClass
    {
        #region . Variables .

        private static RandomClass _instance;
        private static object _instanceLock = new object();

        private Random _rnd;
        private object _rndLock = new object();

        #endregion

        #region . Constructors .

        // Declared private to prevent other people from instantiating this class.
        private RandomClass()
        { }

        #endregion

        #region . Methods .

        #region . Methods: Public .

        /// <summary>
        /// Gets a random integer between zero and the specified number.
        /// </summary>
        /// <param name="max">Upper bound of the random number to generate.</param>
        /// <returns></returns>
        internal static Int32 GetRandomInt(Int32 max)
        {
            return GetRandomInt(0, max);
        }

        /// <summary>
        /// Get a random integer between the min and max specified.
        /// </summary>
        /// <param name="min">Lower bound of the random number to generate.</param>
        /// <param name="max">Upper bound of the random number to generate.</param>
        /// <returns></returns>
        internal static Int32 GetRandomInt(Int32 min, Int32 max)
        {
            CheckInstance();                                    // Check if the singleton is generated.
            return _instance.GetNextInt(min, max);              // Return a random integer between min and max
        }

        #endregion

        #region . Methods: Private .

        private static void CheckInstance()
        {
            if (_instance == null)                          // Is the instance variable null?
            {
                lock (_instanceLock)                        // Yes, obtain a lock on the instance object
                {
                    if (_instance == null)                  // Check if the instance variable is null again
                    {
                        _instance = new RandomClass();      // Instantiate a Random class
                        _instance.InitInstance();           // Initialize the instance
                    }
                }
            }
        }

        private void InitInstance()
        {
            if (_rnd == null)                               // Is the random variable null?
            {
                lock (_rndLock)                             // Yes, obtain a lock on the random object
                {
                    if (_rnd == null)                       // Check if the random object is null again
                    {                                       // It is so create a seed and create a new random class
                        TimeSpan tsp = new TimeSpan(DateTime.Now.Ticks);
                        Int32 seed = (int)(((tsp.TotalMilliseconds * 10000) % Int32.MaxValue) % 10000);
                        Debug.WriteLine("Random seed = {0}", seed);
                        _rnd = new Random(seed);
                    }
                }
            }
        }

        private Int32 GetNextInt(Int32 min, Int32 max)
        {
            if (_rnd == null)                               // If random object is null
                lock (_instance)                            // Lock the instance object because other thread
                { }                                         // is still probably creating the instance
            lock (_rndLock)                                 // Obtain a lock on the random object
            {
                return _rnd.Next(min, max);                 // Return a random number between min and max
            }
        }

        #endregion

        #endregion
    }
}
