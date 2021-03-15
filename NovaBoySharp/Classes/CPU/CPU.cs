using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace NovaBoySharp
{
    public partial class CPU
    {
        #region Instruction Class

        private class Instruction
        {
            public delegate void ExecuteDelegate(int op, int data);

            public int Code { get; private set; }
            public string Name { get; private set; }
            public int Length { get; private set; }
            public readonly ExecuteDelegate Execute;

            internal Instruction(int code, string name, int length, ExecuteDelegate execute)
            {
                this.Code = code;
                this.Name = name;
                this.Length = length;
                this.Execute = execute;
            }

            public override string ToString()
            {
                return string.Format("OP: ${0:X2}\tCode: {1}\tLength: {2}", this.Code, this.Name, this.Length);
            }
        }

        #endregion

        #region Register Class

        private class CPURegisters
        {
            public readonly RegisterByte Single;
            public readonly RegisterPair Pair;
            public readonly CPU Parent;

            public CPURegisters(CPU parent)
            {
                this.Parent = parent;
                this.Single = new RegisterByte(this);
                this.Pair = new RegisterPair(this);
            }

            #region 8-bit Registers

            public byte Accumulator
            {
                get => this.Single[7];
                set => this.Single[7] = value;
            }

            public byte A
            {
                get => this.Accumulator;
                set => this.Accumulator = value;
            }

            public byte B
            {
                get { return this.Single[0]; }
                set { this.Single[0] = value; }
            }
            public byte C
            {
                get { return this.Single[1]; }
                set { this.Single[1] = value; }
            }
            public byte D
            {
                get { return this.Single[2]; }
                set { this.Single[2] = value; }
            }
            public byte E
            {
                get { return this.Single[3]; }
                set { this.Single[3] = value; }
            }
            public byte H
            {
                get { return this.Single[4]; }
                set { this.Single[4] = value; }
            }
            public byte L
            {
                get { return this.Single[5]; }
                set { this.Single[5] = value; }
            }

            #endregion

            #region 16-bit Registers

            public ushort AF
            {
                get => (ushort)((this.Accumulator << 8) | (this.Parent.Flags.Value));
                set
                {
                    unchecked
                    {
                        this.Parent.Flags.Value = (byte)((value & 0x00FF) >> 0);
                        this.Accumulator = (byte)((value & 0xFF00) >> 8);
                    }
                }
            }

            public ushort BC
            {
                get => (ushort)((this.B << 8) | this.C);
                set
                {
                    byte lsb = (byte)((value & 0x00FF) >> 0);
                    byte msb = (byte)((value & 0xFF00) >> 8);
                    this.B = msb;
                    this.C = lsb;
                }
            }

            public ushort DE
            {
                get => (ushort)((this.D << 8) | this.E);
                set
                {
                    byte lsb = (byte)((value & 0x00FF) >> 0);
                    byte msb = (byte)((value & 0xFF00) >> 8);
                    this.D = msb;
                    this.E = lsb;
                }
            }

            public ushort HL
            {
                get => (ushort)((this.H << 8) | this.L);
                set
                {
                    byte lsb = (byte)((value & 0x00FF) >> 0);
                    byte msb = (byte)((value & 0xFF00) >> 8);
                    this.H = msb;
                    this.L = lsb;
                }
            }

            public ushort SP { get; set; }

            public ushort PC { get; set; }

            #endregion

            #region 8-bit Register Class

            public class RegisterByte
            {
                private readonly byte[] Registers;
                private CPURegisters Parent;

                internal RegisterByte(CPURegisters parent)
                {
                    this.Registers = new byte[8];
                    this.Parent = parent;
                }

                public byte this[int index]
                {
                    get
                    {
                        switch (index)
                        {
                            case 6:
                                return this.Parent.Parent.Bus[this.Parent.HL];
                            default:
                                return this.Registers[index];
                        }
                    }
                    set
                    {
                        switch (index)
                        {
                            case 6:
                                this.Parent.Parent.Bus[this.Parent.HL] = value;
                                break;
                            default:
                                this.Registers[index] = value;
                                break;
                        }
                    }
                }
            }
            
            #endregion

            #region 16bit Register Class

            // 16-bit register class
            public class RegisterPair
            {
                private CPURegisters Parent;

                internal RegisterPair(CPURegisters parent)
                {
                    this.Parent = parent;
                }

                public ushort this[int index]
                {
                    get
                    {
                        switch (index)
                        {
                            case 0:
                                return this.Parent.BC;
                            case 1:
                                return this.Parent.DE;
                            case 2:
                                return this.Parent.HL;
                            case 3:
                                return this.Parent.SP;
                            case 4:
                                return this.Parent.PC;
                        }

                        return 0;
                    }
                    set
                    {
                        switch (index)
                        {
                            case 0:
                                this.Parent.BC = value;
                                break;
                            case 1:
                                this.Parent.DE = value;
                                break;
                            case 2:
                                this.Parent.HL = value;
                                break;
                            case 3:
                                this.Parent.SP = value;
                                break;
                            case 4:
                                this.Parent.PC = value;
                                break;
                        }
                    }
                }
            }

            #endregion
        }

        #endregion

        #region Flags Class

        private class CPUFlags
        {
            private const byte CarryFlag = 0x10;
            private const byte HalfCarryFlag = 0x20;
            private const byte SubtractFlag = 0x40;
            private const byte ZeroFlag = 0x80;

            public byte Value;

            public bool Carry
            {
                get
                {
                    if ((this.Value & CarryFlag) == CarryFlag)
                        return true;

                    return false;
                }

                set
                {
                    if (value)
                    {
                        this.Value |= CarryFlag;
                    }
                    else
                    {
                        unchecked
                        {
                            this.Value &= (byte)(~CarryFlag);
                        }
                    }
                }
            }

            public bool HalfCarry
            {
                get
                {
                    if ((this.Value & HalfCarryFlag) == HalfCarryFlag)
                        return true;

                    return false;
                }

                set
                {
                    if (value)
                    {
                        this.Value |= HalfCarryFlag;
                    }
                    else
                    {
                        unchecked
                        {
                            this.Value &= (byte)(~HalfCarryFlag);
                        }
                    }
                }
            }

            public bool Subtract
            {
                get
                {
                    if ((this.Value & SubtractFlag) == SubtractFlag)
                        return true;

                    return false;
                }

                set
                {
                    if (value)
                    {
                        this.Value |= SubtractFlag;
                    }
                    else
                    {
                        unchecked
                        {
                            this.Value &= (byte)(~SubtractFlag);
                        }
                    }
                }
            }

            public bool Zero
            {
                get
                {
                    if ((this.Value & ZeroFlag) == ZeroFlag)
                        return true;

                    return false;
                }

                set
                {
                    if (value)
                    {
                        this.Value |= ZeroFlag;
                    }
                    else
                    {
                        unchecked
                        {
                            this.Value &= (byte)(~ZeroFlag);
                        }
                    }
                }
            }
        }

        #endregion

        // Variables
        private CPUFlags Flags = new CPUFlags();
        private CPURegisters Registers;
        private readonly byte[] Bus;
        private readonly Instruction[] Instructions;
        private const string Format =
@"A: ${0:X2} ({0,3}) F: ${1:X2} ({1,3})
B: ${2:X2} ({2,3}) C: ${3:X2} ({3,3})
D: ${4:X2} ({4,3}) E: ${5:X2} ({5,3})
H: ${6:X2} ({6,3}) L: ${7:X2} ({7,3})
Z: {8} N: {9} H: {10} C: {11}
BC: ${12:X4} ({12,5})
DE: ${13:X4} ({13,5})
HL: ${14:X4} ({14,5})
SP: ${15:X4} ({15,5})
PC: ${16:X4} ({16,5})";

        // Delegates and Events
        public delegate void CodeRunDelegate(int code, int? data, string name, string status);
        public delegate void HaltDelegate();

        public event CodeRunDelegate InstructionExecuted;
        public event HaltDelegate CPUHalt;

        public CPU(byte[] bus)
        {
            //this.Registers.PC = 0x100;            
            this.Bus = bus;
            this.Registers = new CPURegisters(this);
            this.Instructions = new[]
            {
#region Instruction Setup
                new Instruction(0x00, "NOP", 1, this.NoOperation),
                new Instruction(0x01, "LD BC,d16", 3, this.Load_RegisterPairWithWord),
                new Instruction(0x02, "LD (BC),A", 1, this.Load_RegisterPairWithAccumulator),
                new Instruction(0x03, "INC BC", 1, this.Increment_RegisterPair),
                new Instruction(0x04, "INC B", 1, this.Increment_Register),
                new Instruction(0x05, "DEC B", 1, this.Decrement_Register),
                new Instruction(0x06, "LD B,d8", 2, this.Load_RegisterWithByte),
                new Instruction(0x07, "RLCA", 1, this.RotateLeft_Carry),
                new Instruction(0x08, "LD (a16),SP", 3, this.Load_AddressWithStackPointer),
                new Instruction(0x09, "ADD HL,BC", 1, this.Add_HLWithRegisterPair),
                new Instruction(0x0A, "LD A,(BC)", 1, this.Load_AccumulatorWithRegisterPair),
                new Instruction(0x0B, "DEC BC", 1, this.Decrement_RegisterPair),
                new Instruction(0x0C, "INC C", 1, this.Increment_Register),
                new Instruction(0x0D, "DEC C", 1, this.Decrement_Register),
                new Instruction(0x0E, "LD C,d8", 2, this.Load_RegisterWithByte),
                new Instruction(0x0F, "RRCA", 1, this.RotateRight_Carry),

                new Instruction(0x10, "STOP 0", 2, this.Stop),
                new Instruction(0x11, "LD DE,d16", 3, this.Load_RegisterPairWithWord),
                new Instruction(0x12, "LD (DE),A", 1, this.Load_RegisterPairWithAccumulator),
                new Instruction(0x13, "INC DE", 1, this.Increment_RegisterPair),
                new Instruction(0x14, "INC D", 1, this.Increment_Register),
                new Instruction(0x15, "DEC D", 1, this.Decrement_Register),
                new Instruction(0x16, "LD D,d8", 2, this.Load_RegisterWithByte),
                new Instruction(0x17, "RLA", 1, this.RotateLeft),
                new Instruction(0x18, "JR r8", 2, this.Jump_Relative),
                new Instruction(0x19, "ADD HL,DE", 1, this.Add_HLWithRegisterPair),
                new Instruction(0x1A, "LD A,(DE)", 1, this.Load_AccumulatorWithRegisterPair),
                new Instruction(0x1B, "DEC DE", 1, this.Decrement_RegisterPair),
                new Instruction(0x1C, "INC E", 1, this.Increment_Register),
                new Instruction(0x1D, "DEC E", 1, this.Decrement_Register),
                new Instruction(0x1E, "LD E,d8", 2, this.Load_RegisterWithByte),
                new Instruction(0x1F, "RRA", 1, this.RotateRight),

                new Instruction(0x20, "JR NZ,r8", 2, this.JumpRelative_Conditional),
                new Instruction(0x21, "LD HL,d16", 3, this.Load_RegisterPairWithWord),
                new Instruction(0x22, "LD (HL+),A", 1, this.Load_RegisterPairWithAccumulator),
                new Instruction(0x23, "INC HL", 1, this.Increment_RegisterPair),
                new Instruction(0x24, "INC H", 1, this.Increment_Register),
                new Instruction(0x25, "DEC H", 1, this.Decrement_Register),
                new Instruction(0x26, "LD H,d8", 2, this.Load_RegisterWithByte),
                new Instruction(0x27, "DAA", 1, this.DecimalAdjustAccumulator),
                new Instruction(0x28, "JR Z,r8", 2, this.JumpRelative_Conditional),
                new Instruction(0x29, "ADD HL,HL", 1, this.Add_HLWithRegisterPair),
                new Instruction(0x2A, "LD A,(HL+)", 1, this.Load_AccumulatorWithRegisterPair),
                new Instruction(0x2B, "DEC HL", 1, this.Decrement_RegisterPair),
                new Instruction(0x2C, "INC L", 1, this.Increment_Register),
                new Instruction(0x2D, "DEC L", 1, this.Decrement_Register),
                new Instruction(0x2E, "LD L,d8", 2, this.Load_RegisterWithByte),
                new Instruction(0x2F, "CPL", 1, this.ComplementAccumulator),

                new Instruction(0x30, "JR NC,r8", 2, this.JumpRelative_Conditional),
                new Instruction(0x31, "LD SP,d16", 3, this.Load_RegisterPairWithWord),
                new Instruction(0x32, "LD (HL-),A", 1, this.Load_RegisterPairWithAccumulator),
                new Instruction(0x33, "INC SP", 1, this.Increment_RegisterPair),
                new Instruction(0x34, "INC (HL)", 1, this.Increment_Register),
                new Instruction(0x35, "DEC (HL)", 1, this.Decrement_Register),
                new Instruction(0x36, "LD (HL),d8", 2, this.Load_RegisterWithByte),
                new Instruction(0x37, "SCF", 1, this.SetCarryFlag),
                new Instruction(0x38, "JR C,r8", 2, this.JumpRelative_Conditional),
                new Instruction(0x39, "ADD HL,SP", 1, this.Add_HLWithRegisterPair),
                new Instruction(0x3A, "LD A,(HL-)", 1, this.Load_AccumulatorWithRegisterPair),
                new Instruction(0x3B, "DEC SP", 1, this.Decrement_RegisterPair),
                new Instruction(0x3C, "INC A", 1, this.Increment_Register),
                new Instruction(0x3D, "DEC A", 1, this.Decrement_Register),
                new Instruction(0x3E, "LD A,d8", 2, this.Load_RegisterWithByte),
                new Instruction(0x3F, "CCF", 1, this.ComplementCarryFlag),

                new Instruction(0x40, "LD B,B", 1, this.Load_RegisterWithRegister),
                new Instruction(0x41, "LD B,C", 1, this.Load_RegisterWithRegister),
                new Instruction(0x42, "LD B,D", 1, this.Load_RegisterWithRegister),
                new Instruction(0x43, "LD B,E", 1, this.Load_RegisterWithRegister),
                new Instruction(0x44, "LD B,H", 1, this.Load_RegisterWithRegister),
                new Instruction(0x45, "LD B,L", 1, this.Load_RegisterWithRegister),
                new Instruction(0x46, "LD B,(HL)", 1, this.Load_RegisterWithRegister),
                new Instruction(0x47, "LD B,A", 1, this.Load_RegisterWithRegister),
                new Instruction(0x48, "LD C,B", 1, this.Load_RegisterWithRegister),
                new Instruction(0x49, "LD C,C", 1, this.Load_RegisterWithRegister),
                new Instruction(0x4A, "LD C,D", 1, this.Load_RegisterWithRegister),
                new Instruction(0x4B, "LD C,E", 1, this.Load_RegisterWithRegister),
                new Instruction(0x4C, "LD C,H", 1, this.Load_RegisterWithRegister),
                new Instruction(0x4D, "LD C,L", 1, this.Load_RegisterWithRegister),
                new Instruction(0x4E, "LD C,(HL)", 1, this.Load_RegisterWithRegister),
                new Instruction(0x4F, "LD C,A", 1, this.Load_RegisterWithRegister),

                new Instruction(0x50, "LD D,B", 1, this.Load_RegisterWithRegister),
                new Instruction(0x51, "LD D,C", 1, this.Load_RegisterWithRegister),
                new Instruction(0x52, "LD D,D", 1, this.Load_RegisterWithRegister),
                new Instruction(0x53, "LD D,E", 1, this.Load_RegisterWithRegister),
                new Instruction(0x54, "LD D,H", 1, this.Load_RegisterWithRegister),
                new Instruction(0x55, "LD D,L", 1, this.Load_RegisterWithRegister),
                new Instruction(0x56, "LD D,(HL)", 1, this.Load_RegisterWithRegister), 
                new Instruction(0x57, "LD D,A", 1, this.Load_RegisterWithRegister),
                new Instruction(0x58, "LD E,B", 1, this.Load_RegisterWithRegister),
                new Instruction(0x59, "LD E,C", 1, this.Load_RegisterWithRegister),
                new Instruction(0x5A, "LD E,D", 1, this.Load_RegisterWithRegister),
                new Instruction(0x5B, "LD E,E", 1, this.Load_RegisterWithRegister),
                new Instruction(0x5C, "LD E,H", 1, this.Load_RegisterWithRegister),
                new Instruction(0x5D, "LD E,L", 1, this.Load_RegisterWithRegister),
                new Instruction(0x5E, "LD E,(HL)", 1, this.Load_RegisterWithRegister),
                new Instruction(0x5F, "LD E,A", 1, this.Load_RegisterWithRegister),

                new Instruction(0x60, "LD H,B", 1, this.Load_RegisterWithRegister),
                new Instruction(0x61, "LD H,C", 1, this.Load_RegisterWithRegister),
                new Instruction(0x62, "LD H,D", 1, this.Load_RegisterWithRegister),
                new Instruction(0x63, "LD H,E", 1, this.Load_RegisterWithRegister),
                new Instruction(0x64, "LD H,H", 1, this.Load_RegisterWithRegister),
                new Instruction(0x65, "LD H,L", 1, this.Load_RegisterWithRegister),
                new Instruction(0x66, "LD H,(HL)", 1, this.Load_RegisterWithRegister),
                new Instruction(0x67, "LD H,A", 1, this.Load_RegisterWithRegister),
                new Instruction(0x68, "LD L,B", 1, this.Load_RegisterWithRegister),
                new Instruction(0x69, "LD L,C", 1, this.Load_RegisterWithRegister),
                new Instruction(0x6A, "LD L,D", 1, this.Load_RegisterWithRegister),
                new Instruction(0x6B, "LD L,E", 1, this.Load_RegisterWithRegister),
                new Instruction(0x6C, "LD L,H", 1, this.Load_RegisterWithRegister),
                new Instruction(0x6D, "LD L,L", 1, this.Load_RegisterWithRegister),
                new Instruction(0x6E, "LD L,(HL)", 1, this.Load_RegisterWithRegister),
                new Instruction(0x6F, "LD L,A", 1, this.Load_RegisterWithRegister),

                new Instruction(0x70, "LD (HL),B", 1, this.Load_RegisterWithRegister),
                new Instruction(0x71, "LD (HL),C", 1, this.Load_RegisterWithRegister),
                new Instruction(0x72, "LD (HL),D", 1, this.Load_RegisterWithRegister),
                new Instruction(0x73, "LD (HL),E", 1, this.Load_RegisterWithRegister),
                new Instruction(0x74, "LD (HL),H", 1, this.Load_RegisterWithRegister),
                new Instruction(0x75, "LD (HL),L", 1, this.Load_RegisterWithRegister),
                new Instruction(0x76, "HALT", 1, this.Halt),
                new Instruction(0x77, "LD (HL),A", 1, this.Load_RegisterWithRegister),
                new Instruction(0x78, "LD A,B", 1, this.Load_RegisterWithRegister),
                new Instruction(0x79, "LD A,C", 1, this.Load_RegisterWithRegister),
                new Instruction(0x7A, "LD A,D", 1, this.Load_RegisterWithRegister),
                new Instruction(0x7B, "LD A,E", 1, this.Load_RegisterWithRegister),
                new Instruction(0x7C, "LD A,H", 1, this.Load_RegisterWithRegister),
                new Instruction(0x7D, "LD A,L", 1, this.Load_RegisterWithRegister),
                new Instruction(0x7E, "LD A,(HL)", 1, this.Load_RegisterWithRegister),
                new Instruction(0x7F, "LD A,A", 1, this.Load_RegisterWithRegister),

                new Instruction(0x80, "ADD A,B", 1, this.Add_AccumulatorWithRegister),
                new Instruction(0x81, "ADD A,C", 1, this.Add_AccumulatorWithRegister),
                new Instruction(0x82, "ADD A,D", 1, this.Add_AccumulatorWithRegister),
                new Instruction(0x83, "ADD A,E", 1, this.Add_AccumulatorWithRegister),
                new Instruction(0x84, "ADD A,H", 1, this.Add_AccumulatorWithRegister),
                new Instruction(0x85, "ADD A,L", 1, this.Add_AccumulatorWithRegister),
                new Instruction(0x86, "ADD A,(HL)", 1, this.Add_AccumulatorWithRegister),
                new Instruction(0x87, "ADD A,A", 1, this.Add_AccumulatorWithRegister),
                new Instruction(0x88, "ADC A,B", 1, this.AddCarry_AccumulatorWithRegister),
                new Instruction(0x89, "ADC A,C", 1, this.AddCarry_AccumulatorWithRegister),
                new Instruction(0x8A, "ADC A,D", 1, this.AddCarry_AccumulatorWithRegister),
                new Instruction(0x8B, "ADC A,E", 1, this.AddCarry_AccumulatorWithRegister),
                new Instruction(0x8C, "ADC A,H", 1, this.AddCarry_AccumulatorWithRegister),
                new Instruction(0x8D, "ADC A,L", 1, this.AddCarry_AccumulatorWithRegister),
                new Instruction(0x8E, "ADC A,(HL)", 1, this.AddCarry_AccumulatorWithRegister),
                new Instruction(0x8F, "ADC A,A", 1, this.AddCarry_AccumulatorWithRegister),

                new Instruction(0x90, "SUB B", 1, this.Subtract_AccumulatorWithRegister),
                new Instruction(0x91, "SUB C", 1, this.Subtract_AccumulatorWithRegister),
                new Instruction(0x92, "SUB D", 1, this.Subtract_AccumulatorWithRegister),
                new Instruction(0x93, "SUB E", 1, this.Subtract_AccumulatorWithRegister),
                new Instruction(0x94, "SUB H", 1, this.Subtract_AccumulatorWithRegister),
                new Instruction(0x95, "SUB L", 1, this.Subtract_AccumulatorWithRegister),
                new Instruction(0x96, "SUB (HL)", 1, this.Subtract_AccumulatorWithRegister),
                new Instruction(0x97, "SUB A", 1, this.Subtract_AccumulatorWithRegister),
                new Instruction(0x98, "SBC A,B", 1, this.SubtractCarry_AccumulatorWithRegister),
                new Instruction(0x99, "SBC A,C", 1, this.SubtractCarry_AccumulatorWithRegister),
                new Instruction(0x9A, "SBC A,D", 1, this.SubtractCarry_AccumulatorWithRegister),
                new Instruction(0x9B, "SBC A,E", 1, this.SubtractCarry_AccumulatorWithRegister),
                new Instruction(0x9C, "SBC A,H", 1, this.SubtractCarry_AccumulatorWithRegister),
                new Instruction(0x9D, "SBC A,L", 1, this.SubtractCarry_AccumulatorWithRegister),
                new Instruction(0x9E, "SBC A,(HL)", 1, this.SubtractCarry_AccumulatorWithRegister),
                new Instruction(0x9F, "SBC A,A", 1, this.SubtractCarry_AccumulatorWithRegister),

                new Instruction(0xA0, "AND B", 1, this.And_Register),
                new Instruction(0xA1, "AND C", 1, this.And_Register),
                new Instruction(0xA2, "AND D", 1, this.And_Register),
                new Instruction(0xA3, "AND E", 1, this.And_Register),
                new Instruction(0xA4, "AND H", 1, this.And_Register),
                new Instruction(0xA5, "AND L", 1, this.And_Register),
                new Instruction(0xA6, "AND (HL)", 1, this.And_Register),
                new Instruction(0xA7, "AND A", 1, this.And_Register),
                new Instruction(0xA8, "XOR B", 1, this.XOR_Register),
                new Instruction(0xA9, "XOR C", 1, this.XOR_Register),
                new Instruction(0xAA, "XOR D", 1, this.XOR_Register),
                new Instruction(0xAB, "XOR E", 1, this.XOR_Register),
                new Instruction(0xAC, "XOR H", 1, this.XOR_Register),
                new Instruction(0xAD, "XOR L", 1, this.XOR_Register),
                new Instruction(0xAE, "XOR (HL)", 1, this.XOR_Register),
                new Instruction(0xAF, "XOR A", 1, this.XOR_Register),

                new Instruction(0xB0, "OR B", 1, this.OR_Register),
                new Instruction(0xB1, "OR C", 1, this.OR_Register),
                new Instruction(0xB2, "OR D", 1, this.OR_Register),
                new Instruction(0xB3, "OR E", 1, this.OR_Register),
                new Instruction(0xB4, "OR H", 1, this.OR_Register),
                new Instruction(0xB5, "OR L", 1, this.OR_Register),
                new Instruction(0xB6, "OR (HL)", 1, this.OR_Register),
                new Instruction(0xB7, "OR A", 1, this.OR_Register),
                new Instruction(0xB8, "CP B", 1, this.Compare_Register),
                new Instruction(0xB9, "CP C", 1, this.Compare_Register),
                new Instruction(0xBA, "CP D", 1, this.Compare_Register),
                new Instruction(0xBB, "CP E", 1, this.Compare_Register),
                new Instruction(0xBC, "CP H", 1, this.Compare_Register),
                new Instruction(0xBD, "CP L", 1, this.Compare_Register),
                new Instruction(0xBE, "CP (HL)", 1, this.Compare_Register),
                new Instruction(0xBF, "CP A", 1, this.Compare_Register),

                new Instruction(0xC0, "RET NZ", 1, this.Return_Conditional),
                new Instruction(0xC1, "POP BC", 1, this.Pop_RegisterPair),
                new Instruction(0xC2, "JP NZ,a16", 3, this.Jump_Conditional),
                new Instruction(0xC3, "JP a16", 3, this.Jump_ToAddress),
                new Instruction(0xC4, "CALL NZ,a16", 3, this.Call_Conditional),
                new Instruction(0xC5, "PUSH BC", 1, this.Push_RegisterPair),
                new Instruction(0xC6, "ADD A,d8", 2, this.Add_AccumulatorWithByte),
                new Instruction(0xC7, "RST 00H", 1, this.RestartAddress),
                new Instruction(0xC8, "RET Z", 1, this.Return_Conditional),
                new Instruction(0xC9, "RET", 1, this.Return),
                new Instruction(0xCA, "JP Z,a16", 3, this.Jump_Conditional),
                new Instruction(0xCB, "PREFIX CB", 1, this.SwapNibbles),
                new Instruction(0xCC, "CALL Z,a16", 3, this.Call_Conditional),
                new Instruction(0xCD, "CALL a16", 3, this.Call_Address),
                new Instruction(0xCE, "ADC A,d8", 2, this.AddCarry_AccumulatorWithByte),
                new Instruction(0xCF, "RST 08H", 1, this.RestartAddress),

                new Instruction(0xD0, "RET NC", 1, this.Return_Conditional),
                new Instruction(0xD1, "POP DE", 1, this.Pop_RegisterPair),
                new Instruction(0xD2, "JP NC,a16", 3, this.Jump_Conditional),
                new Instruction(0x00, "NOP", 1, this.NoOperation),
                new Instruction(0xD4, "CALL NC,a16", 3, this.Call_Conditional),
                new Instruction(0xD5, "PUSH DE", 1, this.Push_RegisterPair),
                new Instruction(0xD6, "SUB d8", 2, this.Subtract_AccumulatorWithByte),
                new Instruction(0xD7, "RST 10H", 1, this.RestartAddress),
                new Instruction(0xD8, "RET C", 1, this.Return_Conditional),
                new Instruction(0xD9, "RETI", 1, this.Return_EnableInterrupt),
                new Instruction(0xDA, "JP C,a16", 3, this.Jump_Conditional),
                new Instruction(0x00, "NOP", 1, this.NoOperation),
                new Instruction(0xDC, "CALL C,a16", 3, this.Call_Conditional),
                new Instruction(0x00, "NOP", 1, this.NoOperation),
                new Instruction(0xDE, "SBC A,d8", 2, this.SubtractCarry_AccumulatorWithByte),
                new Instruction(0xDF, "RST 18H", 1, this.RestartAddress),

                new Instruction(0xE0, "LDH (a8),A", 2, this.Load_AddressWithAccumulator),
                new Instruction(0xE1, "POP HL", 1, this.Pop_RegisterPair),
                new Instruction(0xE2, "LD (C),A", 2, this.Load_RegisterCWithAccumulator),
                new Instruction(0x00, "NOP", 1, this.NoOperation),
                new Instruction(0x00, "NOP", 1, this.NoOperation),
                new Instruction(0xE5, "PUSH HL", 1, this.Push_RegisterPair),
                new Instruction(0xE6, "AND d8", 2, this.And_AccumulatorWithByte),
                new Instruction(0xE7, "RST 20H", 1, this.RestartAddress),
                new Instruction(0xE8, "ADD SP,r8", 2, this.Add_StackPointerWithByte),
                new Instruction(0xE9, "JP (HL)", 1, this.Jump_ToHL),
                new Instruction(0xEA, "LD (a16),A", 3, this.Load_AddressWithAccumulator),
                new Instruction(0x00, "NOP", 1, this.NoOperation),
                new Instruction(0x00, "NOP", 1, this.NoOperation),
                new Instruction(0x00, "NOP", 1, this.NoOperation),
                new Instruction(0xEE, "XOR d8", 2, this.XOR_AccumulatorWithByte),
                new Instruction(0xEF, "RST 28H", 1, this.RestartAddress),

                new Instruction(0xF0, "LDH A,(a8)", 2, this.Load_AccumulatorWithAddress),
                new Instruction(0xF1, "POP AF", 1, this.Pop_RegisterPair),
                new Instruction(0xF2, "LD A,(C)", 2, this.Load_AccumulatorWithRegisterC),
                new Instruction(0xF3, "DI", 1, this.DisableInterrupt),
                new Instruction(0x00, "NOP", 1, this.NoOperation),
                new Instruction(0xF5, "PUSH AF", 1, this.Push_RegisterPair),
                new Instruction(0xF6, "OR d8", 2, this.Or_AccumulatorWithByte),
                new Instruction(0xF7, "RST 30H", 1, this.RestartAddress),
                new Instruction(0xF8, "LD HL,SP+r8", 2, this.Load_HLWithPointer),
                new Instruction(0xF9, "LD SP,HL", 1, this.Load_StackPointerWithHL),
                new Instruction(0xFA, "LD HL,(a16)", 3, this.Load_HLWithAddress),
                new Instruction(0xFB, "EI", 1, this.EnableInterrupt),
                new Instruction(0xFE, "CP d8", 2, this.Compare_AccumulatorWithByte),
                new Instruction(0x00, "NOP", 1, this.NoOperation),
                new Instruction(0x00, "NOP", 1, this.NoOperation),
                new Instruction(0xFF, "RST 38H", 1, this.RestartAddress),
#endregion
            };
        }

        // Methods
        public int Run()
        {
            byte op = this.Bus[this.Registers.PC++];
            if (op == 0x76)            
                this.CPUHalt?.Invoke();

            Instruction instruction = this.Instructions[op];
            int data = 0;
            for (int i = 0; i < instruction.Length - 1; i++)
            {
                byte b = this.Bus[this.Registers.PC++];
                data |= (b << (8 * i));
            }

            try
            {
                instruction.Execute(op, data);
                int? zoop = null;
                if (instruction.Length > 1)
                    zoop = data;

                this.InstructionExecuted?.Invoke(
                    op,
                    zoop,
                    instruction.Name,
                    this.ToString());
            } catch (NotImplementedException ex)
            {
                this.InstructionExecuted?.Invoke(
                    op,
                    null,
                    instruction.Name + " NOT IMPLEMENTED",
                    this.ToString());
            }

#warning return the length of the executed instruction in cycles here
            return 0;
        }

        public override string ToString()
        {
            return string.Format(Format,
                this.Registers.A, this.Flags.Value,
                this.Registers.B, this.Registers.C,
                this.Registers.D, this.Registers.E,
                this.Registers.H, this.Registers.L,
                Convert.ToInt32(this.Flags.Zero), Convert.ToInt32(this.Flags.Subtract), Convert.ToInt32(this.Flags.HalfCarry), Convert.ToInt32(this.Flags.Carry),
                this.Registers.BC,
                this.Registers.DE,
                this.Registers.HL,
                this.Registers.SP,
                this.Registers.PC);
        }
    }
}



