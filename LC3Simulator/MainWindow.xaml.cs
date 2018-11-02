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
            string consoleText = "";
            while (!computer.halt && (computer.programCounter < computer.memory.Length))
            {
                computer.ParseMachineCode(computer.memory[computer.programCounter]);
                prgCounter.Text = Convert.ToString(computer.programCounter);
                if (computer.outCharFlag)
                {
                    consoleText += computer.outChar;
                }
                // consoleOut.Text = computer.registers[1].ToString();
            }
            consoleOut.Text = consoleText;
            if (computer.halt) consoleOut.Text += " Halted!";
            else consoleOut.Text += " End of memory!";
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

        private void clear_Click(object sender, RoutedEventArgs e)
        {
            consoleOut.Text = "";
        }
    }
}
