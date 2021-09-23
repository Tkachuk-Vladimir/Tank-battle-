using UnityEngine;
using UnityEngine.UI;

public class TankShooting : MonoBehaviour
{
    public int m_PlayerNumber = 1;   // это целочисленныя переменная, в которой хранитьсяномер игрока    
    public Rigidbody m_Shell;        // переменная типа твёрдого тела, такая переменная бывает только в тут    
    public Transform m_FireTransform;  // переенная типа точка 
    public Slider m_AimSlider;         // переменная типа слайдр  
    public AudioSource m_ShootingAudio;  
    public AudioClip m_ChargingClip;     
    public AudioClip m_FireClip;         
    public float m_MinLaunchForce = 15f; 
    public float m_MaxLaunchForce = 30f; 
    public float m_MaxChargeTime = 0.75f;

    
    private string m_FireButton;  // преременная типа стринг,туда помещаю слова 
    private float m_CurrentLaunchForce;  
    private float m_ChargeSpeed;         
    private bool m_Fired;                

    // функция что делать танку, когда он не стреляет 
    private void OnEnable()
    {
        m_CurrentLaunchForce = m_MinLaunchForce;
        m_AimSlider.value = m_MinLaunchForce;
    }

    // утстановка клаиши выстрела и скорости перезарядки
    private void Start()
    {
        m_FireButton = "Fire" + m_PlayerNumber; // установка клавиши выстрела - клавиша пробел

        m_ChargeSpeed = (m_MaxLaunchForce - m_MinLaunchForce) / m_MaxChargeTime;// скорость перезарядки
    }
    

    private void Update()
    {
        m_AimSlider.value = m_MinLaunchForce;

        if (m_CurrentLaunchForce >= m_MaxLaunchForce && !m_Fired)
        {
            m_CurrentLaunchForce = m_MaxLaunchForce;
            Fire();
        }
        else if (Input.GetButtonDown(m_FireButton))
        {
            m_Fired = false; // пререводим переменную в состояние false
            m_CurrentLaunchForce = m_MinLaunchForce; // текущую силу устанвливаем в минимально значение 

            m_ShootingAudio.clip = m_ChargingClip; // устанвливаем звук зарядки
            m_ShootingAudio.Play(); // включаем звук зарядки 
        }
        else if (Input.GetButton(m_FireButton) && !m_Fired)
        {
            m_CurrentLaunchForce += m_ChargeSpeed * Time.deltaTime;// каждую секунду увеличиваем текущую силу
            m_AimSlider.value = m_CurrentLaunchForce; // отрисовываем стрелку пропорционально текущей силе
        }
        else if (Input.GetButtonUp(m_FireButton) && !m_Fired)
        {
            Fire();
        }
    }


    private void Fire()
    {
        m_Fired = true;

        Rigidbody shellInstance = Instantiate(m_Shell,m_FireTransform.position, m_FireTransform.rotation) as Rigidbody; //создаюём снаряд в точке выстрела

        shellInstance.velocity = m_CurrentLaunchForce * m_FireTransform.forward;// запускаем снаряд вперём с Текущей скоростью 

        m_ShootingAudio.clip = m_FireClip;// устанавливаем звук выстрела
        m_ShootingAudio.Play(); // включаем звук выстрела

        m_CurrentLaunchForce = m_MinLaunchForce; // задаём минимальное значение текущей силе 
         
    }
}