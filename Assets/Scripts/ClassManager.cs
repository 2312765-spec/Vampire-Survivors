using System.Collections.Generic;
using UnityEngine;

public class ClassManager : MonoBehaviour
{
    public static ClassManager instance;

    [Header("Available Classes (all possible classes in the game)")]
    public List<ClassData> allClasses = new List<ClassData>();

    [Header("Runtime State")]
    public List<ClassData> playerClasses = new List<ClassData>();
    public int activeClassIndex = 0;
    public int maxClasses = 3;

    [HideInInspector]
    public ClassData firstClassSelected;  // Lưu class đầu tiên được chọn

    private void Awake()
    {
        instance = this;
        Debug.Log($"🔍 ClassManager.Awake - playerClasses.Count: {playerClasses.Count}");
        for (int i = 0; i < playerClasses.Count; i++)
            Debug.Log($"  Class {i}: {playerClasses[i]?.className ?? "NULL"}");
    }

    // -----------------------------------------------------------------------
    // Properties
    // -----------------------------------------------------------------------

    public ClassData ActiveClass
    {
        get
        {
            if (playerClasses.Count == 0) return null;
            if (activeClassIndex < 0 || activeClassIndex >= playerClasses.Count) return null;
            return playerClasses[activeClassIndex];
        }
    }

    public bool HasNoClass     => playerClasses.Count == 0;
    public bool CanUnlockNewClass => playerClasses.Count < maxClasses;

    public bool HasClass(ClassData classData)
        => classData != null && playerClasses.Contains(classData);

    // -----------------------------------------------------------------------
    // Class Management
    // -----------------------------------------------------------------------

    public void SetActiveClass(int index)
    {
        if (index < 0 || index >= playerClasses.Count) return;
        activeClassIndex = index;

        if (PlayerController.instance != null)
            PlayerController.instance.ApplyClass(ActiveClass);
    }

    public void UnlockClass(ClassData classData)
    {
        if (classData == null) return;
        if (playerClasses.Contains(classData)) return;
        if (playerClasses.Count >= maxClasses) return;

        if (playerClasses.Count == 0)
            firstClassSelected = classData;

        playerClasses.Add(classData);

        if (playerClasses.Count == 1)
            activeClassIndex = 0;
    }

    public void ApplyClass(ClassData classData)
    {
        if (classData == null) return;

        if (!playerClasses.Contains(classData))
        {
            if (playerClasses.Count >= maxClasses) return;
            playerClasses.Add(classData);
        }

        activeClassIndex = playerClasses.IndexOf(classData);

        if (PlayerController.instance != null)
            PlayerController.instance.ApplyClass(classData);
    }

    public void PromoteClass(ClassData classData)
    {
        if (classData == null || classData.promotionClass == null) return;

        int idx = playerClasses.IndexOf(classData);
        if (idx < 0) return;

        // Disable tất cả weapons trước promote
        if (PlayerController.instance != null)
        {
            Weapon[] allWeapons = PlayerController.instance.GetComponentsInChildren<Weapon>(true);
            foreach (Weapon weapon in allWeapons)
            {
                if (weapon != null)
                {
                    weapon.StopActiveSkill();
                    weapon.gameObject.SetActive(false);
                }
            }
            Debug.Log($"✅ Disabled all {allWeapons.Length} weapons before promote");
        }

        if (classData == firstClassSelected)
        {
            firstClassSelected = classData.promotionClass;
            Debug.Log($"✅ Promoted first class: {classData.className} → {classData.promotionClass.className}");
        }

        playerClasses[idx] = classData.promotionClass;
        activeClassIndex = idx;

        if (PlayerController.instance != null)
            PlayerController.instance.ApplyClass(classData.promotionClass);
    }

    // -----------------------------------------------------------------------
    // FIX: Weapon queries that cover ALL player classes (primary + secondary)
    // -----------------------------------------------------------------------

