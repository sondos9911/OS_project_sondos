
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiniFileSystem
{
    class OS
    {
        public static Directory current;
        public static string currentPath;
        static void Main(string[] args)
        {
            Console.WriteLine("------$------> Welcome to OS Project File System <-----$-------");
            Virtual_file.Initi("FILE");
            currentPath = new string(current.dir_name);
            currentPath = currentPath.Trim(new char[] { '\0', ' ' });
            while (true)
            {
                Console.Write(currentPath + "\\" + ">");
                string input = Console.ReadLine();
                string[] input_list = input.Split(' ');//help  cls
                if (input == "")
                {
                    continue;
                }
                List<string> lest = new List<string>();
                for (int i = 0; i < input_list.Length; i++)
                {
                    if ( input_list[i] != " ")
                    {
                        lest.Add(input_list[i]);
                    }
                }
                string[] arguments = lest.ToArray();
                arguments[0] = arguments[0].ToLower();
                int cont = arguments.Length;

                if (Parser.Check_arg(arguments[0]) == false)
                {
                    Console.WriteLine("Error: " + arguments[0]+ " This command is not supported by the Project.");
                }
                else
                {
                    if (cont > 1)
                    {
                        Parser.Call_command(arguments[0], arguments[1]);
                    }
                    else
                    {
                        Parser.Call_command(arguments[0]);
                    }
                }
            }
        }
    }
}
