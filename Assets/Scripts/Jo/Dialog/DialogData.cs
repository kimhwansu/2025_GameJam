[System.Serializable]
public class DialogueData // 대화 데이터
{
    public string dialogueId;
    public string npcName;
    public string defaultPortraitId;
    public DialogueLine[] lines;
}

[System.Serializable]
public class DialogueLine
{
    public string speaker;     // "NPC" | "PLAYER"
    public string text;
    public string portraitId;  // optional
}
