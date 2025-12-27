using System.Collections.Generic;
using UnityEngine;

// JSON 파일에서 모든 대화 데이터를 로드하는 클래스
public static class DialogueDataLoader
{
    private static List<DialogueData> loadedDialogues;
    private static bool isLoaded = false;
    
    // JSON 파일 경로 (Resources 폴더 기준)
    private const string JSON_FILE_PATH = "Dialogues/DialogueData";
    
    // 대화 데이터 로드
    public static List<DialogueData> LoadDialogues()
    {
        if (isLoaded && loadedDialogues != null)
        {
            return loadedDialogues;
        }
        
        loadedDialogues = new List<DialogueData>();
        
        // Resources 폴더에서 JSON 파일 로드
        TextAsset jsonFile = Resources.Load<TextAsset>(JSON_FILE_PATH);
        
        if (jsonFile == null)
        {
            Debug.LogError($"DialogueDataLoader: JSON 파일을 찾을 수 없습니다. 경로: {JSON_FILE_PATH}");
            return loadedDialogues;
        }
        
        try
        {
            // JSON 배열을 파싱
            DialogueDataArray dialogueArray = JsonUtility.FromJson<DialogueDataArray>(jsonFile.text);
            
            if (dialogueArray != null && dialogueArray.dialogues != null)
            {
                loadedDialogues = new List<DialogueData>(dialogueArray.dialogues);
                isLoaded = true;
                Debug.Log($"DialogueDataLoader: {loadedDialogues.Count}개의 대화 데이터를 로드했습니다.");
            }
            else
            {
                Debug.LogError("DialogueDataLoader: JSON 파싱 실패 또는 데이터가 없습니다.");
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"DialogueDataLoader: JSON 파싱 중 오류 발생: {e.Message}");
        }
        
        return loadedDialogues;
    }
    
    // 랜덤 대화 데이터 가져오기
    public static DialogueData GetRandomDialogue()
    {
        List<DialogueData> dialogues = LoadDialogues();
        
        if (dialogues == null || dialogues.Count == 0)
        {
            return null;
        }
        
        int randomIndex = Random.Range(0, dialogues.Count);
        return dialogues[randomIndex];
    }
    
    // 특정 ID로 대화 데이터 가져오기
    public static DialogueData GetDialogueById(string dialogueId)
    {
        List<DialogueData> dialogues = LoadDialogues();
        
        if (dialogues == null || dialogues.Count == 0)
        {
            return null;
        }
        
        foreach (var dialogue in dialogues)
        {
            if (dialogue.dialogueId == dialogueId)
            {
                return dialogue;
            }
        }
        
        return null;
    }
    
    // 모든 대화 데이터 가져오기
    public static List<DialogueData> GetAllDialogues()
    {
        return LoadDialogues();
    }
}

// JSON 배열을 파싱하기 위한 래퍼 클래스
[System.Serializable]
public class DialogueDataArray
{
    public DialogueData[] dialogues;
}

