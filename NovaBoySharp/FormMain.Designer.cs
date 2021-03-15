namespace NovaBoySharp
{
    partial class FormMain
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormMain));
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.tabControls = new System.Windows.Forms.TabControl();
            this.pageDevelop = new System.Windows.Forms.TabPage();
            this.txtAssemblerOutput = new System.Windows.Forms.TextBox();
            this.txtCode = new ScintillaNET.Scintilla();
            this.pageDebug = new System.Windows.Forms.TabPage();
            this.lblStatus = new System.Windows.Forms.Label();
            this.txtOutput = new System.Windows.Forms.TextBox();
            this.mnuFile = new System.Windows.Forms.ToolStripDropDownButton();
            this.mniOpenROM = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.mniOpenASM = new System.Windows.Forms.ToolStripMenuItem();
            this.mniSaveASM = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.mniClose = new System.Windows.Forms.ToolStripMenuItem();
            this.btnAssemble = new System.Windows.Forms.ToolStripButton();
            this.btnReset = new System.Windows.Forms.ToolStripButton();
            this.btnStop = new System.Windows.Forms.ToolStripButton();
            this.btnStep = new System.Windows.Forms.ToolStripButton();
            this.btnRun = new System.Windows.Forms.ToolStripButton();
            this.btnTurbo = new System.Windows.Forms.ToolStripButton();
            this.toolStrip1.SuspendLayout();
            this.tabControls.SuspendLayout();
            this.pageDevelop.SuspendLayout();
            this.pageDebug.SuspendLayout();
            this.SuspendLayout();
            // 
            // toolStrip1
            // 
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnuFile,
            this.toolStripSeparator1,
            this.btnAssemble,
            this.btnReset,
            this.btnStop,
            this.btnStep,
            this.btnTurbo,
            this.btnRun});
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(1043, 25);
            this.toolStrip1.TabIndex = 6;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 25);
            // 
            // tabControls
            // 
            this.tabControls.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tabControls.Controls.Add(this.pageDevelop);
            this.tabControls.Controls.Add(this.pageDebug);
            this.tabControls.Location = new System.Drawing.Point(8, 32);
            this.tabControls.Name = "tabControls";
            this.tabControls.SelectedIndex = 0;
            this.tabControls.Size = new System.Drawing.Size(1032, 536);
            this.tabControls.TabIndex = 7;
            // 
            // pageDevelop
            // 
            this.pageDevelop.Controls.Add(this.txtAssemblerOutput);
            this.pageDevelop.Controls.Add(this.txtCode);
            this.pageDevelop.Location = new System.Drawing.Point(4, 22);
            this.pageDevelop.Name = "pageDevelop";
            this.pageDevelop.Padding = new System.Windows.Forms.Padding(3);
            this.pageDevelop.Size = new System.Drawing.Size(1024, 510);
            this.pageDevelop.TabIndex = 1;
            this.pageDevelop.Text = "Develop";
            this.pageDevelop.UseVisualStyleBackColor = true;
            // 
            // txtAssemblerOutput
            // 
            this.txtAssemblerOutput.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtAssemblerOutput.Font = new System.Drawing.Font("Consolas", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtAssemblerOutput.Location = new System.Drawing.Point(816, 8);
            this.txtAssemblerOutput.Multiline = true;
            this.txtAssemblerOutput.Name = "txtAssemblerOutput";
            this.txtAssemblerOutput.ReadOnly = true;
            this.txtAssemblerOutput.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtAssemblerOutput.Size = new System.Drawing.Size(193, 496);
            this.txtAssemblerOutput.TabIndex = 9;
            // 
            // txtCode
            // 
            this.txtCode.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtCode.Lexer = ScintillaNET.Lexer.Asm;
            this.txtCode.Location = new System.Drawing.Point(8, 8);
            this.txtCode.Name = "txtCode";
            this.txtCode.Size = new System.Drawing.Size(800, 496);
            this.txtCode.TabIndex = 0;
            // 
            // pageDebug
            // 
            this.pageDebug.Controls.Add(this.lblStatus);
            this.pageDebug.Controls.Add(this.txtOutput);
            this.pageDebug.Location = new System.Drawing.Point(4, 22);
            this.pageDebug.Name = "pageDebug";
            this.pageDebug.Padding = new System.Windows.Forms.Padding(3);
            this.pageDebug.Size = new System.Drawing.Size(1024, 510);
            this.pageDebug.TabIndex = 0;
            this.pageDebug.Text = "Debug";
            this.pageDebug.UseVisualStyleBackColor = true;
            // 
            // lblStatus
            // 
            this.lblStatus.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblStatus.AutoSize = true;
            this.lblStatus.Font = new System.Drawing.Font("Consolas", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblStatus.Location = new System.Drawing.Point(816, 8);
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(73, 13);
            this.lblStatus.TabIndex = 9;
            this.lblStatus.Text = "Status: N/A";
            // 
            // txtOutput
            // 
            this.txtOutput.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtOutput.Font = new System.Drawing.Font("Consolas", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtOutput.Location = new System.Drawing.Point(8, 8);
            this.txtOutput.Multiline = true;
            this.txtOutput.Name = "txtOutput";
            this.txtOutput.ReadOnly = true;
            this.txtOutput.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtOutput.Size = new System.Drawing.Size(800, 496);
            this.txtOutput.TabIndex = 8;
            // 
            // mnuFile
            // 
            this.mnuFile.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.mnuFile.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mniOpenROM,
            this.toolStripSeparator2,
            this.mniOpenASM,
            this.mniSaveASM,
            this.toolStripSeparator3,
            this.mniClose});
            this.mnuFile.Image = ((System.Drawing.Image)(resources.GetObject("mnuFile.Image")));
            this.mnuFile.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.mnuFile.Name = "mnuFile";
            this.mnuFile.Size = new System.Drawing.Size(38, 22);
            this.mnuFile.Text = "File";
            // 
            // mniOpenROM
            // 
            this.mniOpenROM.Name = "mniOpenROM";
            this.mniOpenROM.Size = new System.Drawing.Size(133, 22);
            this.mniOpenROM.Text = "Open ROM";
            this.mniOpenROM.Click += new System.EventHandler(this.mniOpenROM_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(130, 6);
            // 
            // mniOpenASM
            // 
            this.mniOpenASM.Name = "mniOpenASM";
            this.mniOpenASM.Size = new System.Drawing.Size(133, 22);
            this.mniOpenASM.Text = "Open ASM";
            this.mniOpenASM.Click += new System.EventHandler(this.MniOpenASM_Click);
            // 
            // mniSaveASM
            // 
            this.mniSaveASM.Name = "mniSaveASM";
            this.mniSaveASM.Size = new System.Drawing.Size(133, 22);
            this.mniSaveASM.Text = "Save ASM";
            this.mniSaveASM.Click += new System.EventHandler(this.MniSaveASM_Click);
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            this.toolStripSeparator3.Size = new System.Drawing.Size(130, 6);
            // 
            // mniClose
            // 
            this.mniClose.Name = "mniClose";
            this.mniClose.Size = new System.Drawing.Size(133, 22);
            this.mniClose.Text = "Close";
            this.mniClose.Click += new System.EventHandler(this.MniClose_Click);
            // 
            // btnAssemble
            // 
            this.btnAssemble.Image = global::NovaBoySharp.Properties.Resources.diamonds_3;
            this.btnAssemble.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnAssemble.Name = "btnAssemble";
            this.btnAssemble.Size = new System.Drawing.Size(78, 22);
            this.btnAssemble.Text = "Assemble";
            this.btnAssemble.Click += new System.EventHandler(this.BtnAssemble_Click);
            // 
            // btnReset
            // 
            this.btnReset.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.btnReset.Enabled = false;
            this.btnReset.Image = global::NovaBoySharp.Properties.Resources.media_controls_dark_first;
            this.btnReset.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnReset.Name = "btnReset";
            this.btnReset.Size = new System.Drawing.Size(55, 22);
            this.btnReset.Text = "Reset";
            this.btnReset.Click += new System.EventHandler(this.BtnReset_Click);
            // 
            // btnStop
            // 
            this.btnStop.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.btnStop.Enabled = false;
            this.btnStop.Image = global::NovaBoySharp.Properties.Resources.media_controls_dark_stop;
            this.btnStop.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnStop.Name = "btnStop";
            this.btnStop.Size = new System.Drawing.Size(51, 22);
            this.btnStop.Text = "Stop";
            this.btnStop.Click += new System.EventHandler(this.BtnStop_Click);
            // 
            // btnStep
            // 
            this.btnStep.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.btnStep.Enabled = false;
            this.btnStep.Image = global::NovaBoySharp.Properties.Resources.media_controls_dark_play;
            this.btnStep.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnStep.Name = "btnStep";
            this.btnStep.Size = new System.Drawing.Size(50, 22);
            this.btnStep.Text = "Step";
            this.btnStep.Click += new System.EventHandler(this.BtnStep_Click);
            // 
            // btnRun
            // 
            this.btnRun.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.btnRun.Enabled = false;
            this.btnRun.Image = global::NovaBoySharp.Properties.Resources.media_controls_dark_forward;
            this.btnRun.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnRun.Name = "btnRun";
            this.btnRun.Size = new System.Drawing.Size(48, 22);
            this.btnRun.Text = "Run";
            this.btnRun.Click += new System.EventHandler(this.BtnRun_Click);
            // 
            // btnTurbo
            // 
            this.btnTurbo.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.btnTurbo.Enabled = false;
            this.btnTurbo.Image = global::NovaBoySharp.Properties.Resources.media_controls_dark_last;
            this.btnTurbo.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnTurbo.Name = "btnTurbo";
            this.btnTurbo.Size = new System.Drawing.Size(58, 22);
            this.btnTurbo.Text = "Turbo";
            this.btnTurbo.Click += new System.EventHandler(this.BtnTurbo_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1043, 573);
            this.Controls.Add(this.tabControls);
            this.Controls.Add(this.toolStrip1);
            this.Name = "Form1";
            this.Text = "CPU Test";
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.tabControls.ResumeLayout(false);
            this.pageDevelop.ResumeLayout(false);
            this.pageDevelop.PerformLayout();
            this.pageDebug.ResumeLayout(false);
            this.pageDebug.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripButton btnRun;
        private System.Windows.Forms.ToolStripButton btnReset;
        private System.Windows.Forms.ToolStripButton btnStep;
        private System.Windows.Forms.ToolStripButton btnAssemble;
        private System.Windows.Forms.TabControl tabControls;
        private System.Windows.Forms.TabPage pageDebug;
        private System.Windows.Forms.TabPage pageDevelop;
        private System.Windows.Forms.Label lblStatus;
        private System.Windows.Forms.TextBox txtOutput;
        private ScintillaNET.Scintilla txtCode;
        private System.Windows.Forms.ToolStripDropDownButton mnuFile;
        private System.Windows.Forms.ToolStripMenuItem mniOpenROM;
        private System.Windows.Forms.ToolStripMenuItem mniSaveASM;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem mniClose;
        private System.Windows.Forms.TextBox txtAssemblerOutput;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripMenuItem mniOpenASM;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
        private System.Windows.Forms.ToolStripButton btnStop;
        private System.Windows.Forms.ToolStripButton btnTurbo;
    }
}

