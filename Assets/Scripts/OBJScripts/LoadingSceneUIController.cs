using Common;
using Common.UI;
using System.Collections;
using System.Collections.Generic;
using TetraCreations.Attributes;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
namespace OBJScripts
{
    public class LoadingSceneUIController : MonoBehaviour
    {
        public AsyncOperation sceneLoadingOperation;
        private LoadingSceneUIController ()
        {

        }
        private static List<Sprite> Imgs = new();
        [Title("背景图片")]
        public Image LoadingIMG;


        [Title("加载提示")]
        public Text LoadingTitle;


        [Title("加载进度条")]
        public Scrollbar LoadingProgress;
        private bool isLoading;
        private string FormatRandomStr (string str) => $"<color=#EFD62E>你知道吗：</color>{str}";

        private static List<string> RandomStr = new()
        {
            "七濑胡桃的还有一个双胞胎妹妹哦！",
            "七濑胡桃的日语是“七瀬（ななせ）くるみ”，罗马字：Nanase Kurumi），通称メンヘラちゃん。",
            "胡桃的设定是喜欢暴饮暴食，有点小神经。喜欢泡澡、音乐。讨厌孤独。",
            "胡桃有个弟弟七濑太一。",
            "Line上胡桃的表情包每套价格为 50 Coins。",
            "胡桃是2018年出生的16岁少女哦！",
            "不用的话我就发病哦？ 因为啊我是menhera酱呀！请多多使用！爱你哟！♥",
            "花濑是2022年起家，2023年正式成立的哦。",
            "花濑名称的组成是“华为花粉”和“七濑胡桃”的结合哦。",
            "七濑木实的设定是肚子总是会空，有点小神经。喜欢安眠药、音乐。不喜欢吃香菜。",
            "七濑胡桃名字的来历是：郁娇（メンタルヘルス、mental health），简称为メンヘル（men-heal）。而去做心理健康检测的人，便在此基础上加上-er后缀，被称作メンヘラ（men-heal-er）。",
            "《七濑派对》游戏已经在github开源了哦。",
            "胡桃的官方介绍是：可愛いけどすぐに病んじゃう、メンヘラ少女くるみちゃん。（虽然很可爱，但是很快就会生病，七濑胡桃酱。）",
            "七濑胡桃的B站粉丝数突破70万了。",
            "胡桃还有“Menhera酱、古月木兆、有病酱”的别名。",
            "胡桃的生日是8月22日。",
            "胡桃是狮子座哦。"
        };
        private void Start ()
        {

            if (Imgs.Count == 0)
            {
                for (int i = 1; i < 24; i++)
                {
                    var _sprite = Resources.Load<Sprite>($"Texture2D/illustration/{i}");
                    Imgs.Add(_sprite);
                }
            }
            var _scale = LoadingIMG.gameObject.GetComponent<BGScaler>();
            var _random_sprite = Imgs[GameAPI.GetRandomInAB(0, Imgs.Count - 1)];
            _scale.textureOriginSize = new Vector2(_random_sprite.texture.width, _random_sprite.texture.height);
            LoadingIMG.sprite = _random_sprite;
            LoadingTitle.text = FormatRandomStr(RandomStr[GameAPI.GetRandomInAB(0, RandomStr.Count - 1)]);


        }
        public void Load (string name)
        {
            StartCoroutine(LoadScene(name));

        }
        public void Load (int id)
        {

        }
        private IEnumerator LoadScene (string name)
        {
            isLoading = true;
            sceneLoadingOperation = SceneManager.LoadSceneAsync(name);
            sceneLoadingOperation.allowSceneActivation = false;
            //


            yield return sceneLoadingOperation;

        }
        private void Update ()
        {
            if (!isLoading)
            {
                return;
            }
            LoadingProgress.value = sceneLoadingOperation.progress + 0.1f;
            if (sceneLoadingOperation.progress >= 0.95f || LoadingProgress.value >= 0.95f)
            {
                sceneLoadingOperation.allowSceneActivation = true;
            }
        }



    }
}
