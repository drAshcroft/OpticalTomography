using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using ImageViewer.Filters;
using Microsoft.Office.Interop.Excel;

namespace Statistics
{
    public partial class Form1 : Form
    {

        Dictionary<DateTime, DateCellComments> DateComments = new Dictionary<DateTime, DateCellComments>();
        private Dictionary<string, int> ColumnsByName = new Dictionary<string, int>();
        private List<string> ColNames = new List<string>();

        public Form1()
        {
            InitializeComponent();
        }

        private void bLoadData_Click(object sender, EventArgs e)
        {
            StreamWriter sw = new StreamWriter(@"C:\Development\temp.csv");
            if (DialogResult.OK == saveFileDialog1.ShowDialog())
            {
                string STorage = tStorageDir.Text;

                List<string> BadDirs = new List<string>();
                List<string> GoodDirs = new List<string>();
                bool FirstDir = true;
                string[] AllDirs;

                BadDirs.Add(STorage);
                while (FirstDir == true && BadDirs.Count > 0)
                {
                    try
                    {
                        string[] Dirs = Directory.GetDirectories(BadDirs[0]);
                        if (Dirs.Length > 0)
                        {
                            if (Dirs[0].Contains("cct") == true && Path.GetFileName(Dirs[0]).Length > 6)
                                GoodDirs.AddRange(Dirs);
                            else
                                BadDirs.AddRange(Dirs);
                        }
                    }
                    catch { }
                    BadDirs.RemoveAt(0);
                }
                AllDirs = GoodDirs.ToArray();

                Dictionary<string, ReplaceStringDictionary> AllData = new Dictionary<string, ReplaceStringDictionary>();
                for (int i = 0; i < (double)AllDirs.Length; i++)
                {
                    try
                    {
                        string pPath = AllDirs[i].Replace("\"", "").Replace("'", "");

                        if (pPath.EndsWith("\\") == false)
                            pPath += "\\";

                        string dirName = Path.GetFileNameWithoutExtension(Path.GetDirectoryName(pPath));
                        string[] parts = dirName.Split('_');
                        string Prefix = parts[0];
                        string Year = parts[1].Substring(0, 4);
                        string month = parts[1].Substring(4, 2);
                        string day = parts[1].Substring(6, 2);

                        string basePath = pPath;// tStorageDir.Text + Prefix + "\\" + Year + month + "\\" + day + "\\" + dirName;

                        DateTime dt = Convert.ToDateTime(month + "/" + day + "/" + Year);
                        string datapath = basePath + "data\\projectionobject.cct";
                        if (Directory.Exists(basePath) == true)
                            if (File.Exists(datapath) == true)
                            {
                                ReplaceStringDictionary values = new ReplaceStringDictionary();
                                try
                                {
                                    MoveValues(values, LoadComments(basePath));

                                    try
                                    {
                                        MoveValues(values, LoadUserComments(basePath));
                                    }
                                    catch { }

                                    try
                                    {
                                        MoveValues(values, LoadBetterImage(basePath));
                                        System.Diagnostics.Debug.Print("");
                                    }
                                    catch { }
                                }
                                catch
                                { }

                                if (DateComments != null && DateComments.ContainsKey(dt) == true)
                                {

                                    DateCellComments dcc = DateComments[dt];
                                    values.AddSafe("CellType", dcc.CellType);
                                    values.AddSafe("StainType", dcc.StainType);
                                    values.AddSafe("Total Data Collection", dcc.Total_data_collection);
                                    values.AddSafe("Good", dcc.Good);
                                    values.AddSafe("Partial", dcc.Partial);
                                    values.AddSafe("Failure", dcc.Failure);
                                    values.AddSafe("ExcelComments", dcc.Comments);
                                }

                                AllData.Add(basePath, values);
                                TrackCols(values);
                            }
                            else
                            {

                            }
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.Print(ex.Message);
                    }
                }



                StreamWriter outfile = new StreamWriter(saveFileDialog1.FileName);
                outfile.Write(" , , ");
                foreach (string s in ColNames)
                {
                    try
                    {
                        ColumnsByName.Add(s, lDataDirectories.Columns.Count - 1);
                        lDataDirectories.Columns.Add(s, s);
                        outfile.Write(s + ", ");
                    }
                    catch 
                    {
                        outfile.Write(" , ");
                    }
                }

                outfile.WriteLine("");

                int Row = 1;
                lDataDirectories.Rows.Add(Row + 1);

                foreach (KeyValuePair<string, ReplaceStringDictionary> kvp in AllData)
                {
                    string directory = Path.GetFileNameWithoutExtension(kvp.Key);
                    ReplaceStringDictionary rsd = kvp.Value;
                    lDataDirectories.Rows.Add();
                    lDataDirectories.Rows[Row].Cells[0].Value = directory;
                    outfile.Write(directory + ", ");
                    string[] RowData = new string[lDataDirectories.ColumnCount + 1];
                    foreach (KeyValuePair<string, object> kvp2 in rsd)
                    {
                        try
                        {
                            if (ColumnsByName.ContainsKey(kvp2.Key) == true)
                            {
                                int col = ColumnsByName[kvp2.Key];
                                lDataDirectories.Rows[Row].Cells[col].Value = kvp2.Value.ToString();
                                RowData[col] = kvp2.Value.ToString();
                            }
                        }
                        catch
                        {

                        }
                    }

                    outfile.Write(kvp.Key + ", ");
                    for (int i = 0; i < RowData.Length; i++)
                    {
                        try
                        {
                            if (RowData[i] != null)
                                outfile.Write(RowData[i].ToString().Replace(',', '_') + " , ");
                            else
                                outfile.Write(" , ");
                        }
                        catch
                        {
                            outfile.Write(" ,");
                        }
                    }
                    outfile.WriteLine("");
                    Row++;
                }

                outfile.Close();

            }
        }

