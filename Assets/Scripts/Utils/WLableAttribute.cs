using System;

[AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
public class WLableAttribute : Attribute
{
    public WLableAttribute(string text, bool selecttable)
    {
        LabelText = text;
        Selecttable = selecttable;
    }
    public WLableAttribute(string text)
    {
        LabelText = text;
        Selecttable = true;
    }
    
    public string LabelText { get; }
    public bool Selecttable { get; }
}