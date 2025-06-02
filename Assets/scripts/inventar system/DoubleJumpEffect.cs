using UnityEngine;

[CreateAssetMenu(menuName = "PowerUp/Effects/DoubleJump")]
public class DoubleJumpEffect : PowerUpEffect
{
    public float duration = 10f;

    public override void ApplyEffect()
    {
        PlayerMovement player = GameObject.FindObjectOfType<PlayerMovement>();
        if (player != null)
        {
            player.ActivateDoubleJump(duration);
        }
    }
}
