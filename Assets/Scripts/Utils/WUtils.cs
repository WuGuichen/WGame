using System;
using System.Collections.Generic;
#if UNITY_EDITOR
using System.IO;
#endif

public class WUtils
{
    public static int SortPairInt(int from, int to)
    {
        return (to << 16) + from;
    }

    public static int PairInt(int n1, int n2)
    {
        if (n1 > short.MaxValue || n2 > short.MaxValue)
        {
            throw new SystemException("数据过大无法合并");
        }

        int ret;
        if (n1 > n2)
        {
            ret = n2;
            ret <<= 16;
            ret += n1;
        }
        else
        {
            ret = n1;
            ret <<= 16;
            ret += n2;
        }

        return ret;
    }

    public static string GetFileName(string path)
    {
        if (path != null)
        {
            int length = path.Length;
            int index = length;
            while (--index >= 0)
            {
                char ch = path[index];
                if ((int)ch == (int)Path.DirectorySeparatorChar || (int)ch == (int)Path.AltDirectorySeparatorChar ||
                    (int)ch == (int)Path.VolumeSeparatorChar)
                    return path.Substring(index + 1, length - index - 1);
            }
        }

        return path;
    }

    public static string GetFileNameWithoutExtension(string path)
    {
        path = GetFileName(path);
        int length;
        return (length = path.LastIndexOf('.')) == -1 ? path : path.Substring(0, length);
    }


#if UNITY_EDITOR
    /// <summary>
    /// 删除指定文件目录下的所有文件
    /// </summary>
    /// <param name="fullPath">文件路径</param>
    /// <returns>返回bool，true删除成功，false删除失败</returns>
    public static bool DeleteAllFile(string fullPath, SearchOption searchOption = SearchOption.AllDirectories)
    {
        //获取指定路径下面的所有资源文件  然后进行删除
        if (Directory.Exists(fullPath))
        {
            DirectoryInfo direction = new DirectoryInfo(fullPath);
            FileInfo[] files = direction.GetFiles("*", searchOption);

            for (int i = 0; i < files.Length; i++)
            {
                string FilePath = fullPath + "/" + files[i].Name;
                File.Delete(FilePath);
            }

            return true;
        }

        return false;
    }
#endif
}
