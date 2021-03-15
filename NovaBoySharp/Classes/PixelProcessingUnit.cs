using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NovaBoySharp
{
    public class PixelProcessingUnit
    {
        #region PPURegisters Class

        public class PPURegisters
        {
            // Variables
            private Bus Bus;

            // Properties
            // LCD Control
            public bool LCDEnable;
            public bool WindowEnable;
            public bool SpriteEnable;
            public bool BackgroundEnable;
            public byte WindowTileMapAddress;
            public byte BackgroundAndWindowTileMapAddress;
            public byte BackgroundTileMapAddress;

            // LCDC Status
            public byte LYInterrupt;
            public byte LYFlag;
            public byte LYDCYCoordinate;
            public byte LYCompare;

            public PPURegisters(Bus bus)
            {
                this.Bus = bus;
            }
        }

        #endregion
    }
}
