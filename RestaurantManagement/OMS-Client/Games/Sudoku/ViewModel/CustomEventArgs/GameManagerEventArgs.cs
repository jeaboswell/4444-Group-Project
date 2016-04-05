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

using OMS.Games.Sudoku.Model.Enums;

namespace OMS.Games.Sudoku.ViewModel.CustomEventArgs
{
    internal class GameManagerEventArgs : EventArgs
    {
        #region . Constructors .

        /// <summary>
        /// Initializes a new instance of the GameManagerEventArgs class.
        /// </summary>
        /// <param name="level">Difficulty level of the count.</param>
        /// <param name="count">Number of games generated and saved.</param>
        internal GameManagerEventArgs(DifficultyLevels level, Int32 count)
        {
            Level = level;                                  // Save the input paramters.
            Count = count;
        }

        #endregion

        #region . Properties: Public Read-only .

        /// <summary>
        /// Gets the difficulty level this count belongs to.
        /// </summary>
        internal DifficultyLevels Level { get; private set; }
        /// <summary>
        /// Gets the number of games that were generated and saved.
        /// </summary>
        internal Int32 Count { get; private set; }

        #endregion
    }
}
