using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using ImageViewer.Filters;

namespace ImageViewer
{
    public class EffectMenuItem:ToolStripMenuItem 
    {
        public IEffect  MenuEffect;
        public EffectMenuItem(IEffect  MenuEffect, string MenuTitle):base(MenuTitle )
        {
            this.MenuEffect = MenuEffect;
            this.Text = MenuTitle;
            
        }
    }
}
