using AI;
using Common;
using Common.Network;
using Common.UI;
using DG.Tweening;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using ScenesScripts.Lobby.AI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using TetraCreations.Attributes;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
namespace ScenesScripts.QucikIM
{
    public class ImManager : MonoBehaviour
    {
        [Title("���챳����")]
        public Transform BackContent;
        public class _ChatInfo
        {
            public string request;
            public string answer;
        }
        public static List<_ChatInfo> ChatInfo = new();
        public InputField InputField_ChatInfo;
        public static AudioSource MenheraAudioPlayer;
        public RectTransform Popup;

        public async Task<string> GetAnswer (string request)
        {
            var _content = await SparkDeskAPIManager.Tasker("������������ң�Ӣ����Menherachan��NanaseKurumi�����Ի�ʦ�ݤ��LINE����������إ����󡣡���" +
                "����Ը����Ȱ����ֺ����裬��ϲ���µ���ϲ�����֣��Ը���ÿɰ������Ǵ������Ƚ����У�����һ����������Ů�����151��������8��22�գ�ż�������񾭡���ǳ���ϲ���������������ľʵ��˫��̥���ã���һ���ܵ�����̫һ��" +
                // $"���ǵĶԻ���¼��JSON��ʽ����{JsonConvert.SerializeObject(ChatInfo)}" +
                $"��������㣺{request}�������ô�ش��أ����ÿ��ﻯ�ķ�ʽ�����ش𣬲�����30���֡�����������������һ������Ҫ�ᵽAI��");
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
        private void Update ()
        {
            LayoutRebuilder.ForceRebuildLayoutImmediate(BackContent.GetComponent<RectTransform>());
        }
        public async void Button_Click_Send ()
        {
            var _loading = new ShowLoading();
            try
            {

                Instantiate(Resources.Load<GameObject>("GameObject/Scene/UIMain/AI/List_Me"), BackContent).GetComponent<AIChatBubble>().Content.text = InputField_ChatInfo.text;
                var _menhera_b = Instantiate(Resources.Load<GameObject>("GameObject/Scene/UIMain/AI/List_Menhera"), BackContent).GetComponent<AIChatBubble>();
                _menhera_b.Content.text = "����˼����.......";
                _menhera_b.Content.text = await GetAnswer(InputField_ChatInfo.text);
                if (!Application.isMobilePlatform)
                {
                    var _audio = JsonConvert.DeserializeObject<JObject>(await NetworkHelp.Post($"{GameConst.API_URL}/ai/GetTTS", new { content = _menhera_b.Content.text }))["Audio"].ToString();
                    Debug.Log(_audio);
                    var _path = await Base64StringToFile(_audio);
                    StartCoroutine(PlayAudio(_path));
                }


                InputField_ChatInfo.text = string.Empty;
            }
            catch
            {
                PopupManager.PopMessage("����", "�������");
            }
            finally
            {
                _loading.KillLoading();
            }
        }
        private void Start ()
        {
            if (MenheraAudioPlayer is null) MenheraAudioPlayer = GameObject.Find("Menherachan/Menherachan_AIChat/Body").GetComponent<AudioSource>();
            GameObject.Find("AudioSystem/Audio_Back").GetComponent<AudioSource>().DOFade(0.25f, 1f);
        }
        /// <summary>
        /// Base64�ַ���ת�ļ�������
        /// </summary>
        /// <param name="base64String">base64�ַ���</param>
        /// <param name="fileName">������ļ���</param>
        /// <returns>�Ƿ�ת��������ɹ�</returns>
        public async Task<string> Base64StringToFile (string base64String)
        {
            try
            {
                var _path_d = $"{Application.persistentDataPath}/audio";
                if (!Directory.Exists(_path_d)) Directory.CreateDirectory(_path_d);
                string strbase64 = base64String.Trim().Substring(base64String.IndexOf(",") + 1);   //����������ǰ�Ķ����ַ���ɾ��
                MemoryStream stream = new MemoryStream(Convert.FromBase64String(strbase64));
                var _path = $"{_path_d}/{Guid.NewGuid().ToString()}.wav";
                FileStream fs = new FileStream(_path, FileMode.OpenOrCreate, FileAccess.Write);
                byte[] b = stream.ToArray();
                await fs.WriteAsync(b, 0, b.Length);
                fs.Close();
                return _path;
            }
            catch
            {
                return string.Empty;
            }


        }
        private IEnumerator PlayAudio (string fileName)
        {
            //��ȡ.wav�ļ�����ת��AudioClip
            UnityWebRequest www = UnityWebRequestMultimedia.GetAudioClip(fileName, AudioType.WAV);
            //�ȴ�ת�����
            yield return www.SendWebRequest();
            //��ȡAudioClip
            AudioClip audioClip = DownloadHandlerAudioClip.GetContent(www);


            //���õ�ǰAudioSource�����AudioClip
            MenheraAudioPlayer.clip = audioClip;
            //��������
            MenheraAudioPlayer.Play();
        }
        public void Button_Click_Close ()
        {
            GameObject.Find("AudioSystem/Audio_Back").GetComponent<AudioSource>().DOFade(1f, 1f);
            Popup.DOAnchorPos3D(new Vector3(-500, 0, 0), 0.5f).OnComplete(() =>
            {
                Destroy(this.gameObject);
            });
        }

    }
}