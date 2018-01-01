using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;

namespace fpcopy {
    class FPCopy {
        static int ERROR_NONE = 0;
        static int ERROR_INVALID_ARGUMENTS = 1;
        static int ERROR_SOURCE_FILE_NOT_FOUND  = 2;
        static int ERROR_CREATE_DESTINATION_DIRECTORY = 3;
        static int ERROR_DESTINATION_FILE_EXISTS = 4;
        static int ERROR_FILE_COPY_IOEXCEPTION = 5;
        static int ERROR_FILE_COPY_EXCEPTION = 6;

        static bool OPTION_DEBUG_MESSAGES = false;
        static bool OPTION_OVERWRITE_FILES = false;
        static string version = "0.04";

        static void help() {
            Console.WriteLine("Usage:");
            Console.WriteLine("  fpCopy source_file destination_directory [--overwrite] [--debug]");
            Console.WriteLine("Options:");
            Console.WriteLine("  -o, --overwrite   Overwrite files");
            Console.WriteLine("  -d, --debug       Debug messages");
        }

        static void writeLineDebug(String str) {
            if (OPTION_DEBUG_MESSAGES) {
                setDebugColor();
                Console.WriteLine(str);
                Console.ResetColor();
            }
        }

        static void writeLineError(String str) {
                setErrorColor();
                Console.WriteLine(str);
                Console.ResetColor();
        }

        static void writeLineWarning(String str) {
            setWarningColor();
            Console.WriteLine(str);
            Console.ResetColor();
        }

        static void writeLineSuccess(String str) {
            setSuccessColor();
            Console.WriteLine(str);
            Console.ResetColor();
        }

        static void setErrorColor() {
            Console.ForegroundColor = ConsoleColor.Red;
        }
        static void setWarningColor() {
            Console.ForegroundColor = ConsoleColor.Yellow;
        }
        static void setSuccessColor() {
            Console.ForegroundColor = ConsoleColor.Green;
        }
        static void setDebugColor() {
            Console.ForegroundColor = ConsoleColor.Cyan;
        }

        static void Main(string[] args) {

            Console.WriteLine("\nfpCopy v"+version+ " ©2018 Pablo Saavedra López. FullPathCopy copies a file creating full source file path into destination directory.");
            
            if (args.Length<2) {
                help();
                Environment.Exit(ERROR_INVALID_ARGUMENTS);
            } else if (args.Length >= 2) {
                //Check command line arguments
                ArrayList argsAL = new ArrayList(args);
                for (int i=argsAL.Count-1; i>=0; i--) {
                    String param = (String)argsAL[i];
                    //if the argument is an option, change settings
                    if (param[0] == '-') {
                        if (param.Equals("-o") || param.Equals("--overwrite")) {
                            OPTION_OVERWRITE_FILES = true;
                        } else if (param.Equals("-d") || param.Equals("--debug")) {
                            OPTION_DEBUG_MESSAGES = true;
                        } else {
                            writeLineWarning("WARNING: Unknown option ["+param+"]");
                        }
                        argsAL.RemoveAt(i);
                    }
                }

                if (argsAL.Count != 2) {
                    help();
                    Environment.Exit(ERROR_INVALID_ARGUMENTS);
                }

                String sourceFileStr = (String)argsAL[0];
                String destDirStr = (String)argsAL[1];

                //Replace '/' with '\'
                sourceFileStr = sourceFileStr.Replace('/', '\\');
                destDirStr = destDirStr.Replace('/', '\\');

                if (!File.Exists(sourceFileStr)) {
                    writeLineError("ERROR: Source file not found ["+ sourceFileStr + "]");
                    Environment.Exit(ERROR_SOURCE_FILE_NOT_FOUND);
                } else {
                    
                    writeLineDebug("DEBUG: Source file found: [" + sourceFileStr + "]");

                    if (!Directory.Exists(destDirStr)) {
                        writeLineDebug("DEBUG: Destination directory not found: [" + destDirStr + "] Creating...");
                        try {
                            Directory.CreateDirectory(destDirStr);
                        } catch (Exception e) {
                            writeLineError("ERROR: Couldn't create destination directory [" + destDirStr + "]");
                            writeLineDebug(e.StackTrace);
                            Environment.Exit(ERROR_CREATE_DESTINATION_DIRECTORY);
                        }

                        writeLineDebug("DEBUG: Destination directory created: [" + destDirStr + "]");

                    } else {
                        writeLineDebug("DEBUG: Destination directory exists: [" + destDirStr + "]");
                    }
                    
                    String dPath = Path.GetFullPath(destDirStr);
                    String fPath = Path.GetFullPath(sourceFileStr);

                    writeLineDebug("DEBUG: Destination Directory: " + dPath);
                    writeLineDebug("DEBUG: Source File Path: " + fPath);
                    writeLineDebug("DEBUG: File name : " + Path.GetFileName(sourceFileStr));

                    //Add final \ to path if it doesn't have it
                    if (dPath[dPath.Length - 1] != '\\') {
                        dPath = dPath + "\\";
                        writeLineDebug("DEBUG: Destination Directory: " + dPath);
                    }

                    //Remove filename to have only the full directory
                    fPath = fPath.Substring(0, fPath.Length - Path.GetFileName(sourceFileStr).Length);
                    writeLineDebug("DEBUG: Now fPath is: " + fPath);

                    //Remove colon from path
                    fPath = fPath.Replace(":",String.Empty);
                    String finalPath = dPath + fPath;

                    //Create final path
                    if (!Directory.Exists(finalPath)) {
                        try {
                            Directory.CreateDirectory(finalPath);
                        } catch (Exception e) {
                            writeLineError("ERROR: Couldn't create destination directory [" + finalPath + "]");
                            writeLineDebug("DEBUG: "+e.StackTrace);
                            Environment.Exit(ERROR_CREATE_DESTINATION_DIRECTORY);
                        }
                    }

                    String finalDestFile = dPath + fPath + Path.GetFileName(sourceFileStr);
                    writeLineDebug("DEBUG: Final destination file: [" + finalDestFile + "]");

                    if (!OPTION_OVERWRITE_FILES && File.Exists(finalDestFile)) {
                        writeLineError("ERROR: Destination file already exists [" + finalDestFile + "] Use -o to overwrite");
                        Environment.Exit(ERROR_DESTINATION_FILE_EXISTS);
                    }

                    //Console.Write("Copying: " + Path.GetFullPath(sourceFileStr) + " -> " + finalDestFile + " ");
                    Console.WriteLine("Copying... ");
                    Console.WriteLine("SrcFile: " + Path.GetFullPath(sourceFileStr));
                    Console.WriteLine("DstFile: " + finalDestFile);
                    Console.Write("Result : ");

                    try {
                        File.Copy(Path.GetFullPath(sourceFileStr), finalDestFile, OPTION_OVERWRITE_FILES);
                    }catch(IOException ioe) {
                        writeLineError("[I/O ERROR]");
                        writeLineDebug("DEBUG: "+ioe.StackTrace);
                        Environment.Exit(ERROR_FILE_COPY_IOEXCEPTION);
                    } catch(Exception e) {
                        writeLineError("[ERROR]");
                        writeLineDebug("DEBUG: "+e.StackTrace);
                        Environment.Exit(ERROR_FILE_COPY_EXCEPTION);
                    } finally {
                        writeLineSuccess("OK");
                    }
                }
            }
            Environment.Exit(ERROR_NONE);
        }
    }
}