    /// <summary>
    /// Returns available (not-yet-maxed) weapons across ALL classes the player owns.
    /// Used by ExperienceLevelController so secondary-class weapons appear in level-up choices.
    /// </summary>
    public List<Weapon> GetAvailableWeaponsForAllClasses()
    {
        var available = new List<Weapon>();

        foreach (var classData in playerClasses)
        {
            if (classData == null) continue;

            foreach (var w in GetAvailableWeaponsForClass(classData))
            {
                if (!available.Contains(w))   // no duplicates
                    available.Add(w);
            }
        }

        Debug.Log($"🔍 GetAvailableWeaponsForAllClasses: {available.Count} weapons across {playerClasses.Count} classes");
        return available;
    }

    /// <summary>
    /// Returns true only when every weapon of every owned class is fully levelled.
    /// Used to decide when to spawn Ascend Stones.
    /// </summary>
    public bool AreAllWeaponsMaxedForAllClasses()
    {
        foreach (var classData in playerClasses)
        {
            if (!AreAllWeaponsMaxedForClass(classData))
                return false;
        }
        return true;
    }

    // -----------------------------------------------------------------------
    // Per-class weapon queries (unchanged logic, kept for single-class calls)
    // -----------------------------------------------------------------------

    public List<ClassData> GetUnlockableClasses()
    {
        var result = new List<ClassData>();

        foreach (var c in allClasses)
        {
            if (c == null) continue;
            if (playerClasses.Contains(c)) continue;

            // Skip old version if promotion already unlocked
            if (c.promotionClass != null && playerClasses.Contains(c.promotionClass))
                continue;
            if (IsObsoleteByPromotion(c)) continue;    

            result.Add(c);
        }

        return result;
    }
    private bool IsObsoleteByPromotion(ClassData classData)
    {
        ClassData current = classData;

        while (current.promotionClass != null)
        {
        current = current.promotionClass;

        // Nếu bất kỳ version cao hơn nào đã có → class này obsolete
        if (playerClasses.Contains(current))
        {
            Debug.Log($"⚠️ Skipping {classData.className} — " +
                      $"player already has promoted version: {current.className}");
            return true;
        }
    }

        return false;
    }

    public List<Weapon> GetAvailableWeaponsForClass(ClassData classData)
    {
        var available = new List<Weapon>();
        if (classData == null) return available;
        if (PlayerController.instance == null) return available;
        if (classData.classWeapons == null) return available;

        foreach (var weaponPrefab in classData.classWeapons)
        {
            if (weaponPrefab == null) continue;

            Weapon foundWeapon = null;

            foreach (var w in PlayerController.instance.assignedWeapons)
            {
                if (w != null && w.weaponPrefab == weaponPrefab) { foundWeapon = w; break; }
            }

            if (foundWeapon == null)
            {
                foreach (var w in PlayerController.instance.unassignedWeapons)
                {
                    if (w != null && w.weaponPrefab == weaponPrefab) { foundWeapon = w; break; }
                }
            }

            if (foundWeapon != null)
            {
                bool isMaxed = PlayerController.instance.fullyLevelledWeapons.Contains(foundWeapon);
                if (!isMaxed)
                    available.Add(foundWeapon);
            }
        }

        return available;
    }

    public bool AreAllWeaponsMaxedForClass(ClassData classData)
    {
        if (classData == null) return false;
        if (PlayerController.instance == null) return false;
        if (classData.classWeapons == null || classData.classWeapons.Count == 0) return false;

        foreach (var weaponPrefab in classData.classWeapons)
        {
            if (weaponPrefab == null) continue;

            Weapon foundWeapon = null;

            foreach (var w in PlayerController.instance.assignedWeapons)
            {
                if (w != null && w.weaponPrefab == weaponPrefab) { foundWeapon = w; break; }
            }

            if (foundWeapon == null)
            {
                foreach (var w in PlayerController.instance.fullyLevelledWeapons)
                {
                    if (w != null && w.weaponPrefab == weaponPrefab) { foundWeapon = w; break; }
                }
            }

            if (foundWeapon == null || !PlayerController.instance.fullyLevelledWeapons.Contains(foundWeapon))
                return false;
        }

        return true;
    }
}