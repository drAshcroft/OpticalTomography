using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DoRecons.Scripts
{
    public interface IScript
    {
        void RunScript(Dictionary<string, object> Variables);
        IScript CloneScript();
        string GetName();
    }
}
