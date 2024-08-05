using AI;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Threading.Tasks;
using TetraCreations.Attributes;
using UnityEngine;
namespace ScenesScripts.QucikIM
{
    public class ImManager : MonoBehaviour
    {
        public class _ChatInfo
        {
            public string request;
            public string answer;
        }
        public static List<_ChatInfo> ChatInfo = new();

        public async Task<string> GetAnswer (string request)
        {
            var _content = await SparkDeskAPIManager.Tasker("���������������ң�����ߞ����ʤʤ�������ߣ������֣�Nanase Kurumi����ͨ�ƥ��إ�����menhera chan�������ɥ��祤�ͥåȵĻ�ʦ�ݤ���������LINE����������إ����󡣡�����������Ʒ�ĵǳ���ɫ��" +
               "��ɫ�趨������ľʵ��˫��̥���á�ϲ��������ʳ���е�С�񾭡�ϲ�����衢���֡�����¶����и��ܵ�����̫һ��" +
               $"���������ǵ���ʷ�Ի���¼ (json��ʽ�����û����Ϊ��json)��{JsonConvert.SerializeObject(ChatInfo)}" +
                $"��������㣺{request}����Ӧ����ô�ش�" + "�뾡����ʹ�ظ����������ֱ�Ӹ��һظ�����Ҫ���κε������������������������ҡ���ʱ����������AI���ش��в�Ҫ��������AI����˼��");
            AddHistory(request, _content);
            return _content;
        }
        public void AddHistory (string request, string answer)
        {
            ChatInfo.Add(new _ChatInfo { request = request, answer = answer });
            if (ChatInfo.Count >= 5) ChatInfo.RemoveAt(0);
        }

        [Button(nameof(ClearHistory), "�����ʷ��¼")]
        public void ClearHistory ()
        {
            ChatInfo.Clear();
        }
    }
}