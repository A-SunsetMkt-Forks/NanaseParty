using AI;
using UnityEngine;
using UnityEngine.UI;
namespace ScenesScripts.Test
{
    public class TestScript : MonoBehaviour
    {
        public Text text;
        async void Start ()
        {
            var _content = await SparkDeskAPIManager.Tasker("���������������ң�����ߞ����ʤʤ�������ߣ������֣�Nanase Kurumi����ͨ�ƥ��إ�����menhera chan�������ɥ��祤�ͥåȵĻ�ʦ�ݤ���������LINE����������إ����󡣡�����������Ʒ�ĵǳ���ɫ��" +
                "��ɫ�趨������ľʵ��˫��̥���á�ϲ��������ʳ���е�С�񾭡�ϲ�����衢���֡�����¶����и��ܵ�����̫һ��" +
                "$��������㣺����˭����Ӧ����ô�ƺ��㣿" +
                "��Ӧ����ô�ش�" +
                "�뾡����ʹ�ظ����������ֱ�Ӹ��һظ�����Ҫ���κε������������������������ҡ���ʱ����������AI���ش��в�Ҫ��������AI����˼��");
            text.text = _content;
        }

        // Update is called once per frame
        void Update ()
        {

        }
    }

}
