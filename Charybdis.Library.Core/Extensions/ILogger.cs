using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Charybdis.Library.Core
{
    public interface ILogger
    {
        void Log(string data, LogTarget target);
        void Log(Exception e, LogTarget target);
        void Log(string data, Exception e, LogTarget target);
        LogTarget LastTarget { get; }
    }
}
