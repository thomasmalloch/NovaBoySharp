using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NovaBoySharp
{
    public static class Keywords
    {
        private static string _Instructions = 
@"
ADC
ADD
CP
DEC
INC
OR
SBC
SUB
XOR
ADD
DEC
INC
BIT
RES
SET
SWAP
RL
RLA
RLC
RLCA
RR
RR
RRA
RRC
RRCA
SLA
SRA
SRL
LD
CALL
JP
JR
RET
RETI
RST
ADD
DEC
INC
LD
POP
PUSH
CCF
CPL
DAA
DI
EI
HALT
NOP
SCF
STOP
LDH
";

        private static string _Reserved = @"A, B, C, D, E, H, L,BC, DE,HL,Z,NZ,C,NC";

        private static string[] _InstructionList = null;
        public static string[] Instructions()
        {
            if (_InstructionList != null)
                return _InstructionList;

            HashSet<string> set = new HashSet<string>();
            foreach (string instruction in _Instructions.Split(new[] { '\n', '\r', ',' }, StringSplitOptions.RemoveEmptyEntries))
            {
                if (set.Contains(instruction.ToLowerInvariant()))
                    continue;

                set.Add(instruction.ToLowerInvariant());
            }

            _InstructionList = set.ToArray();
            return _InstructionList;
        }

        private static string[] _ReservedList = null;
        public static string[] Reserved()
        {
            if (_ReservedList != null)
                return _ReservedList;

            HashSet<string> set = new HashSet<string>();
            foreach (string instruction in _Reserved.Split(new[] { '\n', '\r', ',' }, StringSplitOptions.RemoveEmptyEntries))
            {
                if (set.Contains(instruction.ToLowerInvariant()))
                    continue;

                set.Add(instruction.ToLowerInvariant());
            }

            _ReservedList = set.ToArray();
            return _ReservedList;
        }
    }
}
