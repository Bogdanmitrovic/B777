  using System.Collections.Generic;
using UnityEngine;

public class Checklist
{
    public List<Check> Checks = new List<Check>();

    public List<string> CheckTexts(int characterCount = 50, int splitNameLimit = 20)
    {
        return Checks.ConvertAll(check => check.Text(characterCount, splitNameLimit));
    }
}
