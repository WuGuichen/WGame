public interface Scope
{
    string ScopeName { get; }
    Scope EnclosingScope { get; }
    void Define(string name, Symbol sym);
    Symbol Resolve(string name);
    bool TryResolveSelf(string name, out Symbol symbol);
}