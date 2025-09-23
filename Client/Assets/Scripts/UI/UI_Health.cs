using GameEngine.DataSequence.DIContainer;
using UnityEngine;
using UnityEngine.UI;

namespace GameEngine.UI
{

    public class UI_Health : MonoBehaviour
    {
        private IUnitAbility playerAbility; 
        private Slider slider;

        // Start is called before the first frame update
        void Start()
        {
            slider = GetComponent<Slider>();
            playerAbility = DIContainer.Resolve<IUnitAbility>();
            playerAbility.Health.OnValueChanged += OnHealthChanged;
            playerAbility.MaxHealth.OnValueChanged += OnMaxHealthChanged;
            
            playerAbility.MaxHealth.ForceNotify();
            playerAbility.Health.ForceNotify();
        }

        private void OnHealthChanged(int health)
        {
            slider.value = health;
        }

        private void OnMaxHealthChanged(int maxHealth)
        {
            slider.maxValue = maxHealth;
        }
    }
}