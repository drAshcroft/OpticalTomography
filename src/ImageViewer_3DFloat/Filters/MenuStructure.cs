using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ImageViewer3D.Filters
{
    public class MenuStructure
    {
        public static string[] MainMenuStructure()
        {
            return new string[] { "File", "Edit", "Adjustment", "Effects","Analysis","Macros","Scripts" };
        }
        public static string[] SubMenuStructure(string MenuName)
        {
            if (MenuName == "Effects")
            {
                return new string[] { "Morphology", "Edge Detection", "Dithering", "Blurs", "Artistic" };
            }
            else if (MenuName == "Edit")
            {
                return new string[] { "Undo", "Redo" };
            }
            else
                return null;
        }
    }
}
