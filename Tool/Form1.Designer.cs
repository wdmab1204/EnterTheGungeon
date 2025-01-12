namespace ByteConverter
{
    partial class Form1
    {
        /// <summary>
        /// 필수 디자이너 변수입니다.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 사용 중인 모든 리소스를 정리합니다.
        /// </summary>
        /// <param name="disposing">관리되는 리소스를 삭제해야 하면 true이고, 그렇지 않으면 false입니다.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form 디자이너에서 생성한 코드

        /// <summary>
        /// 디자이너 지원에 필요한 메서드입니다. 
        /// 이 메서드의 내용을 코드 편집기로 수정하지 마세요.
        /// </summary>
        private void InitializeComponent()
        {
            this.generateBinButton = new System.Windows.Forms.Button();
            this.filePathText = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.button1 = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // generateBinButton
            // 
            this.generateBinButton.Location = new System.Drawing.Point(112, 329);
            this.generateBinButton.Name = "generateBinButton";
            this.generateBinButton.Size = new System.Drawing.Size(75, 23);
            this.generateBinButton.TabIndex = 0;
            this.generateBinButton.Text = "Click";
            this.generateBinButton.UseVisualStyleBackColor = true;
            this.generateBinButton.Click += new System.EventHandler(this.OnClickSerializeButton);
            // 
            // filePathText
            // 
            this.filePathText.Location = new System.Drawing.Point(75, 80);
            this.filePathText.Name = "filePathText";
            this.filePathText.Size = new System.Drawing.Size(200, 21);
            this.filePathText.TabIndex = 1;
            this.filePathText.TextChanged += new System.EventHandler(this.textBox1_TextChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 83);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(57, 12);
            this.label1.TabIndex = 2;
            this.label1.Text = "파일 경로";
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(12, 12);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(119, 23);
            this.button1.TabIndex = 3;
            this.button1.Text = "create test csv";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.OnClickCreateDummy);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(287, 373);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.filePathText);
            this.Controls.Add(this.generateBinButton);
            this.Name = "Form1";
            this.Text = "Form1";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button generateBinButton;
        private System.Windows.Forms.TextBox filePathText;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button button1;
    }
}

