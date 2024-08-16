using GameManager;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.IO;
using TetraCreations.Attributes;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
namespace ScenesScripts.Lobby
{
    public class LobbyManager : MonoBehaviour
    {

        [Title("�øжȽ��ȱ�ǩ")]
        public Text Text_LoveProgress;

        [Title("�û���")]
        public Text Text_UserName;

        [Title("�øжȻ���")]
        public Slider Slider_Love;

        [Title("��Ǯ��ǩ")]
        public TextMeshProUGUI Text_Money;

        [Title("������_���������")]
        public LobbyCamerManager C_LobbyCamerManager;
        private void Update ()
        {
            //GameManager.ServerManager.Config.GameCommonConfig();


        }
        private void FixedUpdate ()
        {
            Text_Money.text = GameDataManager.GameData.Money.ToString();
            Slider_Love.value = GameDataManager.GameData.LoveLevel;
            Text_LoveProgress.text = $"{GameDataManager.GameData.LoveLevel * 100}/100";
            Text_UserName.text = GameDataManager.GameData.Name;
        }
        private async void Start ()
        {
            var _path = $"{Application.persistentDataPath}/gameData.json";
            if (!File.Exists(_path))
            {
                Instantiate(Resources.Load<GameObject>("GameObject/Scene/UIMain/NameSetCanvas")); return;
            }

            if (JsonConvert.DeserializeObject<JObject>(await File.ReadAllTextAsync(_path))["Name"].ToString() == string.Empty)
            {
                Instantiate(Resources.Load<GameObject>("GameObject/Scene/UIMain/NameSetCanvas"));
            }
        }


    }

}
