using System.Collections;
using UnityEngine;

namespace GameEngine
{
    public class CharacterAbility : UnitAbility
    {
        private int prevHealth;
        public float invincibilityDuration = 2f;

        private CameraShake camShake;
        private BoxCollider2D boxCollider;

        protected override void Awake()
        {
            base.Awake();
            camShake = Camera.main.GetComponent<CameraShake>();
            boxCollider = GetComponent<BoxCollider2D>();
            prevHealth = Health.Value;
            Health.OnValueChanged += OnHealthChanged;
        }

        private void OnHealthChanged(int health)
        {
            if (health < prevHealth)
            {
                camShake.Shake(0.5f, 0.1f);
                StartCoroutine(CoInvincibility());
            }

            prevHealth = health;
        }

        private IEnumerator CoInvincibility()
        {
            boxCollider.enabled = false;

            yield return new WaitForSeconds(invincibilityDuration);

            boxCollider.enabled = true;
        }
    }
}
