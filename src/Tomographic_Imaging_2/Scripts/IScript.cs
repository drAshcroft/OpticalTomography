using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tomographic_Imaging_2
{
    public interface IScript
    {
        void RunScript(Dictionary<string, object> Variables);
        IScript CloneScript();
        string GetName();
    }
}
