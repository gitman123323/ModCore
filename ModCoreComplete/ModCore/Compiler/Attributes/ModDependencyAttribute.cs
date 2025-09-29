using System;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
public class ModDependencyAttribute : Attribute
{
    public string ModID { get; }

    public ModDependencyAttribute(string modID)
    {
        ModID = modID;
    }
}
