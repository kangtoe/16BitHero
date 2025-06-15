using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerCharacter : CharacterBase
{
    [Header("Elements")]
    [SerializeField] private MobileJoystick playerJoystick;

    protected override void Start()
    {
        base.Start();
        rig.velocity = Vector2.right;
    }

    private void FixedUpdate()
    {        
        Move(playerJoystick.GetMoveVector().normalized * moveSpeed * Time.fixedDeltaTime);
    }
}