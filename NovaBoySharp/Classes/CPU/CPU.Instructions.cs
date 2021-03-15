using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NovaBoySharp
{
    public partial class CPU
    {
        // Inst:		JR flag,r8
        // Encoding:	001X X000
        // Flags:		-
        // Desc:		Conditional jump to address relative to current position
        private void JumpRelative_Conditional(int op, int data)
        {
            int condition = (op & 0b00011000) >> 3;
            if ((condition == 0) && (this.Flags.Zero))
                return;
            if ((condition == 1) && (!this.Flags.Zero))
                return;
            if ((condition == 2) && (this.Flags.Carry))
                return;
            if ((condition == 3) && (!this.Flags.Carry))
                return;

            this.Registers.PC = (ushort)(this.Registers.PC + (ushort)data);
        }

        // Inst:			LD RP,d16
        // Encoding:		00XX 0001
        // Flags:			-
        // Desc:			Load register Pair with 16bit number			
        private void Load_RegisterPairWithWord(int op, int data)
        {
            int rp = (op & 0b00110000) >> 4;
            this.Registers.Pair[rp] = (ushort)data;
        }

        // Inst:			LD RP,A
        // Encoding:		00XX 0010
        // Flags:			-
        // Desc:			Load register Pair with Accumulator				
        private void Load_RegisterPairWithAccumulator(int op, int data)
        {
            // probably HL here
            int rp = (op & 0b00110000) >> 4;
            this.Registers.Pair[rp] = (ushort)this.Registers.HL;
        }

        // Inst:			LD R,d8	
        // Encoding:		00XX X110
        // Flags:			-
        // Desc:			Load register with 8bit number			
        private void Load_RegisterWithByte(int op, int data)
        {
            int r = (op & 0b00111000) >> 3;
            this.Registers.Single[r] = (byte)data;
        }

        // Inst:			LD A,RP
        // Encoding:		00XX 1010
        // Flags:			-
        // Desc:			Load Accumulator with register Pair				
        private void Load_AccumulatorWithRegisterPair(int op, int data)
        {
            // probably HL here
            int rp = (op & 0b00110000) >> 4;
            this.Registers.HL = this.Registers.Pair[rp];
        }

        // Inst:			LD r1,r2
        // Encoding:		01XX XYYY
        // Flags:			-
        // Desc:			Load register1 with register2		
        private void Load_RegisterWithRegister(int op, int data)
        {
            int r1 = (op & 0b00111000) >> 3;
            int r2 = (op & 0b00000111) >> 0;
            this.Registers.Single[r1] = this.Registers.Single[r2];
        }

        // Inst:			INC RP
        // Encoding:		00XX 0011
        // Flags:			-
        // Desc:			Increment register Pair
        //							
        private void Increment_RegisterPair(int op, int data)
        {
            int rp = (op & 0b00110000) >> 4;
            this.Registers.Pair[rp]++;
        }

        // Inst:			INC R
        // Encoding:		00XX X100	
        // Flags:			Z 0 H -
        // Desc:			Increment register				
        private void Increment_Register(int op, int data)
        {
            int r = (op & 0b00111000) >> 3;     
            this.Registers.Single[r]++;            
            this.Flags.Subtract = false;
            this.Flags.Zero = (this.Registers.Single[r] == 0);
        }

        // Inst:			DEC RP
        // Encoding:		00XX 1011
        // Flags:			-
        // Desc:			Decrement register Pair		
        private void Decrement_RegisterPair(int op, int data)
        {
            int rp = (op & 0b00110000) >> 4;
            this.Registers.Pair[rp]--;
        }

        // Inst:			DEC R
        // Encoding:		00XX X101
        // Flags:			Z 1 H -
        // Desc:			Decrement register				
        private void Decrement_Register(int op, int data)
        {
            int r = (op & 0b00111000) >> 3;
            this.Registers.Single[r]--;
            this.Flags.Subtract = true;
            this.Flags.Zero = (this.Registers.Single[r] == 0);
        }

        // Inst:			ADD HL,RP
        // Encoding:		00XX 1001
        // Flags:			- 0 H C
        // Desc:			Add HL with register Pair
        private void Add_HLWithRegisterPair(int op, int data)
        {
            int rp = (op & 0b00110000) >> 4;

            // process Flags
            this.Flags.Subtract = false;
            this.Flags.Carry = (((this.Registers.Pair[rp] & 0x8000) == 0x8000) && ((this.Registers.HL & 0x8000) == 0x8000));

            // add together
            this.Registers.HL += this.Registers.Pair[rp];
        }

        // Inst:			ADD A,R
        // Encoding:		1000 0XXX
        // Flags:			Z 0 H C
        // Desc:			Add register to Accumulator			
        private void Add_AccumulatorWithRegister(int op, int data)
        {
            int r = (op & 0x07);
            this.Flags.Carry = (((this.Registers.Accumulator & 0x80) == 0x80) && ((this.Registers.Single[r] & 0x80) == 0x80));
            this.Flags.Subtract = false;
            this.Registers.Accumulator += (r == 7) ? this.Registers.Accumulator : this.Registers.Single[r];
            this.Flags.Zero = (this.Registers.Accumulator == 0);
        }

        // Inst:			ADC A,R
        // Encoding:		1000 1XXX
        // Flags:			Z 0 H C
        // Desc:			Add register + Carry to Accumulator				
        private void AddCarry_AccumulatorWithRegister(int op, int data)
        {
            int r = (op & 0x07);
            byte sum = this.Registers.Single[r];
            if (this.Flags.Carry)
                sum++;

            this.Flags.Carry = (((sum & 0x80) == 0x80) && ((this.Registers.Accumulator & 0x80) == 0x80));
            this.Registers.Accumulator += sum;
            this.Flags.Zero = (this.Registers.Accumulator == 0);
            this.Flags.Subtract = false;
        }

        // Inst:			SUB A,R
        // Encoding:		1001 0XXX
        // Flags:			Z 1 H C
        // Desc:			Subtract register from Accumulator				
        private void Subtract_AccumulatorWithRegister(int op, int data)
        {
            int r = (op & 0x07);
            this.Flags.Carry = (this.Registers.Single[r] > this.Registers.Accumulator);
            this.Registers.Accumulator -= this.Registers.Single[r];
            this.Flags.Zero = (this.Registers.Accumulator == 0);
            this.Flags.Subtract = true;
        }

        // Inst:			SBC A,R	
        // Encoding:		1001 1XXX
        // Flags:			Z 1 H C
        // Desc:			Subtract register + Carry from Accumulator		
        private void SubtractCarry_AccumulatorWithRegister(int op, int data)
        {
            int r = (op & 0x07);
            byte sum = this.Registers.Single[r];
            if (this.Flags.Carry)
                sum++;

            this.Flags.Carry = (this.Registers.Single[r] > this.Registers.Accumulator);
            this.Registers.Accumulator -= sum;
            this.Flags.Zero = (this.Registers.Accumulator == 0);
            this.Flags.Subtract = true;
        }

        // Inst:			AND R
        // Encoding:		1010 0XXX
        // Flags:			Z 0 1 0
        // Desc:			And register				
        private void And_Register(int op, int data)
        {
            int r = (op & 0x07);
            this.Registers.Accumulator &= this.Registers.Single[r];
            this.Flags.Zero = (this.Registers.Accumulator == 0);
            this.Flags.Subtract = false;
            this.Flags.HalfCarry = true;
            this.Flags.Carry = false;

        }

        // Inst:			XOR R
        // Encoding:		1010 1XXX
        // Flags:			Z 0 0 0
        // Desc:			XOR register				
        private void XOR_Register(int op, int data)
        {
            int r = (op & 0x07);
            this.Registers.Accumulator ^= this.Registers.Single[r];
            this.Flags.Zero = (this.Registers.Accumulator == 0);
            this.Flags.Subtract = false;
            this.Flags.HalfCarry = false;
            this.Flags.Carry = false;
        }

        // Inst:			OR R	
        // Encoding:		1011 0XXX
        // Flags:			Z 0 0 0
        // Desc:			OR register		
        private void OR_Register(int op, int data)
        {
            int r = (op & 0x07);
            this.Registers.Accumulator |= this.Registers.Single[r];
            this.Flags.Zero = (this.Registers.Accumulator == 0);
            this.Flags.Subtract = false;
            this.Flags.HalfCarry = false;
            this.Flags.Carry = false;
        }

        // Inst:			CP R
        // Encoding:		1011 1XXX
        // Flags:			Z 1 H C
        // Desc:			Compare register			
        private void Compare_Register(int op, int data)
        {
            int r = (op & 0x07);
            this.Flags.Carry = (this.Registers.Single[r] > this.Registers.Accumulator);
            this.Flags.Zero = (this.Registers.Accumulator == 0);
            this.Flags.Subtract = true;
        }

        // Inst:			RET flag
        // Encoding:		110X X000
        // Flags:			-
        // Desc:			Conditional return (pop two bytes from stack & jump to address)				
        private void Return_Conditional(int op, int data)
        {
            int condition = (op & 0b00011000) >> 3;
            if ((condition == 0) && (this.Flags.Zero))
                return;
            if ((condition == 1) && (!this.Flags.Zero))
                return;
            if ((condition == 2) && (this.Flags.Carry))
                return;
            if ((condition == 3) && (!this.Flags.Carry))
                return;

            byte lsb = this.Bus[this.Registers.SP++];
            byte msb = this.Bus[this.Registers.SP++];
            ushort address = (ushort)((ushort)(msb << 4) | lsb);
            this.Registers.PC = address;
        }

        // Inst:			POP RP
        // Encoding:		11XX 0001
        // Flags:			-
        // Desc:			Pop two bytes from stack and save to register Pair				
        private void Pop_RegisterPair(int op, int data)
        {
            int rp = (op & 0b00110000) >> 4;
            byte lsb = this.Bus[this.Registers.SP++];
            byte msb = this.Bus[this.Registers.SP++];
            ushort value = (ushort)((ushort)(msb << 4) | lsb);
            if (rp == 3)
                this.Registers.AF = value;
            else
                this.Registers.Pair[rp] = value;            
        }

        // Inst:			PUSH RP
        // Encoding:		11XX 0101
        // Flags:			-
        // Desc:			Push register Pair to stack						
        private void Push_RegisterPair(int op, int data)
        {
            unchecked
            {
                int rp = (op & 0b00110000) >> 4;
                ushort s = (rp == 3) ? this.Registers.Pair[rp] : this.Registers.AF;
                byte lsb = (byte)((s & (ushort)0x00FF) >> 0);
                byte msb = (byte)((s & (ushort)0xFF00) >> 8);
                this.Bus[this.Registers.SP--] = msb;
                this.Bus[this.Registers.SP--] = lsb;
            }
        }

        // Inst:			JP flag
        // Encoding:		110X X010
        // Flags:			-
        // Desc:			Conditional Jump to address			
        private void Jump_Conditional(int op, int data)
        {
            int condition = (op & 0b0011000) >> 3;
            if ((condition == 0) && (this.Flags.Zero))
                return;
            if ((condition == 1) && (!this.Flags.Zero))
                return;
            if ((condition == 2) && (this.Flags.Carry))
                return;
            if ((condition == 3) && (!this.Flags.Carry))
                return;

            this.Registers.PC = (ushort)data;
        }

        // Inst:			CALL flag
        // Encoding:		110X X100
        // Flags:			-
        // Desc:			Conditional call (push address of next instruction onto stack and then jump to address)
        private void Call_Conditional(int op, int data)
        {
            int condition = (op & 0b0011000) >> 3;
            if ((condition == 0) && (this.Flags.Zero))
                return;
            if ((condition == 1) && (!this.Flags.Zero))
                return;
            if ((condition == 2) && (this.Flags.Carry))
                return;
            if ((condition == 3) && (!this.Flags.Carry))
                return;

            ushort nextInst = (ushort)(this.Registers.PC + 1);
            byte lsb = (byte)((nextInst & 0x00FF) >> 0);
            byte msb = (byte)((nextInst & 0xFF00) >> 8);
            this.Bus[this.Registers.SP--] = msb;
            this.Bus[this.Registers.SP--] = lsb;
            this.Registers.PC = (ushort)data;
        }

        // Inst:			RST	address
        // Encoding:		11XX X111
        // Flags:			-
        // Desc:			Push present address onto stack, jump to address
        private void RestartAddress(int op, int data)
        {
            int address = (op & 0b00111000) >> 3;
            throw new NotImplementedException();
        }

        // NOP				0000 0000
        private void NoOperation(int op, int data)
        {
        }

        // STOP				0001 0000
        private void Stop(int op, int data)
        {
            throw new NotImplementedException();
        }

        // HALT				0111 0110
        private void Halt(int op, int data)
        {
            // halt the CPU here
#warning not implemented, but handled somewhere else
        }

        // RLCA				0000 0111
        private void RotateLeft_Carry(int op, int data)
        {
            this.Flags.Carry = ((this.Registers.Accumulator & 0x80) == 0x80);
            this.Flags.Subtract = false;
            this.Flags.HalfCarry = false;

            this.Registers.Accumulator = (byte)(this.Registers.Accumulator << 1);
            this.Flags.Zero = (this.Registers.Accumulator == 0);
        }

        // RLA				0001 0111
        private void RotateLeft(int op, int data)
        {
            this.RotateLeft_Carry(op, data);
        }

        // RRCA				0000 1111
        private void RotateRight_Carry(int op, int data)
        {
            this.Flags.Carry = ((this.Registers.Accumulator & 0x01) == 0x01);
            this.Flags.Subtract = false;
            this.Flags.HalfCarry = false;

            this.Registers.Accumulator = (byte)(this.Registers.Accumulator >> 1);
            this.Flags.Zero = (this.Registers.Accumulator == 0);
        }

        // RRA				0001 1111
        private void RotateRight(int op, int data)
        {
            this.RotateRight_Carry(op, data);
        }

        // DDA				0011 0111
        private void DecimalAdjustAccumulator(int op, int data)
        {
            // this looks like it needs the half Carry flag working
            // fixes binary decimal jazz
            throw new NotImplementedException();            
        }

        // SCF				0100 0111
        private void SetCarryFlag(int op, int data)
        {
            this.Flags.Subtract = false;
            this.Flags.HalfCarry = false;
            this.Flags.Carry = true;
        }

        // CPL				0011 1111
        private void ComplementAccumulator(int op, int data)
        {
            unchecked
            {
                this.Registers.Accumulator = (byte)(~this.Registers.Accumulator);
                this.Flags.Subtract = true;
                this.Flags.HalfCarry = true;
            }
        }

        // CCF				0100 1111
        private void ComplementCarryFlag(int op, int data)
        {
            this.Flags.Subtract = false;
            this.Flags.HalfCarry = false;
            this.Flags.Carry = !this.Flags.Carry;
        }

        // RET				1100 1000
        // pop two bytes from stack and jump to that address
        private void Return(int op, int data)
        {
            byte lsb = this.Bus[this.Registers.SP++];
            byte msb = this.Bus[this.Registers.SP++];
            ushort address = (ushort)((ushort)(msb << 4) | lsb);
            this.Registers.PC = address;
        }

        // RETI				1101 1001
        // pop two bytes from stack, jump to address, and enable interripts
        private void Return_EnableInterrupt(int op, int data)
        {
            this.Return(op, data);
#warning EnableInterrupt not implemented 
            //EnableInterrupt(op, data);
        }

        // CALL a16			1100 1101	
        // 
        private void Call_Address(int op, int data)
        {
            unchecked
            {
                byte lsb = (byte)((this.Registers.PC & (ushort)0x00FF) >> 0);
                byte msb = (byte)((this.Registers.PC & (ushort)0xFF00) >> 8);
                this.Bus[this.Registers.SP--] = msb;
                this.Bus[this.Registers.SP--] = lsb;
                this.Registers.PC = (ushort)data;
            }
        }

        // LDH a8,A			1110 0000
        // put A into memory address $FF00 + d8
        private void Load_AddressWithAccumulator(int op, int data)
        {
            this.Bus[0xFF00 | (data & 0x00FF)] = this.Registers.Accumulator;
        }

        // LDH A,a8			1111 0000
        // put memory address $FF00 + d8 into A
        private void Load_AccumulatorWithAddress(int op, int data)
        {
            this.Registers.Accumulator = this.Bus[0xFF00 | (data & 0x00FF)];
        }

        // LD C,A			1110 0010
        // put A into address $FF00 + C
        private void Load_RegisterCWithAccumulator(int op, int data)
        {
            this.Bus[0xFF00 | this.Registers.C] = this.Registers.Accumulator;
        }

        // LD A,C			1111 0010
        // put address $FF00 + C into A
        private void Load_AccumulatorWithRegisterC(int op, int data)
        {
            this.Registers.Accumulator = this.Bus[0xFF00 | this.Registers.C];
        }

        // JR				0001 1000
        // add d8 to current address and jump to it
        private void Jump_Relative(int op, int data)
        {
            this.Registers.PC += (ushort)data;
        }

        // EI				1111 1011
        private void EnableInterrupt(int op, int data)
        {
            throw new NotImplementedException();
        }

        // DI				1111 0011
        private void DisableInterrupt(int op, int data)
        {
            throw new NotImplementedException();
        } 

        // Inst:			ADD A,d8
        // Encoding:		1000 0110
        // Flags:			Z 0 H C
        // Desc:			Add a byte to the accumumator
        private void Add_AccumulatorWithByte(int op, int data)
        {
            this.Flags.Carry = (((this.Registers.Accumulator & 0x80) == 0x80) && ((data & 0x80) == 0x80));
            this.Registers.Accumulator += (byte)data;
            this.Flags.Zero = (this.Registers.Accumulator == 0);
            this.Flags.Subtract = false;
        }

        // Inst:			SUB A,d8	
        // Encoding:		1101 0110
        // Flags:			Z 1 H C
        // Desc:			Subtract a byte to the accumumator
        private void Subtract_AccumulatorWithByte(int op, int data)
        {
            this.Flags.Carry = (data > this.Registers.Accumulator);
            this.Registers.Accumulator -= (byte)data;
            this.Flags.Zero = (this.Registers.Accumulator == 0);
            this.Flags.Subtract = true;
        }

        // Inst:			A,d8
        // Encoding:		1110 0110
        // Flags:			Z 0 1 0
        // Desc:			And accumulator with byte
        private void And_AccumulatorWithByte(int op, int data)
        {
            this.Registers.Accumulator &= (byte)data;
            this.Flags.Zero = (this.Registers.Accumulator == 0);
            this.Flags.Subtract = false;
            this.Flags.HalfCarry = true;
            this.Flags.Carry = false;
        }

        // Inst:			OR A,d8	
        // Encoding:		1111 0110
        // Flags:			Z 0 0 0
        // Desc:			OR accumulator with byte	
        private void Or_AccumulatorWithByte(int op, int data)
        {
            this.Registers.Accumulator |= (byte)data;
            this.Flags.Zero = (this.Registers.Accumulator == 0);
            this.Flags.Subtract = false;
            this.Flags.HalfCarry = false;
            this.Flags.Carry = false;
        }

        // ADC A,d8			1100 1110
        private void AddCarry_AccumulatorWithByte(int op, int data)
        {
            int carry = (this.Flags.Carry) ? 1 : 0;
            this.Flags.Carry = (((this.Registers.Accumulator & 0x80) == 0x80) && (((data + carry) & 0x80) == 0x80));
            this.Registers.Accumulator += (byte)(data + carry);
            this.Flags.Zero = (this.Registers.Accumulator == 0);
            this.Flags.Subtract = false;
        }

        // SBC A,d8			1101 1110
        private void SubtractCarry_AccumulatorWithByte(int op, int data)
        {
            int carry = (this.Flags.Carry) ? 1 : 0;
            this.Flags.Carry = ((data + carry) > this.Registers.Accumulator);
            this.Registers.Accumulator -= (byte)(data + carry);
            this.Flags.Zero = (this.Registers.Accumulator == 0);
            this.Flags.Subtract = true;
        } 

        // Inst:			XOR d8
        // Encoding:		1110 1110
        // Flags:			Z 0 0 0
        // Desc:			XOR accumulator with byte	
        private void XOR_AccumulatorWithByte(int op, int data)
        {
            this.Registers.Accumulator ^= (byte)data;
            this.Flags.Zero = (this.Registers.Accumulator == 0);
            this.Flags.Subtract = false;
            this.Flags.HalfCarry = false;
            this.Flags.Carry = false;
        }

        // CP d8			1111 1110
        private void Compare_AccumulatorWithByte(int op, int data)
        {
            this.Flags.Carry = (data > this.Registers.Accumulator);
            int result = this.Registers.Accumulator - (byte)data;
            this.Flags.Zero = (result == 0);
            this.Flags.Subtract = true;
        } 

        // PREFIX CB		1100 1011
        private void SwapNibbles(int op, int data)
        {
            int r = data & 0x0F;
            if (r > 7)
                throw new NotImplementedException();

            switch (r)
            {
                case 7:
                {
                    int lsn = ((this.Registers.Accumulator & 0x0F) >> 0);
                    int msn = ((this.Registers.Accumulator & 0xF0) >> 4);
                    this.Registers.Accumulator = (byte)(lsn << 4 | msn);
                    break;
                }
                case 6:
                {
                    int lsn = ((this.Registers.H & 0x0F) >> 0);
                    int msn = ((this.Registers.H & 0xF0) >> 4);
                    this.Registers.H = (byte)(lsn << 4 | msn);
                    lsn = ((this.Registers.L & 0x0F) >> 0);
                    msn = ((this.Registers.L & 0xF0) >> 4);
                    this.Registers.L = (byte)(lsn << 4 | msn);
                    break;
                }
                default:
                {
                    int lsn = ((this.Registers.Single[r] & 0x0F) >> 0);
                    int msn = ((this.Registers.Single[r] & 0xF0) >> 4);
                    this.Registers.Single[r] = (byte)(lsn << 4 | msn);
                    break;
                }
            }
        }

        // JP a16			1100 0011
        private void Jump_ToAddress(int op, int data)
        {
            this.Registers.PC = (ushort)data;
        } 

        // LD HL,SP+r8
        // F8
        private void Load_HLWithPointer(int op, int data)
        {
            this.Flags.Zero = false;
            this.Flags.Subtract = false;
            this.Flags.Carry = (((this.Registers.SP & 0x80) == 0x80) && ((data & 0x80) == 0x80));
            this.Registers.HL = this.Bus[this.Registers.SP + data];
        }

        // LD (a16),SP
        // 08
        private void Load_AddressWithStackPointer(int op, int data)
        {
            this.Bus[data + 0] = (byte)((this.Registers.SP & 0xFF00) >> 8);
            this.Bus[data + 1] = (byte)((this.Registers.SP & 0x00FF) >> 0);
        }

        // 0xE8, "ADD SP,r8"
        private void Add_StackPointerWithByte(int op, int data)
        {
            this.Flags.Carry = (((this.Registers.SP & 0x80) == 0x80) && ((data & 0x80) == 0x80));
            this.Registers.SP += (byte)data;
            this.Flags.Zero = false;
            this.Flags.Subtract = false;
        }

        // 0xE9, "JP (HL)"
        private void Jump_ToHL(int op, int data)
        {
            this.Registers.PC = this.Registers.HL;
        }

        // 0xF9, "LD SP,HL"
        private void Load_StackPointerWithHL(int op, int data)
        {
            this.Registers.SP = this.Registers.HL;
        }

        // 0xFA, "LD HL,(a16)"
        private void Load_HLWithAddress(int op, int data)
        {
            this.Registers.HL = (ushort)data;
        }
    }
}
