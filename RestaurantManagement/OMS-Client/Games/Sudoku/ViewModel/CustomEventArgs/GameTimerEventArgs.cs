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

namespace OMS.Games.Sudoku.ViewModel.CustomEventArgs
{
    internal class GameTimerEventArgs : EventArgs
    {
        #region . Constructors .

        /// <summary>
        /// Initializes a new instance of the GameTimerEventArgs class.
        /// </summary>
        /// <param name="value">Formatted string repesenting the time elapsed since the timer started.</param>
        internal GameTimerEventArgs(string value)
        {
            ElapsedTime = value;                    // Save the input parameter.
        }

        #endregion

        #region . Properties: Public Read-only .

        /// <summary>
        /// Gets the formatted string representing the time elapsed since the timer started.
        /// </summary>
        internal string ElapsedTime { get; private set; }

        #endregion
    }
}
