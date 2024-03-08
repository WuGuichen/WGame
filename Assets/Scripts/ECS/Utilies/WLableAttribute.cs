using System;

[AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
public class WLableAttribute : Attribute
{
    public WLableAttribute(string text, string labelText)
    {
        LabelText = labelText;
    }
    
    public string LabelText { get; }
}