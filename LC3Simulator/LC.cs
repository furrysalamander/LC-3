using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LC3Simulator
{
    class LC
    {
        public short[] registers = new short[8];
        public short[] memory = new short[5000];
        public bool halt;
        public short programCounter;
        bool[] nzp = new bool[3];

        public int trapFlag;

        public LC()
        {
            programCounter = 0;
        }

        public void ParseMachineCode(short command)
        {
            programCounter += 1;

            short opcode = (short)((command >> 12) & 0b1111);
            int DR = (short)((command >> 9) & 0b111);
            int SR = (short)((command >> 6) & 0b111);
            int SR2 = (short)(command & 0b111);

            short imm5 = (short)(command << 11);
            imm5 = (short)(imm5 >> 11);
            short pc9 = (short)(command << 7);
            pc9 = (short)(pc9 >> 7);
            short o6 = (short)(command << 10);
            o6 = (short)(o6 >> 10);

            switch (opcode)
            {
                case 0b0001: // ADD
                    if ((command & 0b100000) == 0)
                    {
                        registers[DR] = (short)(registers[SR] + registers[SR2]);
                    }
                    else
                    {
                        registers[DR] = (short)(registers[SR] + imm5);
                    }
                    SetFlags(registers[DR]);
                    break;
                case 0b0101: // AND
                    if ((command & 0b100000) == 0)
                    {
                        registers[DR] = (short)(registers[SR] & registers[SR2]);
                    }
                    else
                    {
                        registers[DR] = (short)(registers[SR] & imm5);
                    }
                    SetFlags(registers[DR]);
                    break;
                case 0b0000: // BR
                    // decodes NZP flags
                    bool n = (DR & 0b100) != 0;
                    bool z = (DR & 0b010) != 0;
                    bool p = (DR & 0b001) != 0;
                    // checks to see if any of the NZP flags have been met before executing the branch
                    if ((nzp[0] && n )|| (nzp[1] && z) || (nzp[2] && p) || (n && p && z))
                    {
                        programCounter += pc9;
                    }
                    break;
                case 0b1100: // JMP and RET
                    programCounter = registers[SR];
                    break;
                case 0b0100: // JSR and JSRR
                    if ((DR & 0b100) == 1)
                    {
                        short o11 = (short)(command << 5);
                        programCounter += (short)(o11 >> 5);
                    }
                    else
                        programCounter = registers[SR];
                    break;
                case 0b0010: // LD
                    registers[DR] = memory[programCounter + pc9];
                    SetFlags(registers[DR]);
                    break;
                case 0b1010: // LDI
                    registers[DR] = memory[memory[programCounter + pc9]];
                    SetFlags(registers[DR]);
                    break;
                case 0b0110: // LDR
                    registers[DR] = memory[registers[SR] + o6];
                    SetFlags(registers[DR]);
                    break;
                case 0b1110: // LEA
                    registers[DR] = (short)(programCounter + pc9);
                    SetFlags(registers[DR]);
                    break;
                case 0b1001: // NOT
                    registers[DR] = (short)(~registers[SR]);
                    SetFlags(registers[DR]);
                    break;
                case 0b1000: // RTI
                    
                    // IMPLEMENT

                    break;
                case 0b0011: // ST
                    memory[programCounter + pc9] = registers[DR];
                    break;
                case 0b1011: // STI
                    memory[memory[programCounter + pc9]] = registers[DR];
                    break;
                case 0b0111: // STR
                    memory[registers[SR] + o6] = registers[DR];
                    break;
                case 0b1111: // TRAP
                    trapFlag = command & 0b11111111;
                    break;
                case 0b1101: // RESERVED

                    break;
            }
        }

        private void SetFlags(int value)
        {
            nzp[0] = value < 0;
            nzp[1] = value == 0;
            nzp[2] = value > 0;
        }

        public void ClearRegisters()
        {
            for (int i = 0; i < registers.Length; i++)
            {
                registers[i] = 0;
            }
            
        }

    }
}
