using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.IO;

namespace MathHelpLib
{
    public static  class MathStringHelps
    {
        /// <summary>
        /// Sorts a number of files with the assumption that they follow the pattern c:\testfolder\testimage000.bmp
        /// </summary>
        /// <param name="UnsortedFilenames"></param>
        /// <returns></returns>
        public static string[] SortNumberedFiles(string[] UnsortedFilenames)
        {
            Regex objNaturalPattern = new Regex(@"((\b[0-9]+)?\.)?[0-9]+\b");

            //find the minimum value so it will got back in correctly
            int MinFileN = int.MaxValue;
            int BadFiles = 0;
            SortedList<int, string> FilenameList = new SortedList<int, string>();
            for (int i = 0; i < UnsortedFilenames.Length; i++)
            {
                MatchCollection mc = objNaturalPattern.Matches(Path.GetFileNameWithoutExtension(UnsortedFilenames[i]));
                if (mc.Count != 0)
                {
                    try
                    {
                        string Filenumber = mc[0].Value;
                        int iFileNumber = 0;
                        int.TryParse(Filenumber, out iFileNumber);
                        if (iFileNumber < MinFileN) MinFileN = iFileNumber;
                        FilenameList.Add(iFileNumber, UnsortedFilenames[i]);
                    }
                    catch { }
                }
                else
                    BadFiles++;
            }


            string[] Filenames = new string[FilenameList.Count];
            int ii = 0;
            foreach (string Filename in FilenameList.Values)
            {
                Filenames[ii] = Filename;
                ii++;
            }

            //  string[] Filenames = new string[UnsortedFilenames.Length-BadFiles ];
            /*  for (int i = 0; i < UnsortedFilenames.Length; i++)
              {
                  MatchCollection mc = objNaturalPattern.Matches(Path.GetFileNameWithoutExtension(UnsortedFilenames[i]));
                  if (mc.Count != 0)
                  {
                      string Filenumber = mc[0].Value;
                      int iFileNumber = 0;
                      int.TryParse(Filenumber, out iFileNumber);
                      Filenames[iFileNumber - MinFileN] = UnsortedFilenames[i];
                  }
              }*/
            return Filenames;
        }

    }
}
