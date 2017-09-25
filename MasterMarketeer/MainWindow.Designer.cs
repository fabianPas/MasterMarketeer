namespace MasterMarketeer
{
    partial class MainWindow
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
            this.collectData = new System.Windows.Forms.Button();
            this.selectedCommodityMarket = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.marketImage = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.marketImage)).BeginInit();
            this.SuspendLayout();
            // 
            // collectData
            // 
            this.collectData.Location = new System.Drawing.Point(12, 93);
            this.collectData.Name = "collectData";
            this.collectData.Size = new System.Drawing.Size(260, 33);
            this.collectData.TabIndex = 0;
            this.collectData.Text = "Collect";
            this.collectData.UseVisualStyleBackColor = true;
            // 
            // selectedCommodityMarket
            // 
            this.selectedCommodityMarket.FormattingEnabled = true;
            this.selectedCommodityMarket.Items.AddRange(new object[] {
            "Port Venture",
            "Lionhaven",
            "Night Harbour",
            "Green Oasis",
            "Kreis Island",
            "Pele Island",
            "Traveler\'s Rest",
            "Anole Garden",
            "Cavum Ridge",
            "Kingfisher Island",
            "Pride Island",
            "Ridley\'s Valor",
            "Triumph Island",
            "Devil\'s Heart",
            "Flatback Rift",
            "Moonlight Cove",
            "Nevermore Island",
            "Point Petrify",
            "Whisper Island",
            "Bogong Cove",
            "Ember Eye",
            "Hawksbill Island",
            "Wind Combs Island",
            "Holloway Isle",
            "Huracan Island",
            "Krakentoa Island",
            "Leatherback Island",
            "Dingwall Island",
            "Loggerhead Island",
            "Magpie Island",
            "Melanaster Island",
            "Mitjana Island",
            "Picklepine Ridge",
            "Triplet\'s Treasure",
            "Woodtick Island"});
            this.selectedCommodityMarket.Location = new System.Drawing.Point(12, 31);
            this.selectedCommodityMarket.Name = "selectedCommodityMarket";
            this.selectedCommodityMarket.Size = new System.Drawing.Size(260, 21);
            this.selectedCommodityMarket.TabIndex = 1;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(13, 13);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(94, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "Commodity Market";
            // 
            // marketImage
            // 
            this.marketImage.Image = global::MasterMarketeer.Properties.Resources.Market;
            this.marketImage.InitialImage = null;
            this.marketImage.Location = new System.Drawing.Point(12, 132);
            this.marketImage.Name = "marketImage";
            this.marketImage.Size = new System.Drawing.Size(259, 118);
            this.marketImage.TabIndex = 3;
            this.marketImage.TabStop = false;
            // 
            // MainWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(284, 262);
            this.Controls.Add(this.marketImage);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.selectedCommodityMarket);
            this.Controls.Add(this.collectData);
            this.Name = "MainWindow";
            this.Text = "MasterMarketeer";
            this.Load += new System.EventHandler(this.MainWindow_Load);
            ((System.ComponentModel.ISupportInitialize)(this.marketImage)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button collectData;
        private System.Windows.Forms.ComboBox selectedCommodityMarket;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.PictureBox marketImage;
    }
}

