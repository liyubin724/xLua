using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotEngine.Lua
{
    public class LuaEnvInvalidException : Exception
    {
        public LuaEnvInvalidException() : base("The env is invalid") { }
    }
}
