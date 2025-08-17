using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace GameEngine.UI
{
    public class UI_CreateDungeon : MonoBehaviour
    {
        [SerializeField] private Text seedText;

        public void OnClickConfirm()
        {
            var seedString = seedText.text;
            if (string.IsNullOrEmpty(seedString))
                GameData.Seed = GameUtility.GetRandomSeed();
            else if (int.TryParse(seedString, out var seedInt))
                GameData.Seed = seedInt;
            else
                GameData.Seed = seedString.GetHashCode();
                

            SceneManager.LoadScene("Dungeon");
        }
    }
}
