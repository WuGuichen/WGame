using System;

public class Utils
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
}
