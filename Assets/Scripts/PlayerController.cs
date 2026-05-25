using UnityEngine;

public class PlayerController : BasePlayerController
{
    void Update()
    {
        Vector3 moveInput = Vector3.zero;

        moveInput.x = Input.GetAxisRaw("Horizontal");
        moveInput.y = Input.GetAxisRaw("Vertical");

        moveInput.Normalize();

        transform.position += moveInput * moveSpeed * Time.deltaTime;

        spriteRenderer.flipX = moveInput.x < 0;

        anim.SetBool("isMoving", moveInput != Vector3.zero);
    }
}