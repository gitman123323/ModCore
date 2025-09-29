using System;

[AttributeUsage(AttributeTargets.Class, Inherited = false)]
public class ModInfoAttribute : Attribute
{
    public string ModID { get; }
    public string Author { get; }
    public string Version { get; }

    public ModInfoAttribute(string modID, string author, string version)
    {
        ModID = modID;
        Author = author;
        Version = version;
    }
}
