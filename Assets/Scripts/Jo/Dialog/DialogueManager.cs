using System;
using UnityEngine;
using UnityEngine.UI;

public class DialogueManager : Singleton<DialogueManager>
{
    [Header("UI")]
    [SerializeField] private GameObject dialogueUIObject; // 대화 UI 오브젝트
    public Image portraitImage;
    public Text nameText; // 이름 표시용 Text
    public Text dialogueText; // 대사 표시용 Text

    private DialogueData data;
    private int index;
    
    // 대화 종료 시 호출될 콜백
    public Action OnDialogueEnd;
    
    private void Start()
    {
        // 시작 시 대화 UI 비활성화
        if (dialogueUIObject != null)
        {
            dialogueUIObject.SetActive(false);
        }
    }

    public void StartDialogue(DialogueData dialogue)
    {
        data = dialogue;
        index = 0;

        // 대화 UI 활성화
        if (dialogueUIObject != null)
        {
            dialogueUIObject.SetActive(true);
        }

        // 대화 시작 시 checkoutButton 비활성화
        SetCheckoutButtonInteractable(false);

        // 기본 초상화
        SetPortrait(data.defaultPortraitId);
        ShowLine();
    }

    public void Next() // 다음 대화 호출
    {
        index++;
        if (index >= data.lines.Length)
        {
            EndDialogue();
            return;
        }

        ShowLine();
    }

    void ShowLine()
    {
        var line = data.lines[index];

        // 초상화 변경
        if (!string.IsNullOrEmpty(line.portraitId))
            SetPortrait(line.portraitId);

        // 이름 설정: PLAYER는 "플레이어", NPC는 npcName 사용
        string name;
        if (line.speaker == "PLAYER")
        {
            name = "플레이어";
        }
        else if (line.speaker == "NPC")
        {
            name = data.npcName;
        }
        else
        {
            // 기본값 (예상치 못한 값)
            name = line.speaker;
        }

        // 이름과 대사를 별도로 표시
        if (nameText != null)
        {
            nameText.text = name;
        }
        if (dialogueText != null)
        {
            dialogueText.text = line.text;
        }
    }

    void SetPortrait(string id)
    {
        Sprite sp = PortraitLoader.Load(id);
        if (sp != null)
            portraitImage.sprite = sp;
    }

    void EndDialogue()
    {
        Debug.Log("대화 종료");
        
        // 대화 UI 비활성화
        if (dialogueUIObject != null)
        {
            dialogueUIObject.SetActive(false);
        }
        
        // 대화 종료 시 checkoutButton 활성화
        SetCheckoutButtonInteractable(true);
        
        // 대화 종료 콜백 호출
        OnDialogueEnd?.Invoke();
    }
    
    // 대화 활성화 상태 확인
    public bool IsDialogueActive()
    {
        return dialogueUIObject != null && dialogueUIObject.activeSelf;
    }
    
    // checkoutButton 상호작용 설정
    private void SetCheckoutButtonInteractable(bool interactable)
    {
        if (CheckoutUI.Instance != null)
        {
            CheckoutUI.Instance.SetCheckoutButtonInteractable(interactable);
        }
    }
}

