using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NovaBoySharp
{
    public class Bus
    {

        // ROM              0x0000 - 0x7FFF
        private ROM Cartridge;

        // VRAM             0x8000 - 0x9FFF
        private byte[] VideoMemory;

        // Swicthcable RAM  0xA000 - 0xBFFF
        private byte[] SwitchableMemory;

        // RAM              0xC000 - 0xDFFF
        private byte[] Memory;


        // fix this later
        private byte[] Blarg;

        public Bus()
        {
            this.Blarg = new byte[0xFFFF + 1];
        }

        public byte this[int index]
        {
            get { return this.Blarg[index]; }
            set { this.Blarg[index] = value; }
        }

    }
}
