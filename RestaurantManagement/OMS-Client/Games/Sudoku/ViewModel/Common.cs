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
using System.Text;

using OMS.Games.Sudoku.Model.Enums;
using OMS.Games.Sudoku.Model.Structures;

namespace OMS.Games.Sudoku.ViewModel
{
    internal class Common
    {
        #region . Constant declarations .

        internal const Int32 MaxLevels = 5;                     // This value should correspond to the number of game levels (DifficultyLevels enum)

        #endregion

        #region . Public properties .

        /// <summary>
        /// Return true if the specified index is between 0 and 8.
        /// </summary>
        /// <param name="index">Index value to check.</param>
        /// <returns></returns>
        internal static bool IsValidIndex(Int32 index)
        {
            return ((0 <= index) && (index <= 8));
        }

        /// <summary>
        /// Return true if the specified column and row is between 0 and 8.
        /// </summary>
        /// <param name="col">Column value to check.</param>
        /// <param name="row">Row value to check.</param>
        /// <returns></returns>
        internal static bool IsValidIndex(Int32 col, Int32 row)
        {
            return (IsValidIndex(col) && IsValidIndex(row));
        }

        /// <summary>
        /// Return true if the specified CellIndex class is valid.
        /// </summary>
        /// <param name="uIndex">CellIndex call to check.</param>
        /// <returns></returns>
        internal static bool IsValidIndex(CellIndex uIndex)
        {
            if (uIndex != null)
                if (IsValidIndex(uIndex.Column, uIndex.Row))
                    return IsValidIndex(uIndex.Region);
            return false;
        }

        /// <summary>
        /// Return true if the specified value is between 1 and 9.
        /// </summary>
        /// <param name="value">Integer to check.</param>
        /// <returns></returns>
        internal static bool IsValidAnswer(Int32 value)
        {
            return ((1 <= value) && (value <= 9));
        }

        /// <summary>
        /// Return true if the specified object is a valid cell state enum.
        /// </summary>
        /// <param name="value">Object to check.</param>
        /// <returns></returns>
        internal static bool IsValidStateEnum(object value)
        {
            return Enum.IsDefined(typeof(CellStateEnum), value);
        }

        /// <summary>
        /// Return true if the specified object is a valid difficulty level.
        /// </summary>
        /// <param name="value">Object to check.</param>
        /// <returns></returns>
        internal static bool IsValidGameLevel(object value)
        {
            return Enum.IsDefined(typeof(DifficultyLevels), value);
        }

        /// <summary>
        /// Print the grid out to the immediate window.
        /// </summary>
        /// <param name="cells">Two dimensional array of cells to print out.</param>
        internal static void PrintGrid(CellClass[,] cells)
        {
            for (Int32 col = 0; col < 9; col++)
            {
                StringBuilder sTemp = new StringBuilder();
                for (Int32 row = 0; row < 9; row++)
                    sTemp.AppendFormat("{0} ", cells[col, row].Answer.ToString());
                Debug.WriteLine("{0}) {1}", col, sTemp.ToString());
            }
        }

        #endregion
    }
}
