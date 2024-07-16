using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ImageViewer.Filters
{
    /// <summary>
    /// Defines the default menu structure and its order to make the program easier to use
    /// </summary>
    public class MenuStructure
    {
        /// <summary>
        /// called when creating contructing the menus, returns the headers and order
        /// </summary>
        /// <returns></returns>
        public static string[] MainMenuStructure()
        {
            return new string[] { "File", "Edit", "Adjustment", "Effects","Analysis","Macros","Scripts" };
        }

        /// <summary>
        /// The main routing uses this to ask if the menus have set sub menus.  
        /// </summary>
        /// <param name="MenuName"></param>
        /// <returns></returns>
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
