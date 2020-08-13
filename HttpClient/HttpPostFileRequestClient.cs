using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Text;
using System.Threading.Tasks;

namespace office365.Utils
{
    /// <summary>
    /// 用于以 POST 方式向目标地址提交表单数据, 仅适用于包含文件的请求
    /// </summary>
    public sealed class HttpPostFileRequestClient
    {
        #region - Private -
        private string _boundary;
        private List<byte[]> _postDatas;
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
        /// <summary>
        /// Header值设置
        /// </summary>
        public Dictionary<string,string>  dicion { get; set; }

        /// <summary>
        /// 获取或设置 Cookie 容器
        /// </summary>
        public CookieContainer CookieContainer { get; set; } = new CookieContainer();

        /// <summary>
        /// 初始化一个用于以 POST 方式向目标地址提交表单数据的<see cref="HttpPostFileRequestClient"/>实例
        /// </summary>
        public HttpPostFileRequestClient()
        {
            this._boundary = DateTime.Now.Ticks.ToString("X");
            this._postDatas = new List<byte[]>();
        }

        /// <summary>
        /// 设置表单数据字段, 用于存放文本类型数据
        /// </summary>
        /// <param name="fieldName">指定的字段名称</param>
        /// <param name="fieldValue">指定的字段值</param>
        public void SetField(string fieldName, string fieldValue)
        {
            var field = $"--{this._boundary}\r\n" +
                        $"Content-Disposition: form-data;name=\"{fieldName}\"\r\n\r\n" +
                        $"{fieldValue}\r\n";
            this._postDatas.Add(this.Encoding.GetBytes(field));
        }

        /// <summary>
        /// 设置表单数据字段, 用于文件类型数据
        /// </summary>
        /// <param name="fieldName">字段名称</param>
        /// <param name="fileName">文件名</param>
        /// <param name="contentType">内容类型, 传入 null 将默认使用 application/octet-stream</param>
        /// <param name="fs">文件流</param>
        public void SetField(string fieldName, string fileName, string contentType, Stream fs)
        {
            var fileBytes = new byte[fs.Length];
            using (fs)
            {
                fs.Read(fileBytes, 0, fileBytes.Length);
            }
            SetField(fieldName, fileName, contentType, fileBytes);
        }

        /// <summary>
        /// 设置表单数据字段, 用于文件类型数据
        /// </summary>
        /// <param name="fieldName">字段名称</param>
        /// <param name="fileName">文件名</param>
        /// <param name="contentType">内容类型, 传入 null 将默认使用 application/octet-stream</param>
        /// <param name="fileBytes">文件字节数组</param>
        public void SetField(string fieldName, string fileName, string contentType, byte[] fileBytes)
        {
            var field = $"--{this._boundary}\r\n" +
                        $"Content-Disposition: form-data; name=\"{fieldName}\";filename=\"{fileName}\"\r\n" +
                        $"Content-Type:{contentType ?? "application/octet-stream"}\r\n\r\n";
            this._postDatas.Add(this.Encoding.GetBytes(field));
            this._postDatas.Add(fileBytes);
            this._postDatas.Add(this.Encoding.GetBytes("\r\n"));
        }

        /// <summary>
        /// 以POST方式向目标地址提交表单数据
        /// </summary>
        /// <param name="url">目标地址, http(s)://sample.com</param>
        /// <returns>目标地址的响应</returns>
        public HttpWebResponse Post(string url)
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
            request.ContentType = "multipart/form-data;boundary=" + _boundary;
            request.UserAgent = this.UserAgent;
            request.Accept = this.Accept;
            request.Referer = this.Referer;
            request.CookieContainer = this.CookieContainer;
            //添加Headers
            foreach (var item in dicion)
            {
                request.Headers[item.Key] = item.Value;
            }
            var end = $"--{this._boundary}--\r\n";
            this._postDatas.Add(this.Encoding.GetBytes(end));

            var requestStream = request.GetRequestStream();
            foreach (var item in this._postDatas)
            {
                requestStream.Write(item, 0, item.Length);
            }
            return request.GetResponse() as HttpWebResponse;
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
            request.ContentType = "multipart/form-data;boundary=" + _boundary;
            request.UserAgent = this.UserAgent;
            request.Accept = this.Accept;
            request.Referer = this.Referer;
            request.CookieContainer = this.CookieContainer;
            //添加Headers
            foreach (var item in dicion)
            {
                request.Headers[item.Key] = item.Value;
            }
            var end = $"--{this._boundary}--\r\n";
            this._postDatas.Add(this.Encoding.GetBytes(end));

            var requestStream = await request.GetRequestStreamAsync();
            foreach (var item in this._postDatas)
            {
                await requestStream.WriteAsync(item, 0, item.Length);
            }
            return await request.GetResponseAsync() as HttpWebResponse;
        }
    }
}
