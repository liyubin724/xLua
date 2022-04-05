using System.Collections.Generic;
using XLua;

namespace DotEngine.Lua
{
    class LuaLocalization
    {
        private Dictionary<string, string> cachedTextDic = new Dictionary<string, string>();
        private LuaTable languageTable = null;

        public LuaLocalization()
        {
        }

        public string GetText(string locName)
        {
            string text = string.Empty;
            if (languageTable != null)
            {
                if(!cachedTextDic.TryGetValue(locName,out text))
                {
                    text = languageTable.Get<string>(locName);
                    if(text == null)
                    {
                        text = string.Empty;
                    }
                    cachedTextDic.Add(locName, text);
                }
            }
            return text;
        }

        public void ChangeLanguage(LuaTable language)
        {
            cachedTextDic.Clear();
            if(languageTable!=language)
            {
                if(languageTable!=null)
                {
                    languageTable.Dispose();
                    languageTable = null;
                }
            }
            languageTable = language;
        }

        public void Dispose()
        {
            if(languageTable!=null)
            {
                languageTable.Dispose();
            }
        }
    }
}
