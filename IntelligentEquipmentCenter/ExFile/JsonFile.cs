using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.IO;
using System.Reflection;

namespace IntelligentEquipmentCenter
{
    public static class JsonFile
    {
        public static string Serialize<T>(T Obj,bool indented=true)
        {
            if (Obj == null)
            {
                return "";
            }
            return JsonConvert.SerializeObject(Obj, indented? Formatting.Indented :Formatting.None);
        }

        public static String ToJsonString(this Object Obj)
        {
            return Serialize(Obj);
        }


        public static T DeSerialize<T>(string json)
        {
            if (string.IsNullOrEmpty(json))
            {
                throw new Exception("Json内容不能为空");
            }
            return JsonConvert.DeserializeObject<T>(json);
        }


        public static T ToObj<T>(this string json)
        {
            return DeSerialize<T>(json);
        }


        public static T ReadJsonFile<T>(string filepath)
        {
            if (string.IsNullOrEmpty(filepath))
            {
                throw new Exception("文件路径不能为空");
            }
            if (!File.Exists(filepath))
            {
                throw new Exception($"文件{filepath}不存在");
            }
            string str = File.ReadAllText(filepath, Encoding.UTF8);
            return str.ToObj<T>();
        }

        public static void WriteJsonFile<T>(string filepath, T Obj)
        {
            if (string.IsNullOrEmpty(filepath))
            {
                throw new Exception("文件路径不能为空");
            }
            string json = Obj.ToJsonString();
            File.WriteAllText(filepath, json);
        }

    }

    public  class PathConfig
    {
        private static PathConfig instance = new PathConfig();
        public static PathConfig Instance=>instance;
        private PathConfig() { }
    
        public string BasePath=>AppDomain.CurrentDomain.BaseDirectory;

        public string CfgPath => BasePath + "\\cfg";

        public string ConfigPath => BasePath + "\\cfg\\Config.json";

        public string ProjectPath => BasePath + $"\\cfg\\Project\\{Recipedata.GetCur}.json";

    }
}
