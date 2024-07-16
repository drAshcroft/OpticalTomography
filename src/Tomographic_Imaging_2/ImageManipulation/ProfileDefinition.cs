using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace Tomographic_Imaging_2
{
    public class ProfileDefinition
    {
        public ProfileDefinition()
        {
            P1 = new Point();
            P2 = new Point();
        }
        public ProfileDefinition(Point p1, Point p2)
        {
            P1 = p1;
            P2 = p2;
        }
        public Point P1;
        public Point P2;
    }
}
