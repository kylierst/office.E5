using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Json;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace office365.Utils
{
    /// <summary>
    /// 读取配置文件信息
    /// </summary>
    public class ConfigExtensions
    {

        public static IConfiguration Configuration { get; set; }
        public ConfigExtensions(string contentPath)
        {
            string Path = "appsettings.json";

            //如果你把配置文件 是 根据环境变量来分开了，可以这样写
            //Path = $"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")}.json";

            Configuration = new ConfigurationBuilder()
                .SetBasePath(contentPath)
                .Add(new JsonConfigurationSource { Path = Path, Optional = false, ReloadOnChange = true })//这样的话，可以直接读目录里的json文件，而不是 bin 文件夹下的，所以不用修改复制属性
                .Build();
        }
        /// <summary>
        /// 获得配置文件的对象值
        /// </summary>
        /// <param name="jsonPath"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static string GetJson(string jsonPath, string key)
        {
            if (string.IsNullOrEmpty(jsonPath) || string.IsNullOrEmpty(key)) return null;
            IConfiguration config = new ConfigurationBuilder().AddJsonFile(jsonPath).Build();//json文件地址
            return config.GetSection(key).Value;//json某个对象
        }

        /// <summary>
        /// 获取自定义配置文件配置
        /// </summary>
        /// <typeparam name="T">配置模型</typeparam>
        /// <param name="key">根节点</param>
        /// <param name="configPath">配置文件名称</param>
        /// <returns></returns>
        public static T GetAppSettingss<T>(string key) where T : class, new()
        {
            var appconfig = new ServiceCollection()
                .AddOptions()
                .Configure<T>(Configuration.GetSection(key))
                .BuildServiceProvider()
                .GetService<IOptions<T>>()
                .Value;
            return appconfig;
        }

        /// <summary>
        /// 获取自定义配置文件配置
        /// </summary>
        /// <typeparam name="T">配置模型</typeparam>
        /// <param name="key">根节点</param>
        /// <param name="configPath">配置文件名称</param>
        /// <returns></returns>
        public static List<T> GetListAppSettings<T>(string key)
        {
            var appconfig = new ServiceCollection()
                .AddOptions()
                .Configure<List<T>>(Configuration.GetSection(key))
                .BuildServiceProvider()
                .GetService<IOptions<List<T>>>()
                .Value;
            return appconfig;
        }



        /// <summary>
        /// 根据配置文件和Key获得对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="fileName">文件名称</param>
        /// <param name="key">节点Key</param>
        /// <returns></returns>
        public static T GetAppSettings<T>(string fileName, string key) where T : class, new()
        {
            if (string.IsNullOrEmpty(fileName) || string.IsNullOrEmpty(key)) return null;
            var baseDir = AppContext.BaseDirectory + "json/";
            var currentClassDir = baseDir;

            IConfiguration config = new ConfigurationBuilder()
                .SetBasePath(currentClassDir)
                .Add(new JsonConfigurationSource
                {
                    Path = fileName,
                    Optional = false,
                    ReloadOnChange = true
                }).Build();
            var appconfig = new ServiceCollection().AddOptions()
               .Configure<T>(config.GetSection(key))
               .BuildServiceProvider()
               .GetService<IOptions<T>>()
               .Value;
            return appconfig;
        }


        /// <summary>
        /// 获取自定义配置文件配置
        /// </summary>
        /// <typeparam name="T">配置模型</typeparam>
        /// <param name="key">根节点</param>
        /// <param name="configPath">配置文件名称</param>
        /// <returns></returns>
        public T GetAppSettingss<T>(string key, string configPath) where T : class, new()
        {
            if (string.IsNullOrEmpty(key) || string.IsNullOrEmpty(configPath)) return null;
            IConfiguration config = new ConfigurationBuilder().Add
                (new JsonConfigurationSource { Path = configPath, ReloadOnChange = true }).Build();
            var appconfig = new ServiceCollection()
                .AddOptions()
                .Configure<T>(config.GetSection(key))
                .BuildServiceProvider()
                .GetService<IOptions<T>>()
                .Value;
            return appconfig;
        }
        /// <summary>
        /// 获取自定义配置文件配置（异步方式）
        /// </summary>
        /// <typeparam name="T">配置模型</typeparam>
        /// <param name="key">根节点</param>
        /// <param name="configPath">配置文件名称</param>
        /// <returns></returns>
        public async Task<T> GetAppSettingsAsync<T>(string key, string configPath) where T : class, new()
        {
            if (string.IsNullOrEmpty(key) || string.IsNullOrEmpty(configPath)) return null;
            IConfiguration config = new ConfigurationBuilder()
                .Add(new JsonConfigurationSource
                {
                    Path = configPath,
                    ReloadOnChange = true
                }).Build();
            var appconfig = new ServiceCollection()
                .AddOptions()
                .Configure<T>(config.GetSection(key))
                .BuildServiceProvider()
                .GetService<IOptions<T>>()
                .Value;
            return await Task.Run(() => appconfig);
        }
        /// <summary>
        /// 获取自定义配置文件配置
        /// </summary>
        /// <typeparam name="T">配置模型</typeparam>
        /// <param name="key">根节点</param>
        /// <param name="configPath">配置文件名称</param>
        /// <returns></returns>
        public List<T> GetListAppSettings<T>(string key, string configPath) where T : class, new()
        {
            if (string.IsNullOrEmpty(key) || string.IsNullOrEmpty(configPath)) return null;
            IConfiguration config = new ConfigurationBuilder()
                .Add(new JsonConfigurationSource { Path = configPath, ReloadOnChange = true }).Build();
            var appconfig = new ServiceCollection()
                .AddOptions()
                .Configure<List<T>>(config.GetSection(key))
                .BuildServiceProvider()
                .GetService<IOptions<List<T>>>()
                .Value;
            return appconfig;
        }

        /// <summary>
        /// 获取自定义配置文件配置（异步方式）
        /// </summary>
        /// <typeparam name="T">配置模型</typeparam>
        /// <param name="key">根节点</param>
        /// <param name="configPath">配置文件名称</param>
        /// <returns></returns>
        public async Task<List<T>> GetListAppSettingsAsync<T>(string key, string configPath) where T : class, new()
        {
            if (string.IsNullOrEmpty(key) || string.IsNullOrEmpty(configPath)) return null;
            IConfiguration config = new ConfigurationBuilder()
                .Add(new JsonConfigurationSource { Path = configPath, ReloadOnChange = true })
                .Build();
            var appconfig = new ServiceCollection()
                .AddOptions()
                .Configure<List<T>>(config.GetSection(key))
                .BuildServiceProvider()
                .GetService<IOptions<List<T>>>()
                .Value;
            return await Task.Run(() => appconfig);
        }

        /// <summary>
        /// 封装要操作的字符
        /// </summary>
        /// <param name="sections">节点配置</param>
        /// <returns></returns>
        public static string app(params string[] sections)
        {
            try
            {

                if (sections.Any())
                {
                    return Configuration[string.Join(":", sections)];
                }
            }
            catch (Exception) { }

            return "";
        }
    }
}
