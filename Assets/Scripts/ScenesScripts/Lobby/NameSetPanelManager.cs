using Common;
using DG.Tweening;
using GameManager;
using UnityEngine;
using UnityEngine.UI;
namespace ScenesScripts.Lobby
{
    public class NameSetPanelManager : MonoBehaviour
    {
        public InputField InputField_Name;
        public RectTransform Frame;
        public void Button_Click_Set ()
        {
            if (InputField_Name.text.Trim() == string.Empty)
            {
                PopupManager.PopMessage("��ʾ", "��Ϸ������ǳơ�");
                return;
            }
            Frame.DOScale(0, 0.3f).OnComplete(() =>
            {
                GameDataManager.GameData.Name = InputField_Name.text;
                GameDataManager.SaveData();
                Destroy(this.gameObject);
            });
        }
    }

}
