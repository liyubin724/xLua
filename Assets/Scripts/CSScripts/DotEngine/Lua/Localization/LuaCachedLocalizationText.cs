using System.Collections.Generic;
using XLua;

namespace DotEngine.Lua
{
    class LuaCachedLocalizationText
    {
        private Dictionary<string, string> cachedTextDic = new Dictionary<string, string>();
        private LuaTable languageTable = null;

        public LuaCachedLocalizationText() { }

        public void ChangeLanguage(LuaTable language)
        {
            if(languageTable!=null && languageTable!=language)
            {
                languageTable.Dispose();
            }
            languageTable = language;
            cachedTextDic.Clear();
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
    }
}
