using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerControllerMap2 : MonoBehaviour
{
    public static PlayerControllerMap2 instance;

    [Header("References")]
    public SpriteRenderer spriteRenderer;
    public Animator anim;
    [Header("Movement")]
    public float moveSpeed;
    [Header("Pickup")]
    public float pickupRange = 1.5f;
    [Header("Weapons")]
    public List<Weapon> unassignedWeapons;
    public List<Weapon> assignedWeapons;
    public int maxWeapons = 3;
    [HideInInspector]
    public List<Weapon> fullyLevelledWeapons = new List<Weapon>();
    private Rigidbody2D rb;
    private Vector2 moveInput;

    private void Awake()
    {
        instance = this;
        this.rb = GetComponent<Rigidbody2D>();
    }

    void Start()
    {
        if(assignedWeapons.Count == 0)
        {        
            AddWeapon(Random.Range(0, unassignedWeapons.Count));

        }
        moveSpeed = PlayerStatController.instance.moveSpeed[0].value;
        pickupRange = PlayerStatController.instance.pickupRange[0].value;
        maxWeapons = Mathf.RoundToInt(PlayerStatController.instance.maxWeapons[0].value);
    }

    private void Update()
    {
        moveInput.x = Input.GetAxisRaw("Horizontal");
        moveInput.y = Input.GetAxisRaw("Vertical");

        moveInput.Normalize();

        spriteRenderer.flipX = moveInput.x < 0;
        anim.SetBool("isMoving", moveInput != Vector2.zero);
    }

    private void FixedUpdate()
    {
        rb.velocity = moveInput * moveSpeed;
    }

    public void AddWeapon(int weaponNumber)
    {
        if (weaponNumber < unassignedWeapons.Count)
        {
            assignedWeapons.Add(unassignedWeapons[weaponNumber]);
            unassignedWeapons[weaponNumber].gameObject.SetActive(true);
            unassignedWeapons.RemoveAt(weaponNumber);
        }
    }
    public void AddWeapon(Weapon weaponToAdd)
    {
        weaponToAdd.gameObject.SetActive(true);
        assignedWeapons.Add(weaponToAdd);
        unassignedWeapons.Remove(weaponToAdd);
    }
}