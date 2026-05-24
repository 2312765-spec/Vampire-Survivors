using System.Collections.Generic;
using UnityEngine;

public class ExperienceLevelController : MonoBehaviour
{
    public static ExperienceLevelController instance;

    private void Awake()
    {
        instance = this;
    }

    public int currentExperience;
    public ExpPickup pickup;
    public List<int> expLevels = new List<int>();
    public int currentLevel = 1, levelCount = 100;

    private void Start()
    {
        if (expLevels.Count == 0)
            expLevels.Add(5);

        while (expLevels.Count < levelCount)
            expLevels.Add(Mathf.CeilToInt(expLevels[expLevels.Count - 1] * 1.2f));

        if (UIController.instance != null)
            UIController.instance.UpdateExperience(currentExperience, expLevels[currentLevel], currentLevel);
    }

    public void GetExp(int amountToGet)
    {
        currentExperience += amountToGet;

        while (currentLevel < expLevels.Count && currentExperience >= expLevels[currentLevel])
        {
            LevelUp();
            break; // one level-up at a time for UI
        }

        if (UIController.instance != null)
            UIController.instance.UpdateExperience(currentExperience, expLevels[currentLevel], currentLevel);

        if (SFXManager.instance != null)
            SFXManager.instance.PlaySFXPitched(2);
    }

    public void SpawnExp(Vector3 position, int expValue)
    {
        Instantiate(pickup, position, Quaternion.identity).expValue = expValue;
    }

    private void LevelUp()
    {
        currentExperience -= expLevels[currentLevel - 1];
        currentLevel++;

        if (currentLevel >= expLevels.Count)
            currentLevel = expLevels.Count - 1;

        if (UIController.instance != null && UIController.instance.levelUpPanel != null)
            UIController.instance.levelUpPanel.SetActive(true);

        Time.timeScale = 0f;

        BuildClassBasedLevelUpChoices();

        if (PlayerStatController.instance != null)
            PlayerStatController.instance.UpdateDisplay();
    }

    /// <summary>
    /// Builds the list of upgrade choices for the level-up panel.
    ///
    /// Rules:
    ///   1. No class yet / only Starter → offer class selections.
    ///   2. Has class(es) → offer weapon upgrades from ALL owned classes (primary + secondary).
    ///   3. All weapons maxed across ALL classes → spawn Ascend Stones.
    ///   4. CanUnlockNewClass && slot free → also offer a new class (5 % chance if already has one).
    /// </summary>
    private void BuildClassBasedLevelUpChoices()
    {
        if (UIController.instance == null || ClassManager.instance == null)
            return;

        var buttons = UIController.instance.levelUpButtons;
        var choices  = new List<LevelUpChoice>();
        ClassManager cm = ClassManager.instance;

        Debug.Log($"🔍 BuildClassBasedLevelUpChoices - buttons: {buttons?.Length ?? -1}, HasNoClass: {cm.HasNoClass}");

        UIController.instance.SetLevelUpPanelTitle("Choose Your Path");

        // ------------------------------------------------------------------
        // Case 1: No class yet OR only Starter → show class selection
        // ------------------------------------------------------------------
        bool shouldOfferClasses = cm.HasNoClass ||
            (cm.ActiveClass != null && cm.ActiveClass.className == "Starter");

        if (shouldOfferClasses)
        {
            UIController.instance.SetLevelUpPanelTitle("Choose Your Class");

            var unlockable = cm.GetUnlockableClasses();
            int count = Mathf.Min(unlockable.Count, buttons.Length);
            Debug.Log($"🔍 Case 1: unlockable={unlockable.Count}, showing={count}");

            for (int i = 0; i < count; i++)
            {
                choices.Add(new LevelUpChoice { type = ChoiceType.SelectClass, classData = unlockable[i] });
                Debug.Log($"  ✅ Class choice: {unlockable[i].className}");
            }
        }
        else
        {
            ClassData activeClass = cm.ActiveClass;
            if (activeClass == null) return;

            // FIX: check ALL classes, not just active
            bool primaryWeaponsMaxed = cm.AreAllWeaponsMaxedForClass(activeClass);

            bool allWeaponsMaxed = cm.playerClasses.Count > 1
            ? cm.AreAllWeaponsMaxedForAllClasses()
            : primaryWeaponsMaxed;

            Debug.Log($"🔍 allWeaponsMaxed (all classes): {allWeaponsMaxed}");

            // Spawn Ascend Stones when every weapon across all classes is maxed
            if (primaryWeaponsMaxed)
            {
                if (AscendStoneManager.instance != null)
            {
                AscendStoneManager.instance.SpawnAscendStones();
                Debug.Log("✅ Primary weapons maxed! Ascend stones spawned.");
            }
                else
            {
                Debug.LogWarning("❌ AscendStoneManager.instance is NULL");
            }
            }

            // ------------------------------------------------------------------
            // Case 3: Offer weapon upgrades — FIX: from ALL owned classes
            // ------------------------------------------------------------------

            // FIX: if player has secondary class, show weapons from both classes
            List<Weapon> available = cm.playerClasses.Count > 1
                ? cm.GetAvailableWeaponsForAllClasses()
                : cm.GetAvailableWeaponsForClass(activeClass);

            Debug.Log($"✅ Available weapons across all classes: {available.Count}");

            int weaponSlots = Mathf.Min(available.Count, buttons.Length);
            UIController.instance.SetLevelUpPanelTitle("Choose Your Upgrade");

            for (int i = 0; i < weaponSlots && available.Count > 0; i++)
            {
                int idx = Random.Range(0, available.Count);
                choices.Add(new LevelUpChoice { type = ChoiceType.Weapon, weapon = available[idx] });
                available.RemoveAt(idx);
            }

            // ------------------------------------------------------------------
            // Case 4: Offer unlocking a new class (if slot available)
            // ------------------------------------------------------------------
            if (cm.CanUnlockNewClass && choices.Count < buttons.Length)
            {
                var unlockable = cm.GetUnlockableClasses();
                foreach (var candidate in unlockable)
                {
                    if (choices.Count >= buttons.Length) break;
                    if (cm.HasClass(candidate)) continue;

                    float roll      = Random.value;
                    float baseChance = cm.HasNoClass ? 1f : 0.05f;
                    if (allWeaponsMaxed) baseChance *= 2f;

                    if (roll <= baseChance)
                    {
                        choices.Add(new LevelUpChoice { type = ChoiceType.SelectClass, classData = candidate });
                        Debug.Log($"✅ Offered class: {candidate.className} (roll={roll:F2}, chance={baseChance:F2})");
                    }
                }
            }
        }

        // ------------------------------------------------------------------
        // Fallback: must always have at least one choice
        // ------------------------------------------------------------------
        if (choices.Count == 0)
        {
            if (cm.HasNoClass)
            {
                var unlockable = cm.GetUnlockableClasses();
                int count = Mathf.Min(unlockable.Count, buttons.Length);
                for (int i = 0; i < count; i++)
                    choices.Add(new LevelUpChoice { type = ChoiceType.SelectClass, classData = unlockable[i] });
            }
            else
            {
                // FIX: fallback also uses all-class weapons
                List<Weapon> fallback = cm.playerClasses.Count > 1
                    ? cm.GetAvailableWeaponsForAllClasses()
                    : cm.GetAvailableWeaponsForClass(cm.ActiveClass);

                int count = Mathf.Min(fallback.Count, buttons.Length);
                for (int i = 0; i < count; i++)
                    choices.Add(new LevelUpChoice { type = ChoiceType.Weapon, weapon = fallback[i] });
            }
        }

        // ------------------------------------------------------------------
        // Apply choices to buttons
        // ------------------------------------------------------------------
        Debug.Log($"🔍 Final choices: {choices.Count}, buttons: {buttons.Length}");

        for (int i = 0; i < buttons.Length; i++)
        {
            if (i < choices.Count)
            {
                buttons[i].gameObject.SetActive(true);
                buttons[i].SetChoice(choices[i]);
                Debug.Log($"  ✅ Button {i}: {choices[i].type}" +
                          (choices[i].type == ChoiceType.Weapon
                              ? $" → {choices[i].weapon?.name}"
                              : $" → {choices[i].classData?.className}"));
            }
            else
            {
                buttons[i].gameObject.SetActive(false);
            }
        }

        if (UIController.instance != null)
            UIController.instance.UpdateActiveClassDisplay();
    }

