using UnityEngine;

public class ParticleEffectTrigger : MonoBehaviour
{
    [SerializeField] ParticleSystem _particles;
    
    public void OneShot()
    {
        _particles.Play();
    }
}
