using UnityEngine;
using Spine.Unity;

public class CharacterSkinManager : MonoBehaviour
{
    public static CharacterSkinManager Instance;

    [Header("Spine Reference")]
    [SerializeField] private SkeletonAnimation skeletonAnimation;

    [Header("Skin Names")]
    [SerializeField] private string[] clothSkins = { "angel", "davil", "star" };

    private string currentClothSkin = "basic"; // 기본 스킨

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    void Start()
    {
        // 사용 가능한 모든 스킨 출력
        PrintAvailableSkins();

        // 초기 스킨 적용
        ApplySkin(currentClothSkin);
    }

    // 사용 가능한 스킨 목록 출력
    private void PrintAvailableSkins()
    {
        if (skeletonAnimation == null) return;

        var skeletonData = skeletonAnimation.Skeleton.Data;
        Debug.Log("=== 사용 가능한 스킨 목록 ===");

        foreach (var skin in skeletonData.Skins)
        {
            Debug.Log($"스킨 이름: '{skin.Name}'");
        }

        Debug.Log("========================");
    }

    // 옷 번호로 스킨 변경
    public void ChangeClothSkin(int clothNumber)
    {
        if (clothNumber < 1 || clothNumber > clothSkins.Length)
        {
            Debug.LogWarning($"잘못된 옷 번호: {clothNumber}");
            return;
        }

        string skinName = clothSkins[clothNumber - 1];
        ApplySkin(skinName);
        currentClothSkin = skinName;

        Debug.Log($"스킨 변경: {skinName}");
    }

    // 스킨 이름으로 직접 변경
    public void ApplySkin(string skinName)
    {
        if (skeletonAnimation == null)
        {
            Debug.LogError("SkeletonAnimation이 할당되지 않았습니다!");
            return;
        }

        var skeleton = skeletonAnimation.Skeleton;
        var skeletonData = skeleton.Data;

        // 스킨 존재 여부 확인
        var skin = skeletonData.FindSkin(skinName);
        if (skin == null)
        {
            Debug.LogError($"스킨을 찾을 수 없습니다: {skinName}");
            return;
        }

        // 스킨 적용
        skeleton.SetSkin(skin);
        skeleton.SetSlotsToSetupPose();
        skeletonAnimation.AnimationState.Apply(skeleton);
    }

    // 현재 스킨 이름 반환
    public string GetCurrentSkin()
    {
        return currentClothSkin;
    }
}