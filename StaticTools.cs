using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoyuBook {
    static class StaticTools {
        /// <summary>
        /// 带格式化序列化 json 输出 string
        /// </summary>
        /// <param name="old"></param>
        /// <returns></returns>
        public static string ToJson (this object old) {
            Newtonsoft.Json.JsonSerializerSettings settings = new Newtonsoft.Json.JsonSerializerSettings();
            settings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;  //不序列自我循环引用，循环引用的字段会丢失
            System.IO.TextReader tr = new System.IO.StringReader(Newtonsoft.Json.JsonConvert.SerializeObject(GlobalData.Self.Config , settings));
            Newtonsoft.Json.JsonTextReader jtr = new Newtonsoft.Json.JsonTextReader(tr);
            Newtonsoft.Json.JsonSerializer serializer = new Newtonsoft.Json.JsonSerializer();
            object obj = serializer.Deserialize(jtr);
            System.IO.StringWriter textWriter = new System.IO.StringWriter();
            Newtonsoft.Json.JsonTextWriter jsonWriter = new Newtonsoft.Json.JsonTextWriter(textWriter) {
                Formatting = Newtonsoft.Json.Formatting.Indented ,
                Indentation = 4 ,
                IndentChar = ' '
            };
            serializer.Serialize(jsonWriter , obj);
            return textWriter.ToString();
        }
    }
}
