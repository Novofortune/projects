﻿namespace FileManager
{
    partial class FileExplorerForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.PathBox = new System.Windows.Forms.TextBox();
            this.fileTreeView = new System.Windows.Forms.TreeView();
            this.label1 = new System.Windows.Forms.Label();
            this.FileCountBox = new System.Windows.Forms.TextBox();
            this.FilterBox = new System.Windows.Forms.TextBox();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.closeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.treeViewToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exportTreeViewToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.importTreeViewToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.formSize = new System.Windows.Forms.Label();
            this.btnBack = new System.Windows.Forms.Button();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // PathBox
            // 
            this.PathBox.Location = new System.Drawing.Point(119, 35);
            this.PathBox.Name = "PathBox";
            this.PathBox.Size = new System.Drawing.Size(289, 20);
            this.PathBox.TabIndex = 0;
            // 
            // fileTreeView
            // 
            this.fileTreeView.AllowDrop = true;
            this.fileTreeView.CheckBoxes = true;
            this.fileTreeView.Location = new System.Drawing.Point(14, 64);
            this.fileTreeView.MaximumSize = new System.Drawing.Size(380, 357);
            this.fileTreeView.Name = "fileTreeView";
            this.fileTreeView.Size = new System.Drawing.Size(287, 269);
            this.fileTreeView.TabIndex = 1;
            this.fileTreeView.BeforeCollapse += new System.Windows.Forms.TreeViewCancelEventHandler(this.fileTreeView_BeforeCollapse);
            this.fileTreeView.BeforeExpand += new System.Windows.Forms.TreeViewCancelEventHandler(this.fileTreeView_BeforeExpand);
            this.fileTreeView.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.treeView1_AfterSelect);
            this.fileTreeView.MouseDown += new System.Windows.Forms.MouseEventHandler(this.fileTreeView_MouseDown);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(84, 38);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(29, 13);
            this.label1.TabIndex = 4;
            this.label1.Text = "Path";
            // 
            // FileCountBox
            // 
            this.FileCountBox.Enabled = false;
            this.FileCountBox.Location = new System.Drawing.Point(308, 78);
            this.FileCountBox.MaximumSize = new System.Drawing.Size(230, 21);
            this.FileCountBox.Name = "FileCountBox";
            this.FileCountBox.Size = new System.Drawing.Size(100, 20);
            this.FileCountBox.TabIndex = 8;
            // 
            // FilterBox
            // 
            this.FilterBox.Location = new System.Drawing.Point(308, 104);
            this.FilterBox.MaximumSize = new System.Drawing.Size(230, 21);
            this.FilterBox.Name = "FilterBox";
            this.FilterBox.Size = new System.Drawing.Size(100, 20);
            this.FilterBox.TabIndex = 9;
            this.FilterBox.Text = "Extension Filter";
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.treeViewToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(424, 25);
            this.menuStrip1.TabIndex = 10;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.closeToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(39, 21);
            this.fileToolStripMenuItem.Text = "File";
            // 
            // closeToolStripMenuItem
            // 
            this.closeToolStripMenuItem.Name = "closeToolStripMenuItem";
            this.closeToolStripMenuItem.Size = new System.Drawing.Size(108, 22);
            this.closeToolStripMenuItem.Text = "Close";
            // 
            // treeViewToolStripMenuItem
            // 
            this.treeViewToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.exportTreeViewToolStripMenuItem,
            this.importTreeViewToolStripMenuItem});
            this.treeViewToolStripMenuItem.Name = "treeViewToolStripMenuItem";
            this.treeViewToolStripMenuItem.Size = new System.Drawing.Size(73, 21);
            this.treeViewToolStripMenuItem.Text = "TreeView";
            // 
            // exportTreeViewToolStripMenuItem
            // 
            this.exportTreeViewToolStripMenuItem.Name = "exportTreeViewToolStripMenuItem";
            this.exportTreeViewToolStripMenuItem.Size = new System.Drawing.Size(173, 22);
            this.exportTreeViewToolStripMenuItem.Text = "Export TreeView";
            this.exportTreeViewToolStripMenuItem.Click += new System.EventHandler(this.exportTreeViewToolStripMenuItem_Click);
            // 
            // importTreeViewToolStripMenuItem
            // 
            this.importTreeViewToolStripMenuItem.Name = "importTreeViewToolStripMenuItem";
            this.importTreeViewToolStripMenuItem.Size = new System.Drawing.Size(173, 22);
            this.importTreeViewToolStripMenuItem.Text = "Import TreeView";
            this.importTreeViewToolStripMenuItem.Click += new System.EventHandler(this.importTreeViewToolStripMenuItem_Click);
            // 
            // formSize
            // 
            this.formSize.AutoSize = true;
            this.formSize.Location = new System.Drawing.Point(311, 12);
            this.formSize.Name = "formSize";
            this.formSize.Size = new System.Drawing.Size(35, 13);
            this.formSize.TabIndex = 11;
            this.formSize.Text = "label2";
            // 
            // btnBack
            // 
            this.btnBack.Location = new System.Drawing.Point(14, 33);
            this.btnBack.Name = "btnBack";
            this.btnBack.Size = new System.Drawing.Size(64, 25);
            this.btnBack.TabIndex = 12;
            this.btnBack.Text = "Back";
            this.btnBack.UseVisualStyleBackColor = true;
            this.btnBack.Click += new System.EventHandler(this.btnBack_Click);
            // 
            // FileExplorerForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(424, 337);
            this.Controls.Add(this.btnBack);
            this.Controls.Add(this.formSize);
            this.Controls.Add(this.FilterBox);
            this.Controls.Add(this.FileCountBox);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.fileTreeView);
            this.Controls.Add(this.PathBox);
            this.Controls.Add(this.menuStrip1);
            this.MainMenuStrip = this.menuStrip1;
            this.MaximumSize = new System.Drawing.Size(650, 484);
            this.MinimumSize = new System.Drawing.Size(440, 376);
            this.Name = "FileExplorerForm";
            this.Text = "File Explorer";
            this.Resize += new System.EventHandler(this.FileExplorerForm_Resize);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        public System.Windows.Forms.TextBox PathBox;
        public System.Windows.Forms.TreeView fileTreeView;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox FileCountBox;
        private System.Windows.Forms.TextBox FilterBox;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem closeToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem treeViewToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem exportTreeViewToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem importTreeViewToolStripMenuItem;
        private System.Windows.Forms.Label formSize;
        private System.Windows.Forms.Button btnBack;
    }
}