# LC#3

An implementation of the LC-3 in C#!

All opcodes except for RTI have been implemented, at least at a basic level.

TRAP does not yet support input.
I'm not sure if the program counter jumps have been implemented properly.  

Honestly, at this point it's really buggy.  But hey, at least it doesn't crash anymore when you have invalid input characters, and it won't let you run opcodes of invalid length.  None of the register text boxes actually display the values.  Most of the LC-3 code is pretty clean and efficient, but all of the code in the form needs to be overhauled.  

Roadmap:

Ensure that the program counter jumps correctly

TRAP character input

Overhaul the form so that all of the boxes work correctly

Saving and restoring memory dumps

Stepping through programs

RTI opcode

An assembler!

Saving and loading programs

Other things as requested...

Anyone that wants to help is welcome to!
