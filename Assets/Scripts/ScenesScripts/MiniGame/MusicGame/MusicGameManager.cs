using Common;
using DG.Tweening;
using GameManager;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TetraCreations.Attributes;
using UnityEngine;
using UnityEngine.UI;
namespace ScenesScripts.MiniGame.MusicGame
{
    public class MusicGameManager : MonoBehaviour
    {
        [Title("�������")]
        public Transform Panel;
        public AudioSource AudioPlayer;
        private static List<MusicBeat> Beats = new();
        public GameObject GamePanel_Mask;
        private static GameObject Food;
        public double TotalSeconds;
        public GameObject Button_Back;
        /// <summary>
        /// �÷�
        /// </summary>
        public Text Text_Scoring;
        /// <summary>
        /// �÷�
        /// </summary>
        public static long Score = 0;
        /// <summary>
        /// �Ƿ����ڽ���
        /// </summary>
        public static bool IsPlaying;
        /// <summary>
        /// ��ͣ���
        /// </summary>
        public static GameObject Obj_Pause;
        public void Start ()
        {
            try
            {
                GamePanel_Mask.SetActive(true);
                Food = Resources.Load<GameObject>("GameObject/Scene/MiniGame/MusicGame/Food");

                Debug.Log("��������" + Beats.Count);
            }
            catch (Exception ex)
            {
                PopupManager.PopMessage("����", ex.Message);
                AppLogger.Log(ex.Message);
                throw;
            }


        }
        public void Button_Click_StartGame (string id = "1")
        {
            try
            {
                if (ListItemController.SelectID == string.Empty)
                {
                    PopupManager.PopMessage("��ʾ", "��ѡ�������Ŀ�������ࡣ");
                    return;
                }
                Score = 0;
                GameObject.Find("MainCanvas/GamePanel-Mask/Panel").GetComponent<RectTransform>().DOAnchorPos3D(new Vector3(0f, -1000f, 0f), 0.5f).OnComplete(() =>
                {

                    IsPlaying = true;
                    Button_Back.SetActive(true);
                    Beats.Clear();
                    GamePanel_Mask.SetActive(false);
                    var _audioclip = Resources.Load<AudioClip>($"Audio/MenheraMusic/music/music ({ListItemController.SelectID})");
                    Beats = JsonConvert.DeserializeObject<List<MusicBeat>>(Resources.Load<TextAsset>($"Audio/MenheraMusic/bets/bets{ListItemController.SelectID}").text);
                    AudioPlayer.clip = _audioclip;
                    AudioPlayer.Play();
                    TotalSeconds = 0;
                    StartCoroutine(CreatBreats());
                    StartCoroutine(TimeEvent());

                });
            }
            catch (Exception ex)
            {
                PopupManager.PopMessage("����", ex.Message);
                AppLogger.Log(ex.Message);
                throw;
            }
        }
        private IEnumerator CreatBreats ()
        {

            while (true)
            {
                if (!IsPlaying)
                {
                    yield return new WaitForSeconds(0.1f);
                    continue;
                }
                try
                {

                    if (Beats.Count == 0) break;
                    var _beat = Beats.First();
                    var _time = DateTime.Now;

                    if (TotalSeconds - 0.0f + 1.30f >= _beat.seconds)
                    {
                        Beats.RemoveAt(0);
                        Debug.Log(_beat.seconds);
                        var _food = Instantiate(Food, Panel);
                        _food.GetComponent<FoodManager>().Create(_beat.direction);
                        continue;
                    }
                }
                catch (Exception ex)
                {
                    PopupManager.PopMessage("����", ex.Message);
                    AppLogger.Log(ex.Message);
                    throw;
                }
                yield return new WaitForSeconds(0.005f);
            }

            while (AudioPlayer.isPlaying)
            {
                yield return new WaitForSeconds(0.005f);
            }
            MusicOver();

        }

        /// <summary>
        /// ���Ž���
        /// </summary>
        [Button(nameof(MusicOver), "ֱ�Ӳ������")]
        public void MusicOver ()
        {
            IsPlaying = false;
            Button_Back.SetActive(false);
            StopAllCoroutines();
            Instantiate(Resources.Load<GameObject>("GameObject/Scene/MiniGame/MusicGame/OverCanvas"));
        }
        private IEnumerator TimeEvent ()
        {
            while (true)
            {
                if (!IsPlaying)
                {
                    yield return new WaitForSeconds(0.1f);
                    continue;
                }

                Tween tween = DOTween.To(() => TotalSeconds, x => TotalSeconds = x, TotalSeconds + 1, 1f).SetEase(Ease.Linear);
                yield return new WaitForSeconds(1);
            }
        }
        public void Button_Click_Close ()
        {
            var _ = new LoadingSceneManager<string>("Game-Lobby");
        }
        private void Update ()
        {
            if (!IsPlaying)
            {
                AudioPlayer.Pause();
                return;
            }

            Text_Scoring.text = $"�÷� : {Score} ��";
        }
        public void Button_Click_Pause ()
        {
            if (!IsPlaying) return;
            IsPlaying = false;
            Obj_Pause = Instantiate(Resources.Load<GameObject>("GameObject/Scene/MiniGame/MusicGame/PauseCanvas"));

        }
    }
}


