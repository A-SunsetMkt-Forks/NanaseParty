using Common;
using Common.Network;
using Common.UI;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;
namespace GameManager
{
    public class GameDataManager : MonoBehaviour
    {
        public class Model_PlotSatus
        {
            public string id { get; set; } = string.Empty;
            public bool isDone { get; set; } = false;
        }
        public class Model_UserInfo
        {
            /// <summary>
            /// ��������
            /// </summary>
            public DateTime DataTime { get; set; }
            /// <summary>
            /// ����
            /// </summary>
            public string Name { get; set; } = string.Empty;
            /// <summary>
            /// �øеȼ�
            /// </summary>
            public float LoveLevel { get; set; } = 0f;

            /// <summary>
            /// �������
            /// </summary>
            public long Money { get; set; } = 0;
            public List<Model_PlotSatus> PlotData { get; set; } = new();
        }

        public static Model_UserInfo GameData = new();
        private static readonly object lockObject = new object();
        /// <summary>
        /// Json����Ϸ����
        /// </summary>
        public static string JsonGameData;
        private static string Path;
        public static int ErrorTimes;
        private static ShowLoading Loading;
        public async void Awake ()
        {
            try
            {
                Path = $"{Application.persistentDataPath}/gameData.json";
                if (File.Exists(Path))
                {
                    var _data_json = await File.ReadAllTextAsync(Path);
                    var _data = JsonConvert.DeserializeObject<Model_UserInfo>(_data_json);
                    if ((GameManager.ServerManager.Config.GameCommonConfig.GetValue("UserInfo", "LoginType") == "account" && GameManager.ServerManager.Config.GameCommonConfig.GetValue("UserInfo", "IsLogin") == "True"))
                    {
                        var _server_json = JsonConvert.DeserializeObject<JObject>(await GetServerData());

                        if (_server_json["status"].ToString() == "success")
                        {
                            var _server_data = JsonConvert.DeserializeObject<Model_UserInfo>(_server_json["value"].ToString());

                            if (_server_data.DataTime > DateTime.Now)//���������ݽ���
                                GameData = _server_data;
                            else GameData = _data;
                        }
                        else
                        {
                            throw new Exception("�������쳣");
                        }
                    }
                    else
                    {
                        //δ��¼���߱��ص�¼
                        GameData = _data;
                    }
                }
                else
                {
                    var _data = await GetServerData();
                    if (JsonConvert.DeserializeObject<JObject>(_data)["status"].ToString() == "success")
                    {
                        GameData = JsonConvert.DeserializeObject<Model_UserInfo>(JsonConvert.DeserializeObject<JObject>(_data)["value"].ToString());
                        PopupManager.PopMessage("ͬ���ɹ�", "�ɹ�ͬ�����ݡ�");
                    }
                    else
                    {
                        if ((GameManager.ServerManager.Config.GameCommonConfig.GetValue("UserInfo", "LoginType") == "account" && GameManager.ServerManager.Config.GameCommonConfig.GetValue("UserInfo", "IsLogin") == "True"))
                        {
                            //�Ѿ���¼
                            throw new Exception("�������쳣");
                        }
                        GameData = new();
                    }
                }
            }
            catch (Exception ex)
            {
                PopupManager.PopMessage("����", $"�������ݴ���������������ͷ�����ʹ�ñ������ݡ�������Ϣ:{ex.Message}");
            }
            finally
            {
                StartCoroutine(SynchronizationData());//ͬ��������ѯ
                Loading = new ShowLoading();
                Loading.SetActive(false);
            }
        }
        public async Task<string> GetServerData ()
        {
            return await NetworkHelp.Post($"{GameConst.API_URL}/player/GetUserData",
                              new
                              {
                                  token = ServerManager.Config.GameCommonConfig.GetValue("UserInfo", "Token"),
                                  key = "GameUserJsonData"
                              });
        }

