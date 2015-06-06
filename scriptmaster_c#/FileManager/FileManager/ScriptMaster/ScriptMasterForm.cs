using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace ScriptMaster
{
    public partial class ScriptMasterForm : Form
    {
        public  string content;
        public  string program_version;
        public  string file_path;

        public  List<CodeTreeView> CodeTreeViews;
        public  CodeTreeView codeTreeView;
        public ScriptMasterForm()
        {
            InitializeComponent();
        }

        public void load(string FilePath)
        {
            if (System.IO.File.Exists(FilePath))
            {
                System.IO.Stream fileStream = new FileStream(FilePath, FileMode.Open);

                using (System.IO.StreamReader reader = new System.IO.StreamReader(fileStream))
                {
                    // Read the first line from the file and write it the textbox.
                    this.file_path = FilePath;
                    this.Text = FilePath;
                    this.textBox1.Enabled = true;
                    this.content = reader.ReadToEnd();

                }
                fileStream.Close();

                this.program_version = "ScriptMaster 1.0";
                this.CodeTreeViews = new List<CodeTreeView>();
                this.codeTreeView = new CodeTreeView();
                this.CodeTreeViews.Add(this.treeView1);
                this.setEvents();
                this.Text = this.program_version;
                this.textBox1.Text = this.content;
            }
        }
        private void setEvents()
        {
            this.FormClosed += Form1_FormClosed;
            this.textBox1.TextChanged += textBox1_TextChanged;
        }
        void textBox1_TextChanged(object sender, EventArgs e)
        {
            this.content = this.textBox1.Text;
        }

        void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            //Environment.Exit(0);
        }

        private void menuStrip1_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {

        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Environment.Exit(0);
        }

        private void newToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.file_path = Directory.GetCurrentDirectory() + "\\new file";
            this.Text = "new file";
            this.textBox1.Enabled = true;
            this.content = "Please Enter Your Content Here...";

        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {

          //  try
            {
                // Open the selected file to read.
                string FileName = this.file_path;
                DialogResult res1 = DialogResult.No;
                if (!File.Exists(FileName))
                {
                    res1 = MessageBox.Show("File Does Not Exist, Create?", "Warning", MessageBoxButtons.YesNo);
                }


                if (res1 == DialogResult.Yes||File.Exists(FileName))
                {
                    System.IO.Stream fileStream = new FileStream(FileName, FileMode.Create);

                    using (System.IO.StreamWriter writer = new System.IO.StreamWriter(fileStream))
                    {
                        // Read the first line from the file and write it the textbox.
                        writer.Write(this.content);
                    }
                    fileStream.Close();
                }
            }


            //catch
            {
              //  throw new Exception();
            }
        }

        private void saveAsToolStripMenuItem_Click(object sender, EventArgs e)
        {

            // Create an instance of the open file dialog box.
            SaveFileDialog saveFileDialog1 = new SaveFileDialog();

            // Set filter options and filter index.
            saveFileDialog1.Filter = "All Files (*.*)|*.*|Text Files (.txt)|*.txt";
            saveFileDialog1.FilterIndex = 1;

           // saveFileDialog1.Multiselect = true;

            // Call the ShowDialog method to show the dialog box.
            DialogResult? userClickedOK = saveFileDialog1.ShowDialog();

            // Process input if the user clicked OK.
            if (userClickedOK.HasValue == true)
            {
                if (userClickedOK.Value.ToString() == "Cancel")
                {
                    return;
                }
                try
                {
                    // Open the selected file to read.
                    string FileName = saveFileDialog1.FileName;
                    DialogResult res1 = DialogResult.No;
                    if (saveFileDialog1.CheckFileExists)
                    {
                        res1 = MessageBox.Show("File Exists, Overwrite?", "Warning", MessageBoxButtons.YesNo);
                    }

                    if (res1 == DialogResult.Yes || !saveFileDialog1.CheckFileExists)
                    {
                        System.IO.Stream fileStream = new FileStream(FileName, FileMode.Create);

                        using (System.IO.StreamWriter writer = new System.IO.StreamWriter(fileStream))
                        {
                            // Read the first line from the file and write it the textbox.
                            writer.Write(this.content);
                        }
                        fileStream.Close();
                    }
                    else if (res1 == DialogResult.No) { }
                }


                catch
                {
                    throw new Exception();
                }
            }
        }

        private void closeFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (this.file_path != null)
            {
                this.file_path = null;
                this.content = "";
                this.textBox1.Enabled = false;
                this.Text = this.program_version;
            }
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Create an instance of the open file dialog box.
            OpenFileDialog openFileDialog1 = new OpenFileDialog();

            // Set filter options and filter index.
            openFileDialog1.Filter = "All Files (*.*)|*.*|Text Files (.txt)|*.txt";
            openFileDialog1.FilterIndex = 1;

            openFileDialog1.Multiselect = true;

            // Call the ShowDialog method to show the dialog box.
            DialogResult? userClickedOK = openFileDialog1.ShowDialog();

            // Process input if the user clicked OK.
            if (userClickedOK.HasValue == true)
            {
              // Console.WriteLine( userClickedOK.Value.ToString());
                if (userClickedOK.Value.ToString() == "Cancel")
                {
                    return;
                }
              //  try
                {
                    // Open the selected file to read.
                    string FileName = openFileDialog1.FileName;
                    System.IO.Stream fileStream = new FileStream(FileName, FileMode.Open);

                    using (System.IO.StreamReader reader = new System.IO.StreamReader(fileStream))
                    {
                        // Read the first line from the file and write it the textbox.
                        this.file_path = FileName;
                        this.Text = FileName;
                        this.textBox1.Enabled = true;
                        this.content = reader.ReadToEnd();
                    }
                    fileStream.Close();
                }
                //catch
                {
                   // throw new Exception();
                }
            }
        }
        
    }
}
