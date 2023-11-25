public class LocalScope : BaseScope
{
    public LocalScope(Scope parent) : base(parent)
    {
    }

    public override string ScopeName => "Local";
}