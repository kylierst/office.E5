using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using office365.ResponseParam;
using office365.Utils;

namespace office365.Controllers
{
    public class SendController : BackgroundService
    {
        private readonly ILogger<SendController> _logger;
        private readonly IHostingEnvironment _hostingEnvironment;

        public SendController(ILogger<SendController> logger, IHostingEnvironment hostingEnvironment)
        {
            _logger = logger;
            _hostingEnvironment = hostingEnvironment;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    string token = refreshToken();
                    var urlArral = ConfigExtensions.GetListAppSettings<string>("url");
                    HttpRequestClient http = new HttpRequestClient();
                    http.ContentType = "application/json";
                    Dictionary<string, string> dictionary = new Dictionary<string, string>();
                    dictionary.Add("Authorization", "Bearer " + token);
                    http.dicion = dictionary;
                    foreach (var url in from url in urlArral let html = http.HttpGet(url) where html.StatusCode==HttpStatusCode.OK select url)
                    {
                        _logger.LogInformation("执行------>>>>>" + url + "-------<<<<成功");
                    }

                    //需要执行的任务
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex.Message);
                }
                
                await Task.Delay(Convert.ToInt32(ConfigExtensions.Configuration["Interval"]), stoppingToken);
            }
        }

        /// <summary>
        /// 刷新token
        /// </summary>
        /// <returns></returns>
        public string refreshToken()
        {
            try
            {
                string txtPath = _hostingEnvironment.ContentRootPath + "/refreshToken.txt"; ;
                HttpRequestClient http = new HttpRequestClient();
                http.SetField("client_id", ConfigExtensions.Configuration["ClientId"]);
                http.SetField("client_secret", ConfigExtensions.Configuration["ClientSecret"]);
                http.SetField("refresh_token", FileHelp.FileStream(txtPath));
                http.SetField("grant_type", "refresh_token");
                http.SetField("redirect_uri", ConfigExtensions.Configuration["Redirect_uri"]);
                ResponseToken token = 
                    http.HttpPost<ResponseToken>(ConfigExtensions.Configuration["RedirectTokenUrl"]);
                FileHelp.FileWriter(txtPath, token.refresh_token);
                return token.access_token;
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
            }

            return null;
        }
    }
}