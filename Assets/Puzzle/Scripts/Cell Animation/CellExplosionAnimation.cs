using UnityEngine;

public class CellExplosionAnimation : MonoBehaviour
{
    public ParticleSystem particles;
    public UITweenRectPosition trembling;

    public GameObject FX;

    void Start()
    {

        particles.gameObject.SetActive(false);
        trembling.OnFinished += () =>
        {
            particles.gameObject.SetActive(true);
            FX.SetActive(false);
            Destroy(gameObject, particles.duration + particles.startLifetime);
        };
    }
}
