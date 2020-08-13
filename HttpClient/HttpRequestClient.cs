using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace office365.Utils
{
    /// <summary>
    /// 用于以 POST 方式向目标地址提交表达数据
    /// 使用 application/x-www-form-urlencoded 编码方式
    /// 不支持上传文件, 若上传文件, 请使用<see cref="HttpPostFileRequestClient"/>
    /// </summary>
    public class HttpRequestClient
    {
        #region - Private -
        private List<KeyValuePair<string, string>> _postDatas;
        #endregion

        /// <summary>
        /// 获取或设置数据字符编码, 默认使用<see cref="System.Text.Encoding.UTF8"/>
        /// </summary>
        public Encoding Encoding { get; set; } = Encoding.UTF8;

        /// <summary>
        /// 获取或设置 UserAgent
        /// </summary>
        public string UserAgent { get; set; } = "Mozilla/5.0 (Windows NT 6.1; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/68.0.3440.106 Safari/537.36";

        /// <summary>
        /// 获取或设置 Accept
        /// </summary>
        public string Accept { get; set; } = "*/*";

        /// <summary>
        /// 获取或设置 Referer
        /// </summary>
        public string Referer { get; set; }

        public string ContentType { get; set; } = "application/x-www-form-urlencoded";

        /// <summary>
        /// Header值设置
        /// </summary>
        public Dictionary<string, string> dicion { get; set; }


        /// <summary>
        /// 获取或设置 Cookie 容器
        /// </summary>
        public CookieContainer CookieContainer { get; set; } = new CookieContainer();

        /// <summary>
        /// 初始化一个用于以 POST 方式向目标地址提交不包含文件表单数据<see cref="HttpRequestClient"/>实例
        /// </summary>
        public HttpRequestClient()
        {
            this._postDatas = new List<KeyValuePair<string, string>>();
        }

        /// <summary>
        /// 设置表单数据字段, 用于存放文本类型数据
        /// </summary>
        /// <param name="fieldName">指定的字段名称</param>
        /// <param name="fieldValue">指定的字段值</param>
        public void SetField(string fieldName, string fieldValue)
        {
            this._postDatas.Add(new KeyValuePair<string, string>(fieldName, fieldValue));
        }

        /// <summary>
        /// 以POST方式向目标地址提交表单数据
        /// </summary>
        /// <param name="url">目标地址, http(s)://sample.com</param>
        /// <returns>目标地址的响应</returns>
        public HttpWebResponse HttpPost(string url)
        {
            HttpWebRequest request = null;
            HttpWebResponse wbResponse = null;
            try
            {
                if (string.IsNullOrWhiteSpace(url))
                    throw new ArgumentNullException(nameof(url));

                if (url.ToLowerInvariant().StartsWith("https"))
                {
                    request = WebRequest.Create(url) as HttpWebRequest;
                    ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback((s, c, ch, ss) => { return true; });
                    request.ProtocolVersion = HttpVersion.Version11;
                    ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls;
                    request.KeepAlive = true;
                    ServicePointManager.CheckCertificateRevocationList = true; ServicePointManager.DefaultConnectionLimit = 100;
                    ServicePointManager.Expect100Continue = false;
                }
                else
                {
                    request = WebRequest.Create(url) as HttpWebRequest;
                }


                request.Method = "POST";
                request.ContentType = this.ContentType;
                request.UserAgent = this.UserAgent;
                request.Accept = this.Accept;
                request.Referer = this.Referer;
                request.CookieContainer = this.CookieContainer;
                if (dicion != null)
                {
                    //添加Headers
                    foreach (var item in dicion)
                    {
                        request.Headers[item.Key] = item.Value;
                    }
                }
                var postData = string.Join("&", this._postDatas.Select(p => $"{p.Key}={p.Value}"));

                using (var requestStream = request.GetRequestStream())
                {
                    var bytes = this.Encoding.GetBytes(postData);
                    requestStream.Write(bytes, 0, bytes.Length);
                }
                wbResponse = request.GetResponse() as HttpWebResponse;
                return wbResponse;
            }
            finally
            {
                wbResponse?.Close();
                request?.Abort();

            }
        }

        /// <summary>
        /// 以POST方式向目标地址提交表单数据
        /// </summary>
        /// <param name="url">目标地址, http(s)://sample.com</param>
        /// <returns>目标地址的响应</returns>
        public T HttpPost<T>(string url)
        {
            HttpWebRequest request = null;
            HttpWebResponse wbResponse = null;
            try
            {
                if (string.IsNullOrWhiteSpace(url))
                    throw new ArgumentNullException(nameof(url));

                if (url.ToLowerInvariant().StartsWith("https"))
                {
                    request = WebRequest.Create(url) as HttpWebRequest;
                    ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback((s, c, ch, ss) => { return true; });
                    request.ProtocolVersion = HttpVersion.Version11;
                    ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls;
                    request.KeepAlive = true;
                    ServicePointManager.CheckCertificateRevocationList = true; ServicePointManager.DefaultConnectionLimit = 100;
                    ServicePointManager.Expect100Continue = false;
                }
                else
                {
                    request = WebRequest.Create(url) as HttpWebRequest;
                }


                request.Method = "POST";
                request.ContentType = this.ContentType;
                request.UserAgent = this.UserAgent;
                request.Accept = this.Accept;
                request.Referer = this.Referer;
                request.CookieContainer = this.CookieContainer;
                if (dicion != null)
                {
                    //添加Headers
                    foreach (var item in dicion)
                    {
                        request.Headers[item.Key] = item.Value;
                    }
                }
                var postData = string.Join("&", this._postDatas.Select(p => $"{p.Key}={p.Value}"));

                using (var requestStream = request.GetRequestStream())
                {
                    var bytes = this.Encoding.GetBytes(postData);
                    requestStream.Write(bytes, 0, bytes.Length);
                }
                wbResponse = request.GetResponse() as HttpWebResponse;
                var htmlData = responseStream(wbResponse.GetResponseStream());
                return JsonConvert.DeserializeObject<T>(htmlData);
            }
            finally
            {
                wbResponse?.Close();
                request?.Abort();
            }
        }

        /// <summary>
        /// 以POST方式向目标地址提交表单数据
        /// </summary>
        /// <param name="url">目标地址, http(s)://sample.com</param>
        /// <returns>目标地址的响应</returns>
        public async Task<HttpWebResponse> PostAsync(string url)
        {
            if (string.IsNullOrWhiteSpace(url))
                throw new ArgumentNullException(nameof(url));

            HttpWebRequest request = null;
            if (url.ToLowerInvariant().StartsWith("https"))
            {
                request = WebRequest.Create(url) as HttpWebRequest;
                ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback((s, c, ch, ss) => { return true; });
                request.ProtocolVersion = HttpVersion.Version11;
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls;
                request.KeepAlive = true;
                ServicePointManager.CheckCertificateRevocationList = true; ServicePointManager.DefaultConnectionLimit = 100;
                ServicePointManager.Expect100Continue = false;
            }
            else
            {
                request = WebRequest.Create(url) as HttpWebRequest;
            }

            request.Method = "POST";
            request.ContentType = this.ContentType;
            request.UserAgent = this.UserAgent;
            request.Accept = this.Accept;
            request.Referer = this.Referer;
            request.CookieContainer = this.CookieContainer;
            if (dicion != null)
            {
                //添加Headers
                foreach (var item in dicion)
                {
                    request.Headers[item.Key] = item.Value;
                }
            }
            var postData = string.Join("&", this._postDatas.Select(p => $"{p.Key}={p.Value}"));

            using (var requestStream = await request.GetRequestStreamAsync())
            {
                var bytes = this.Encoding.GetBytes(postData);
                requestStream.Write(bytes, 0, bytes.Length);
            }
            return await request.GetResponseAsync() as HttpWebResponse;
        }
        /// <summary>
        /// 发送GET请求
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public HttpWebResponse HttpGet(string url)
        {
            HttpWebRequest request = null;
            HttpWebResponse wbResponse = null;
            try
            {
                if (string.IsNullOrWhiteSpace(url))
                    throw new ArgumentNullException(nameof(url));

                request = (HttpWebRequest)WebRequest.Create(url);
                request.Method = "GET";
                request.ContentType = this.ContentType;
                request.UserAgent = this.UserAgent;
                request.Accept = this.Accept;
                request.Referer = this.Referer;
                request.CookieContainer = this.CookieContainer;
                if (dicion != null)
                {
                    //添加Headers
                    foreach (var item in dicion)
                    {
                        request.Headers[item.Key] = item.Value;
                    }
                }

                wbResponse = (HttpWebResponse)request.GetResponse();
                //request.Abort();
                return wbResponse;
                //responseStream(wbResponse.GetResponseStream());
            }
            finally
            {
                // wbResponse?.Close();
                request?.Abort();
            }

        }

        /// <summary>
        /// 发送GET请求返回请求状态
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public HttpStatusCode HttpGetCode(string url)
        {
            HttpWebRequest request = null;
            HttpWebResponse wbResponse = null;
            try
            {
                if (string.IsNullOrWhiteSpace(url))
                    throw new ArgumentNullException(nameof(url));

                request = (HttpWebRequest)WebRequest.Create(url);
                request.Method = "GET";
                request.ContentType = this.ContentType;
                request.UserAgent = this.UserAgent;
                request.Accept = this.Accept;
                request.Referer = this.Referer;
                request.CookieContainer = this.CookieContainer;
                if (dicion != null)
                {
                    //添加Headers
                    foreach (var item in dicion)
                    {
                        request.Headers[item.Key] = item.Value;
                    }
                }

                wbResponse = (HttpWebResponse)request.GetResponse();
                //request.Abort();
                return wbResponse.StatusCode;
                //responseStream(wbResponse.GetResponseStream());
            }
            finally
            {
                wbResponse?.Close();
                request?.Abort();
            }

        }

        /// <summary>
        /// 发送GET请求（返回string类型）
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public string HttpGetRetStr(string url)
        {
            HttpWebRequest request = null;
            HttpWebResponse wbResponse = null;
            try
            {
                if (string.IsNullOrWhiteSpace(url))
                    throw new ArgumentNullException(nameof(url));

                request = (HttpWebRequest)WebRequest.Create(url);
                request.Method = "GET";
                request.ContentType = this.ContentType;
                request.UserAgent = this.UserAgent;
                request.Accept = this.Accept;
                request.Referer = this.Referer;
                request.CookieContainer = this.CookieContainer;
                if (dicion != null)
                {
                    //添加Headers
                    foreach (var item in dicion)
                    {
                        request.Headers[item.Key] = item.Value;
                    }
                }

                wbResponse = (HttpWebResponse)request.GetResponse();
                var htmlData = responseStream(wbResponse.GetResponseStream());
                return htmlData;
            }
            finally
            {
                wbResponse?.Close();
                request?.Abort();
            }

        }

        /// <summary>
        /// 发送GET请求
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public T HttpGet<T>(string url)
        {
            HttpWebRequest request = null;
            HttpWebResponse wbResponse = null;
            try
            {
                if (string.IsNullOrWhiteSpace(url))
                    throw new ArgumentNullException(nameof(url));

                request = (HttpWebRequest)WebRequest.Create(url);
                request.Method = "GET";
                request.ContentType = this.ContentType;
                request.UserAgent = this.UserAgent;
                request.Accept = this.Accept;
                request.Referer = this.Referer;
                request.CookieContainer = this.CookieContainer;
                if (dicion != null)
                {
                    //添加Headers
                    foreach (var item in dicion)
                    {
                        request.Headers[item.Key] = item.Value;
                    }
                }

                wbResponse = (HttpWebResponse)request.GetResponse();
                var htmlData = responseStream(wbResponse.GetResponseStream());
                return JsonConvert.DeserializeObject<T>(htmlData);
            }
            finally
            {
                wbResponse?.Close();
                request?.Abort();
            }

        }
        /// <summary>
        /// 获取网页内容
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public string responseStream(Stream str)
        {
            string result = string.Empty;
            try
            {
                using (Stream responseStream = str)
                {
                    using (StreamReader sReader = new StreamReader(responseStream))
                    {
                        result = sReader.ReadToEnd();
                    }
                }
                return result;
            }
            catch (Exception e)
            {

                throw e;
            }

        }

    }
}
