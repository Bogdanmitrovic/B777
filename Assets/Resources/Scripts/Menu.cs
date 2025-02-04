using System;
using System.Collections.Generic;

[Serializable]
public class Menu
{
    public string menuName;
    public List<Checklist> checklists;
}

[Serializable]
public class ValueOption
{
    public Dictionary<string, string> Yes;
    public Dictionary<string, string> No;
}