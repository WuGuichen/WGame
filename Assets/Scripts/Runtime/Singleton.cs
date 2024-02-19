namespace WGame.Runtime
{
    public abstract class Singleton<T> where T : new()
    {
        private static readonly System.Lazy<T> _lazy = new System.Lazy<T>(() => new T());

        public static T Inst => _lazy.Value;

        public virtual void InitInstance()
        {
        }
    }
}