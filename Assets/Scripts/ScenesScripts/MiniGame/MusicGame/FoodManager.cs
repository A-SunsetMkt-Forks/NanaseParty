using Common;
using DG.Tweening;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace ScenesScripts.MiniGame.MusicGame
{
    public class FoodManager : MonoBehaviour
    {
        private static List<Sprite> FoodsIMG = new();
        private static GameObject BoomClickEffice;
        public Image FoodImgObj;

        public string Direction;
        private bool IsClick;
        private Tweener Event_AutoKill;

        private void Start ()
        {
            if (BoomClickEffice == null) BoomClickEffice = Resources.Load<GameObject>("GameObject/Scene/MiniGame/MusicGame/BoomClickEffice");
            if (FoodsIMG.Count == 0)
            {
                for (int i = 1; i <= 7; i++)
                {
                    FoodsIMG.Add(Resources.Load<Sprite>($"Texture2D/MusicGame/food{i}"));
                }
            }
            FoodImgObj.sprite = FoodsIMG[GameAPI.GetRandomInAB(0, FoodsIMG.Count - 1)];
            // ����ʳ��ͼƬ��RectTransform����Ļ�����λ��
            Event_AutoKill = this.FoodImgObj.DOColor(Color.white, 2f).OnComplete(() =>
            {
                //�����ɫ�仯û���ã�ֻ��Ϊ��0.72f���١�
                Destroy(this.gameObject);
            });

        }
        public void Button_Click_Xiao ()
        {
            Event_AutoKill.Kill();
            var _obj = Instantiate(BoomClickEffice, this.transform.parent);
            _obj.transform.position = this.transform.position;

            Destroy(this.gameObject);
        }
        public void Creat (int direction)
        {
            Direction = direction == 1 ? "���" : "�Ҳ�";
            // ��ȡ��Ļ�Ŀ�Ⱥ͸߶�

            float screenWidth = Screen.width - 200f; // ���������С
            float screenHeight = Screen.height - 200f;

            float randomX; float randomY;
            if (direction == 1)
            {
                // ���������
                randomX = GameAPI.GetRandomInAB(0, Convert.ToInt32(screenWidth / 2));
                randomY = GameAPI.GetRandomInAB(0, Convert.ToInt32(screenHeight));
            }
            else if (direction == 2)
            {
                // �������Ҳ�
                randomX = GameAPI.GetRandomInAB(Convert.ToInt32(screenWidth / 2), Convert.ToInt32(screenWidth));
                randomY = GameAPI.GetRandomInAB(0, Convert.ToInt32(screenHeight));
            }
            else
            {
                return;
            }
            //float objectCenterX = randomX - (this.gameObject.GetComponent<RectTransform>().rect.width / 2);
            // ����FoodImgObj��RectTransformλ��
            this.gameObject.GetComponent<RectTransform>().position = new Vector3(randomX, randomY, 0);
        }
    }
}