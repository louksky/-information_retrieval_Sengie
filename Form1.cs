using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.IO.MemoryMappedFiles;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace RicycleInform
{

    [XmlRoot("fORM")]
    public partial class Form1 : Form
    {
        [XmlArray("DataBase"), XmlArrayItem(typeof(List<Node<string>>), ElementName = "List<Node<string>>")]
        List<Node<string>> DataBase;
        List<DocPath> pathsbase;
        XMLH1 help;
        xmlDwice helps;
        int numOfdocs = 1000;
        string[] keyValue ;
        string linesr;
        string[] docs;
        string fileName = "DocFollower.txt";
        static string pathXMLServer = "C:/Users/asafl/Desktop/שנקר/שנה ג/אחזור מידע/ServerFolder/ServerXML.txt";
        static string pathDocFollower = "C:/Users/asafl/Desktop/שנקר/שנה ג/אחזור מידע/ServerFolder/DocFollower.txt";
        static string paths = "C:/Users/asafl/Desktop/שנקר/שנה ג/אחזור מידע/ServerFolder/Paths.txt";
        public Form1()
        {
            InitializeComponent();
            help= new XMLH1();
            
            if(DataBase == null)
            DataBase = new List<Node<string>>();
            pathsbase = new List<DocPath>();
            docs = new string[numOfdocs];
            Init();
        }
       
           
        
        public void Init()
        {
            
            try
            {
                fileName = File.ReadAllText(pathDocFollower);

            }
            catch
            {
                return;
            }
            
        }
       
        private List<Doc> Ops(string Seach)
        {
            List<Doc> show = KMP_ADDITION_L(Seach);
            
           
            List<Doc> executeing = new List<Doc>();
            string s1 = "", s2 = "";
            var lineWords = Seach.Split(' ');
            int cou = 0, j = 0; ;
            for (int i = 0; i < lineWords.Length; ++i)
            {
                cou = 0;
                switch (lineWords[i])
                {
                    case "AND":

                        for (j = i+1; j < lineWords.Length && lineWords[j] != "AND" && lineWords[j] != "OR" && lineWords[j] != "NOT"; j++)
                        {
                            s2 += " " + lineWords[j]; cou++;
                        }
                        executeing = KMP_ADDITION_L(s1 + s2);
                        foreach (Doc d in executeing)
                        {
                            if (d.Hit != cou)
                            {
                                foreach (Doc d2 in show)
                                {
                                    if (d.Name == d2.Name)
                                    {
                                        show.Remove(d2);
                                        break;
                                    }
                                }
                            }
                        }
                        if (j < lineWords.Length && (lineWords[j] == "AND" || lineWords[j] == "OR" || lineWords[j] == "NOT")) { i = j - 1; cou = 0; };
                        break;
                    case "NOT":
                        for (j = i+1; j < lineWords.Length && lineWords[j] != "AND" && lineWords[j] != "OR" && lineWords[j] != "NOT"; j++)
                        {
                            s2 += " " + lineWords[j];
                        }
                        executeing = KMP_ADDITION_L(s2);
                        foreach (Doc d in executeing)
                        {
                            foreach (Doc d2 in show)
                            {
                                if (d.Name == d2.Name)
                                {
                                    show.Remove(d2);
                                    break;
                                }
                            }
                        }
                        if (j < lineWords.Length && (lineWords[j] == "AND" || lineWords[j] == "OR" || lineWords[j] == "NOT")) { i = j - 1; cou = 0; };
                        break;
                    case "OR":
                        for (j = i+1; j < lineWords.Length && lineWords[j] != "AND" && lineWords[j] != "OR" && lineWords[j] != "NOT"; j++)
                        {
                            s2 += " " + lineWords[j];
                        }
                        executeing = KMP_ADDITION_L(s1+s2);
                        bool flagisin = true;
                        foreach (Doc d in executeing)
                        {
                            flagisin = false;
                            foreach (Doc d2 in show)
                            {
                                if (d.Name == d2.Name)
                                {
                                    flagisin = true;
                                    d2.Hit++;
                                    d2.Hitsum += d.Hit + 1;
                                }
                                
                            }
                            if (flagisin == false)
                            {
                                Doc newd = new Doc();
                                newd.Hit = 1;
                                newd.Hitsum = d.Hit + 1;
                                newd.Name = d.Name;
                                show.Add(newd);

                            }
                        }
                        if (j < lineWords.Length && (lineWords[j] == "AND" || lineWords[j] == "OR" || lineWords[j] == "NOT")) { i = j - 1; cou = 0; };
                        break;
                    default:
                        s1 += " " + lineWords[i];cou++;
                        break;
                }

            }


            return show;
        }
        private void SearchB(object sender, EventArgs e)
        {
            if (SeachTxt.Text.ElementAt(SeachTxt.Text.ToArray().Length-1) == ' ') SeachTxt.Text.Remove(SeachTxt.Text.ToArray().Length-1);
            //ALPAH ADDITION SEARCH BY KMP
            string str = SeachTxt.Text.ToString();
            linesr = SeachTxt.Text;
            List<Doc> show =KMP_ADDITION_L(str);
            
            //DEMO ADDITION USING OPERATORS [AND NOT OR]
            List<Doc> executeing = new List<Doc>(), tempo=null;
            string s1 = "", s2 = "";
            var lineWords = SeachTxt.Text.Split(' ');
            keyValue = SeachTxt.Text.Split(' ');
            int cou = 0, j = 0; ;
            for(int i= 0;i < lineWords.Length;++i)
            {
                
                switch (lineWords[i])
                {
                    case "(":
                        tempo = new List<Doc>();
                        tempo = Ops(SeachTxt.Text.Substring(SeachTxt.Text.IndexOf('(') + 1, SeachTxt.Text.IndexOf(')') - 1));
                        break;
                    case "AND":
                        for (j = i + 1; j < lineWords.Length && lineWords[j] != "AND" && lineWords[j] != "OR" && lineWords[j] != "NOT"; j++)
                        {
                            if (lineWords[j] == "(") {
                                tempo = new List<Doc>();
                                tempo = Ops(SeachTxt.Text.Substring(SeachTxt.Text.IndexOf('(') + 1, SeachTxt.Text.IndexOf(')') - 1));
                                break;
                            }
                            s2 += " " + lineWords[j]; cou++;
                        }
                        if (tempo == null)
                        { executeing = KMP_ADDITION_L(s1 + s2); s2 = ""; s1 = ""; }
                        else
                        {
                            executeing = KMP_ADDITION_L(s1); s1 = "";
                            foreach (Doc d in tempo)
                                executeing.Add(d);
                            tempo = null;
                        }
                        foreach (Doc d in executeing)
                        {
                            if (d.Hit != cou)
                            {
                                foreach (Doc d2 in show)
                                {
                                    if (d.Name == d2.Name)
                                    {
                                        show.Remove(d2);
                                        break;
                                    }
                                }
                            }
                        }
                        if (j < lineWords.Length && (lineWords[j] == "AND" || lineWords[j] == "OR" || lineWords[j] == "NOT")) { i = j - 1; cou = 0; };
                        break;
                    case "NOT":

                        for (j = i + 1; j < lineWords.Length && lineWords[j] != "AND" && lineWords[j] != "OR" && lineWords[j] != "NOT"; j++)
                        {
                            if (lineWords[j] == "(")
                            {
                                tempo = new List<Doc>();
                                tempo = Ops(SeachTxt.Text.Substring(SeachTxt.Text.IndexOf('(') + 1, SeachTxt.Text.IndexOf(')') - 1));
                                break;
                            }
                            s2 += " " + lineWords[j];
                        }
                        if (tempo == null)
                        { executeing = KMP_ADDITION_L(s2); s2 = ""; }
                        else
                        {
                            foreach (Doc d in tempo)
                            {
                                foreach (Doc d2 in show)
                                {
                                    if (d.Name == d2.Name)
                                    {
                                        show.Remove(d2);
                                        break;
                                    }
                                }
                            }
                            tempo = null;
                        }
                        foreach (Doc d in executeing)
                        {
                            foreach (Doc d2 in show)
                            {
                                if (d.Name == d2.Name)
                                {
                                    show.Remove(d2);
                                    break;
                                }
                            }
                        }
                        if (j < lineWords.Length &&( lineWords[j] == "AND" || lineWords[j] == "OR" || lineWords[j] == "NOT")) { i = j - 1; cou = 0; };
                        break;
                    case "OR":
                        for (j = i + 1; j < lineWords.Length && lineWords[j] != "AND" && lineWords[j] != "OR" && lineWords[j] != "NOT"; j++)
                        {
                            if (lineWords[j] == "(")
                            {
                                tempo = new List<Doc>();
                                tempo = Ops(SeachTxt.Text.Substring(SeachTxt.Text.IndexOf('(') + 1, SeachTxt.Text.IndexOf(')') - 1));
                                break;
                            }
                            s2 += " " + lineWords[j];
                        }
                        if (tempo == null)
                        { executeing = KMP_ADDITION_L(s1+s2); s2 = "";s1 = ""; }
                        else
                        {
                            executeing = KMP_ADDITION_L(s1); s1 = "";
                            foreach (Doc d in tempo)
                                executeing.Add(d);
                            tempo = null;
                        }

                
                        bool flagisin = true;
                        foreach (Doc d in executeing)
                        {
                            flagisin = false;
                            foreach (Doc d2 in show)
                            {
                                if (d.Name == d2.Name)
                                 {
                                    flagisin = true;
                                    d2.Hit++;
                                    d2.Hitsum += d.Hit + 1;
                                 }
                               
                            }
                            if (flagisin == false)
                            {
                                Doc newd = new Doc();
                                newd.Hit = 1;
                                newd.Hitsum = d.Hit + 1;
                                newd.Name = d.Name;
                                show.Add(newd);

                            }
                        }
                        if (j < lineWords.Length && (lineWords[j] == "AND" || lineWords[j] == "OR" || lineWords[j] == "NOT")) { i = j - 1; cou = 0; };
                        break;
                    default:
                        s1 += " "+lineWords[i];cou++;
                        break;
                }
                
            }
            foreach(Doc k in show)
            {
                k.Inf = GetFileinfo(k);
                for(int i=0; i< lineWords.Length; i++)
                    if(lineWords[i]!=""&& lineWords[i] != "AND" && lineWords[i] != "OR" && lineWords[i] != "NOT")
                        if (k.Inf.IndexOf(lineWords[i]) > 0)
                        {
                            k.Inf.Replace(k.Inf.Substring(k.Inf.IndexOf(lineWords[i]), lineWords[i].ToArray().Length), @"\b " + k.Inf.Substring(k.Inf.IndexOf(lineWords[i]), lineWords[i].ToArray().Length) + @"\b0 ");
                        }

            
            }

            var bindingList = new BindingList<Doc>(show);
            var source = new BindingSource(bindingList, null);
            dataGridView1.DataSource = source;
            foreach(DataGridViewColumn drd in dataGridView1.Columns)
            {
                drd.DefaultCellStyle.WrapMode = DataGridViewTriState.True;
                drd.DefaultCellStyle.Font = new Font(SeachTxt.Font, FontStyle.Bold);
                
            }


            


        }

       
        private void DataGridView1_CellPainting(object sender, DataGridViewCellPaintingEventArgs e)
        {
           
                
                if (e.Value == null) return;

                StringFormat sf = StringFormat.GenericTypographic;
                sf.FormatFlags = sf.FormatFlags | StringFormatFlags.MeasureTrailingSpaces | StringFormatFlags.DisplayFormatControl;
                e.PaintBackground(e.CellBounds, true);

                SolidBrush br = new SolidBrush(Color.White);
                if (((int)e.State & (int)DataGridViewElementStates.Selected) == 0)
                    br.Color = Color.Black;

                string text = e.Value.ToString();
                SizeF textSize = e.Graphics.MeasureString(text, Font, e.CellBounds.Width, sf);

                int keyPos = text.IndexOf(txtMark.Text, StringComparison.OrdinalIgnoreCase);
                if (keyPos >= 0)
                {
                    SizeF textMetricSize = new SizeF(0, 0);
                    if (keyPos >= 1)
                    {
                        string textMetric = text.Substring(0, keyPos);
                        textMetricSize = e.Graphics.MeasureString(textMetric, Font, txtMark.Text.ToArray().Length*2, sf);
                    }

                    SizeF keySize = e.Graphics.MeasureString(text.Substring(keyPos, txtMark.Text.ToArray().Length*2), Font, txtMark.Text.ToArray().Length*2, sf);
                    float left = e.CellBounds.Left + (keyPos <= 0 ? 0 : textMetricSize.Width) + 2;
                    RectangleF keyRect = new RectangleF(left, e.CellBounds.Top + 1, keySize.Width,16);
                   
                    //
                   
                    //
                    var fillBrush = new SolidBrush(Color.Aqua);
                    e.Graphics.FillRectangle(fillBrush, keyRect);
                    fillBrush.Dispose();
                   
                e.Graphics.DrawString(text, Font, br, new PointF(e.CellBounds.Left + 2, e.CellBounds.Top + (e.CellBounds.Height - textSize.Height) / 2), sf);
                e.Handled = true;

                br.Dispose();
            }
            
        }
        private string GetFileinfo(Doc d)
        {
            string line = "";
            foreach(DocPath dp in pathsbase)
            {
                if(dp.name == d.Name) using (var mappedFile1 = MemoryMappedFile.CreateFromFile(dp.path))
                    {
                        using (Stream mmStream = mappedFile1.CreateViewStream())
                        {
                            using (StreamReader sr = new StreamReader(mmStream, ASCIIEncoding.ASCII))
                            {
                                while (!sr.EndOfStream)
                                {
                                    line += sr.ReadLine()+Environment.NewLine;
                                    if (line.ToArray().Length > 280) { d.Name = dp.path.Substring(dp.path.IndexOf("doc") + 3);
                                        d.Name = d.Name.Replace("\\", string.Empty);
                                        return line; }
                                }
                            }
                        }
                       
                    }
            }
           
                            return line;
        }
        private List<Doc> KMP_ADDITION_L(string serachable)
        {
            serachable =  serachable.ToLower();
            List<Doc> ReturnList = new List<Doc>();
            Node<string> temp;
            var lineWords = serachable.Split(' ');
            bool flagisin=true;
            foreach (string word in lineWords)
            {
                serachable = UseReplace(word);
                if (serachable == "") continue;
                else
                {
                    temp = GetifExist(serachable);
                    if (temp == null) continue;
                    else
                    {
                        foreach(Doc d in temp.DocList)
                        {
                            flagisin = false;
                            foreach(Doc d2 in ReturnList)
                            {
                                if(d.Name == d2.Name)
                                {
                                    flagisin = true;
                                    d2.Hit++;
                                    d2.Hitsum += d.Hit + 1;
                                }
                            }
                            if (flagisin == false)
                            {
                                Doc newd = new Doc();
                                newd.Hit = 1;
                                newd.Hitsum = d.Hit + 1;
                                newd.Name = d.Name;
                                ReturnList.Add(newd);

                            }

                        }

                    }
                }
            }
            ReturnList.Sort((p, q) => q.Hit.CompareTo(p.Hit));


                return ReturnList;
        }
        private void ChoseFile_Click(object sender, EventArgs e)
        {
            try
            {
                openFileDialog1.ShowDialog();
                if (openFileDialog1.CheckFileExists)
                    FileDialogTxt.Text = openFileDialog1.FileName;
                else
                    FileDialogTxt.Text = "no file exeist";
            }
            catch
            {
                return;
            }
            
        }
        public Node<string> GetifExist(string word)
        {
            foreach(Node<string> t in DataBase)
            {
                if (t.info == word) return t;
            }
            return null;
        }
        public  Node<string> GetOrSet(string word,string docPre)
        {
             
            foreach(Node<string> Now in DataBase)
            {
                if (Now.GetInfo() == word)
                {
                    
                    foreach(Doc s in Now.DocList)
                    {
                        if (s.Name == docPre) { s.Hit++; return Now; }
                    }
                    Doc d = new Doc();
                    d.Name = docPre;
                    d.Hit = 1;
                    Now.hits++;
                    Now.DocList.Add(d);
                    return Now;
                }
            }
            if (word != "" && word!=null)
            {
                Node<string> n = new Node<string>(word, 1, docPre);
                DataBase.Add(n);
                
            }
           
            return null;
        }
        private void MethodEx()
        {

            //Update The counting of files
            int n = 0;
            string news = fileName.Substring(1);
            n = Convert.ToInt32(news);
            n++;
            fileName = "D" + n;
            fileName = UseReplace(fileName);
            File.WriteAllText(pathDocFollower, fileName);


        }
        private void updatefileserver(string toset,string location)
        {

        }
      
        private void Button2_Click(object sender, EventArgs e)
        {
            Node<string> temp;
            string t = "";
            try
            {
                string r = "";
                if (openFileDialog1.CheckFileExists)
                {
                    using (var mappedFile1 = MemoryMappedFile.CreateFromFile(openFileDialog1.FileName))
                    {
                        using (Stream mmStream = mappedFile1.CreateViewStream())
                        {
                            using (StreamReader sr = new StreamReader(mmStream, ASCIIEncoding.ASCII))
                            {
                                while (!sr.EndOfStream)
                                {
                                    var line = sr.ReadLine();
                                    r += (string)line;
                                    var lineWords = line.Split(' ');
                                    
                                    foreach (string word in lineWords)
                                    {
                                        if (word == null || word == "" || word.IndexOf('[')>0||word == " " || word[0] == '[') continue;
                                        else
                                        {
                                            t = UseReplace(word);
                                            t=t.ToLower();
                                            temp = GetOrSet(t, fileName);
                                           
                                        }
                                    }
                                }
                            }
                        }
                    }
                    for (int y = 0; y < 2; ++y) DataBase.RemoveAt(DataBase.Count-1);
                    MessageBox.Show(r);
                   
                    pathsbase.Add(new DocPath(openFileDialog1.FileName, fileName));
                    Newmeth();
                    MethodEx();//index file names

                    DataBase.Sort((p, q) => p.info.CompareTo(q.info));


                }
                else
                {
                    return;
                }
               
            }
            catch(Exception ez) { return; }
           
        }
        public  string UseReplace(string dirtyString)
        {
           dirtyString= dirtyString.ToLower();

            string removeChars = " ..?&^$#@!()+-,:;<>’\'-_*/%&--_-_- ";
            string result = dirtyString;

            foreach (char c in removeChars)
            {
                result = result.Replace(c.ToString(), string.Empty);
            }
            result.Replace(".", string.Empty);
            return result;
        }
        

        private void OnClose(object sender, FormClosingEventArgs e)
        {
            SaveInClose();
        }

        private void OnOpen(object sender, EventArgs e)
        {
            OpenInClose();
        }

        private void TestBoT(object sender, EventArgs e)
        {
            XMLH1 helper = new XMLH1();
            helper.Fill(DataBase);
            ExportToXML(helper,pathXMLServer);
           // SerializeObject(DataBase);
        }

        private void dataGridView1_CellMouseDoubleClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            string myValue = dataGridView1[e.ColumnIndex, e.RowIndex].Value.ToString();
            //myValue = myValue.Replace("\\",string.Empty);
            Process.Start("notepad.exe", "C:/Users/asafl/Desktop/שנקר/שנה ג/אחזור מידע/ServerFolder/doc/"+myValue);
            return;
        }

        private void button4_Click(object sender, EventArgs e)
        {

        }
    }
    [XmlRoot("DocPath")]
    public class DocPath
    {
        [XmlAttribute("name")]
        public string name { get; set; }
        [XmlAttribute("path")]
        public string path { get; set; }
        public DocPath(string ph , string name )
        {
            this.path = ph;
            this.name = name;
            
        }
        public DocPath()
        {
            path = "h";
            name = "empty";
            
        }
        public void Filll()
        {

        }
    }
    [XmlRoot("XMLH1")]
    public class XMLH1
    {
        [XmlArray("items")]
        [XmlArrayItem("Items")]
        public XMLH2[] Items;
        public List<Node<string>> Pull()
        {
            int i = 0, j = 0;
            List<Node<string>> newone = new List<Node<string>>();
            for (; i < Items.Length; ++i)
            {
                Node<string> x = new Node<string>();
                x.hits = Items[i].Hits;
                x.info = Items[i].Name;
                x.DocList = new List<Doc>();
                for (j = 0; j < Items[i].Items.Length; j++)
                {
                    Doc y = new Doc();
                    y.Hit = Items[i].Items[j].Hit;
                    y.Name = Items[i].Items[j].Name;
                    x.DocList.Add(y);
                }
               
                newone.Add(x);

            }
            return newone;
        }
        public void Fill(List<Node<string>> W)
        {
            int i = 0, j = 0;
            Items = new XMLH2[W.Count];
            for(i =0; i< W.Count; ++i)
            {
                Items[i] = new XMLH2();
            }
            i = 0;
            foreach(Node<string> s in W)
            {
                Items[i].Hits = s.hits;
                Items[i].Name = s.info;
                Items[i].Items = new Doc[s.DocList.Count];
                for (j = 0; j < s.DocList.Count; ++j)
                {
                    Items[i].Items[j] = new Doc();
                }
                j = 0;
                foreach (Doc d in s.DocList){
                    Items[i].Items[j].Hit = d.Hit;
                    Items[i].Items[j].Name = d.Name;
                    j++;
                }
                i++;
            }
            

        }
       
    }
    [XmlType("XMLH2")]
    public class XMLH2
    {
        [XmlArray("items")]
        [XmlArrayItem("Items")]
        public Doc[] Items;
        [XmlAttribute("hits")]
        public int Hits;
        [XmlAttribute("name")]
        public string Name;
       
    }
    [XmlType("DOC")]
    public class Doc
    {
        [XmlAttribute("hit")]
        public int Hit { get; set; }
        [XmlAttribute("name")]
        public string Name { get; set; }
        [XmlIgnore]
        public int Hitsum { get; set; }
        [XmlIgnore]
        public string Inf { get; set; }
        
    }

    class KMP
    {
        public List<int> KMPSearch(string text, string pattern)
        {
            int N = text.Length;
            int M = pattern.Length;

            if (N < M) return new List<int> { -1};
            if (N == M && text == pattern) return new  List<int>{0};
            if (M == 0) return new List<int> {0};

            int[] lpsArray = new int[M];
            List<int> matchedIndex = new List<int>();

            LongestPrefixSuffix(pattern, ref lpsArray);

            int i = 0, j = 0;
            while (i < N)
            {
                if (text[i] == pattern[j])
                {
                    i++;
                    j++;
                }

                // match found at i-j
                if (j == M)
                {
                    matchedIndex.Add(i - j);
                    Console.WriteLine((i - j).ToString());
                    j = lpsArray[j - 1];
                }
                else if (i < N && text[i] != pattern[j])
                {
                    if (j != 0)
                    {
                        j = lpsArray[j - 1];
                    }
                    else
                    {
                        i++;
                    }
                }
            }

            return matchedIndex;
        }

        public void LongestPrefixSuffix(string pattern, ref int[] lpsArray)
        {
            int M = pattern.Length;
            int len = 0;
            lpsArray[0] = 0;
            int i = 1;

            while (i < M)
            {
                if (pattern[i] == pattern[len])
                {
                    len++;
                    lpsArray[i] = len;
                    i++;
                }
                else
                {
                    if (len == 0)
                    {
                        lpsArray[i] = 0;
                        i++;
                    }
                    else
                    {
                        len = lpsArray[len - 1];
                    }
                }
            }
        }
    }
    [XmlRoot("xmlDwice")]
    public class xmlDwice
    {
        [XmlArray("names")]
        [XmlArrayItem("nit")]
        public string[] names;
        [XmlArray("paths")]
        [XmlArrayItem("pit")]
        public string[] paths;
    }

    [XmlRoot("Node")]
    public class Node<T> 
    {
        public T info { get; set; }
        public int hits { get; set; }
        [XmlArray("DocList"), XmlArrayItem(typeof(List<string>), ElementName = "List<string>")]
        public List<Doc> DocList { get; set; }
        
        //public Node<T> next;
        /* הפעולה בונה ומחזירה חוליה שהערך שלה הוא info ואין לה חוליה עוקבת **/
        public Node(T info)
        {
            this.info = info;
            //this.next = null;
        }
        public Node()
        {
            
            //this.next = null;
        }
        /*הפעולה בונה ומחזירה חוליה, שהערך שלה הוא info
          והחוליה העוקבת לה היא החוליה next */
        //public Node(T info/*, Node<T> next*/)
        //{
        //    this.info = info;
        //    //this.next = next;
        //}
        public Node(T info, int a, string str)
        {
            this.info = info;
            //this.next = next;
            DocList = new List<Doc>();
            Doc d = new Doc();
            d.Name = str;
            d.Hit = 1;
            DocList.Add(d);
        }
        /* הפעולה מחזירה את הערך של החוליה הנוכחית **/
        public T GetInfo()
        {
            return info;
        }
        /* הפעולה מחזירה את החוליה העוקבת לחוליה הנוכחית **/
        public Node<T> GetNext()
        {
            // return next;
            return this;
        }
        /* הפעולה קובעת את ערך החוליה הנוכחית להיות  info **/
        public void SetInfo(T info)
        {
            this.info = info;
        }
        /* הפעולה קובעת את החוליה העוקבת לחוליה הנוכחית להיות החוליה next **/
        public void SetNext(Node<T> next)
        {
            //this.next = next;
        }
        /* הפעולה מחזירה מחרוזת המתארת את החוליה הנוכחית */
        public override string ToString()
        {
            return this.info.ToString();
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            throw new NotImplementedException();
        }

        public static implicit operator Node<T>(Node<string> v)
        {
            throw new NotImplementedException();
        }
    }
}
