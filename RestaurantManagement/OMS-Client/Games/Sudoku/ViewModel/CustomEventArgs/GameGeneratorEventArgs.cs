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

using OMS.Games.Sudoku.Model.Structures;

namespace OMS.Games.Sudoku.ViewModel.CustomEventArgs
{
    internal class GameGeneratorEventArgs : EventArgs
    {
        #region . Constructors .

        /// <summary>
        /// Intializes a new instance of the GameGeneratorEventArgs class.
        /// </summary>
        /// <param name="cells">Two dimensional array of CellClass objects.</param>
        internal GameGeneratorEventArgs(CellClass[,] cells)
        {
            Cells = cells;                                      // Save the input parameter.
        }

        #endregion

        #region . Properties: Public Read-only .

        /// <summary>
        /// Gets the two dimensional array of CellClass objects that was saved in this instance.
        /// </summary>
        internal CellClass[,] Cells { get; private set; }

        #endregion
    }
}