        string CopyString = "";

        private void TrackCols(ReplaceStringDictionary values)
        {
            foreach (KeyValuePair<string, object> kvp in values)
            {
                if (ColNames.Contains(kvp.Key) == false)
                {
                    ColNames.Add(kvp.Key);
                }
            }
        }

        private void MoveValues(ReplaceStringDictionary OriginalValues, ReplaceStringDictionary NewValues)
        {
            foreach (KeyValuePair<string, object> kvp in NewValues)
            {
                OriginalValues.AddSafe(kvp.Key, kvp.Value);
            }
        }

        private ReplaceStringDictionary LoadUserComments(string experimentFolder)
        {

            String line;
            using (StreamReader sr = new StreamReader(experimentFolder + "\\data\\UserComments.txt"))
            {
                line = sr.ReadToEnd();
            }

            line = line.ToLower();
            string[] Parts = line.Split(new string[] { "\r\n", "\r", "\n", "==" }, StringSplitOptions.RemoveEmptyEntries);

            ReplaceStringDictionary Values = new ReplaceStringDictionary();

            for (int i = 0; i < Parts.Length; i += 2)
            {
                Values.AddSafe(Parts[i], Parts[i + 1]);
            }

            return Values;
        }

        private ReplaceStringDictionary LoadBetterImage(string experimentFolder)
        {

            String line;
            using (StreamReader sr = new StreamReader(experimentFolder + "\\data\\correctedQualityScore.txt"))
            {
                line = sr.ReadToEnd();
            }

            line = line.ToLower();
            string[] Parts = line.Split(new string[] { "\r\n", "\r", "\n", "==" }, StringSplitOptions.RemoveEmptyEntries);

            ReplaceStringDictionary Values = new ReplaceStringDictionary();

            for (int i = 0; i < Parts.Length; i += 2)
            {
                Values.AddSafe(Parts[i], Parts[i + 1]);
            }

            return Values;
        }