        public static void SaveData ()
        {
            RemoveDuplicatePlots();
            GameData.DataTime = DateTime.Now;
            JsonGameData = JsonConvert.SerializeObject(GameData);
            Debug.Log(JsonGameData);
            var _file = FileManager.CreatTextFile(Path);
            _file.Write(JsonGameData);
            _file.Dispose();
        }
        /// <summary>
        /// ����ȥ��
        /// </summary>
        /// <returns></returns>
        public static void RemoveDuplicatePlots ()
        {
            var uniquePlotDataById = new Dictionary<string, Model_PlotSatus>();

            foreach (var plot in GameData.PlotData)
            {
                if (!uniquePlotDataById.ContainsKey(plot.id))
                {
                    uniquePlotDataById[plot.id] = plot;
                }
                else
                {
                    if (plot.isDone == true)
                    {
                        uniquePlotDataById[plot.id] = plot;
                    }
                }
            }

            GameData.PlotData = uniquePlotDataById.Values.ToList();
        }
        private IEnumerator SynchronizationData ()
        {
            while (true)
            {
                yield return new WaitForSeconds(3);
                Loading.SetActive(false);
                SaveData();
                var _token = ServerManager.Config.GameCommonConfig.GetValue("UserInfo", "Token");
                if (!(GameManager.ServerManager.Config.GameCommonConfig.GetValue("UserInfo", "LoginType") == "account" && GameManager.ServerManager.Config.GameCommonConfig.GetValue("UserInfo", "IsLogin") == "True")) continue;
                var _data = "";
                var form = new WWWForm();
                form.AddField("token", _token);
                form.AddField("key", $"GameUserJsonData");
                form.AddField("value", Encrypt(JsonGameData, _token));
                using UnityWebRequest www = UnityWebRequest.Post($"{GameConst.API_URL}/player/SynchronizeData", form);
                yield return www.SendWebRequest();
                try
                {
                    if (www.result == UnityWebRequest.Result.Success)
                    {
                        _data = www.downloadHandler.text;
                        if (JsonConvert.DeserializeObject<JObject>(_data)["status"].ToString() != "success")
                        {
                            throw new Exception("��¼��ϢʧЧ��");
                        }
                    }
                    else
                    {

                        throw new Exception("����ʧ�ܡ�");
                    }
                }
                catch (Exception ex)
                {
                    AppLogger.Log(ex.Message, "error");
                    ErrorTimes++;
                    Loading.SetActive(true);

                    if (ErrorTimes >= 5)
                    {
                        PopupManager.PopMessage("����", "�������ͨѶʧ�ܣ�ͬ���浵��ʧЧ����ǳ��˺����µ�¼��������Ϣ��" + ex.Message);
                        ErrorTimes = 0;
                    }
                }

                Debug.Log("ͬ�����:" + _data);
            }
        }
        /// <summary>
        /// �ı�_����
        /// </summary>
        /// <param name="str">�����ܵ��ı�</param>
        /// <param name="pass">���ܵ�����</param>
        /// <returns></returns>
        public static string Encrypt (string str, string pass, Encoding encoding = null)
        {
            encoding ??= Encoding.UTF8;

            byte[] bin = encoding.GetBytes(str);
            List<byte> list = new();
            for (int i = 0; i < bin.Length; i++)
            {
                var ch = (byte)(bin[i] ^ 3600);
                list.Add(ch);
            }

            string md5 = ComputeMD5Hash(pass).Substring(2, 9);

            string hex = ByteToHex(list.ToArray());


            return hex + md5.ToUpper();
        }
        public static string ByteToHex (byte[] bytes)
        {
            string returnStr = "";
            if (bytes != null)
            {
                for (int i = 0; i < bytes.Length; i++)
                {
                    returnStr += bytes[i].ToString("X2");
                }
            }
            return returnStr;
        }
        public static string ComputeMD5Hash (string input)
        {


            //���ַ�����UTF-8��ʽתΪbyte����

            byte[] resultBytes = Encoding.UTF8.GetBytes(input);

            //����һ��MD5�Ķ���

            MD5 md5 = new MD5CryptoServiceProvider();

            //����MD5��ComputeHash�������ֽ��������

            byte[] outPut = md5.ComputeHash(resultBytes);

            StringBuilder hashString = new StringBuilder();

            //���Ѽ��ܺ���ֽ�����תΪ�ַ���
            for (int i = 0; i < outPut.Length; i++)
            {
                hashString.Append(Convert.ToString(outPut[i], 16).PadLeft(2, '0'));
            }
            return hashString.ToString();

        }

    }
}