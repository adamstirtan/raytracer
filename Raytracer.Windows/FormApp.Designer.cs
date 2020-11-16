
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
            this.pboxRender = new System.Windows.Forms.PictureBox();
            this.buttonRender = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.pboxRender)).BeginInit();
            this.SuspendLayout();
            // 
            // pboxRender
            // 
            this.pboxRender.BackColor = System.Drawing.Color.Black;
            this.pboxRender.Location = new System.Drawing.Point(0, 0);
            this.pboxRender.Margin = new System.Windows.Forms.Padding(1, 1, 1, 1);
            this.pboxRender.Name = "pboxRender";
            this.pboxRender.Size = new System.Drawing.Size(800, 600);
            this.pboxRender.TabIndex = 0;
            this.pboxRender.TabStop = false;
            // 
            // buttonRender
            // 
            this.buttonRender.Location = new System.Drawing.Point(10, 612);
            this.buttonRender.Margin = new System.Windows.Forms.Padding(1, 1, 1, 1);
            this.buttonRender.Name = "buttonRender";
            this.buttonRender.Size = new System.Drawing.Size(780, 43);
            this.buttonRender.TabIndex = 1;
            this.buttonRender.Text = "Render";
            this.buttonRender.UseVisualStyleBackColor = true;
            this.buttonRender.Click += new System.EventHandler(this.buttonRender_Click);
            // 
            // FormApp
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 664);
            this.Controls.Add(this.buttonRender);
            this.Controls.Add(this.pboxRender);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Margin = new System.Windows.Forms.Padding(1, 1, 1, 1);
            this.MaximizeBox = false;
            this.Name = "FormApp";
            this.Text = "Ray Tracer";
            ((System.ComponentModel.ISupportInitialize)(this.pboxRender)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.PictureBox pboxRender;
        private System.Windows.Forms.Button buttonRender;
    }
}

