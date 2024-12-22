
namespace Raytracer.Windows
{
    partial class FormApp
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            pboxRender = new System.Windows.Forms.PictureBox();
            label1 = new System.Windows.Forms.Label();
            NumericTraceDepth = new System.Windows.Forms.NumericUpDown();
            CheckBoxDisableReflections = new System.Windows.Forms.CheckBox();
            tabControl1 = new System.Windows.Forms.TabControl();
            tabPage1 = new System.Windows.Forms.TabPage();
            CheckBoxDisableSpeculation = new System.Windows.Forms.CheckBox();
            CheckBoxDisableDiffuse = new System.Windows.Forms.CheckBox();
            tabPage2 = new System.Windows.Forms.TabPage();
            CheckBoxMoveAutomatically = new System.Windows.Forms.CheckBox();
            ButtonCameraBackward = new System.Windows.Forms.Button();
            NumericCameraZ = new System.Windows.Forms.NumericUpDown();
            NumericCameraY = new System.Windows.Forms.NumericUpDown();
            NumericCameraX = new System.Windows.Forms.NumericUpDown();
            ButtonCameraForward = new System.Windows.Forms.Button();
            ButtonCameraRight = new System.Windows.Forms.Button();
            ButtonCameraLeft = new System.Windows.Forms.Button();
            label4 = new System.Windows.Forms.Label();
            label3 = new System.Windows.Forms.Label();
            label2 = new System.Windows.Forms.Label();
            TextboxElapsed = new System.Windows.Forms.TextBox();
            label5 = new System.Windows.Forms.Label();
            label6 = new System.Windows.Forms.Label();
            tabControl2 = new System.Windows.Forms.TabControl();
            tabPage3 = new System.Windows.Forms.TabPage();
            tabPage4 = new System.Windows.Forms.TabPage();
            TextBoxScene = new System.Windows.Forms.TextBox();
            ButtonLoadScene = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)pboxRender).BeginInit();
            ((System.ComponentModel.ISupportInitialize)NumericTraceDepth).BeginInit();
            tabControl1.SuspendLayout();
            tabPage1.SuspendLayout();
            tabPage2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)NumericCameraZ).BeginInit();
            ((System.ComponentModel.ISupportInitialize)NumericCameraY).BeginInit();
            ((System.ComponentModel.ISupportInitialize)NumericCameraX).BeginInit();
            tabControl2.SuspendLayout();
            tabPage3.SuspendLayout();
            tabPage4.SuspendLayout();
            SuspendLayout();
            // 
            // pboxRender
            // 
            pboxRender.BackColor = System.Drawing.Color.Black;
            pboxRender.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            pboxRender.Location = new System.Drawing.Point(4, 4);
            pboxRender.Margin = new System.Windows.Forms.Padding(1);
            pboxRender.Name = "pboxRender";
            pboxRender.Size = new System.Drawing.Size(500, 500);
            pboxRender.TabIndex = 0;
            pboxRender.TabStop = false;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new System.Drawing.Point(180, 56);
            label1.Name = "label1";
            label1.Size = new System.Drawing.Size(116, 20);
            label1.TabIndex = 3;
            label1.Text = "Recursion depth";
            // 
            // NumericTraceDepth
            // 
            NumericTraceDepth.Location = new System.Drawing.Point(24, 54);
            NumericTraceDepth.Maximum = new decimal(new int[] { 10, 0, 0, 0 });
            NumericTraceDepth.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
            NumericTraceDepth.Name = "NumericTraceDepth";
            NumericTraceDepth.Size = new System.Drawing.Size(150, 27);
            NumericTraceDepth.TabIndex = 4;
            NumericTraceDepth.Value = new decimal(new int[] { 3, 0, 0, 0 });
            NumericTraceDepth.ValueChanged += NumericTraceDepth_ValueChanged;
            // 
            // CheckBoxDisableReflections
            // 
            CheckBoxDisableReflections.AutoSize = true;
            CheckBoxDisableReflections.Location = new System.Drawing.Point(24, 24);
            CheckBoxDisableReflections.Name = "CheckBoxDisableReflections";
            CheckBoxDisableReflections.Size = new System.Drawing.Size(158, 24);
            CheckBoxDisableReflections.TabIndex = 3;
            CheckBoxDisableReflections.Text = "Disable Reflections";
            CheckBoxDisableReflections.UseVisualStyleBackColor = true;
            CheckBoxDisableReflections.CheckedChanged += CheckBoxDisableReflections_CheckedChanged;
            // 
            // tabControl1
            // 
            tabControl1.Controls.Add(tabPage1);
            tabControl1.Controls.Add(tabPage2);
            tabControl1.Location = new System.Drawing.Point(528, 10);
            tabControl1.Name = "tabControl1";
            tabControl1.SelectedIndex = 0;
            tabControl1.Size = new System.Drawing.Size(390, 536);
            tabControl1.TabIndex = 3;
            // 
            // tabPage1
            // 
            tabPage1.Controls.Add(CheckBoxDisableSpeculation);
            tabPage1.Controls.Add(CheckBoxDisableDiffuse);
            tabPage1.Controls.Add(label1);
            tabPage1.Controls.Add(CheckBoxDisableReflections);
            tabPage1.Controls.Add(NumericTraceDepth);
            tabPage1.Location = new System.Drawing.Point(4, 29);
            tabPage1.Name = "tabPage1";
            tabPage1.Padding = new System.Windows.Forms.Padding(3);
            tabPage1.Size = new System.Drawing.Size(382, 503);
            tabPage1.TabIndex = 0;
            tabPage1.Text = "Options";
            tabPage1.UseVisualStyleBackColor = true;
            // 
            // CheckBoxDisableSpeculation
            // 
            CheckBoxDisableSpeculation.AutoSize = true;
            CheckBoxDisableSpeculation.Location = new System.Drawing.Point(24, 127);
            CheckBoxDisableSpeculation.Name = "CheckBoxDisableSpeculation";
            CheckBoxDisableSpeculation.Size = new System.Drawing.Size(163, 24);
            CheckBoxDisableSpeculation.TabIndex = 8;
            CheckBoxDisableSpeculation.Text = "Disable Speculation";
            CheckBoxDisableSpeculation.UseVisualStyleBackColor = true;
            CheckBoxDisableSpeculation.CheckedChanged += CheckBoxDisableSpeculation_CheckedChanged;
            // 
            // CheckBoxDisableDiffuse
            // 
            CheckBoxDisableDiffuse.AutoSize = true;
            CheckBoxDisableDiffuse.Location = new System.Drawing.Point(24, 97);
            CheckBoxDisableDiffuse.Name = "CheckBoxDisableDiffuse";
            CheckBoxDisableDiffuse.Size = new System.Drawing.Size(132, 24);
            CheckBoxDisableDiffuse.TabIndex = 7;
            CheckBoxDisableDiffuse.Text = "Disable Diffuse";
            CheckBoxDisableDiffuse.UseVisualStyleBackColor = true;
            CheckBoxDisableDiffuse.CheckedChanged += CheckBoxDisableDiffuse_CheckedChanged;
            // 
            // tabPage2
            // 
            tabPage2.Controls.Add(CheckBoxMoveAutomatically);
            tabPage2.Controls.Add(ButtonCameraBackward);
            tabPage2.Controls.Add(NumericCameraZ);
            tabPage2.Controls.Add(NumericCameraY);
            tabPage2.Controls.Add(NumericCameraX);
            tabPage2.Controls.Add(ButtonCameraForward);
            tabPage2.Controls.Add(ButtonCameraRight);
            tabPage2.Controls.Add(ButtonCameraLeft);
            tabPage2.Controls.Add(label4);
            tabPage2.Controls.Add(label3);
            tabPage2.Controls.Add(label2);
            tabPage2.Location = new System.Drawing.Point(4, 29);
            tabPage2.Name = "tabPage2";
            tabPage2.Padding = new System.Windows.Forms.Padding(3);
            tabPage2.Size = new System.Drawing.Size(382, 503);
            tabPage2.TabIndex = 1;
            tabPage2.Text = "Camera";
            tabPage2.UseVisualStyleBackColor = true;
            // 
            // CheckBoxMoveAutomatically
            // 
            CheckBoxMoveAutomatically.AutoSize = true;
            CheckBoxMoveAutomatically.Location = new System.Drawing.Point(24, 169);
            CheckBoxMoveAutomatically.Name = "CheckBoxMoveAutomatically";
            CheckBoxMoveAutomatically.Size = new System.Drawing.Size(164, 24);
            CheckBoxMoveAutomatically.TabIndex = 10;
            CheckBoxMoveAutomatically.Text = "Move Automatically";
            CheckBoxMoveAutomatically.UseVisualStyleBackColor = true;
            CheckBoxMoveAutomatically.CheckedChanged += CheckBoxMoveAutomatically_CheckedChanged;
            // 
            // ButtonCameraBackward
            // 
            ButtonCameraBackward.Location = new System.Drawing.Point(151, 117);
            ButtonCameraBackward.Name = "ButtonCameraBackward";
            ButtonCameraBackward.Size = new System.Drawing.Size(62, 29);
            ButtonCameraBackward.TabIndex = 9;
            ButtonCameraBackward.Text = "⬇️";
            ButtonCameraBackward.UseVisualStyleBackColor = true;
            ButtonCameraBackward.Click += ButtonCameraBackward_Click;
            // 
            // NumericCameraZ
            // 
            NumericCameraZ.DecimalPlaces = 1;
            NumericCameraZ.Increment = new decimal(new int[] { 25, 0, 0, 131072 });
            NumericCameraZ.Location = new System.Drawing.Point(262, 22);
            NumericCameraZ.Maximum = new decimal(new int[] { 10, 0, 0, 0 });
            NumericCameraZ.Minimum = new decimal(new int[] { 10, 0, 0, int.MinValue });
            NumericCameraZ.Name = "NumericCameraZ";
            NumericCameraZ.Size = new System.Drawing.Size(74, 27);
            NumericCameraZ.TabIndex = 8;
            NumericCameraZ.ValueChanged += NumericCameraZ_ValueChanged;
            // 
            // NumericCameraY
            // 
            NumericCameraY.DecimalPlaces = 1;
            NumericCameraY.Increment = new decimal(new int[] { 25, 0, 0, 131072 });
            NumericCameraY.Location = new System.Drawing.Point(151, 22);
            NumericCameraY.Maximum = new decimal(new int[] { 10, 0, 0, 0 });
            NumericCameraY.Minimum = new decimal(new int[] { 10, 0, 0, int.MinValue });
            NumericCameraY.Name = "NumericCameraY";
            NumericCameraY.Size = new System.Drawing.Size(74, 27);
            NumericCameraY.TabIndex = 7;
            NumericCameraY.ValueChanged += NumericCameraY_ValueChanged;
            // 
            // NumericCameraX
            // 
            NumericCameraX.DecimalPlaces = 1;
            NumericCameraX.Increment = new decimal(new int[] { 25, 0, 0, 131072 });
            NumericCameraX.Location = new System.Drawing.Point(48, 22);
            NumericCameraX.Maximum = new decimal(new int[] { 10, 0, 0, 0 });
            NumericCameraX.Minimum = new decimal(new int[] { 10, 0, 0, int.MinValue });
            NumericCameraX.Name = "NumericCameraX";
            NumericCameraX.Size = new System.Drawing.Size(74, 27);
            NumericCameraX.TabIndex = 6;
            NumericCameraX.ValueChanged += NumericCameraX_ValueChanged;
            // 
            // ButtonCameraForward
            // 
            ButtonCameraForward.Location = new System.Drawing.Point(151, 82);
            ButtonCameraForward.Name = "ButtonCameraForward";
            ButtonCameraForward.Size = new System.Drawing.Size(62, 29);
            ButtonCameraForward.TabIndex = 5;
            ButtonCameraForward.Text = "⬆️";
            ButtonCameraForward.UseVisualStyleBackColor = true;
            ButtonCameraForward.Click += ButtonCameraForward_Click;
            // 
            // ButtonCameraRight
            // 
            ButtonCameraRight.Location = new System.Drawing.Point(220, 117);
            ButtonCameraRight.Name = "ButtonCameraRight";
            ButtonCameraRight.Size = new System.Drawing.Size(62, 29);
            ButtonCameraRight.TabIndex = 5;
            ButtonCameraRight.Text = ">";
            ButtonCameraRight.UseVisualStyleBackColor = true;
            ButtonCameraRight.Click += ButtonCameraRight_Click;
            // 
            // ButtonCameraLeft
            // 
            ButtonCameraLeft.Location = new System.Drawing.Point(84, 117);
            ButtonCameraLeft.Name = "ButtonCameraLeft";
            ButtonCameraLeft.Size = new System.Drawing.Size(62, 29);
            ButtonCameraLeft.TabIndex = 4;
            ButtonCameraLeft.Text = "<";
            ButtonCameraLeft.UseVisualStyleBackColor = true;
            ButtonCameraLeft.Click += ButtonCameraLeft_Click;
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Location = new System.Drawing.Point(238, 24);
            label4.Name = "label4";
            label4.Size = new System.Drawing.Size(18, 20);
            label4.TabIndex = 5;
            label4.Text = "Z";
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new System.Drawing.Point(128, 24);
            label3.Name = "label3";
            label3.Size = new System.Drawing.Size(17, 20);
            label3.TabIndex = 3;
            label3.Text = "Y";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new System.Drawing.Point(24, 24);
            label2.Name = "label2";
            label2.Size = new System.Drawing.Size(18, 20);
            label2.TabIndex = 1;
            label2.Text = "X";
            // 
            // TextboxElapsed
            // 
            TextboxElapsed.BackColor = System.Drawing.Color.White;
            TextboxElapsed.Location = new System.Drawing.Point(360, 552);
            TextboxElapsed.Name = "TextboxElapsed";
            TextboxElapsed.ReadOnly = true;
            TextboxElapsed.Size = new System.Drawing.Size(126, 27);
            TextboxElapsed.TabIndex = 4;
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Location = new System.Drawing.Point(261, 555);
            label5.Name = "label5";
            label5.Size = new System.Drawing.Size(93, 20);
            label5.TabIndex = 5;
            label5.Text = "Render Time";
            // 
            // label6
            // 
            label6.AutoSize = true;
            label6.Location = new System.Drawing.Point(492, 553);
            label6.Name = "label6";
            label6.Size = new System.Drawing.Size(28, 20);
            label6.TabIndex = 6;
            label6.Text = "ms";
            // 
            // tabControl2
            // 
            tabControl2.Controls.Add(tabPage3);
            tabControl2.Controls.Add(tabPage4);
            tabControl2.Location = new System.Drawing.Point(12, 10);
            tabControl2.Name = "tabControl2";
            tabControl2.SelectedIndex = 0;
            tabControl2.Size = new System.Drawing.Size(514, 540);
            tabControl2.TabIndex = 7;
            // 
            // tabPage3
            // 
            tabPage3.Controls.Add(pboxRender);
            tabPage3.Location = new System.Drawing.Point(4, 29);
            tabPage3.Name = "tabPage3";
            tabPage3.Padding = new System.Windows.Forms.Padding(3);
            tabPage3.Size = new System.Drawing.Size(506, 507);
            tabPage3.TabIndex = 0;
            tabPage3.Text = "Render";
            tabPage3.UseVisualStyleBackColor = true;
            // 
            // tabPage4
            // 
            tabPage4.Controls.Add(TextBoxScene);
            tabPage4.Location = new System.Drawing.Point(4, 29);
            tabPage4.Name = "tabPage4";
            tabPage4.Padding = new System.Windows.Forms.Padding(3);
            tabPage4.Size = new System.Drawing.Size(506, 507);
            tabPage4.TabIndex = 1;
            tabPage4.Text = "Scene";
            tabPage4.UseVisualStyleBackColor = true;
            // 
            // TextBoxScene
            // 
            TextBoxScene.Font = new System.Drawing.Font("Consolas", 9F);
            TextBoxScene.Location = new System.Drawing.Point(6, 6);
            TextBoxScene.Multiline = true;
            TextBoxScene.Name = "TextBoxScene";
            TextBoxScene.Size = new System.Drawing.Size(494, 495);
            TextBoxScene.TabIndex = 0;
            // 
            // ButtonLoadScene
            // 
            ButtonLoadScene.Location = new System.Drawing.Point(16, 552);
            ButtonLoadScene.Name = "ButtonLoadScene";
            ButtonLoadScene.Size = new System.Drawing.Size(94, 29);
            ButtonLoadScene.TabIndex = 8;
            ButtonLoadScene.Text = "Load Scene";
            ButtonLoadScene.UseVisualStyleBackColor = true;
            // 
            // FormApp
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(8F, 20F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            ClientSize = new System.Drawing.Size(929, 591);
            Controls.Add(ButtonLoadScene);
            Controls.Add(tabControl2);
            Controls.Add(label6);
            Controls.Add(label5);
            Controls.Add(TextboxElapsed);
            Controls.Add(tabControl1);
            FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            Margin = new System.Windows.Forms.Padding(1);
            MaximizeBox = false;
            Name = "FormApp";
            Text = "Ray Tracer";
            ((System.ComponentModel.ISupportInitialize)pboxRender).EndInit();
            ((System.ComponentModel.ISupportInitialize)NumericTraceDepth).EndInit();
            tabControl1.ResumeLayout(false);
            tabPage1.ResumeLayout(false);
            tabPage1.PerformLayout();
            tabPage2.ResumeLayout(false);
            tabPage2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)NumericCameraZ).EndInit();
            ((System.ComponentModel.ISupportInitialize)NumericCameraY).EndInit();
            ((System.ComponentModel.ISupportInitialize)NumericCameraX).EndInit();
            tabControl2.ResumeLayout(false);
            tabPage3.ResumeLayout(false);
            tabPage4.ResumeLayout(false);
            tabPage4.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private System.Windows.Forms.PictureBox pboxRender;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.NumericUpDown NumericTraceDepth;
        private System.Windows.Forms.CheckBox CheckBoxDisableReflections;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button ButtonCameraLeft;
        private System.Windows.Forms.Button ButtonCameraRight;
        private System.Windows.Forms.Button ButtonCameraForward;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox TextboxElapsed;
        private System.Windows.Forms.NumericUpDown NumericCameraZ;
        private System.Windows.Forms.NumericUpDown NumericCameraY;
        private System.Windows.Forms.NumericUpDown NumericCameraX;
        private System.Windows.Forms.Button ButtonCameraBackward;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.CheckBox CheckBoxDisableSpeculation;
        private System.Windows.Forms.CheckBox CheckBoxDisableDiffuse;
        private System.Windows.Forms.CheckBox CheckBoxMoveAutomatically;
        private System.Windows.Forms.TabControl tabControl2;
        private System.Windows.Forms.TabPage tabPage3;
        private System.Windows.Forms.TabPage tabPage4;
        private System.Windows.Forms.TextBox TextBoxScene;
        private System.Windows.Forms.Button ButtonLoadScene;
    }
}

