using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiniFileSystem
{
    public static class Parser
    {
        public static bool Check_arg(string arg)
        {
            string[] command = { "cd", "help", "dir", "quit", "copy", "cls", "del", "md", "rd", "rename", "type", "import", "export" };
            foreach (string i in command)
            {
                if (i == arg)
                {
                    return true;
                }
            }
            return false;
        }
        public static void Call_command(string arg = " ", string arg2 = " ")
        {
            if (arg == "help")
            {
              
                Commands.Help(arg2);
            }
            else if (arg == "quit")
            {
                Commands.Quit();
            }
            else if (arg == "cls")
            {
                Commands.Clear();
            }
            else if (arg == "md")
            {
                Commands.CreateDirectory(arg2);
            }
            else if (arg == "rd")
            {
                Commands.removeDir(arg2);
            }
            else if (arg == "cd")
            {
                Commands.changeDir(arg2);
            }
            else if (arg == "dir")
            {
                Commands.lestDir();
            }
        }
    }
}
