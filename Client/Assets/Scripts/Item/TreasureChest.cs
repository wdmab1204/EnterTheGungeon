using UnityEngine;

//public class Inventory
//{
//    private List<ItemStack> list = new();

//    public bool TryAdd(ItemStack item)
//    {
//        return true;
//    }

//    public bool TryRemove(ItemStack item)
//    {
//        return true;
//    }

//    public bool Contain(int id)
//    {
//        return true;
//    }
//}

namespace GameEngine.Item
{
    public class TreasureChest : MonoBehaviour, IInteractable
    {
        IChestView view;

        private void Awake()  
        {
            view = GetComponent<SpriteChestView>();
            view.ShowClosed();
        }

        public void OnInteractable()
        {
            var prefab = Resources.Load<WeaponObject>("WeaponObject");

            var obj = UnityEngine.Object.Instantiate(prefab);
            obj.transform.position = this.transform.position;

            int id = Random.Range(1, 5);
            obj.Id = id;
            obj.SetSprite(Resources.Load<Sprite>($"GunSprite_{id}"));

            view.ShowOpened();
        }
    }
}