using UnityEngine;
namespace ScenesScripts.FocusClock
{
    public class HutaoClockStateManager : MonoBehaviour
    {
        /// <summary>
        /// ����0��animator�����ٶ�
        /// </summary>
        public void SetZeroSpeed ()
        {
            GameObject.Find("EventSystem").GetComponent<FocusClockManager>().SetZeroSpeed();
        }
    }

}
