using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Text.RegularExpressions;

namespace LC3Simulator
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        LC computer = new LC();
        bool executingFlag;

        public MainWindow()
        {
            InitializeComponent();

        }

        private void Execute_Click(object sender, RoutedEventArgs e)
        {
            executingFlag = true;
            loadMachineCode();
            ExecuteCode();
        }

        private void ExecuteCode()
        {
            while (!computer.halt && (computer.programCounter < computer.memory.Length))
            {
                computer.ParseMachineCode(computer.memory[computer.programCounter]);
                TrapFlags();
                //prgCounter.Text = Convert.ToString(computer.programCounter);
            }
            //consoleOut.Text = consoleText;
            updateRegText();
            if (computer.programCounter == computer.memory.Length - 1) consoleOut.Text += " End of Memory Reached!";
        }

        private void updateRegText()
        {
            registerZeroBox.Text = Convert.ToString(computer.registers[0]);
            registerOneBox.Text = Convert.ToString(computer.registers[1]);
            registerTwoBox.Text = Convert.ToString(computer.registers[2]);
            registerThreeBox.Text = Convert.ToString(computer.registers[3]);
            registerFourBox.Text = Convert.ToString(computer.registers[4]);
            registerFiveBox.Text = Convert.ToString(computer.registers[5]);
            registerSixBox.Text = Convert.ToString(computer.registers[6]);
            registerSevenBox.Text = Convert.ToString(computer.registers[7]);
            prgCounter.Text = Convert.ToString(computer.programCounter);
        }

        string StringFromRichTextBox(RichTextBox rtb)
        {
            TextRange textRange = new TextRange(
                // TextPointer to the start of content in the RichTextBox.
                rtb.Document.ContentStart,
                // TextPointer to the end of content in the RichTextBox.
                rtb.Document.ContentEnd
            );

            // The Text property on a TextRange object returns a string
            // representing the plain text content of the TextRange.
            return textRange.Text;
        }

        private void program_TextChanged(object sender, TextChangedEventArgs e)
        {
        }

        private void Clear_Click(object sender, RoutedEventArgs e)
        {
            consoleOut.Text = "";
        }

        private void prgRst_Click(object sender, RoutedEventArgs e)
        {
            computer.programCounter = 0;
            computer.ClearRegisters();
            updateRegText();
        }

        private void step_Click(object sender, RoutedEventArgs e)
        {
            executingFlag = false;
            if (computer.halt == false)
            {
                Step();
            }
            else
                MessageBox.Show("Can't step over halt!");

        }

        private void Step()
        {
            computer.ParseMachineCode(computer.memory[computer.programCounter]);
            TrapFlags();
            updateRegText();
        }

        private void consoleOut_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!consoleOut.IsReadOnly)
            {
                consoleOut.IsReadOnly = true;
                computer.registers[0] = (short)consoleOut.Text[consoleOut.Text.Length - 1];
                consoleOut.Text += '\n';
                computer.halt = false;
                if (executingFlag)
                    ExecuteCode();
                else
                    Step();
            }
        }

        public void TrapFlags()
        {
            switch (computer.trapFlag)
            {
                case 0x20: // GETC

                    break;
                case 0x21: // OUT
                    consoleOut.Text += (char)computer.registers[0];
                    break;
                case 0x22: // PUTS
                    int i = 0;
                    while (computer.memory[computer.registers[0] + i] != 0)
                    {
                        consoleOut.Text += (char)computer.memory[computer.registers[0] + i];
                        i++;
                    }
                    break;
                case 0x23:  // IN
                    consoleOut.Text += "\nInput a character>";
                    consoleOut.IsReadOnly = false;
                    computer.halt = true;
                    break;
                case 0x24:  // PUTSP

                    break;
                case 0x25: // HALT
                    consoleOut.Text += " Halted!\n";
                    computer.halt = true;
                    break;
            }
            if (computer.trapFlag != 0)
                computer.trapFlag = 0;
        }

        private void load_Click(object sender, RoutedEventArgs e)
        {
            loadMachineCode();
        }

        private void loadMachineCode()
        {
            computer.programCounter = 0;
            computer.halt = false;
            string inProgram = program.Text;

            string[] commands = Regex.Split(inProgram, "\r\n");
            Array.Resize<string>(ref commands, commands.Length - 1); // Removes the empty line at end of array


            foreach (string command in commands)
            {
                foreach (char test in command)
                {
                    if ((test != '1') && (test != '0'))
                    {
                        MessageBox.Show("Invalid characters in input! (Character: " + test + ')');
                        return;
                    }
                }
                if (command.Length != 16)
                {
                    MessageBox.Show("Input has invalid command lengths!");
                    return;
                }
            }
            for (int i = 0; i < commands.Length; i++)
            {
                computer.memory[i] = Convert.ToInt16(commands[i], 2);
            }
            //dataGrid.ItemsSource = computer.memory;
        }

        private void listView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
    }
}
