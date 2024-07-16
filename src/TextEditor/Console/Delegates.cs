using System;
using System.Collections.Generic;

using System.Text;

namespace IronPythonEditor.Console
{
    public delegate string[] AutoCompletionListFillEvent(string ObjectName);
    public delegate void EventCommandEntered(object sender, CommandEnteredEventArgs e);
}
