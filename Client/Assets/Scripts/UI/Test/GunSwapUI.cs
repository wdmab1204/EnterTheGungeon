using UnityEngine;
using UnityEngine.UI;

namespace GameEngine.Test
{
    public class GunSwapUI : MonoBehaviour
    {
        [SerializeField] Button btn;
        [SerializeField] InputField inputField;

        // Start is called before the first frame update
        void Start()
        {
            btn.onClick.AddListener(() =>
            {
                var controller = GameData.Player.GetComponent<GunController.GunController>();
                var id = int.Parse(inputField.text);
                controller.Equip(id);
            });
        }

    }
}
