using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public static PlayerController instance;

    public SpriteRenderer spriteRenderer;
    public float moveSpeed;
    public Animator anim;
    public float pickupRange = 1.5f;

    public List<Weapon> unassignedWeapons = new List<Weapon>();
    public List<Weapon> assignedWeapons = new List<Weapon>();
    public int maxWeapons = 3;

    [HideInInspector] public List<Weapon> fullyLevelledWeapons = new List<Weapon>();
    [HideInInspector] private List<GameObject> activeOverlays = new List<GameObject>();

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        CacheComponents();

        if (assignedWeapons == null) assignedWeapons = new List<Weapon>();
        if (unassignedWeapons == null) unassignedWeapons = new List<Weapon>();

        if (assignedWeapons.Count == 0 && unassignedWeapons.Count > 0)
        {
            AddWeapon(Random.Range(0, unassignedWeapons.Count));
        }

        if (PlayerStatController.instance != null)
        {
            moveSpeed = PlayerStatController.instance.moveSpeed[0].value;
            pickupRange = PlayerStatController.instance.pickupRange[0].value;
            maxWeapons = Mathf.RoundToInt(PlayerStatController.instance.maxWeapons[0].value);
        }

        if (ClassManager.instance != null && ClassManager.instance.ActiveClass != null)
        {
            Debug.Log($"✅ Start ApplyClass: {ClassManager.instance.ActiveClass.className}");
            ApplyClass(ClassManager.instance.ActiveClass);
        }
        else
        {
            Debug.Log("❌ No active class at start");
        }

        if (UIController.instance != null)
            UIController.instance.UpdateActiveClassDisplay();
    }

    // -----------------------------------------------------------------------
    // Visual helpers
    // -----------------------------------------------------------------------

    public void ApplyClassVisualOnly(ClassData classData)
    {
        if (classData == null) return;
        CacheComponents();

        if (classData.animatorController != null && anim != null)
            anim.runtimeAnimatorController = classData.animatorController;

        ApplyCharacterVisualFromPrefab(classData.characterPrefab);
    }

    private void CacheComponents()
    {
        if (anim == null) anim = GetComponent<Animator>();
        if (spriteRenderer == null) spriteRenderer = GetComponent<SpriteRenderer>();
    }

    // -----------------------------------------------------------------------
    // Overlay management
    // -----------------------------------------------------------------------

    public void AddOverlay(GameObject overlayPrefab)
    {
        if (overlayPrefab == null) return;

        GameObject overlay = Instantiate(overlayPrefab, transform);
        overlay.transform.localPosition = Vector3.zero;
        overlay.transform.localRotation = Quaternion.identity;
        activeOverlays.Add(overlay);
        Debug.Log($"✅ Added overlay: {overlayPrefab.name}");
    }

    public void RemoveAllOverlays()
    {
        foreach (var overlay in activeOverlays)
            if (overlay != null) Destroy(overlay);

        activeOverlays.Clear();
        Debug.Log("✅ Removed all overlays");
    }

    // -----------------------------------------------------------------------
    // Movement
    // -----------------------------------------------------------------------

    private void Update()
    {
        if (instance == null) return;
        CacheComponents();

        Vector3 moveInput = new Vector3(
            Input.GetAxisRaw("Horizontal"),
            Input.GetAxisRaw("Vertical"), 0f).normalized;

        transform.position += moveInput * moveSpeed * Time.deltaTime;

        if (spriteRenderer != null)
        {
            if (moveInput.x > 0) spriteRenderer.flipX = false;
            else if (moveInput.x < 0) spriteRenderer.flipX = true;
        }

        if (anim != null)
            anim.SetBool("isMoving", moveInput != Vector3.zero);
    }

    // -----------------------------------------------------------------------
    // ApplyClass — primary keeps all weapons; secondary MERGES weapons in
    // -----------------------------------------------------------------------

    public void ApplyClass(ClassData classData)
    {
        if (classData == null) return;
        CacheComponents();

        bool isPrimaryClass = ClassManager.instance != null &&
                              classData == ClassManager.instance.firstClassSelected;

        if (isPrimaryClass)
        {
            if (classData.animatorController != null && anim != null)
                anim.runtimeAnimatorController = classData.animatorController;
            else
                Debug.LogWarning("⚠️ Animator or animatorController is null!");

            ApplyCharacterVisualFromPrefab(classData.characterPrefab);
        }
        else
        {
            Debug.Log($"ℹ️ Not applying visuals — classData ({classData.className}) is not firstClassSelected");
        }

        bool isSecondaryClass = ClassManager.instance != null &&
                                ClassManager.instance.ActiveClass != null &&
                                classData != ClassManager.instance.ActiveClass;

        if (!isSecondaryClass)
        {
            DisableCurrentWeapons();
            assignedWeapons.Clear();
            unassignedWeapons.Clear();
            fullyLevelledWeapons.Clear();
        }

        if (!isSecondaryClass && classData.starterWeapon != null)
            SpawnAndAssignWeapon(classData.starterWeapon);

        if (classData.classWeapons != null)
        {
            foreach (GameObject weaponPrefab in classData.classWeapons)
            {
                if (weaponPrefab == null) continue;

                if (!isSecondaryClass && classData.starterWeapon != null && weaponPrefab == classData.starterWeapon)
                    continue;

                if (WeaponAlreadyExists(weaponPrefab))
                {
                    Debug.Log($"⚠️ Weapon {weaponPrefab.name} already exists — skipping to avoid duplicate");
                    continue;
                }

                GameObject weaponObj = Instantiate(weaponPrefab, transform);
                weaponObj.transform.localPosition = Vector3.zero;
                weaponObj.transform.localRotation = Quaternion.identity;
                weaponObj.SetActive(false);

                Weapon weapon = weaponObj.GetComponent<Weapon>();
                if (weapon != null)
                {
                    weapon.weaponPrefab = weaponPrefab;
                    unassignedWeapons.Add(weapon);
                    Debug.Log($"✅ [{(isSecondaryClass ? "SECONDARY" : "PRIMARY")}] Added to unassigned: {weapon.name}");
                }
            }
        }

        if (isSecondaryClass && classData.overlayPrefab != null)
            AddOverlay(classData.overlayPrefab);

        if (UIController.instance != null)
            UIController.instance.UpdateActiveClassDisplay();
    }

    // -----------------------------------------------------------------------
    // Weapon helpers
    // -----------------------------------------------------------------------

    private bool WeaponAlreadyExists(GameObject weaponPrefab)
    {
        foreach (var w in assignedWeapons)
            if (w != null && w.weaponPrefab == weaponPrefab) return true;

        foreach (var w in unassignedWeapons)
            if (w != null && w.weaponPrefab == weaponPrefab) return true;

        foreach (var w in fullyLevelledWeapons)
            if (w != null && w.weaponPrefab == weaponPrefab) return true;

        return false;
    }

    private void DisableCurrentWeapons()
    {
        foreach (Weapon weapon in GetComponentsInChildren<Weapon>(true))
            if (weapon != null) weapon.gameObject.SetActive(false);
    }

    private void SpawnAndAssignWeapon(GameObject weaponPrefab)
    {
        if (weaponPrefab == null) return;

        GameObject weaponObj = Instantiate(weaponPrefab, transform);
        weaponObj.transform.localPosition = Vector3.zero;
        weaponObj.transform.localRotation = Quaternion.identity;
        weaponObj.SetActive(true);

        Weapon weapon = weaponObj.GetComponent<Weapon>();
        if (weapon != null)
        {
            weapon.weaponPrefab = weaponPrefab;

            if (weapon.IsMaxLevel())
            {
                fullyLevelledWeapons.Add(weapon);
                Debug.Log($"✅ Spawned weapon (MAX LEVEL): {weapon.name}");
            }
            else
            {
                assignedWeapons.Add(weapon);
                Debug.Log($"✅ Spawned weapon (Level {weapon.weaponLevel}): {weapon.name}");
            }
        }
        else
        {
            Debug.LogWarning($"⚠️ {weaponObj.name} has no Weapon component!");
        }
    }

    public bool HasWeapon(Weapon weapon)
        => weapon != null && (assignedWeapons.Contains(weapon) || fullyLevelledWeapons.Contains(weapon));

    public void AddWeapon(int weaponNumber)
    {
        if (weaponNumber < 0 || weaponNumber >= unassignedWeapons.Count) return;

        Weapon weapon = unassignedWeapons[weaponNumber];
        if (weapon == null || HasWeapon(weapon)) return;

        assignedWeapons.Add(weapon);
        weapon.gameObject.SetActive(true);
        unassignedWeapons.RemoveAt(weaponNumber);

        if (UIController.instance != null)
            UIController.instance.UpdateActiveClassDisplay();
    }

    public void AddWeapon(Weapon weaponToAdd)
    {
        if (weaponToAdd == null || HasWeapon(weaponToAdd)) return;

        weaponToAdd.gameObject.SetActive(true);

        if (weaponToAdd.IsMaxLevel())
        {
            fullyLevelledWeapons.Add(weaponToAdd);
            Debug.Log($"✅ Added {weaponToAdd.name} (MAX LEVEL) to fullyLevelledWeapons");
        }
        else
        {
            assignedWeapons.Add(weaponToAdd);
            Debug.Log($"✅ Added {weaponToAdd.name} (Level {weaponToAdd.weaponLevel}) to assignedWeapons");
        }

        unassignedWeapons.Remove(weaponToAdd);

        if (UIController.instance != null)
            UIController.instance.UpdateActiveClassDisplay();
    }

    public void GiveStarterWeapon(GameObject starterWeaponObj)
    {
        if (starterWeaponObj == null) return;
        Weapon starterWeapon = starterWeaponObj.GetComponent<Weapon>();
        if (starterWeapon != null) GiveStarterWeapon(starterWeapon);
    }

    public void GiveStarterWeapon(Weapon starterWeapon)
    {
        if (starterWeapon == null) return;
        if (!HasWeapon(starterWeapon)) AddWeapon(starterWeapon);
    }

    // -----------------------------------------------------------------------
    // Visual
    // -----------------------------------------------------------------------

    private void ApplyCharacterVisualFromPrefab(GameObject characterPrefab)
    {
        if (characterPrefab == null)
        {
            Debug.LogWarning("⚠️ characterPrefab is NULL!");
            return;
        }

        Animator prefabAnimator = characterPrefab.GetComponentInChildren<Animator>();
        SpriteRenderer prefabSprite = characterPrefab.GetComponentInChildren<SpriteRenderer>();

        if (prefabAnimator != null && anim != null)
            anim.runtimeAnimatorController = prefabAnimator.runtimeAnimatorController;

        if (prefabSprite != null && spriteRenderer != null)
        {
            spriteRenderer.sprite = prefabSprite.sprite;
            spriteRenderer.color = prefabSprite.color;
            spriteRenderer.sortingLayerID = prefabSprite.sortingLayerID;
            spriteRenderer.sortingOrder = prefabSprite.sortingOrder;
            Debug.Log($"✅ Applied sprite: {prefabSprite.sprite?.name ?? "NULL"}");
        }
        else
        {
            Debug.LogWarning("❌ prefabSprite or spriteRenderer is NULL!");
        }
    }

    private void ClearCurrentWeapons()
    {
        foreach (var w in assignedWeapons)
            if (w != null) w.gameObject.SetActive(false);

        assignedWeapons.Clear();
        unassignedWeapons.Clear();
        fullyLevelledWeapons.Clear();
    }
}
