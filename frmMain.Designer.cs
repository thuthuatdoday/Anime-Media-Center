namespace Anime_Media_Center
{
    partial class frmMain
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmMain));
            this.txtSearch = new System.Windows.Forms.TextBox();
            this.btScan = new System.Windows.Forms.Button();
            this.lstAnime = new System.Windows.Forms.ListBox();
            this.btAdd = new System.Windows.Forms.Button();
            this.btEdit = new System.Windows.Forms.Button();
            this.btRemove = new System.Windows.Forms.Button();
            this.imgCover = new System.Windows.Forms.PictureBox();
            this.lbUrl = new System.Windows.Forms.Label();
            this.txtUrl = new System.Windows.Forms.TextBox();
            this.btGet = new System.Windows.Forms.Button();
            this.lbTitle = new System.Windows.Forms.Label();
            this.txtTitle = new System.Windows.Forms.TextBox();
            this.lbType = new System.Windows.Forms.Label();
            this.cbType = new System.Windows.Forms.ComboBox();
            this.txtType = new System.Windows.Forms.TextBox();
            this.lbGenre = new System.Windows.Forms.Label();
            this.txtGenre = new System.Windows.Forms.TextBox();
            this.lbStatus = new System.Windows.Forms.Label();
            this.txtStatus = new System.Windows.Forms.TextBox();
            this.cbStatus = new System.Windows.Forms.ComboBox();
            this.lbRelease = new System.Windows.Forms.Label();
            this.txtRelease = new System.Windows.Forms.TextBox();
            this.txtSeason = new System.Windows.Forms.TextBox();
            this.lbInfo = new System.Windows.Forms.Label();
            this.txtInfo = new System.Windows.Forms.TextBox();
            this.lbCover = new System.Windows.Forms.Label();
            this.txtCover = new System.Windows.Forms.TextBox();
            this.btOpenImg = new System.Windows.Forms.Button();
            this.lbLink = new System.Windows.Forms.Label();
            this.txtLink = new System.Windows.Forms.TextBox();
            this.txtEpisode = new System.Windows.Forms.TextBox();
            this.btOpenDir = new System.Windows.Forms.Button();
            this.lstEpisode = new System.Windows.Forms.ListBox();
            this.lbMessage = new System.Windows.Forms.Label();
            this.txtMessage = new System.Windows.Forms.TextBox();
            this.btSave = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.imgCover)).BeginInit();
            this.SuspendLayout();
            // 
            // txtSearch
            // 
            this.txtSearch.Location = new System.Drawing.Point(12, 12);
            this.txtSearch.Name = "txtSearch";
            this.txtSearch.Size = new System.Drawing.Size(156, 20);
            this.txtSearch.TabIndex = 0;
            this.txtSearch.TextChanged += new System.EventHandler(this.txtSearch_TextChanged);
            // 
            // btScan
            // 
            this.btScan.Location = new System.Drawing.Point(174, 11);
            this.btScan.Name = "btScan";
            this.btScan.Size = new System.Drawing.Size(75, 23);
            this.btScan.TabIndex = 1;
            this.btScan.Text = "Scan Folder";
            this.btScan.UseVisualStyleBackColor = true;
            this.btScan.Click += new System.EventHandler(this.btScan_Click);
            // 
            // lstAnime
            // 
            this.lstAnime.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.lstAnime.FormattingEnabled = true;
            this.lstAnime.HorizontalScrollbar = true;
            this.lstAnime.Location = new System.Drawing.Point(12, 38);
            this.lstAnime.Name = "lstAnime";
            this.lstAnime.Size = new System.Drawing.Size(237, 485);
            this.lstAnime.TabIndex = 2;
            this.lstAnime.DrawItem += new System.Windows.Forms.DrawItemEventHandler(this.lstAnime_DrawItem);
            this.lstAnime.SelectedIndexChanged += new System.EventHandler(this.lstAnime_SelectedIndexChanged);
            // 
            // btAdd
            // 
            this.btAdd.Location = new System.Drawing.Point(12, 531);
            this.btAdd.Name = "btAdd";
            this.btAdd.Size = new System.Drawing.Size(75, 23);
            this.btAdd.TabIndex = 3;
            this.btAdd.Text = "Add";
            this.btAdd.UseVisualStyleBackColor = true;
            this.btAdd.Click += new System.EventHandler(this.btAdd_Click);
            // 
            // btEdit
            // 
            this.btEdit.Enabled = false;
            this.btEdit.Location = new System.Drawing.Point(93, 531);
            this.btEdit.Name = "btEdit";
            this.btEdit.Size = new System.Drawing.Size(75, 23);
            this.btEdit.TabIndex = 4;
            this.btEdit.Text = "Edit";
            this.btEdit.UseVisualStyleBackColor = true;
            this.btEdit.Click += new System.EventHandler(this.btEdit_Click);
            // 
            // btRemove
            // 
            this.btRemove.Enabled = false;
            this.btRemove.Location = new System.Drawing.Point(174, 531);
            this.btRemove.Name = "btRemove";
            this.btRemove.Size = new System.Drawing.Size(75, 23);
            this.btRemove.TabIndex = 5;
            this.btRemove.Text = "Remove";
            this.btRemove.UseVisualStyleBackColor = true;
            this.btRemove.Click += new System.EventHandler(this.btRemove_Click);
            // 
            // imgCover
            // 
            this.imgCover.ErrorImage = null;
            this.imgCover.InitialImage = null;
            this.imgCover.Location = new System.Drawing.Point(255, 11);
            this.imgCover.Name = "imgCover";
            this.imgCover.Size = new System.Drawing.Size(225, 334);
            this.imgCover.TabIndex = 6;
            this.imgCover.TabStop = false;
            // 
            // lbUrl
            // 
            this.lbUrl.AutoSize = true;
            this.lbUrl.Location = new System.Drawing.Point(486, 17);
            this.lbUrl.Name = "lbUrl";
            this.lbUrl.Size = new System.Drawing.Size(52, 13);
            this.lbUrl.TabIndex = 7;
            this.lbUrl.Text = "Get URL:";
            // 
            // txtUrl
            // 
            this.txtUrl.Location = new System.Drawing.Point(544, 13);
            this.txtUrl.Name = "txtUrl";
            this.txtUrl.ReadOnly = true;
            this.txtUrl.Size = new System.Drawing.Size(155, 20);
            this.txtUrl.TabIndex = 8;
            // 
            // btGet
            // 
            this.btGet.Enabled = false;
            this.btGet.Location = new System.Drawing.Point(705, 12);
            this.btGet.Name = "btGet";
            this.btGet.Size = new System.Drawing.Size(75, 23);
            this.btGet.TabIndex = 9;
            this.btGet.Text = "Get Info";
            this.btGet.UseVisualStyleBackColor = true;
            this.btGet.Click += new System.EventHandler(this.btGet_Click);
            // 
            // lbTitle
            // 
            this.lbTitle.AutoSize = true;
            this.lbTitle.Location = new System.Drawing.Point(486, 43);
            this.lbTitle.Name = "lbTitle";
            this.lbTitle.Size = new System.Drawing.Size(30, 13);
            this.lbTitle.TabIndex = 10;
            this.lbTitle.Text = "Title:";
            // 
            // txtTitle
            // 
            this.txtTitle.Location = new System.Drawing.Point(544, 39);
            this.txtTitle.Name = "txtTitle";
            this.txtTitle.ReadOnly = true;
            this.txtTitle.Size = new System.Drawing.Size(236, 20);
            this.txtTitle.TabIndex = 11;
            // 
            // lbType
            // 
            this.lbType.AutoSize = true;
            this.lbType.Location = new System.Drawing.Point(486, 69);
            this.lbType.Name = "lbType";
            this.lbType.Size = new System.Drawing.Size(34, 13);
            this.lbType.TabIndex = 12;
            this.lbType.Text = "Type:";
            // 
            // cbType
            // 
            this.cbType.Enabled = false;
            this.cbType.FormattingEnabled = true;
            this.cbType.Items.AddRange(new object[] {
            "TV Series",
            "Season",
            "Movies",
            "Specials",
            "OVA",
            "ONA",
            "Live Action"});
            this.cbType.Location = new System.Drawing.Point(544, 64);
            this.cbType.Name = "cbType";
            this.cbType.Size = new System.Drawing.Size(121, 21);
            this.cbType.TabIndex = 13;
            this.cbType.SelectedIndexChanged += new System.EventHandler(this.cbType_SelectedIndexChanged);
            // 
            // txtType
            // 
            this.txtType.Location = new System.Drawing.Point(671, 65);
            this.txtType.Name = "txtType";
            this.txtType.ReadOnly = true;
            this.txtType.Size = new System.Drawing.Size(109, 20);
            this.txtType.TabIndex = 14;
            // 
            // lbGenre
            // 
            this.lbGenre.AutoSize = true;
            this.lbGenre.Location = new System.Drawing.Point(486, 95);
            this.lbGenre.Name = "lbGenre";
            this.lbGenre.Size = new System.Drawing.Size(39, 13);
            this.lbGenre.TabIndex = 15;
            this.lbGenre.Text = "Genre:";
            // 
            // txtGenre
            // 
            this.txtGenre.Location = new System.Drawing.Point(544, 91);
            this.txtGenre.Name = "txtGenre";
            this.txtGenre.ReadOnly = true;
            this.txtGenre.Size = new System.Drawing.Size(236, 20);
            this.txtGenre.TabIndex = 16;
            // 
            // lbStatus
            // 
            this.lbStatus.AutoSize = true;
            this.lbStatus.Location = new System.Drawing.Point(486, 121);
            this.lbStatus.Name = "lbStatus";
            this.lbStatus.Size = new System.Drawing.Size(40, 13);
            this.lbStatus.TabIndex = 17;
            this.lbStatus.Text = "Status:";
            // 
            // txtStatus
            // 
            this.txtStatus.Location = new System.Drawing.Point(544, 117);
            this.txtStatus.Name = "txtStatus";
            this.txtStatus.ReadOnly = true;
            this.txtStatus.Size = new System.Drawing.Size(109, 20);
            this.txtStatus.TabIndex = 18;
            // 
            // cbStatus
            // 
            this.cbStatus.Enabled = false;
            this.cbStatus.FormattingEnabled = true;
            this.cbStatus.Items.AddRange(new object[] {
            "Ongoing",
            "Dropping",
            "Complete"});
            this.cbStatus.Location = new System.Drawing.Point(659, 117);
            this.cbStatus.Name = "cbStatus";
            this.cbStatus.Size = new System.Drawing.Size(121, 21);
            this.cbStatus.TabIndex = 19;
            // 
            // lbRelease
            // 
            this.lbRelease.AutoSize = true;
            this.lbRelease.Location = new System.Drawing.Point(486, 147);
            this.lbRelease.Name = "lbRelease";
            this.lbRelease.Size = new System.Drawing.Size(49, 13);
            this.lbRelease.TabIndex = 20;
            this.lbRelease.Text = "Release:";
            // 
            // txtRelease
            // 
            this.txtRelease.Location = new System.Drawing.Point(544, 143);
            this.txtRelease.Name = "txtRelease";
            this.txtRelease.ReadOnly = true;
            this.txtRelease.Size = new System.Drawing.Size(155, 20);
            this.txtRelease.TabIndex = 21;
            this.txtRelease.Leave += new System.EventHandler(this.txtRelease_Leave);
            // 
            // txtSeason
            // 
            this.txtSeason.Location = new System.Drawing.Point(705, 143);
            this.txtSeason.Name = "txtSeason";
            this.txtSeason.ReadOnly = true;
            this.txtSeason.Size = new System.Drawing.Size(75, 20);
            this.txtSeason.TabIndex = 22;
            // 
            // lbInfo
            // 
            this.lbInfo.AutoSize = true;
            this.lbInfo.Location = new System.Drawing.Point(486, 173);
            this.lbInfo.Name = "lbInfo";
            this.lbInfo.Size = new System.Drawing.Size(62, 13);
            this.lbInfo.TabIndex = 23;
            this.lbInfo.Text = "Information:";
            // 
            // txtInfo
            // 
            this.txtInfo.Location = new System.Drawing.Point(486, 189);
            this.txtInfo.Multiline = true;
            this.txtInfo.Name = "txtInfo";
            this.txtInfo.ReadOnly = true;
            this.txtInfo.Size = new System.Drawing.Size(294, 130);
            this.txtInfo.TabIndex = 24;
            // 
            // lbCover
            // 
            this.lbCover.AutoSize = true;
            this.lbCover.Location = new System.Drawing.Point(486, 329);
            this.lbCover.Name = "lbCover";
            this.lbCover.Size = new System.Drawing.Size(39, 13);
            this.lbCover.TabIndex = 25;
            this.lbCover.Text = "Image:";
            // 
            // txtCover
            // 
            this.txtCover.Location = new System.Drawing.Point(531, 325);
            this.txtCover.Name = "txtCover";
            this.txtCover.ReadOnly = true;
            this.txtCover.Size = new System.Drawing.Size(168, 20);
            this.txtCover.TabIndex = 26;
            // 
            // btOpenImg
            // 
            this.btOpenImg.Enabled = false;
            this.btOpenImg.Location = new System.Drawing.Point(705, 324);
            this.btOpenImg.Name = "btOpenImg";
            this.btOpenImg.Size = new System.Drawing.Size(75, 23);
            this.btOpenImg.TabIndex = 27;
            this.btOpenImg.Text = "Open Image";
            this.btOpenImg.UseVisualStyleBackColor = true;
            this.btOpenImg.Click += new System.EventHandler(this.btOpenImg_Click);
            // 
            // lbLink
            // 
            this.lbLink.AutoSize = true;
            this.lbLink.Location = new System.Drawing.Point(255, 355);
            this.lbLink.Name = "lbLink";
            this.lbLink.Size = new System.Drawing.Size(67, 13);
            this.lbLink.TabIndex = 28;
            this.lbLink.Text = "List Episode:";
            // 
            // txtLink
            // 
            this.txtLink.Location = new System.Drawing.Point(328, 351);
            this.txtLink.Name = "txtLink";
            this.txtLink.ReadOnly = true;
            this.txtLink.Size = new System.Drawing.Size(328, 20);
            this.txtLink.TabIndex = 29;
            // 
            // txtEpisode
            // 
            this.txtEpisode.Location = new System.Drawing.Point(662, 351);
            this.txtEpisode.Name = "txtEpisode";
            this.txtEpisode.ReadOnly = true;
            this.txtEpisode.Size = new System.Drawing.Size(37, 20);
            this.txtEpisode.TabIndex = 30;
            // 
            // btOpenDir
            // 
            this.btOpenDir.Enabled = false;
            this.btOpenDir.Location = new System.Drawing.Point(705, 350);
            this.btOpenDir.Name = "btOpenDir";
            this.btOpenDir.Size = new System.Drawing.Size(75, 23);
            this.btOpenDir.TabIndex = 31;
            this.btOpenDir.Text = "Open Folder";
            this.btOpenDir.UseVisualStyleBackColor = true;
            this.btOpenDir.Click += new System.EventHandler(this.btOpenDir_Click);
            // 
            // lstEpisode
            // 
            this.lstEpisode.FormattingEnabled = true;
            this.lstEpisode.Location = new System.Drawing.Point(258, 377);
            this.lstEpisode.Name = "lstEpisode";
            this.lstEpisode.Size = new System.Drawing.Size(522, 147);
            this.lstEpisode.TabIndex = 32;
            this.lstEpisode.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.lstEpisode_MouseDoubleClick);
            this.lstEpisode.SelectedIndexChanged += new System.EventHandler(this.lstEpisode_SelectedIndexChanged);
            // 
            // lbMessage
            // 
            this.lbMessage.AutoSize = true;
            this.lbMessage.Location = new System.Drawing.Point(255, 536);
            this.lbMessage.Name = "lbMessage";
            this.lbMessage.Size = new System.Drawing.Size(53, 13);
            this.lbMessage.TabIndex = 33;
            this.lbMessage.Text = "Message:";
            // 
            // txtMessage
            // 
            this.txtMessage.Location = new System.Drawing.Point(314, 532);
            this.txtMessage.Name = "txtMessage";
            this.txtMessage.ReadOnly = true;
            this.txtMessage.Size = new System.Drawing.Size(385, 20);
            this.txtMessage.TabIndex = 34;
            // 
            // btSave
            // 
            this.btSave.Enabled = false;
            this.btSave.Location = new System.Drawing.Point(705, 531);
            this.btSave.Name = "btSave";
            this.btSave.Size = new System.Drawing.Size(75, 23);
            this.btSave.TabIndex = 35;
            this.btSave.Text = "Save";
            this.btSave.UseVisualStyleBackColor = true;
            this.btSave.Click += new System.EventHandler(this.btSave_Click);
            // 
            // frmMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(792, 565);
            this.Controls.Add(this.btSave);
            this.Controls.Add(this.txtMessage);
            this.Controls.Add(this.lbMessage);
            this.Controls.Add(this.lstEpisode);
            this.Controls.Add(this.btOpenDir);
            this.Controls.Add(this.txtEpisode);
            this.Controls.Add(this.txtLink);
            this.Controls.Add(this.lbLink);
            this.Controls.Add(this.btOpenImg);
            this.Controls.Add(this.txtCover);
            this.Controls.Add(this.lbCover);
            this.Controls.Add(this.txtInfo);
            this.Controls.Add(this.lbInfo);
            this.Controls.Add(this.txtSeason);
            this.Controls.Add(this.txtRelease);
            this.Controls.Add(this.lbRelease);
            this.Controls.Add(this.cbStatus);
            this.Controls.Add(this.txtStatus);
            this.Controls.Add(this.lbStatus);
            this.Controls.Add(this.txtGenre);
            this.Controls.Add(this.lbGenre);
            this.Controls.Add(this.txtType);
            this.Controls.Add(this.cbType);
            this.Controls.Add(this.lbType);
            this.Controls.Add(this.txtTitle);
            this.Controls.Add(this.lbTitle);
            this.Controls.Add(this.btGet);
            this.Controls.Add(this.txtUrl);
            this.Controls.Add(this.lbUrl);
            this.Controls.Add(this.imgCover);
            this.Controls.Add(this.btRemove);
            this.Controls.Add(this.btEdit);
            this.Controls.Add(this.btAdd);
            this.Controls.Add(this.lstAnime);
            this.Controls.Add(this.btScan);
            this.Controls.Add(this.txtSearch);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "frmMain";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Anime Media Center";
            this.Load += new System.EventHandler(this.frmMain_Load);
            ((System.ComponentModel.ISupportInitialize)(this.imgCover)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox txtSearch;
        private System.Windows.Forms.Button btScan;
        private System.Windows.Forms.ListBox lstAnime;
        private System.Windows.Forms.Button btAdd;
        private System.Windows.Forms.Button btEdit;
        private System.Windows.Forms.Button btRemove;
        private System.Windows.Forms.PictureBox imgCover;
        private System.Windows.Forms.Label lbUrl;
        private System.Windows.Forms.TextBox txtUrl;
        private System.Windows.Forms.Button btGet;
        private System.Windows.Forms.Label lbTitle;
        private System.Windows.Forms.TextBox txtTitle;
        private System.Windows.Forms.Label lbType;
        private System.Windows.Forms.ComboBox cbType;
        private System.Windows.Forms.TextBox txtType;
        private System.Windows.Forms.Label lbGenre;
        private System.Windows.Forms.TextBox txtGenre;
        private System.Windows.Forms.Label lbStatus;
        private System.Windows.Forms.TextBox txtStatus;
        private System.Windows.Forms.ComboBox cbStatus;
        private System.Windows.Forms.Label lbRelease;
        private System.Windows.Forms.TextBox txtRelease;
        private System.Windows.Forms.TextBox txtSeason;
        private System.Windows.Forms.Label lbInfo;
        private System.Windows.Forms.TextBox txtInfo;
        private System.Windows.Forms.Label lbCover;
        private System.Windows.Forms.TextBox txtCover;
        private System.Windows.Forms.Button btOpenImg;
        private System.Windows.Forms.Label lbLink;
        private System.Windows.Forms.TextBox txtLink;
        private System.Windows.Forms.TextBox txtEpisode;
        private System.Windows.Forms.Button btOpenDir;
        private System.Windows.Forms.ListBox lstEpisode;
        private System.Windows.Forms.Label lbMessage;
        private System.Windows.Forms.TextBox txtMessage;
        private System.Windows.Forms.Button btSave;
    }
}

