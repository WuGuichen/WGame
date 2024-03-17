using System;

[AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
public class WLableAttribute : Attribute
{
    public WLableAttribute(string text)
    {
        LabelText = text;
    }
    
    public string LabelText { get; }
}