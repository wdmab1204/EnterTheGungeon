using GameEngine.DataSequence.DIContainer;
using GameEngine.GunController;
using UnityEngine;
using UnityEngine.UI;

namespace GameEngine.Test
{
    public class GunSwapUI : MonoBehaviour
    {
#if UNITY_EDITOR

        [SerializeField] Button btn;
        [SerializeField] InputField inputField;

        // Start is called before the first frame update
        void Start()
        {
            btn.onClick.AddListener(() =>
            {
                var controller = DIContainer.Resolve<IGunController>();
                var id = int.Parse(inputField.text);
                controller.Equip(id);
            });
        }
#else
        void Start()
        {
            this.gameObject.SetActive(false);
        }
#endif
    }
}
