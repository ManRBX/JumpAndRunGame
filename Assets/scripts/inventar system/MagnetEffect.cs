using UnityEngine;

[CreateAssetMenu(menuName = "PowerUp/Effects/Magnet")]
public class MagnetEffect : PowerUpEffect
{
    public float duration = 5f;
    public float magnetRange = 5f;
    public float attractionSpeed = 10f;

    public override void ApplyEffect()
    {
        PlayerMagnet magnet = GameObject.FindObjectOfType<PlayerMagnet>();
        if (magnet != null)
        {
            magnet.ActivateMagnet(duration, magnetRange, attractionSpeed);
        }
    }
}
