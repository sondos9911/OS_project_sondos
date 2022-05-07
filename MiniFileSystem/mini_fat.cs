using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiniFileSystem
{
    public static class FAT
    {
        public static int[] fat = new int[1024];
        public static void CreateFat()
        {
            fat[0] = -1;
            fat[1] = 2;
            fat[2] = 3;
            fat[3] = 4;
            fat[4] = -1;

            for (int i = 5; i < fat.Length; i++)
            {
                fat[i] = 0;  
            }
        }
        public static List<byte[]> SplitFatBytes(byte[] fat_bytes)
        {
            List<byte[]> lest = new List<byte[]>();
            int number_of_arrays = fat_bytes.Length / 1024;
            int remender = fat_bytes.Length % 1024;
            for (int i = 0; i < number_of_arrays; i++)
            {
                byte[] arr = new byte[1024];
                for (int j = i * 1024, k = 0; k < 1024; j++, k++)
                {
                    arr[k] = fat_bytes[j];
                }
                lest.Add(arr);
            }
            if (remender > 0)
            {
                byte[] arr = new byte[1024];

                for (int i = number_of_arrays * 1024, k = 0; k < remender; i++, k++)
                {
                    arr[k] = fat_bytes[i];
                }
                lest.Add(arr);
            }
            return lest;
        }
        public static byte[] FatToBytes(int[] fat_int)
        {
            byte[] fat_bytes = new byte[fat_int.Length * sizeof(int)];
            int cont = 0;
            for (int i = 0; i < fat_int.Length; i++)
            {
                byte[] arr = BitConverter.GetBytes(fat_int[i]);
                for (int j = 0; j < arr.Length; j++)
                {
                    fat_bytes[cont] = arr[j];
                    cont++;
                }
            }
            return fat_bytes;
        }
        public static void WriteFat()
        {
            byte[] FatToByte = FatToBytes(fat);
            List<byte[]> lest = SplitFatBytes(FatToByte);
            for (int i = 0; i < lest.Count; i++)
            {
                Virtual_file.WriteCluster(lest[i], i + 1, 0, lest[i].Length);
            }
        }
        public static int[] FatToInt(byte[] fat_bytes)
        {
            
            int[] fat_int = new int[fat_bytes.Length / 4];
            int cont = 0;
            for (int i = 0; i < fat_int.Length; i++)
            {
                byte[] arr = new byte[4];
                for (int j = 0; j < arr.Length; j++)
                {
                    arr[j] = fat_bytes[cont];
                    cont++;
                }
                fat_int[i] = BitConverter.ToInt32(arr, 0);
            }
            return fat_int;
        }
        public static void ReadFat()
        {
            List<byte> lest = new List<byte>();
            for (int i = 1; i <= 4; i++)
            {
                lest.AddRange(Virtual_file.ReadCluster(i));
            }
            fat = FatToInt(lest.ToArray());
        }
        public static void PrintFat()
        {
            for (int i = 0; i < fat.Length; i++)
                Console.WriteLine("fat[" + i + "] = " + fat[i]);
        }
        public static void SetFat(int[] arr)
        {
            if (arr.Length <= 1024)
                fat = arr;
        }
        public static int GetAvilableCluster()
        {
            for (int i = 0; i < fat.Length; i++)
            {
                if (fat[i] == 0)
                    return i;
            }
            return -1;
        }
        public static void SetNexCluster(int index, int next)
        {
            fat[index] = next;
        }
        public static int getNextCluster(int index)
        {
            if (index >= 0 && index < fat.Length)
                return fat[index];
            else
                return -1;
        }
    }
}
