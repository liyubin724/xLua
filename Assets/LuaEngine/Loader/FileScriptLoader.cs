using System.IO;

namespace LuaEngine
{
    public class FileScriptLoader : ALuaScriptLoader
    {
        private string m_ScriptDir;
        private string m_ScriptExtension;

        public FileScriptLoader(string scriptDir, string extension = ".txt")
        {
            m_ScriptDir = scriptDir;
            m_ScriptExtension = extension;
        }

        protected override byte[] ReadScriptBytes(string path, out string fullPath)
        {
            fullPath = path;
            if (string.IsNullOrEmpty(path) || string.IsNullOrEmpty(m_ScriptDir))
            {
                return null;
            }

            fullPath = $"{m_ScriptDir}/{path}{m_ScriptExtension}";
            if (File.Exists(fullPath))
            {
                return File.ReadAllBytes(fullPath);
            }
            return null;
        }
    }
}
