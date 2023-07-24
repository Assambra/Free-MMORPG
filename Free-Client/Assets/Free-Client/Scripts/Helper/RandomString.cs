using UnityEngine;

public static class RandomString
{
    public static string GetNumericString(int n)
    {
        return Random.Range(0, n).ToString();
    }
}
