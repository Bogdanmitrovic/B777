using System;
using System.Collections.Generic;

[Serializable]
public class MenuItem
{
    public string MenuName;
    public List<ListItem> Lists;
}

[Serializable]
public class ListItem
{
    public string ListName;
    public List<ChecklistItem> List;
}

[Serializable]
public class ChecklistItem
{
    public string name;
    public string expectedValue;
    public List<ValueOption> value; // For nested "value" field
}

[Serializable]
public class ValueOption
{
    public Dictionary<string, string> Yes;
    public Dictionary<string, string> No;
}
[Serializable]
public class Wrapper
{
    public List<MenuItem> Menus;
}