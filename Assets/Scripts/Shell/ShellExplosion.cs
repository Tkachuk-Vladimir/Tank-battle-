using UnityEngine;

public class ShellExplosion : MonoBehaviour
{
    public LayerMask m_TankMask;                        // Used to filter what the explosion affects, this should be set to "Players".
    public ParticleSystem m_ExplosionParticles;         // Reference to the particles that will play on explosion.
    public AudioSource m_ExplosionAudio;                // Reference to the audio that will play on explosion.
    public float m_MaxDamage = 100f;                    // The amount of damage done if the explosion is centred on a tank.
    public float m_ExplosionForce = 1000f;              // The amount of force added to a tank at the centre of the explosion.
    public float m_MaxLifeTime = 2f;                    // The time in seconds before the shell is removed.
    public float m_ExplosionRadius = 5f;                // The maximum distance away from the explosion tanks can be and are still affected.


    private void Start()
    {
        // If it isn't destroyed by then, destroy the shell after it's lifetime.
        Destroy(gameObject, m_MaxLifeTime);
    }


    private void OnTriggerEnter(Collider other)
    {
        
        Collider[] colliders = Physics.OverlapSphere(transform.position, m_ExplosionRadius, m_TankMask); // создаётся массив калайдеров сферических
        // в цикле ищем с каким сферическим калайдером столкнётся танк
        for (int i = 0; i < colliders.Length; i++)
        {
            Rigidbody targetRigidbody = colliders[i].GetComponent<Rigidbody>(); // если сфера сталкнулась с танком то получаем его свойство тела

            if (!targetRigidbody)// If they don't have a rigidbody, go on to the next collider.
                continue;

            targetRigidbody.AddExplosionForce(m_ExplosionForce, transform.position, m_ExplosionRadius);// если сфера сталкнулась с танком, толкаем танк

            TankHealth targetHealth = targetRigidbody.GetComponent<TankHealth>();// если сфера сталкнулась с танком получаем доступ к коду Жизни

            if (!targetHealth)
                continue;
            
            float damage = CalculateDamage(targetRigidbody.position);// вычисляем урон, в зависимости от расстояния до снаряда   

            targetHealth.TakeDamage(damage);// получаем доступ к коду Жизни, меняем уровень жизни
        }

        m_ExplosionParticles.transform.parent = null; 
        m_ExplosionParticles.Play(); //включается эффект взрыва снаряда 
        m_ExplosionAudio.Play(); // включается звук эффекта взрыва снаряда 
        Destroy(m_ExplosionParticles.gameObject, m_ExplosionParticles.duration); // удаление эффекта взрыва
        Destroy(gameObject); // удадаление снаряда
    }

    // функция вычисления урона. Чем ближе снаряд упал, тем больше урона и наоборот 
    private float CalculateDamage(Vector3 targetPosition)
    {
        Vector3 explosionToTarget = targetPosition - transform.position; // вычисляем растояние(вектор) до цели

        float explosionDistance = explosionToTarget.magnitude;

        // Calculate the proportion of the maximum distance (the explosionRadius) the target is away.
        float relativeDistance = (m_ExplosionRadius - explosionDistance) / m_ExplosionRadius;

        // Calculate damage as this proportion of the maximum possible damage.
        float damage = relativeDistance * m_MaxDamage;

        // Make sure that the minimum damage is always 0.
        damage = Mathf.Max(0f, damage);

        return damage;
    }
}