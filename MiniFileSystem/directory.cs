using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiniFileSystem
{
    public class Directory : Directory_Entry
    {
        public List<Directory_Entry> file_dir;
        public Directory parent;
        public Directory(string name, byte dir_attr, int dir_firstCluster, Directory parant) : base(name, dir_attr, dir_firstCluster)
        {
            file_dir = new List<Directory_Entry>();
            
            if (parant != null)
            {
                parent = parant;
            }
        }
        public void UpdateContent(Directory_Entry dir)
        {
            int index = SearchDirectory(new string(dir.dir_name));
            if (index != -1)
            {
                file_dir.RemoveAt(index);
                file_dir.Insert(index, dir);
            }
        }
        public Directory_Entry GetDirectoryEntry()
        {
            Directory_Entry op = new Directory_Entry(new string(this.dir_name), this.dir_attr, this.dir_firstCluster);
            return op;
        }
        public static byte[] DirectoryToBytes(Directory_Entry dir)
        {
            byte[] dir_bytes = new byte[32];
            for (int i = 0; i < dir.dir_name.Length; i++)
            {
                dir_bytes[i] = (byte)dir.dir_name[i];
            }
            dir_bytes[11] = dir.dir_attr;
            int j = 12;
            for (int i = 0; i < dir.dir_empty.Length; i++)
            {
                dir_bytes[j] = dir.dir_empty[i];
                j++;
            }
            byte[] first_cluster = BitConverter.GetBytes(dir.dir_firstCluster);
            for (int i = 0; i < first_cluster.Length; i++)
            {
                dir_bytes[j] = first_cluster[i];
                j++;
            }

            byte[] size = BitConverter.GetBytes(dir.dir_filesize);
            for (int i = 0; i < size.Length; i++)
            {
                dir_bytes[j] = size[i];
                j++;
            }
            return dir_bytes;

        }
        public static List<byte[]> SplitBytes(byte[] bytes)
        {
            List<byte[]> lest = new List<byte[]>();
            int number_of_arrays = bytes.Length / 1024;
            int rem = bytes.Length % 1024;
            for (int i = 0; i < number_of_arrays; i++)
            {
                byte[] arr = new byte[1024];
                for (int j = i * 1024, k = 0; k < 1024; j++, k++)
                {
                   arr[k] = bytes[j];
                }
                lest.Add(arr);
            }
            if (rem > 0)
            {
                byte[] arr2 = new byte[1024];
                for (int i = number_of_arrays * 1024, k = 0; k < rem; i++, k++)
                {
                    arr2[k] = bytes[i];
                }
                lest.Add(arr2);
            }
            return lest;
        }
        public void WriteDirectory()
        {
            byte[] dirsorFilesByte = new byte[file_dir.Count * 32];
            for (int i = 0; i < file_dir.Count; i++)
            {
                byte[] b = DirectoryToBytes(this.file_dir[i]);
                for (int j = i * 32, k = 0; k < b.Length; k++, j++)
                    dirsorFilesByte[j] = b[k];
            }
            List<byte[]> lest_bytes = SplitBytes(dirsorFilesByte);
            int clusterFatIndex;
            if (this.dir_firstCluster != 0)
            {
                clusterFatIndex = this.dir_firstCluster;
            }
            else
            {
                clusterFatIndex = FAT.GetAvilableCluster();
                this.dir_firstCluster = clusterFatIndex;
            }
            int lastCluster = -1;
            for (int i = 0; i < lest_bytes.Count; i++)
            {
                if (clusterFatIndex != -1)
                {
                    Virtual_file.WriteCluster(lest_bytes[i], clusterFatIndex, 0, lest_bytes[i].Length);
                    FAT.SetNexCluster(clusterFatIndex, -1);
                    if (lastCluster != -1)
                        FAT.SetNexCluster(lastCluster, clusterFatIndex);
                    lastCluster = clusterFatIndex;
                    clusterFatIndex = FAT.GetAvilableCluster();
                }
            }
            if (this.parent != null)
            {
                
                this.parent.WriteDirectory();
            }
            FAT.WriteFat();
        }
        public static Directory_Entry BytesToDirectory(byte[] bytes)
        {
            char[] name = new char[11];
            for (int i = 0; i < name.Length; i++)
            {
                name[i] = (char)bytes[i];
            }
            byte attr = bytes[11];
            byte[] empty = new byte[12];
            int j = 12;
            for (int i = 0; i < empty.Length; i++)
            {
                empty[i] = bytes[j];
                j++;
            }
            byte[] fc = new byte[4];
            for (int i = 0; i < fc.Length; i++)
            {
                fc[i] = bytes[j];
                j++;
            }
            int firstcluster = BitConverter.ToInt32(fc, 0);
            byte[] size = new byte[4];
            for (int i = 0; i < size.Length; i++)
            {
                size[i] = bytes[j];
                j++;
            }
            int filesize = BitConverter.ToInt32(size, 0);
            Directory_Entry dir= new Directory_Entry(new string(name), attr, firstcluster);
            dir.dir_empty = empty;
            dir.dir_filesize = filesize;
            return dir;

        }
        public void ReadDirectory()
        {
            if (this.dir_firstCluster != 0)
            {
                file_dir = new List<Directory_Entry>();
                int cluster = this.dir_firstCluster;
                int next = FAT.getNextCluster(cluster);
                List<byte> ls = new List<byte>();
                do
                {
                    ls.AddRange(Virtual_file.ReadCluster(cluster));
                    cluster = next;
                    if (cluster != -1)
                        next = FAT.getNextCluster(cluster);
                }
                while (next != -1);
                for (int i = 0; i < ls.Count; i++)
                {
                    byte[] b = new byte[32];
                    for (int k = i * 32, m = 0; m < b.Length && k < ls.Count; m++, k++)
                    {
                        b[m] = ls[k];
                    }
                    if (b[0] == 0)
                        break;
                    file_dir.Add(BytesToDirectory(b));
                }
            }
        }
        public void DeleteDirectory()
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
                   
                }
            }
            if (OS.current == this)
            {
                if (this.parent != null)
                {
                    OS.current = this.parent;
                    OS.currentPath = OS.currentPath.Substring(0, OS.currentPath.LastIndexOf('\\'));
                    OS.current.ReadDirectory();
                }
            }
            FAT.WriteFat();
        }
        public int SearchDirectory(string name)
        {
           
            if (name.Length >= 11)
            {
                name = name.Substring(0, 11);
            }
            else
            {
                name += "\0";
                for (int i = name.Length + 1; i < 12; i++)
                    name += "#";
            }
            for (int i = 0; i < file_dir.Count; i++)
            {
                string n = new string(file_dir[i].dir_name);
                
                if (n.Equals(name))
                    return i;
            }
            return -1;
        }
    }
}
