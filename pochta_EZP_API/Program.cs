using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RestSharp;
using System.Net.Http;
using RestSharp.Serialization;
using System.Net.Http.Headers;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Net;
using System.IO;

namespace pochta_EZP_API
{
    class Program
    {
        public static string token = "RSD0UpdAu5tSZ4gC18yiYI9bz1SQLSq0";
        //public static string auth = Base64Encode("m.rogencov@filbert.pro:Gtxfkm2020");

        public static string auth = Base64Encode("UD@filbert.pro:Abk,thn2020");
        public static string host = string.Empty;
        static void Main(string[] args)
        {



            if (args.Length == 0)
            {
                Console.WriteLine("Укажите хост");
                return;
            }
            host = args[0];
            
            
            
            // Отпроавление письма
           // MultipartDataUpload.reqREST(args[0] + "1.0/erl/send");

            JObject json = JObject.Parse(@"{""request-code"": ""878c2fd1-0abb-454a-a1d7-e2631f12a325""}");

            // Статус
            // Console.WriteLine(GetStatus(json));

            //  Поиск
            SearchID(json);

            //форма 103
          //  Console.WriteLine(Get103F(json));


            Console.In.Read();
        }

        static string Get103F(JObject json)
        {

            string url = host + @"/1.0/erl/forms/f103-by-id/?id=" + json["request-code"].ToString();
            return req(url, "GET");

        }

        static string SearchID(JObject json)
        {

            string url = host + @"/1.0/erl/shipment/find-by-id/?id=23156";
            return req(url, "GET");

        }

        static string GetStatus(JObject json)
        {

            string url = host + "/1.0/erl/status/?request-code=" + json["request-code"].ToString();
            return req(url, "GET");

        }

        private static string req(string url, string method, JObject json=null)
        {


            byte[] body = (method == "POST" ?Encoding.Default.GetBytes("[" + json.ToString() + "]"):null);

            Uri uri = new Uri(url);


            //  string u = HttpUtility.UrlEncode(par);

            var httpWebRequest = (HttpWebRequest)WebRequest.Create(url);

            httpWebRequest.ContentType = @"application/json; charset=utf-8";
            httpWebRequest.Method = method;
            httpWebRequest.Host = uri.Host;
            httpWebRequest.Accept = @"*/*";
            


            //  httpWebRequest.Timeout = 600000;
            httpWebRequest.KeepAlive = true;


                       WebHeaderCollection myWebHeaderCollection = httpWebRequest.Headers;
            myWebHeaderCollection.Add("Authorization: AccessToken " + token);
            myWebHeaderCollection.Add("X-User-Authorization: Basic " + auth);
           // myWebHeaderCollection.Add("Accept-Encoding: gzip, deflate, br");
           // myWebHeaderCollection.Add("Accept-Language", "ru-RU");


            //  httpWebRequest.CookieContainer = new CookieContainer();
            //  httpWebRequest.CookieContainer.Add(cookie);
            //  httpWebRequest.ContentLength = body.Length;
            /*
              using (Stream stream = httpWebRequest.GetRequestStream())
              {
                  stream.Write(body, 0, body.Length);
                  stream.Close();
              }

              */

            if (body != null)
            {

                httpWebRequest.ContentLength = body.Length;

                using (Stream stream = httpWebRequest.GetRequestStream())
                {
                    stream.Write(body, 0, body.Length);
                    stream.Close();
                }

            }




            string orig = string.Empty;
            try
            {
                var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();


                using (StreamReader streamReader = new StreamReader(httpResponse.GetResponseStream(), Encoding.GetEncoding("UTF-8"), true))
                {
                    //ответ от сервера

                    orig = streamReader.ReadToEnd();
                    //  if (typ == 1) goto end;
                    ///////////////////////////////////////////ПИШЕМ ответ в файл//////////////////////////////////////////
                    System.IO.StreamWriter sw = null;
                    string path = DateTime.Now.ToString();
                    char[] denied = new[] { ':', '.', ' ' };
                    StringBuilder newString = new StringBuilder();
                    foreach (var ch in path)
                        if (!denied.Contains(ch))
                            newString.Append(ch);
                    path = newString.ToString();



                    path = "c:\\temp\\pochta_ru\\work\\post_ezp_" + path + ".json";
                    System.IO.FileStream fs = new System.IO.FileStream(path, System.IO.FileMode.Append, System.IO.FileAccess.Write);     //, System.IO.FileShare.ReadWrite

                    sw = new System.IO.StreamWriter(fs, Encoding.GetEncoding("UTF-8"));
                    sw.WriteLine(orig);
                    sw.Close();

                    fs.Dispose();
                    fs.Close();

                    ///////////////////////////////////////////
                }
            }

            catch (Exception ex)
            {
                Console.Out.Write(ex.Message);

                if (ex.Message.Contains("(404)"))
                    //Console.Out.Write(ex.Message);
                    return "(404)";
                else if (ex.Message.Contains("(429)"))
                    return "(429)";
                else if (ex.Message.Contains("(400)"))
                {

                    return "(400)";
                }

            }



            // end:


            return orig;





        }






        public static string Base64Encode(string plainText)
        {
            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(plainText);
            return System.Convert.ToBase64String(plainTextBytes);
        }
        // раскодировать
        public static string Base64Decode(string base64EncodedData)
        {
            var base64EncodedBytes = System.Convert.FromBase64String(base64EncodedData);
            return System.Text.Encoding.UTF8.GetString(base64EncodedBytes);
        }

    }

    class MultipartDataUpload
    {
       



        public static object reqREST(string url)
        {

            Uri uri = new Uri(url);

            var client = new RestClient(uri);
            //client.Timeout = -1;
            var request = new RestRequest(Method.POST);


            request.AddHeader("Content-Type", "multipart/form-data");
            request.AddHeader("Accept", "*/*");
            request.AddHeader("Accept-Encoding", "gzip, deflate, br");

            request.AddHeader("X-User-Authorization", "Basic " + Program.auth);
            request.AddHeader("Authorization", "AccessToken " + Program.token);



            request.AddFile("shipment-info", "C:/TEMP/pochta_ru/zapros/shipment-info.json", "application/json");
            request.AddFile("attachment", "C:/TEMP/pochta_ru/zapros/32159.pdf");
            request.AddFile("attachment-signature", "C:/TEMP/pochta_ru/zapros/32159.pdf.sig");

            // request.AlwaysMultipartFormData = true;

            //request.AddParameter("application/json; charset=utf-8", content, ParameterType.RequestBody);
            // request.RequestFormat = DataFormat.Json;
            // request.Timeout = 600 * 1000;

            IRestResponse response = client.Execute(request);

            if (response.StatusCode != HttpStatusCode.OK)
            {
                if (response.Content == null)
                    return $"Ошибка {response.StatusCode} при обращении к серверу";
            }

            var responseString = response.Content;

            return JObject.Parse(responseString);




        }

       

    }
}
