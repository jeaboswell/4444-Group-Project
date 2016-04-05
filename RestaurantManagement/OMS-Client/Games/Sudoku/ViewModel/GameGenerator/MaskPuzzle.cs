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
using System.Collections.Generic;

using OMS.Games.Sudoku.Model.Enums;
using OMS.Games.Sudoku.Model.Structures;

namespace OMS.Games.Sudoku.ViewModel.GameGenerator
{
    internal class MaskPuzzle
    {
        #region . Variables, Constants, And other Declarations .

        #region . Constants .

        private Int32 _cMaxInterations = 10;    // Maximum number of iterations before we give up.

        #endregion

        #region . Variables .

        private DifficultyLevels _level;

        #endregion

        #endregion

        #region . Constructors .

        /// <summary>
        /// Initializes a new instance of the MaskPuzzle class.
        /// </summary>
        /// <param name="Level">The difficulty level to mask to.</param>
        internal MaskPuzzle(DifficultyLevels Level)
        {
            _level = Level;
        }

        #endregion

        #region . Properties: Public .

        /// <summary>
        /// Gets a flag indicating whether the masking was good or not.
        /// </summary>
        internal bool NotGood { get; private set; }

        #endregion

        #region . Methods .

        #region . Methods: Public .

        /// <summary>
        /// Masks the specified puzzle board.
        /// </summary>
        /// <param name="cells">Puzzle to mask.</param>
        internal void MaskBoard(CellClass[,] cells)
        {
            List<CellClass> list = TransferToList(cells);                       // Transfer the 2D array to a list.
            Int32 mask = 81 - GetMaskedValue(_level);                           // Get the number of cells to mask based on the difficulty level.
            SolveGame cSolve = new SolveGame();                                 // Instantiate a the game solver class
            Int32 numIterations = 0;                                            // Initialize the iteration counter
            NotGood = false;                                                    // Clear the flag.
            do
            {
                CellIndex index1 = FindRandomCell(list);                        // Find a random cell
                CellIndex index2 = GetMirror(index1);                           // Get a mirror cell on the vertical axis
                SetCellState(cells, index1, index2, CellStateEnum.Blank);       // Mask the cells
                if (cSolve.SolvePuzzle(cells))                                  // Try to sell the puzzle
                {
                    mask -= RemoveCells(list);                                  // Solvable, remove the masked cells from the list
                    numIterations = 0;                                          // Zero out the interation counter
                }
                else
                {                                                               // Not solvable
                    SetCellState(cells, index1, index2, CellStateEnum.Answer);  // Restore the cell states
                    numIterations++;                                            // Increment the interation counter
                    if (numIterations > _cMaxInterations)                       // Did we exceed the maximum iterations?
                    {
                        NotGood = true;                                         // Yes, raise the flag
                        break;                                                  // Exit out of the loop
                    }
                }
            } while (mask > 0);                                                 // Keep loop until there are no more cells to mask
        }

        #endregion

        #region . Methods: Private .

        private static List<CellClass> TransferToList(CellClass[,] cells)
        {
            List<CellClass> retList = new List<CellClass>(81);                  // Instantiate a new list
            for (Int32 col = 0; col < 9; col++)                                 // Loop through the columns
                for (Int32 row = 0; row < 9; row++)                             // Loop through the rows
                    retList.Add(cells[col, row]);                               // Add the cell to the list
            return retList;                                                     // Return the list
        }

        private static CellIndex GetMirror(CellIndex index)
        {
            Int32 col = 8 - index.Column;                                       // Get the index on the other side
            return new CellIndex(col, index.Row);                               // Return a new CellIndex class
        }

        private static CellIndex FindRandomCell(List<CellClass> list)
        {
            Int32 index = RandomClass.GetRandomInt(list.Count - 1);             // Find a random cell in the list
            return list[index].CellIndex;                                       // Return the CellIndex of the selected cell
        }

        private static void SetCellState(CellClass[,] cells, CellIndex index1, CellIndex index2, CellStateEnum state)
        {
            cells[index1.Column, index1.Row].CellState = state;                 // Set the cell state of the first cell
            cells[index2.Column, index2.Row].CellState = state;                 // Set the cell state of the second cell
        }

        private static Int32 RemoveCells(List<CellClass> cells)
        {
            Int32 index = FindFirstMaskedCell(cells);                           // Find the index of the first masked cell
            cells.RemoveAt(index);                                              // Remove it from the list
            Int32 count = 1;                                                    // Set the counter to 1
            index = FindFirstMaskedCell(cells);                                 // Find the index of the next masked cell
            if (index >= 0)                                                     // Found?
            {
                cells.RemoveAt(index);                                          // Yes, remove it as well
                count++;                                                        // Increment the count
            }
            return count;                                                       // Return the count of masked cells removed
        }

        private static Int32 FindFirstMaskedCell(List<CellClass> cells)
        {
            for (Int32 i = 0; i < cells.Count; i++)                             // Loop through the list of cells
                if (cells[i].CellState == CellStateEnum.Blank)                  // Find first blank cell
                    return i;                                                   // Return the index
            return -1;                                                          // None found, return -1
        }

        private static Int32 GetMaskedValue(DifficultyLevels level)
        {
            Int32 min;
            Int32 max;
            switch (level)
            {
                case DifficultyLevels.VeryEasy:             // If the difficulty level is very easy
                    min = 50;                               // Number of given is between 50 and 60
                    max = 60;
                    break;

                case DifficultyLevels.Easy:                 // If the difficulty level is easy       
                    min = 36;                               // Number of given is between 36 and 49
                    max = 49;
                    break;

                case DifficultyLevels.Medium:               // If the difficulty level is medium
                    min = 32;                               // Number of given is between 32 and 35
                    max = 35;
                    break;

                case DifficultyLevels.Hard:                 // If the difficulty level is hard
                    min = 28;                               // Number of given is between 28 and 31
                    max = 31;
                    break;

                default:                                    // Default is expert level.
                    min = 22;                               // Number of given is between 22 and 27
                    max = 27;
                    break;
            }
            return RandomClass.GetRandomInt(min, max);      // Return a random number between the min and max
        }

        #endregion

        #endregion
    }
}