    /// <summary>
    /// Displays the promotion menu when the player picks up an Ascend Stone.
    /// </summary>
    public void ShowAscendStonePromotionMenu()
    {
        if (ClassManager.instance == null) return;
        if (UIController.instance == null || UIController.instance.promotionPanel == null) return;

        ClassData activeClass = ClassManager.instance.ActiveClass;
        if (activeClass == null) return;

    // FIX: chỉ check PRIMARY class (activeClass), không check all classes
    // Secondary class chưa max không được phép block promotion của primary
        bool primaryWeaponsMaxed = ClassManager.instance.AreAllWeaponsMaxedForClass(activeClass);

        if (!primaryWeaponsMaxed)
        {
            Debug.LogWarning($"⚠️ Cannot promote: {activeClass.className} weapons not all maxed!");
            return;
        }

        var buttons = UIController.instance.promotionButtons;
        var choices  = new List<LevelUpChoice>();

        UIController.instance.SetPromotionPanelTitle("Choose Your Promotion");

        foreach (var classData in ClassManager.instance.playerClasses)
        {
            if (classData == null || classData.promotionClass == null) continue;

            choices.Add(new LevelUpChoice { type = ChoiceType.Promotion, classData = classData });
            Debug.Log($"✅ Offered promotion: {classData.className} → {classData.promotionClass.className}");

            if (choices.Count >= buttons.Length) break;
        }

        if (choices.Count == 0)
        {
            Debug.LogWarning("⚠️ No promotions available!");
            UIController.instance.promotionPanel.SetActive(false);
            Time.timeScale = 1f;
            return;
        }

        for (int i = 0; i < buttons.Length; i++)
        {
            if (i < choices.Count)
            {
                buttons[i].gameObject.SetActive(true);
                buttons[i].SetChoice(choices[i]);
            }
            else
            {
                buttons[i].gameObject.SetActive(false);
            }
        }

        UIController.instance.promotionPanel.SetActive(true);
        Time.timeScale = 0f;

        if (UIController.instance != null)
            UIController.instance.UpdateActiveClassDisplay();
    }
}

// ---------------------------------------------------------------------------

public enum ChoiceType
{
    Weapon,
    SelectClass,
    Promotion
}

[System.Serializable]
public class LevelUpChoice
{
    public ChoiceType type;
    public Weapon     weapon;
    public ClassData  classData;
}