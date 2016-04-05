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

namespace OMS.Games.Sudoku.ViewModel.GameGenerator.Solver
{
    internal class DancingColumn : DancingNode
    {
        #region . Constructors .

        internal DancingColumn(Int32 col)
            : base(0, col)
        { }

        #endregion

        #region . Properties: Public Read-only .

        internal Int32 Rows { get; private set; }

        #endregion

        #region . Methods: Public .

        internal void IncRows()
        {
            Rows++;
        }

        internal void DecRows()
        {
            Rows--;
        }

        internal void AddNode(DancingNode node)
        {
            DancingNode last = this.Upper;
            node.Upper = last;
            node.Lower = this;
            last.Lower = node;
            this.Upper = node;
            IncRows();
        }

        public override string ToString()
        {
            return string.Format("{0}, rows {1}", base.ToString(), Rows);
        }

        #endregion
    }
}
