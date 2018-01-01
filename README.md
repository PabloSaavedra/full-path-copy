# FullPathCopy
This command-line utility for Windows copies a file creating full source file path into destination directory. It was coded in C# using Visual Studio 2015.

# Usage
  `fpCopy source_file destination_directory [--overwrite] [--debug]`
### Options
  `-o, --overwrite   Overwrite files`  
  `-d, --debug       Debug messages`  

# Examples
`fpcopy c:\Users\Public\Pictures\picture01.jpg d:\temp\backup`

This command creates the folder `d:\temp\backup\c\Users\Public\Pictures\` and copies the file `picture01.jpg` on it. So the new file full path will be:  
`d:\temp\backup\c\Users\Public\Pictures\picture01.jpg`  

# Notes
The destination directory does not necessarily have to exist. You can use / or \ as directory separator.
