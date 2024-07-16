using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using ImageViewer.Filters;
using ImageViewer3D.Filters;

namespace ImageViewer3D
{
    public class EffectMenuItem3D:ToolStripMenuItem 
    {
        public IEffect3D  MenuEffect;
        public EffectMenuItem3D(IEffect3D  MenuEffect, string MenuTitle):base(MenuTitle )
        {
            this.MenuEffect = MenuEffect;
            this.Text = MenuTitle;
            
        }
    }
}
