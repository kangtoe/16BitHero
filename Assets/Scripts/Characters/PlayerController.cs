using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerCharacter : CharacterBase
{
    [Header("Elements")]
    [SerializeField] private MobileJoystick playerJoystick;

    public Vector2 CenterPos => rig.position;

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