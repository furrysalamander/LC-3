using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

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
                    if ((nzp[0] && n) || (nzp[1] && z) || (nzp[2] && p) || (n && p && z))
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

        public static List<ushort> Assemble(string inString)
        {
            int programCounter = 0;
            int startAddress;
            //List<short> assembledProgram;
            List<ushort> machineCode = new List<ushort>();

            inString = inString.Replace("\r", "");
            inString = inString.ToUpper();
            List<string> parser = Regex.Replace(inString, ";.*", "").Split('\n').ToList();
            for (int i = 0; i < parser.Count(); i++)
            {
                if (parser[i] == "")
                {
                    parser.RemoveAt(i);
                }
            }
            List<List<string>> parsedData = new List<List<string>>();
            foreach (string value in parser)
            {
                parsedData.Add(value.Split().ToList());
            }

            SymTable symbolTable = new SymTable();

            for (int i = 0; i < parsedData.Count; i++)
            {
                //PASS ONE
                switch (parsedData[i][0])
                {
                    case ".ORIG":
                        startAddress = ConvertNumber(parsedData[i][1]);
                        break;
                    case ".END":
                        break;
                    case ".BLKW":
                        programCounter += Convert.ToInt32(parsedData[i][1]);
                        break;
                    case ".STRINGZ":
                        programCounter += parsedData[i][1].Length - 2;
                        break;
                    case "BRN":
                    case "BRNZ":
                    case "BRNZP":
                    case "BRZP":
                    case "BRZ":
                    case "BRP":
                    case "BRNP":
                    case ".FILL":
                    case "ADD":
                    case "AND":
                    case "BR":
                    case "JMP":
                    case "RET":
                    case "JSR":
                    case "JSRR":
                    case "LD":
                    case "LDI":
                    case "LDR":
                    case "LEA":
                    case "NOT":
                    case "RTI":
                    case "ST":
                    case "STI":
                    case "STR":
                    case "TRAP":
                    case "GETC":
                    case "OUT":
                    case "IN":
                    case "PUTS":
                    case "HALT":
                        programCounter++;
                        break;
                    default:
                        symbolTable.AddSymbol(parsedData[i][0], programCounter);
                        // May want to fix this part later, it's kinda sketchy.
                        if (parsedData[i].Count == 1)
                        {
                            parsedData.RemoveAt(i);
                            i--;
                        }
                        else parsedData[i].RemoveAt(0);
                        break;
                }
            }


            // PASS 2
            foreach (List<string> command in parsedData)
            {
                ushort outCommand = 0;
                switch (command[0])
                {
                    case ".ORIG":
                        //    startAddress = ConvertNumber(command[i + 1]);
                        break;
                    case ".END":
                        break;
                    case ".FILL":
                        outCommand = ConvertNumber(command[1]);
                        programCounter++;
                        break;
                    case ".BLKW":
                        break;
                    case ".STRINGZ":
                        break;
                    case "ADD":
                        outCommand = 0x1000;
                        outCommand |= (ushort)(ConvertRegister(command[1]) << 9);
                        outCommand |= (ushort)(ConvertRegister(command[2]) << 6);
                        if (command[3][0] == 'R')
                        {
                            outCommand |= (ushort)(ConvertRegister(command[3]));
                        }
                        else
                        {
                            outCommand |= 0b100000;
                            outCommand |= (ushort)(ConvertNumber(command[3]));
                        }
                        programCounter++;
                        break;
                    case "AND":
                        outCommand = 0x5000;
                        ushort potato = ConvertRegister(command[1]);
                        outCommand |= (ushort)(ConvertRegister(command[1]) << 9);
                        outCommand |= (ushort)(ConvertRegister(command[2]) << 6);
                        if (command[3][0] == 'R')
                        {
                            outCommand |= (ushort)(ConvertRegister(command[3]));
                        }
                        else
                        {
                            outCommand |= 0b100000;
                            outCommand |= (ushort)(ConvertNumber(command[3]));
                        }
                        programCounter++;
                        break;
                    case "BR": // THIS NEEDS TO BE FIXED - maybe?
                    case "BRN":
                    case "BRNZ":
                    case "BRNZP":
                    case "BRZP":
                    case "BRZ":
                    case "BRP":
                    case "BRNP":
                        outCommand = 0x0000;
                        programCounter++;
                        break;
                    case "JMP":
                        outCommand = 0xC000;
                        programCounter++;
                        break;
                    case "RET":
                        outCommand = 0b110000011100000;
                        programCounter++;
                        break;
                    case "JSR":
                        outCommand = 0x4000;
                        programCounter++;
                        break;
                    case "JSRR":
                        outCommand = 0x4000;
                        programCounter++;
                        break;
                    case "LD":
                        outCommand = 0x2000;
                        programCounter++;
                        break;
                    case "LDI":
                        outCommand = 0xA000;
                        programCounter++;
                        break;
                    case "LDR":
                        outCommand = 0x6000;
                        programCounter++;
                        break;
                    case "LEA":
                        outCommand = 0xE000;
                        programCounter++;
                        break;
                    case "NOT":
                        outCommand = 0x9000;
                        programCounter++;
                        break;
                    case "RTI":
                        outCommand = 0x8000;
                        programCounter++;
                        break;
                    case "ST":
                        outCommand = 0x3000;
                        programCounter++;
                        break;
                    case "STI":
                        outCommand = 0xB000;
                        programCounter++;
                        break;
                    case "STR":
                        outCommand = 0x7000;
                        programCounter++;
                        break;
                    case "TRAP":
                        outCommand = 0xF000;
                        outCommand |= ConvertNumber(command[1]);
                        programCounter++;
                        break;
                    case "GETC":
                        outCommand = 0xF020;
                        programCounter++;
                        break;
                    case "OUT":
                        outCommand = 0xF021;
                        programCounter++;
                        break;
                    case "IN":
                        outCommand = 0xF023;
                        programCounter++;
                        break;
                    case "PUTS":
                        outCommand = 0xF022;
                        programCounter++;
                        break;
                    case "HALT":
                        outCommand = 0xF025;
                        programCounter++;
                        break;
                    default:
                        //blablabla
                        break;
                }

                machineCode.Add(outCommand);
            }
            return machineCode;
        }

        private static ushort ConvertNumber(string inString)
        {
            switch (inString[0])
            {
                case '#':
                    return (ushort)Convert.ToInt16(inString.Substring(1));
                case 'B':
                    return Convert.ToUInt16(inString.Substring(1), 2);
                case 'X':
                    return Convert.ToUInt16(inString.Substring(1), 16);
            }
            // need to add a throw for try-catch here
            return 0;
        }

        private static ushort ConvertRegister(string inString) => Convert.ToUInt16(inString.Substring(1,1));


    }
    public class SymTable
    {
        private List<string> names;
        private List<int> addresses;

        public SymTable()
        {
            names = new List<string>();
            addresses = new List<int>();
        }

        public void AddSymbol(string name, int address)
        {
            names.Add(name);
            addresses.Add(address);
        }
        public int GetAddress(string inName)
        {
            for (int i = 0; i < names.Count; i++)
            {
                if (names[i] == inName)
                {
                    return addresses[i];
                }
            }
            throw new IndexOutOfRangeException();
        }
    }

}
