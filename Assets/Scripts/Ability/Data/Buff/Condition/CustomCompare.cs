using System;

namespace WGame.Ability
{
    public static class CustomCompare<T> where T : IComparable<T>
    {
        public static bool Compare(CompareType type, T lhs, T rhs)
        {
            bool flag = false;

            switch (type)
            {
                case CompareType.Equal:
                    flag = (lhs.CompareTo(rhs) == 0);
                    break;
                case CompareType.UnEqual:
                    flag = (lhs.CompareTo(rhs) != 0);
                    break;
                case CompareType.Greater:
                    flag = (lhs.CompareTo(rhs) > 0);
                    break;
                case CompareType.Less:
                    flag = (lhs.CompareTo(rhs) < 0);
                    break;
                case CompareType.GreaterEqual:
                    flag = (lhs.CompareTo(rhs) >= 0);
                    break;
                case CompareType.LessEqual:
                    flag = (lhs.CompareTo(rhs) <= 0);
                    break;
                default:
                    flag = false;
                    break;
            }

            return flag;
        }
    }
}