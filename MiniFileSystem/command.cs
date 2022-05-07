using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace MiniFileSystem
{
    public static class Commands
    {
        public static void Clear()
        {
            Console.Clear();
        }
        public static void Quit()
        {
            Environment.Exit(0);
        }

        public static void Help(string com = " ")
        {
            bool fund = false;
            string[] command = { "cd", "help", "dir", "quit", "copy", "cls", "del", "md", "rd", "rename", "type", "import", "export" };
            foreach (string i in command)
            {
                if (i != com.ToLower())
                {
                    continue;
                }
                fund = true;
            }
            if (com == " ")
            {
                Console.WriteLine("help   -------> Provides Help information for commands.");
                Console.WriteLine("cd     -------> Change the current default directory to .");
                Console.WriteLine("cls    -------> Clear the screen.");
                Console.WriteLine("dir    -------> List the contents of directory .");
                Console.WriteLine("quit   -------> Quit the shell.");
                Console.WriteLine("copy   -------> Copies one or more files to another location");
                Console.WriteLine("del    -------> Deletes one or more files.");
                Console.WriteLine("md     -------> Creates a directory.");
                Console.WriteLine("rd     -------> Removes a directory.");
                Console.WriteLine("rename -------> Renames a file.");
                Console.WriteLine("type   -------> Displays the contents of a text file.");
                Console.WriteLine("import -------> import text file(s) from your computer");
                Console.WriteLine("export -------> export text file(s) to your computer");
            }
            else if (com != " " && fund)
            {
                switch (com)
                {
                    case "cd":
                        Console.WriteLine("Change the current default directory to the directory given in the argument.");
                        break;
                    case "cls":
                        Console.WriteLine("Clear the screen.");
                        break;
                    case "dir":
                        Console.WriteLine("List the contents of directory given in the argument.");
                        break;
                    case "quit":
                        Console.WriteLine("Quit the shell.");
                        break;
                    case "copy":
                        Console.WriteLine("Copies one or more files to another location.");

                        break;
                    case "del":
                        Console.WriteLine("Delete file.");

                        break;
                    case "help":
                        Console.WriteLine("Provides Help information for commands.");

                        break;
                    case "md":
                        Console.WriteLine("Creates directory.");

                        break;
                    case "rd":
                        Console.WriteLine("Removes  directory.");

                        break;
                    case "rename":
                        Console.WriteLine("Rename file.");

                        break;
                    case "type":
                        Console.WriteLine("Displays the contents of a text file.");

                        break;
                    case "import":
                        Console.WriteLine("import text file(s) from your computer ");
                        break;
                    case "export":
                        Console.WriteLine("export text file(s) to your computer ");
                        break;
                }
            }
            else if (fund == false)
            {
                Console.WriteLine("Error: " + com + " This command is not supported by the project.");
            }
        }
        public static void removeDir(string name)
        {
            int index = OS.current.SearchDirectory(name);
            if (index != -1)
            {
                int firstCluster = OS.current.file_dir[index].dir_filesize;
                Directory d1 = new Directory(name, 0x10, firstCluster, OS.current);
                d1.DeleteDirectory();
                OS.currentPath = new string(OS.current.dir_name).Trim();
            }
            else
            {
                Console.WriteLine("The system cannot find the path specified.");
            }
        }
        public static void lestDir()
        {
            int file_count = 0;
            int dir_count = 0;
            int files_size = 0;
            Console.WriteLine(" Directory of " + OS.currentPath);
            Console.WriteLine();
            int start = 1;
            if (OS.current.parent != null)
            {
                start = 2;
                Console.WriteLine("\t<DIR>    ");
                dir_count++;
            }
            for (int i = start; i < OS.current.file_dir.Count; i++)
            {
                if (OS.current.file_dir[i].dir_attr == 0x0)
                {
                    Console.WriteLine("          " + OS.current.file_dir[i].dir_filesize + new string(OS.current.file_dir[i].dir_name));
                    file_count++;
                    files_size += OS.current.file_dir[i].dir_filesize;
                }
                else if (OS.current.file_dir[i].dir_attr == 0x10)
                {
                    Console.WriteLine("      <DIR>    " + new string(OS.current.file_dir[i].dir_name));
                    dir_count++;
                }
            }
            Console.WriteLine("              " + file_count + " File(s)" + files_size + "bytes");
            Console.WriteLine("              " + dir_count + " Dir(s)" + Virtual_file.GetFreeSpace() + "bytes free");
        }
        public static void changeDir(string name)
        {
            int index = OS.current.SearchDirectory(name);

            if (index != -1)
            {
                int firstCluster = OS.current.file_dir[index].dir_firstCluster;
                Directory d1 = new Directory(name, 0x10, firstCluster, OS.current);
                OS.currentPath = new string(OS.current.dir_name).Trim() + "\\" + new string(d1.dir_name).Trim();
                OS.current.ReadDirectory();
            }
            else
            {
                Console.WriteLine("The system cannot find the path specified.");
            }
        }
        public static void CreateDirectory(string name = " ")
        {
            if (name != " ")
            {
                if (OS.current.SearchDirectory(name) == -1)
                {
                    if (FAT.GetAvilableCluster() != -1)
                    {
                        Directory_Entry d = new Directory_Entry(name, 0x10, 0);
                        OS.current.file_dir.Add(d);
                        OS.current.WriteDirectory();
                        if (OS.current.parent != null)
                        {
                            OS.current.parent.UpdateContent(OS.current.GetDirectoryEntry());
                            OS.current.parent.WriteDirectory();
                        }
                        FAT.WriteFat();
                    }
                    else
                    {
                        Console.WriteLine("Error : sorry the disk is full!");
                    }
                }
                else
                {
                    Console.WriteLine("Error : this directory \" " + name + "\" is already exists!");
                }
            }
            else
            {
                Console.WriteLine("Error: md command syntax is \n md [directory]\n[directory] can be a new directory name or fullpath of a new directory\nCreates a directory.");
            }
        }
    
    }
}
