using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiniFileSystem
{
    public static class Virtual_file
    {
        public static FileStream file;
        public static void CreatOrOpenFile(string path)
        {
            file = new FileStream(path, FileMode.OpenOrCreate, FileAccess.ReadWrite);
        }
       
        public static void Initi(string path)
        {
            if (!File.Exists(path))
            {
                CreatOrOpenFile(path);
                byte[] arr = new byte[1024];
                for (int i = 0; i < arr.Length; i++)
                    arr[i] = 0;
                WriteCluster(arr, 0);
                FAT.CreateFat();
                Directory root = new Directory("E:", 0x10, 5, null);
                root.WriteDirectory();
                FAT.SetNexCluster(5, -1);
                OS.current = root;
                FAT.WriteFat();
            }
            else
            {
                CreatOrOpenFile(path);
                FAT.ReadFat();
                Directory root = new Directory("E:", 0x10, 5, null);
                root.ReadDirectory();
                OS.current = root;
            }
        }
         
        public static void WriteCluster(byte[] cluster, int Index, int set = 0, int count = 1024)
        {
            file.Seek(Index * 1024, SeekOrigin.Begin);
            file.Write(cluster, set, count);
            file.Flush();
        }
        public static int GetFreeSpace()
        {
            return (1024 * 1024) - (int)file.Length;
        }
        public static byte[] ReadCluster(int Index)
        {
            file.Seek(Index * 1024, SeekOrigin.Begin);
            byte[] bytes = new byte[1024];
            file.Read(bytes, 0, 1024);
            return bytes;
        }
        
    }
}
