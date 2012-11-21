namespace HaloScreenshots
{
    partial class Form1
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.mainMenuStrip = new System.Windows.Forms.MenuStrip();
            this.menuFile = new System.Windows.Forms.ToolStripMenuItem();
            this.submenuSingle = new System.Windows.Forms.ToolStripMenuItem();
            this.submenuBatch = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripSeparator();
            this.submenuRemove = new System.Windows.Forms.ToolStripMenuItem();
            this.submenuRemoveSelection = new System.Windows.Forms.ToolStripMenuItem();
            this.submenuRemoveInverseSelection = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem2 = new System.Windows.Forms.ToolStripSeparator();
            this.submenuExit = new System.Windows.Forms.ToolStripMenuItem();
            this.menuSave = new System.Windows.Forms.ToolStripMenuItem();
            this.submenuSaveAll = new System.Windows.Forms.ToolStripMenuItem();
            this.submenuSaveSelected = new System.Windows.Forms.ToolStripMenuItem();
            this.menuMore = new System.Windows.Forms.ToolStripMenuItem();
            this.submenuAutosave = new System.Windows.Forms.ToolStripMenuItem();
            this.submenuUpdate = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem3 = new System.Windows.Forms.ToolStripSeparator();
            this.submenuAbout = new System.Windows.Forms.ToolStripMenuItem();
            this.fileList = new System.Windows.Forms.ListView();
            this.filenameCol = new System.Windows.Forms.ColumnHeader();
            this.titleCol = new System.Windows.Forms.ColumnHeader();
            this.gameCol = new System.Windows.Forms.ColumnHeader();
            this.mapCol = new System.Windows.Forms.ColumnHeader();
            this.offsetCol = new System.Windows.Forms.ColumnHeader();
            this.filesizeCol = new System.Windows.Forms.ColumnHeader();
            this.dateCol = new System.Windows.Forms.ColumnHeader();
            this.fbd = new System.Windows.Forms.FolderBrowserDialog();
            this.ofd = new System.Windows.Forms.OpenFileDialog();
            this.sfd = new System.Windows.Forms.SaveFileDialog();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.toolStripProgressBar1 = new System.Windows.Forms.ToolStripProgressBar();
            this.toolStripStatusLabel1 = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripContainer1 = new System.Windows.Forms.ToolStripContainer();
            this.contextMenu1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.contextMenuSaveThis = new System.Windows.Forms.ToolStripMenuItem();
            this.contextMenuSaveSelected = new System.Windows.Forms.ToolStripMenuItem();
            this.contextMenuRemove = new System.Windows.Forms.ToolStripMenuItem();
            this.submenuRemoveAll = new System.Windows.Forms.ToolStripMenuItem();
            this.bgw = new System.ComponentModel.BackgroundWorker();
            this.mainMenuStrip.SuspendLayout();
            this.statusStrip1.SuspendLayout();
            this.toolStripContainer1.BottomToolStripPanel.SuspendLayout();
            this.toolStripContainer1.ContentPanel.SuspendLayout();
            this.toolStripContainer1.TopToolStripPanel.SuspendLayout();
            this.toolStripContainer1.SuspendLayout();
            this.contextMenu1.SuspendLayout();
            this.SuspendLayout();
            // 
            // mainMenuStrip
            // 
            this.mainMenuStrip.Dock = System.Windows.Forms.DockStyle.None;
            this.mainMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuFile,
            this.menuSave,
            this.menuMore});
            this.mainMenuStrip.Location = new System.Drawing.Point(0, 0);
            this.mainMenuStrip.Name = "mainMenuStrip";
            this.mainMenuStrip.Size = new System.Drawing.Size(922, 24);
            this.mainMenuStrip.TabIndex = 0;
            this.mainMenuStrip.Text = "menuStrip1";
            // 
            // menuFile
            // 
            this.menuFile.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.submenuSingle,
            this.submenuBatch,
            this.toolStripMenuItem1,
            this.submenuRemove,
            this.toolStripMenuItem2,
            this.submenuExit});
            this.menuFile.Name = "menuFile";
            this.menuFile.Size = new System.Drawing.Size(37, 20);
            this.menuFile.Text = "File";
            // 
            // submenuSingle
            // 
            this.submenuSingle.Name = "submenuSingle";
            this.submenuSingle.Size = new System.Drawing.Size(152, 22);
            this.submenuSingle.Text = "Open Single...";
            this.submenuSingle.Click += new System.EventHandler(this.submenuSingle_Click);
            // 
            // submenuBatch
            // 
            this.submenuBatch.Name = "submenuBatch";
            this.submenuBatch.Size = new System.Drawing.Size(152, 22);
            this.submenuBatch.Text = "Batch...";
            this.submenuBatch.Click += new System.EventHandler(this.submenuBatch_Click);
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new System.Drawing.Size(149, 6);
            // 
            // submenuRemove
            // 
            this.submenuRemove.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.submenuRemoveSelection,
            this.submenuRemoveInverseSelection,
            this.submenuRemoveAll});
            this.submenuRemove.Name = "submenuRemove";
            this.submenuRemove.Size = new System.Drawing.Size(152, 22);
            this.submenuRemove.Text = "Remove";
            // 
            // submenuRemoveSelection
            // 
            this.submenuRemoveSelection.Enabled = false;
            this.submenuRemoveSelection.Name = "submenuRemoveSelection";
            this.submenuRemoveSelection.Size = new System.Drawing.Size(162, 22);
            this.submenuRemoveSelection.Text = "Selection";
            this.submenuRemoveSelection.Click += new System.EventHandler(this.submenuRemoveSelection_Click);
            // 
            // submenuRemoveInverseSelection
            // 
            this.submenuRemoveInverseSelection.Enabled = false;
            this.submenuRemoveInverseSelection.Name = "submenuRemoveInverseSelection";
            this.submenuRemoveInverseSelection.Size = new System.Drawing.Size(162, 22);
            this.submenuRemoveInverseSelection.Text = "Inverse Selection";
            this.submenuRemoveInverseSelection.Click += new System.EventHandler(this.submenuRemoveInverseSelection_Click);
            // 
            // toolStripMenuItem2
            // 
            this.toolStripMenuItem2.Name = "toolStripMenuItem2";
            this.toolStripMenuItem2.Size = new System.Drawing.Size(149, 6);
            // 
            // submenuExit
            // 
            this.submenuExit.Name = "submenuExit";
            this.submenuExit.Size = new System.Drawing.Size(152, 22);
            this.submenuExit.Text = "Exit";
            this.submenuExit.Click += new System.EventHandler(this.submenuExit_Click);
            // 
            // menuSave
            // 
            this.menuSave.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.submenuSaveAll,
            this.submenuSaveSelected});
            this.menuSave.Name = "menuSave";
            this.menuSave.Size = new System.Drawing.Size(43, 20);
            this.menuSave.Text = "Save";
            // 
            // submenuSaveAll
            // 
            this.submenuSaveAll.Enabled = false;
            this.submenuSaveAll.Name = "submenuSaveAll";
            this.submenuSaveAll.Size = new System.Drawing.Size(145, 22);
            this.submenuSaveAll.Text = "Save All...";
            this.submenuSaveAll.Click += new System.EventHandler(this.submenuSaveAll_Click);
            // 
            // submenuSaveSelected
            // 
            this.submenuSaveSelected.Enabled = false;
            this.submenuSaveSelected.Name = "submenuSaveSelected";
            this.submenuSaveSelected.Size = new System.Drawing.Size(145, 22);
            this.submenuSaveSelected.Text = "Save Selected";
            this.submenuSaveSelected.Click += new System.EventHandler(this.submenuSaveSelected_Click);
            // 
            // menuMore
            // 
            this.menuMore.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.submenuAutosave,
            this.submenuUpdate,
            this.toolStripMenuItem3,
            this.submenuAbout});
            this.menuMore.Name = "menuMore";
            this.menuMore.Size = new System.Drawing.Size(61, 20);
            this.menuMore.Text = "Options";
            // 
            // submenuAutosave
            // 
            this.submenuAutosave.Name = "submenuAutosave";
            this.submenuAutosave.Size = new System.Drawing.Size(158, 22);
            this.submenuAutosave.Text = "Autosave Single";
            this.submenuAutosave.Click += new System.EventHandler(this.submenuAutosave_Click);
            // 
            // submenuUpdate
            // 
            this.submenuUpdate.Name = "submenuUpdate";
            this.submenuUpdate.Size = new System.Drawing.Size(158, 22);
            this.submenuUpdate.Text = "Update";
            this.submenuUpdate.Click += new System.EventHandler(this.submenuUpdate_Click);
            // 
            // toolStripMenuItem3
            // 
            this.toolStripMenuItem3.Name = "toolStripMenuItem3";
            this.toolStripMenuItem3.Size = new System.Drawing.Size(155, 6);
            // 
            // submenuAbout
            // 
            this.submenuAbout.Name = "submenuAbout";
            this.submenuAbout.Size = new System.Drawing.Size(158, 22);
            this.submenuAbout.Text = "About";
            this.submenuAbout.Click += new System.EventHandler(this.submenuAbout_Click);
            // 
            // fileList
            // 
            this.fileList.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.filenameCol,
            this.titleCol,
            this.gameCol,
            this.mapCol,
            this.offsetCol,
            this.filesizeCol,
            this.dateCol});
            this.fileList.Dock = System.Windows.Forms.DockStyle.Fill;
            this.fileList.FullRowSelect = true;
            this.fileList.GridLines = true;
            this.fileList.Location = new System.Drawing.Point(0, 0);
            this.fileList.Name = "fileList";
            this.fileList.Size = new System.Drawing.Size(922, 328);
            this.fileList.TabIndex = 1;
            this.fileList.UseCompatibleStateImageBehavior = false;
            this.fileList.View = System.Windows.Forms.View.Details;
            this.fileList.ItemActivate += new System.EventHandler(this.fileList_ItemActivate);
            this.fileList.MouseClick += new System.Windows.Forms.MouseEventHandler(this.fileList_MouseClick);
            // 
            // filenameCol
            // 
            this.filenameCol.Tag = "filename";
            this.filenameCol.Text = "Filename";
            this.filenameCol.Width = 215;
            // 
            // titleCol
            // 
            this.titleCol.Tag = "title";
            this.titleCol.Text = "Screenshot Title";
            this.titleCol.Width = 110;
            // 
            // gameCol
            // 
            this.gameCol.Tag = "game";
            this.gameCol.Text = "Game";
            this.gameCol.Width = 115;
            // 
            // mapCol
            // 
            this.mapCol.Tag = "map";
            this.mapCol.Text = "Map";
            this.mapCol.Width = 112;
            // 
            // offsetCol
            // 
            this.offsetCol.Tag = "offset";
            this.offsetCol.Text = "JPEG Offset";
            this.offsetCol.Width = 90;
            // 
            // filesizeCol
            // 
            this.filesizeCol.Tag = "filesize";
            this.filesizeCol.Text = "JPEG Filesize";
            this.filesizeCol.Width = 90;
            // 
            // dateCol
            // 
            this.dateCol.Tag = "date";
            this.dateCol.Text = "File Date";
            this.dateCol.Width = 169;
            // 
            // fbd
            // 
            this.fbd.RootFolder = System.Environment.SpecialFolder.MyComputer;
            this.fbd.ShowNewFolderButton = false;
            // 
            // ofd
            // 
            this.ofd.AddExtension = false;
            this.ofd.Filter = "CON Files|*.*";
            // 
            // sfd
            // 
            this.sfd.DefaultExt = "jpg";
            this.sfd.Filter = "JPEG Image|*.jpg";
            // 
            // statusStrip1
            // 
            this.statusStrip1.Dock = System.Windows.Forms.DockStyle.None;
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripProgressBar1,
            this.toolStripStatusLabel1});
            this.statusStrip1.Location = new System.Drawing.Point(0, 0);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.RenderMode = System.Windows.Forms.ToolStripRenderMode.ManagerRenderMode;
            this.statusStrip1.Size = new System.Drawing.Size(922, 22);
            this.statusStrip1.SizingGrip = false;
            this.statusStrip1.TabIndex = 2;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // toolStripProgressBar1
            // 
            this.toolStripProgressBar1.Name = "toolStripProgressBar1";
            this.toolStripProgressBar1.Size = new System.Drawing.Size(100, 16);
            this.toolStripProgressBar1.Step = 1;
            this.toolStripProgressBar1.Style = System.Windows.Forms.ProgressBarStyle.Continuous;
            this.toolStripProgressBar1.Visible = false;
            // 
            // toolStripStatusLabel1
            // 
            this.toolStripStatusLabel1.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.toolStripStatusLabel1.Image = global::HaloScreenshots.Properties.Resources.loading_spinner;
            this.toolStripStatusLabel1.Name = "toolStripStatusLabel1";
            this.toolStripStatusLabel1.Size = new System.Drawing.Size(226, 17);
            this.toolStripStatusLabel1.Text = "Click File > Open Single or Batch to begin";
            // 
            // toolStripContainer1
            // 
            // 
            // toolStripContainer1.BottomToolStripPanel
            // 
            this.toolStripContainer1.BottomToolStripPanel.Controls.Add(this.statusStrip1);
            // 
            // toolStripContainer1.ContentPanel
            // 
            this.toolStripContainer1.ContentPanel.AutoScroll = true;
            this.toolStripContainer1.ContentPanel.Controls.Add(this.fileList);
            this.toolStripContainer1.ContentPanel.Size = new System.Drawing.Size(922, 328);
            this.toolStripContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.toolStripContainer1.LeftToolStripPanelVisible = false;
            this.toolStripContainer1.Location = new System.Drawing.Point(0, 0);
            this.toolStripContainer1.Name = "toolStripContainer1";
            this.toolStripContainer1.RightToolStripPanelVisible = false;
            this.toolStripContainer1.Size = new System.Drawing.Size(922, 374);
            this.toolStripContainer1.TabIndex = 3;
            // 
            // toolStripContainer1.TopToolStripPanel
            // 
            this.toolStripContainer1.TopToolStripPanel.Controls.Add(this.mainMenuStrip);
            // 
            // contextMenu1
            // 
            this.contextMenu1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.contextMenuSaveThis,
            this.contextMenuSaveSelected,
            this.contextMenuRemove});
            this.contextMenu1.Name = "contextMenu1";
            this.contextMenu1.Size = new System.Drawing.Size(146, 70);
            // 
            // contextMenuSaveThis
            // 
            this.contextMenuSaveThis.Name = "contextMenuSaveThis";
            this.contextMenuSaveThis.Size = new System.Drawing.Size(145, 22);
            this.contextMenuSaveThis.Text = "Save";
            this.contextMenuSaveThis.Click += new System.EventHandler(this.contextMenuSaveThis_Click);
            // 
            // contextMenuSaveSelected
            // 
            this.contextMenuSaveSelected.Name = "contextMenuSaveSelected";
            this.contextMenuSaveSelected.Size = new System.Drawing.Size(145, 22);
            this.contextMenuSaveSelected.Text = "Save Selected";
            this.contextMenuSaveSelected.Visible = false;
            this.contextMenuSaveSelected.Click += new System.EventHandler(this.contextMenuSaveSelected_Click);
            // 
            // contextMenuRemove
            // 
            this.contextMenuRemove.Name = "contextMenuRemove";
            this.contextMenuRemove.Size = new System.Drawing.Size(145, 22);
            this.contextMenuRemove.Text = "Remove";
            this.contextMenuRemove.Click += new System.EventHandler(this.contextMenuRemove_Click);
            // 
            // submenuRemoveAll
            // 
            this.submenuRemoveAll.Enabled = false;
            this.submenuRemoveAll.Name = "submenuRemoveAll";
            this.submenuRemoveAll.Size = new System.Drawing.Size(162, 22);
            this.submenuRemoveAll.Text = "All";
            this.submenuRemoveAll.Click += new System.EventHandler(this.allToolStripMenuItem_Click);
            // 
            // bgw
            // 
            this.bgw.DoWork += new System.ComponentModel.DoWorkEventHandler(this.bgw_DoWork);
            this.bgw.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.bgw_RunWorkerCompleted);
            // 
            // Form1
            // 
            this.AllowDrop = true;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(922, 374);
            this.Controls.Add(this.toolStripContainer1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.mainMenuStrip;
            this.MinimumSize = new System.Drawing.Size(826, 412);
            this.Name = "Form1";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Halo Screenshot Extractor";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.DragDrop += new System.Windows.Forms.DragEventHandler(this.Form1_DragDrop);
            this.DragEnter += new System.Windows.Forms.DragEventHandler(this.Form1_DragEnter);
            this.mainMenuStrip.ResumeLayout(false);
            this.mainMenuStrip.PerformLayout();
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.toolStripContainer1.BottomToolStripPanel.ResumeLayout(false);
            this.toolStripContainer1.BottomToolStripPanel.PerformLayout();
            this.toolStripContainer1.ContentPanel.ResumeLayout(false);
            this.toolStripContainer1.TopToolStripPanel.ResumeLayout(false);
            this.toolStripContainer1.TopToolStripPanel.PerformLayout();
            this.toolStripContainer1.ResumeLayout(false);
            this.toolStripContainer1.PerformLayout();
            this.contextMenu1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.MenuStrip mainMenuStrip;
        private System.Windows.Forms.ToolStripMenuItem menuFile;
        private System.Windows.Forms.ListView fileList;
        private System.Windows.Forms.ToolStripMenuItem submenuSingle;
        private System.Windows.Forms.ToolStripMenuItem submenuBatch;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem submenuExit;
        private System.Windows.Forms.ToolStripMenuItem menuSave;
        private System.Windows.Forms.ToolStripMenuItem submenuSaveAll;
        private System.Windows.Forms.ToolStripMenuItem submenuSaveSelected;
        private System.Windows.Forms.ToolStripMenuItem menuMore;
        private System.Windows.Forms.ColumnHeader filenameCol;
        private System.Windows.Forms.ColumnHeader titleCol;
        private System.Windows.Forms.ColumnHeader offsetCol;
        private System.Windows.Forms.ColumnHeader filesizeCol;
        private System.Windows.Forms.FolderBrowserDialog fbd;
        private System.Windows.Forms.OpenFileDialog ofd;
        private System.Windows.Forms.SaveFileDialog sfd;
        private System.Windows.Forms.ColumnHeader gameCol;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel1;
        private System.Windows.Forms.ToolStripMenuItem submenuAbout;
        private System.Windows.Forms.ToolStripContainer toolStripContainer1;
        private System.Windows.Forms.ToolStripProgressBar toolStripProgressBar1;
        private System.Windows.Forms.ColumnHeader dateCol;
        private System.Windows.Forms.ToolStripMenuItem submenuRemove;
        private System.Windows.Forms.ToolStripMenuItem submenuRemoveSelection;
        private System.Windows.Forms.ToolStripMenuItem submenuRemoveInverseSelection;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem2;
        private System.Windows.Forms.ColumnHeader mapCol;
        private System.Windows.Forms.ContextMenuStrip contextMenu1;
        private System.Windows.Forms.ToolStripMenuItem contextMenuSaveThis;
        private System.Windows.Forms.ToolStripMenuItem contextMenuRemove;
        private System.Windows.Forms.ToolStripMenuItem contextMenuSaveSelected;
        private System.Windows.Forms.ToolStripMenuItem submenuAutosave;
        private System.Windows.Forms.ToolStripMenuItem submenuUpdate;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem3;
        private System.Windows.Forms.ToolStripMenuItem submenuRemoveAll;
        private System.ComponentModel.BackgroundWorker bgw;
    }
}