        private ReplaceStringDictionary LoadComments(string experimentFolder)
        {
            ReplaceStringDictionary Values = new ReplaceStringDictionary();
            string DataPath = experimentFolder + "\\data\\";
            if (File.Exists(DataPath + "comments.txt") == false)
            {
                Values.AddSafe("recon", "No Data");
                return Values;
            }

            String line;
            using (StreamReader sr = new StreamReader(DataPath + "comments.txt"))
            {
                line = sr.ReadToEnd();
            }

            line = line.Replace("\r", "").Replace("\n", "").ToLower();
            string[] Parts = line.Split(new string[] { "<", "/>", ">" }, StringSplitOptions.RemoveEmptyEntries);

            for (int i = 0; i < Parts.Length; i += 2)
            {
                Values.AddSafe(Parts[i], Parts[i + 1]);
            }

            return Values;
        }

        private void bCopy_Click(object sender, EventArgs e)
        {
            /*string CopyOut = "";

            for (int j = 0; j < lDataDirectories.Columns.Count; j++)
            {
                if (lDataDirectories.Columns[j].HeaderText != null)
                    CopyOut += lDataDirectories.Columns[j].HeaderText + "\t";
                else
                    CopyOut += "\t";
            }
            CopyOut += "\r\n";

            for (int i = 0; i < lDataDirectories.Rows.Count; i++)
            {
                for (int j = 0; j < lDataDirectories.Columns.Count; j++)
                {
                    if (lDataDirectories.Rows[i].Cells[j].Value != null)
                        CopyOut += lDataDirectories.Rows[i].Cells[j].Value.ToString() + "\t";
                    else
                        CopyOut += "\t";
                }
                CopyOut += "\r\n";
            }
            Clipboard.SetText(CopyOut);*/
            Clipboard.SetText(CopyString);
        }

        private void bExcel_Click(object sender, EventArgs e)
        {
            object oMissing = System.Reflection.Missing.Value;
            object missing = System.Reflection.Missing.Value;
            object Visible = true;


            /*   Microsoft.Office.Interop.Word._Application oWord;
               Microsoft.Office.Interop.Word._Document oDoc;
               oWord = new Microsoft.Office.Interop.Word.Application();
               oWord.Visible = true;
               oDoc = oWord.Documents.Add(ref oMissing, ref oMissing,
                   ref oMissing, ref oMissing);*/

            // Range rng = oDoc.Range(ref start1, ref oMissing);
            Microsoft.Office.Interop.Excel._Application oExcel;
            Microsoft.Office.Interop.Excel._Workbook oWorkBook;

            oExcel = new Microsoft.Office.Interop.Excel.Application();
            oExcel.Visible = true;

            oWorkBook = oExcel.Workbooks.Open(tExcelFile.Text);



            // [1];
            foreach (Microsoft.Office.Interop.Excel._Worksheet oWorkSheet in oWorkBook.Worksheets)
            {
                Microsoft.Office.Interop.Excel.Range Headers = oWorkSheet.Range["A1:Z100"];

                object[,] Data = new object[Headers.Columns.Count, Headers.Rows.Count];

                int rowC = 0;
                List<System.Drawing.Point> Dates = new List<System.Drawing.Point>();
                foreach (Microsoft.Office.Interop.Excel.Range row in Headers.Rows)
                {
                    int colC = 0;
                    foreach (Microsoft.Office.Interop.Excel.Range cell in row.Columns)
                    {

                        string address = cell.Address;
                        object value = cell.Value;

                        if (value != null)
                        {
                            Data[colC, rowC] = value;
                            if (value.ToString().ToLower() == "date")
                                Dates.Add(new System.Drawing.Point(colC, rowC));
                        }
                        colC++;
                    }
                    rowC++;
                }

                foreach (System.Drawing.Point p in Dates)
                {
                    List<string> CellAndStains = new List<string>();
                    for (int y = 0; y < p.Y; y++)
                    {
                        for (int x = p.X; x < p.X + 5; x++)
                        {
                            try
                            {
                                if (Data[x, y] != null && Data[x, y].GetType() == typeof(string))
                                {
                                    string value = Data[x, y].ToString().ToLower();
                                    if (!(value.Contains("staintype") == true || value.Contains("stain type") || value.Contains("stain") || value.Contains("cell type") || value.Contains("celltype")))
                                    {
                                        CellAndStains.Add(Data[x, y].ToString());
                                    }
                                }
                            }
                            catch { }
                        }
                    }
                    string CellType = "";

                    try
                    {
                        CellType = CellAndStains[0];
                    }
                    catch { }
                    string StainType = "";
                    try
                    {
                        StainType = CellAndStains[1];
                    }
                    catch { }


                    for (int y = p.Y + 1; y < Data.GetLength(1); y++)
                    {
                        if (Data[p.X, y] != null)
                        {
                            try
                            {
                                DateTime dt;
                                if (Data[p.X, y].GetType() == typeof(DateTime))
                                    dt = (DateTime)Data[p.X, y];
                                else
                                    dt = Convert.ToDateTime((string)Data[p.X, y]);

                                if (DateComments.ContainsKey(dt) == false)
                                {
                                    if (Data[p.X, y] != null && Data[p.X, y].GetType() == typeof(System.DateTime))
                                    {
                                        string Total_data_collection = "";
                                        try
                                        {
                                            Total_data_collection = Data[p.X + 1, y].ToString();
                                        }
                                        catch { }

                                        string Good = "";
                                        try
                                        {
                                            Good = Data[p.X + 2, y].ToString();
                                        }
                                        catch { }
                                        string Partial = "";
                                        try
                                        {
                                            Partial = Data[p.X + 3, y].ToString();
                                        }
                                        catch { }
                                        string Failure = "";
                                        try
                                        {
                                            Failure = Data[p.X + 4, y].ToString();
                                        }
                                        catch { }
                                        string Comments = "";
                                        try
                                        {
                                            Comments = Data[p.X + 5, y].ToString();
                                        }
                                        catch { }

                                        DateComments.Add(dt, new DateCellComments(dt, Total_data_collection, Good, Partial, Failure, Comments, CellType, StainType));
                                    }
                                }
                                else
                                {
                                    DateComments.Remove(dt);
                                }
                            }
                            catch { }
                        }

                    }

                }


            }
            /*   string junk = "";
                  foreach (KeyValuePair<DateTime, DateCellComments> kvp in DateComments)
                  {
                      if (kvp.Value != null)
                      {
                          junk += kvp.Value.ToString() + "\r\n";
                      }
                  }
                  Clipboard.SetText(junk);
                  MessageBox.Show("Excel File is loaded");*/
            lExcelDone.Text = "Done";
        }

