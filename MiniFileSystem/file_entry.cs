using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiniFileSystem
{
    public class File_Entry : Directory_Entry
    {
        public string content;
        public Directory parent;
        public File_Entry(string name, byte dir_attr, int dir_firstCluster, Directory pa) : base(name, dir_attr, dir_firstCluster)
        {
            content = string.Empty;
            if (pa != null)
                parent = pa;
        }
        public Directory_Entry GetDirectory_Entry()
        {
            Directory_Entry op = new Directory_Entry(new string(this.dir_name), this.dir_attr, this.dir_firstCluster);
            return op;
        }
        public static byte[] StringToBytes(string streng)
        {
            int length=streng.Length;
            byte[] string_bytes = new byte[length];
            for (int i = 0; i < length; i++)
            {
                string_bytes[i] = (byte)streng[i];
            }
            return string_bytes;
        }
        public void WriteFileContent()
        {
            byte[] content_byte = StringToBytes(content);
            List<byte[]> bytesls = Directory.SplitBytes(content_byte);
            int clusterFatIndex , lastCluster = -1;
            if (this.dir_firstCluster != 0)
            {
                clusterFatIndex = this.dir_firstCluster;
            }
            else
            {
                clusterFatIndex = FAT.GetAvilableCluster();
                this.dir_firstCluster = clusterFatIndex;
            }
            
            for (int i = 0; i < bytesls.Count; i++)
            {
                if (clusterFatIndex != -1)
                {
                    Virtual_file.WriteCluster(bytesls[i], clusterFatIndex, 0, bytesls[i].Length);
                    FAT.SetNexCluster(clusterFatIndex, -1);
                    if (lastCluster != -1)
                        FAT.SetNexCluster(lastCluster, clusterFatIndex);
                    lastCluster = clusterFatIndex;
                    clusterFatIndex = FAT.GetAvilableCluster();
                }
            }
        }
        public static string BytesToString(byte[] string_bytes)
        {
            string streng = string.Empty;
            for (int i = 0; i < string_bytes.Length; i++)
            {
                if ((char)string_bytes[i] != '\0')
                    streng += (char)string_bytes[i];
                else
                    break;
            }
            return streng;
        }
        public void ReadFileContent()
        {
            if (this.dir_firstCluster != 0)
            {
                content = string.Empty;
                int cluster = this.dir_firstCluster;
                int next = FAT.getNextCluster(cluster);
                List<byte> lest = new List<byte>();
                do
                {
                    lest.AddRange(Virtual_file.ReadCluster(cluster));
                    cluster = next;
                    if (cluster != -1)
                        next = FAT.getNextCluster(cluster);
                }
                while (next != -1);
                content = BytesToString(lest.ToArray());
            }
        }
        public void DeleteFile()
        {
            if (this.dir_firstCluster != 0)
            {
                int cluster = this.dir_firstCluster;
                int next = FAT.getNextCluster(cluster);
                do
                {
                    FAT.SetNexCluster(cluster, 0);
                    cluster = next;
                    if (cluster != -1)
                        next = FAT.getNextCluster(cluster);
                }
                while (cluster != -1);
            }
            if (this.parent != null)
            {
                int index = this.parent.SearchDirectory(new string(this.dir_name));
                if (index != -1)
                {
                    this.parent.file_dir.RemoveAt(index);
                    this.parent.WriteDirectory();
                    FAT.WriteFat();
                }
            }
        }
    }
}
