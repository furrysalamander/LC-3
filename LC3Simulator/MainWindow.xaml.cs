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

using System.Security;
using System.Runtime.InteropServices;
using System.Diagnostics.CodeAnalysis;

namespace LC3Simulator
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        LC computer = new LC();

        public MainWindow()
        {
            InitializeComponent();
            
            // computer.r1 = 5;
        }

        private void Execute_Click(object sender, RoutedEventArgs e)
        {
            computer.programCounter = 0;
            computer.halt = false;
            string inProgram = StringFromRichTextBox(program);

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
            while (!computer.halt && (computer.programCounter < computer.memory.Length))
            {
                computer.ParseMachineCode(computer.memory[computer.programCounter]);
                prgCounter.Text = Convert.ToString(computer.programCounter);
                
                /*if (computer.outCharFlag)
                {
                    consoleText += computer.outChar;
                }*/
                //consoleOut.Text = computer.registers[1].ToString();
            }
            //consoleOut.Text = consoleText;
            updateRegText();
            if (computer.halt) Console.WriteLine("Halted!");
            else Console.WriteLine("End of Memory Reached!");
            //if (computer.halt) consoleOut.Text += " Halted!";
            //else consoleOut.Text += " End of memory!";
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
            computer.ParseMachineCode(computer.memory[computer.programCounter]);
            prgCounter.Text = Convert.ToString(computer.programCounter);
            updateRegText();

        }
    }
}
