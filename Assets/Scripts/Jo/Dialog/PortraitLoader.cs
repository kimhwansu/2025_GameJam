using UnityEngine;

public static class PortraitLoader
{
    public static Sprite Load(string portraitId)
    {
        if (string.IsNullOrEmpty(portraitId))
            return null;

        return Resources.Load<Sprite>($"Portraits/{portraitId}");
    }
}
