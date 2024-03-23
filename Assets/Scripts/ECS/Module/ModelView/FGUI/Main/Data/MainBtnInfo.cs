namespace WGame.UI
{
    public class MainBtnInfo
    {
        public string Name { get; set; }
        public int ID { get; private set; }
        
        public bool IsShow { get; set; }

        public bool IsEmpty => ID < 0;

        public MainBtnInfo(int id,string name)
        {
            ID = id;
            Name = name;
            IsShow = true;
        }

        public static MainBtnInfo Empty = new MainBtnInfo(-1, "");
    }
}