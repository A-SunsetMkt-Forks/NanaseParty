using Common;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AI
{
    public class SparkDeskAPIManager
    {
        public static ClientWebSocket webSocket0;
        public static CancellationToken cancellation;
        // Ӧ��APPID������Ϊwebapi����Ӧ�ã�����ͨ�ǻ���֪��ģ����Ȩ��
        public const string x_appid = "546dd8fa";
        // �ӿ���Կ��webapi����Ӧ�ÿ�ͨ�ǻ���֪��ģ�ͺ󣬿���̨--�ҵ�Ӧ��---�ǻ���֪��ģ��---��Ӧ�����apikey��
        public const string api_secret = "MmQ3ZGU5OGFjZWFmNmNlNjE2ZTE2MGQ3";
        // �ӿ���Կ��webapi����Ӧ�ÿ�ͨ�ǻ���֪��ģ�ͺ󣬿���̨--�ҵ�Ӧ��---�ǻ���֪��ģ��---��Ӧ�����apisecret��
        public const string api_key = "c8f2dd3a73b7a5558cecaa0cfb8a430a";

        public static string hostUrl = "https://spark-api.xf-yun.com/v1.1/chat";
        public async static Task<string> Tasker (string requestText)
        {

            string authUrl = GetAuthUrl();
            string url = authUrl.Replace("http://", "ws://").Replace("https://", "wss://");
            using (webSocket0 = new ClientWebSocket())
            {
                try
                {
                    await webSocket0.ConnectAsync(new Uri(url), cancellation);
                    JsonRequest request = new JsonRequest();
                    request.header = new Header()
                    {
                        app_id = x_appid,
                        uid = GameAPI.GetRandomInAB(0, 999999).ToString(),
                    };
                    request.parameter = new Parameter()
                    {
                        chat = new Chat()
                        {
                            domain = "general",//ģ������Ĭ��Ϊ�ǻ�ͨ�ô�ģ��
                            temperature = 1,//�¶Ȳ�����ֵ�����ڿ����������ݵ�����ԺͶ����ԣ�ֵԽ�������Խ�ߣ���Χ��0��1��
                            max_tokens = 1024,//�������ݵ���󳤶ȣ���Χ��0��4096��
                        }
                    };
                    request.payload = new Payload()
                    {
                        message = new Message()
                        {
                            text = new List<Content>
                                                {
                                                    new Content() { role = "user", content = requestText},

                                                }
                        }
                    };

                    string jsonString = JsonConvert.SerializeObject(request);
                    //���ӳɹ�����ʼ��������


                    var frameData2 = System.Text.Encoding.UTF8.GetBytes(jsonString.ToString());


                    await webSocket0.SendAsync(new ArraySegment<byte>(frameData2), WebSocketMessageType.Text, true, cancellation);

                    // ������ʽ���ؽ�����н���
                    byte[] receiveBuffer = new byte[1024];
                    WebSocketReceiveResult result = await webSocket0.ReceiveAsync(new ArraySegment<byte>(receiveBuffer), cancellation);
                    String resp = "";
                    while (!result.CloseStatus.HasValue)
                    {
                        if (result.MessageType == WebSocketMessageType.Text)
                        {
                            string receivedMessage = Encoding.UTF8.GetString(receiveBuffer, 0, result.Count);
                            //���������Ϊjson

                            JObject jsonObj = JObject.Parse(receivedMessage);
                            int code = (int)jsonObj["header"]["code"];


                            if (0 == code)
                            {
                                int status = (int)jsonObj["payload"]["choices"]["status"];


                                JArray textArray = (JArray)jsonObj["payload"]["choices"]["text"];
                                string content = (string)textArray[0]["content"];
                                resp += content;

                                if (status != 2)
                                {
                                    Console.WriteLine($"�ѽ��յ����ݣ� {receivedMessage}");

                                }
                                else
                                {

                                    int totalTokens = (int)jsonObj["payload"]["usage"]["text"]["total_tokens"];
                                    return $"���巵�ؽ���� {resp}";
                                    //Console.WriteLine($"��������token���� {totalTokens}");

                                }

                            }
                            else
                            {
                                return $"���󱨴� {receivedMessage}";
                            }


                        }
                        else if (result.MessageType == WebSocketMessageType.Close)
                        {
                            return "�ѹر�WebSocket����";

                        }

                        result = await webSocket0.ReceiveAsync(new ArraySegment<byte>(receiveBuffer), cancellation);
                    }
                    return $"����";
                }
                catch (Exception e)
                {
                    return e.Message;
                }
            }
        }
        // ����codeΪ������ʱ�����ѯhttps://www.xfyun.cn/document/error-code�������
        static string GetAuthUrl ()
        {
            string date = DateTime.UtcNow.ToString("r");

            Uri uri = new Uri(hostUrl);
            StringBuilder builder = new StringBuilder("host: ").Append(uri.Host).Append("\n").//
                                    Append("date: ").Append(date).Append("\n").//
                                    Append("GET ").Append(uri.LocalPath).Append(" HTTP/1.1");

            string sha = HMACsha256(api_secret, builder.ToString());
            string authorization = string.Format("api_key=\"{0}\", algorithm=\"{1}\", headers=\"{2}\", signature=\"{3}\"", api_key, "hmac-sha256", "host date request-line", sha);
            //System.Web.HttpUtility.UrlEncode

            string NewUrl = "https://" + uri.Host + uri.LocalPath;

            string path1 = "authorization" + "=" + Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(authorization));
            date = date.Replace(" ", "%20").Replace(":", "%3A").Replace(",", "%2C");
            string path2 = "date" + "=" + date;
            string path3 = "host" + "=" + uri.Host;

            NewUrl = NewUrl + "?" + path1 + "&" + path2 + "&" + path3;
            return NewUrl;
        }




        public static string HMACsha256 (string apiSecretIsKey, string buider)
        {
            byte[] bytes = System.Text.Encoding.UTF8.GetBytes(apiSecretIsKey);
            System.Security.Cryptography.HMACSHA256 hMACSHA256 = new System.Security.Cryptography.HMACSHA256(bytes);
            byte[] date = System.Text.Encoding.UTF8.GetBytes(buider);
            date = hMACSHA256.ComputeHash(date);
            hMACSHA256.Clear();

            return Convert.ToBase64String(date);

        }








    }
    //����������
    public class JsonRequest
    {
        public Header header { get; set; }
        public Parameter parameter { get; set; }
        public Payload payload { get; set; }
    }

    public class Header
    {
        public string app_id { get; set; }
        public string uid { get; set; }
    }

    public class Parameter
    {
        public Chat chat { get; set; }
    }

    public class Chat
    {
        public string domain { get; set; }
        public double temperature { get; set; }
        public int max_tokens { get; set; }
    }

    public class Payload
    {
        public Message message { get; set; }
    }

    public class Message
    {
        public List<Content> text { get; set; }
    }

    public class Content
    {
        public string role { get; set; }
        public string content { get; set; }
    }
}