        private class DateCellComments
        {
            public DateTime Date;
            public string Total_data_collection;
            public string Good;
            public string Partial;
            public string Failure;
            public string Comments;
            public string CellType;
            public string StainType;

            public DateCellComments(DateTime Date, string Total_data_collection, string Good, string Partial, string Failure, string Comments, string CellType, string StainType)
            {
                this.Date = Date;
                this.Total_data_collection = Total_data_collection;
                this.Good = Good;
                this.Partial = Partial;
                this.Failure = Failure;
                this.Comments = Comments;
                this.CellType = CellType;
                this.StainType = StainType;
            }

            public override string ToString()
            {
                return Date.ToString() + "\t" + Total_data_collection + "\t" + Good + "\t" + Partial + "\t" + Failure + "\t" + Comments + "\t" + CellType + "\t" + StainType;
            }
        }

        private void bAll_Click(object sender, EventArgs e)
        {
            bExcel_Click(this, EventArgs.Empty);
            bLoadData_Click(this, EventArgs.Empty);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string CopyOut = "";

            for (int j = 0; j < lDataDirectories.Columns.Count; j++)
            {
                if (lDataDirectories.Columns[j].HeaderText != null)
                    CopyOut += lDataDirectories.Columns[j].HeaderText + "\t";
                else
                    CopyOut += "\t";
            }
            CopyOut += "\r\n";

            for (int i = 0; i < lDataDirectories.Rows.Count; i++)
            {
                for (int j = 0; j < lDataDirectories.Columns.Count; j++)
                {
                    if (lDataDirectories.Rows[i].Cells[j].Value != null)
                        CopyOut += lDataDirectories.Rows[i].Cells[j].Value.ToString() + "\t";
                    else
                        CopyOut += "\t";
                }
                CopyOut += "\r\n";
            }
            Clipboard.SetText(CopyOut);
        }

    }
}
