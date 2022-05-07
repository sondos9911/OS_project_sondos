using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiniFileSystem
{
    public class Directory_Entry
    {
        public char[] dir_name = new char[11];
        public byte dir_attr;
        public byte[] dir_empty = new byte[12];
        public int dir_firstCluster;
        public int dir_filesize;
        public Directory_Entry(string dir_name, byte dir_attr, int dir_firstCluster)
        {
            this.dir_attr = dir_attr;
            this.dir_firstCluster = dir_firstCluster;
            if (dir_attr == 0x10)
            {
                DirNameNoExtention(dir_name.ToCharArray());
            }

            else if (dir_attr == 0x0)
            {
                string[] fileName = dir_name.Split('.');
                DirNameWithExtention(fileName[0].ToCharArray(), fileName[1].ToCharArray());
            }
    
        }
        public void DirNameWithExtention(char[] file_name, char[] extension)
        {
            int length = file_name.Length, cont = 0, len_extention = extension.Length;
            if (length >= 7)
            {
                for (int i = 0; i < 7; i++)
                {
                    this.dir_name[cont] = file_name[i];
                    cont++;
                }
                this.dir_name[cont] = '.';
                cont++;
            }
            else if (length < 7)
            {
                for (int i = 0; i < 7; i++)
                {
                    this.dir_name[cont] = file_name[i];
                    cont++;
                }
                for (int i = 0; i < 7-length; i++)
                {
                    this.dir_name[cont] = ' ';
                    cont++;
                }
                this.dir_name[cont] = '.';
                cont++;
            }
            for(int i=0; i<len_extention; i++)
            {
                this.dir_name[cont] = extension[i];
                cont++;
            }
            for (int i = 0; i < 3-len_extention; i++)
            {
                this.dir_name[cont] = ' ';
                cont++;
            }
        }
        public void DirNameNoExtention(char[] file_name)
        {
            int length = file_name.Length, cont = 0;
            if (length <= 11)
            {
                for (int i = 0; i < length; i++)
                {
                    this.dir_name[i] = file_name[i];
                    cont++;
                }
                for (int i = 0; i < dir_name.Length - length; i++) 
                {
                    this.dir_name[cont] = ' ';
                    cont++;
                }
            }
            else
            {
                for (int i = 0; i < 11; i++)
                {
                    this.dir_name[i] = file_name[i];
                }
            }
        }
    }
}