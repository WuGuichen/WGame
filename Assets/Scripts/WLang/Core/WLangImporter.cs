public enum ImporterType
{
    BTree,
    FSM,
}

public struct WLangImporter
{
    public string name;
    public ImporterType type;
}
