using Common;
using DG.Tweening;
using GameManager;
using System;
using System.Collections.Generic;
using TetraCreations.Attributes;
using UnityEngine;
using UnityEngine.UI;
namespace ScenesScripts.MiniGame.XiaoXiaoLe
{
    public class XiaoLeManager : MonoBehaviour
    {
        [Title("�÷����")]
        public GameObject GameBoard;

        [Title("��ʼ��Ϸ���")]
        public GameObject GameStarttingBoard;
        public static bool IsGaming;
        public Image Menhera_Img;
        private List<Sprite> Menhera_Medium = new();
        private void Awake ()
        {
            for (int i = 1; i <= 5; i++)
            {
                var a_ = Resources.Load<Sprite>($"Texture2D/Menhera/Medium/medium{i}");
                Menhera_Medium.Add(a_);
            }

        }
        public void Button_Click_Close ()
        {
            var _ = new LoadingSceneManager<string>("Game-Lobby");

        }
        public void ChangeMenhera ()
        {
            if (!IsGaming) return;
            Menhera_Img.sprite = Menhera_Medium[GameAPI.GetRandomInAB(0, Menhera_Medium.Count - 1)];

        }
        /// <summary>
        /// �޾�ģʽ
        /// </summary>
        /// <returns>���ؿ�ʼ��嶯���ص�</returns>
        public void StartGame_Infinity ()
        {
            Debug.Log("�޾�ģʽ");
            GameStarttingBoard.GetComponent<RectTransform>().DOAnchorPos3D(new Vector3(0f, -1000f, 0f), 0.5f).OnComplete(() =>
           {
               GameBoard.SetActive(true);
               var _ = Instantiate(Resources.Load<GameObject>("GameObject/Scene/MiniGame/XiaoXiaoLe/Play"));
               _.name = "Play";
               GamePanel.IntiTime = DateTime.Now;
               IsGaming = true;
           });

        }
        /// <summary>
        /// ��ʱģʽ
        /// </summary>
        public void StartGame_Limit ()
        {
            Debug.Log("��ʱģʽ");
            StartGame_Infinity();
            Invoke(nameof(Call_Limit), 60f);
        }

        [Button(nameof(Call_Limit), "��ʱģʽ�ص�")]
        public void Call_Limit ()
        {
            try
            {
                Destroy(GameObject.Find("Play"));
                GameStarttingBoard.GetComponent<RectTransform>().DOAnchorPos3D(new Vector3(0f, 0f, 0f), 0.5f).OnComplete(() =>
                {
                    GameBoard.SetActive(false);
                    PopupManager.PopMessage("��Ϸ����", $"���ĵ÷�Ϊ��{GamePanel.m_totalScore}");
                    GamePanel.m_totalScore = 0;
                    IsGaming = false;
                });
            }
            catch (Exception ex)
            {
                PopupManager.PopMessage("����", $"������Ϣ��{ex.Message}");
                Debug.LogError(ex.Message);
            }

        }
    }

}
