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
using System.Diagnostics;

namespace OMS.Games.Sudoku.ViewModel.GameGenerator.Solver
{
    internal abstract class DancingArena
    {
        #region . Variables .

        private DancingNode[] _solutionsRows;
        private DancingColumn[] _headerColumns;

        #endregion

        #region . Constructors .

        internal DancingArena(Int32 primary, Int32 secondary)
        {
            InitClass(primary, secondary);

            for (Int32 i = 0; i < primary; i++)
            {
                _solutionsRows[i] = null;
                _headerColumns[i] = new DancingColumn(i + 1);
                _headerColumns[i].Right = null;
                if (i > 0)
                {   // Connect the current column to the previous column.
                    _headerColumns[i].Left = _headerColumns[i - 1];
                    _headerColumns[i - 1].Right = _headerColumns[i];
                }
            }

            // Connect first and last primary column to the root node.
            _headerColumns[0].Left = Root;
            Root.Right = _headerColumns[0];
            _headerColumns[primary - 1].Right = Root;
            Root.Left = _headerColumns[primary - 1];

            // Adding self referential secondary columns.
            if (secondary > 0)
                for (Int32 i = 0; i < secondary; i++)
                    _headerColumns[primary + i] = new DancingColumn(primary + i + 1);
        }

        internal DancingArena(Int32 columns)
            : this(columns, 0)
        { }

        #endregion

        #region . Properties: Private .

        private Int32 Initial { get; set; }
        private DancingColumn Root { get; set; }
        private Int32 Rows { get; set; }
        private DancingColumn FirstColumn
        {
            get
            {
                return (DancingColumn)Root.Right;
            }
        }
        private DancingColumn LastColumn
        {
            get
            {
                return (DancingColumn)Root.Left;
            }
        }

        #endregion

        #region . Methods .

        #region . Methods: Public .

        internal DancingNode AddRow(Int32[] positions)
        {
            DancingNode result = null;
            if (positions.Length > 0)
            {
                bool found;
                DancingNode thisNode = null;
                DancingNode prevNode = null;
                Rows++;
                for (Int32 i = 0; i < positions.Length; i++)
                {
                    if (positions[i] > 0)
                    {
                        thisNode = new DancingNode(Rows, positions[i]);
                        thisNode.Left = prevNode;
                        thisNode.Right = null;
                        if (prevNode != null)
                            prevNode.Right = thisNode;
                        else
                            result = thisNode;
                        found = false;
                        for (Int32 j = 0; j < _headerColumns.Length; j++)
                        {
                            DancingColumn col = _headerColumns[j];
                            if (col.Col == positions[i])
                            {
                                thisNode.Header = col;
                                col.AddNode(thisNode);
                                found = true;
                            }
                        }
                        if (!found)
                            Debug.WriteLine("Can't find header for {0}.", positions[i]);
                        prevNode = thisNode;
                        result.Left = prevNode;
                        prevNode.Right = result;
                    }
                }
            }
            return result;
        }

        internal void Solve()
        {
            SolveRecurse(Initial);
        }

        internal void RemoveKnown(List<DancingNode> solutions)
        {
            for (Int32 i = 0; i < solutions.Count; i++)
            {
                DancingNode row = solutions[i];
                _solutionsRows[Initial] = row;
                Initial++;
                CoverColumn(row.Header);
                DancingNode col = row.Right;
                while (Equals(col, row) == false)
                {
                    CoverColumn(col.Header);
                    col = col.Right;
                }

            }
        }

        internal void ShowState()
        {
            DancingColumn col = FirstColumn;
            while (Equals(col, Root) == false)
            {
                Debug.WriteLine("C : {0}", col.ToString());
                DancingNode row = col.Lower;
                while (Equals(row, col) == false)
                {
                    Debug.WriteLine("  R : {0}", row.ToString());
                    row = row.Lower;
                }
                col = (DancingColumn)col.Right;
            }
        }

        internal abstract void HandleSolution(DancingNode[] rows);

        #endregion

        #region . Methods: Private .

        private void InitClass(Int32 primary, Int32 secondary)
        {
            Rows = 0;
            Initial = 0;
            Root = new DancingColumn(0);

            // Only primary columns form the solution.
            _solutionsRows = new DancingNode[primary];
            _headerColumns = new DancingColumn[primary + secondary];
        }

        private void CoverColumn(DancingColumn column)
        {
            column.Left.Right = column.Right;
            column.Right.Left = column.Left;
            DancingNode row = column.Lower;

            while (Equals(row, column) == false)
            {
                DancingNode col = row.Right;
                while (Equals(col, row) == false)
                {
                    col.Upper.Lower = col.Lower;
                    col.Lower.Upper = col.Upper;
                    col.Header.DecRows();
                    col = col.Right;
                }
                row = row.Lower;
            }
        }

        private void UncoverColumn(DancingColumn column)
        {
            DancingNode row = column.Upper;
            while (Equals(row, column) == false)
            {
                DancingNode col = row.Left;
                while (Equals(col, row) == false)
                {
                    col.Upper.Lower = col;
                    col.Lower.Upper = col;
                    col.Header.IncRows();
                    col = col.Left;
                }
                row = row.Upper;
            }
            column.Left.Right = column;
            column.Right.Left = column;
        }

        private DancingColumn NextColumn()
        {
            DancingColumn result = (DancingColumn)Root.Right;
            Int32 minRows = result.Rows;
            DancingColumn scanner = (DancingColumn)Root.Left;
            while (Equals(scanner, Root.Right) == false)
            {
                if (scanner.Rows < minRows)
                {
                    result = scanner;
                    minRows = scanner.Rows;
                }
                scanner = (DancingColumn)scanner.Left;
            }
            return result;
        }

        private void SolveRecurse(Int32 index)
        {
            if (Equals(Root, Root.Right))
                HandleSolution(_solutionsRows);             // No more columns, we found one solution.
            else
            {
                DancingColumn nextCol = NextColumn();       // Select next column using some selection algorithm.
                CoverColumn(nextCol);                       // Exclude selected column from the matrix.
                DancingNode row = nextCol.Lower;            // Try for each row in selected column.
                while (Equals(row, nextCol) == false)
                {
                    _solutionsRows[index] = row;            // Add row to solutions array.
                    DancingNode col = row.Right;            // Exclude all columns covered by this row
                    while (Equals(col, row) == false)
                    {
                        CoverColumn(col.Header);
                        col = col.Right;
                    }
                    SolveRecurse(index + 1);                // And try to solve the reduced matrix.

                    col = row.Left;                         // Now, restore all columns covered by this row.
                    while (Equals(col, row) == false)
                    {
                        UncoverColumn(col.Header);
                        col = col.Left;
                    }
                    _solutionsRows[index] = null;           // And remove row from solution array.
                    row = row.Lower;
                }
                UncoverColumn(nextCol);                     // Return excluded column back to list.
            }
        }

        #endregion

        #endregion
    }
}
