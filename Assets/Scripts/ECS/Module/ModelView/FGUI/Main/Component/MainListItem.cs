namespace WGame.UI.Main
{
    public partial class FUI_MainListItem
    {
        public int index;
        public MainBtnInfo Info { get; private set; }

        public void SetData(int index)
        {
			Info = MainDefine.AllMainBtnList[index];
            RefreshInfo();
			this.index = index;
            MainModel.Inst.RegisterMainViewBtn(this);
        }
        
        public void RefreshInfo()
        {
            visible = Info.IsShow;
            hello.text = Info.Name;
        }

        public override void Dispose()
        {
            MainModel.Inst.UnregisterMainViewBtn(this);
            base.Dispose();
        }
    }
}