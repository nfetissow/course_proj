namespace PlanetGenerator
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
            this.pbScene = new System.Windows.Forms.PictureBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.tbUI = new System.Windows.Forms.TabControl();
            this.tbSettings = new System.Windows.Forms.TabPage();
            this.panelDetalisation = new System.Windows.Forms.Panel();
            this.label7 = new System.Windows.Forms.Label();
            this.rbHigh = new System.Windows.Forms.RadioButton();
            this.rbMedium = new System.Windows.Forms.RadioButton();
            this.rbLow = new System.Windows.Forms.RadioButton();
            this.panelSurface = new System.Windows.Forms.Panel();
            this.rbBiomes = new System.Windows.Forms.RadioButton();
            this.rbHeightMap = new System.Windows.Forms.RadioButton();
            this.rbPlates = new System.Windows.Forms.RadioButton();
            this.lbSurfaceChoice = new System.Windows.Forms.Label();
            this.btGenerate = new System.Windows.Forms.Button();
            this.pbLoading = new System.Windows.Forms.ProgressBar();
            this.tbAdvanced = new System.Windows.Forms.TabPage();
            this.lbHeatLevel = new System.Windows.Forms.Label();
            this.lbMoistureLevel = new System.Windows.Forms.Label();
            this.lbTectonicPlateCount = new System.Windows.Forms.Label();
            this.lbOceanicRate = new System.Windows.Forms.Label();
            this.lbDistortionLevel = new System.Windows.Forms.Label();
            this.lbDetailLevel = new System.Windows.Forms.Label();
            this.lbSeed = new System.Windows.Forms.Label();
            this.tbSeed = new System.Windows.Forms.TextBox();
            this.tbMoistureLevel = new System.Windows.Forms.TrackBar();
            this.tbOceanicRate = new System.Windows.Forms.TrackBar();
            this.tbHeatLevel = new System.Windows.Forms.TrackBar();
            this.tbTectonicPlateCount = new System.Windows.Forms.TrackBar();
            this.tbDistorionLevel = new System.Windows.Forms.TrackBar();
            this.tbDetailLevel = new System.Windows.Forms.TrackBar();
            this.bindingSource1 = new System.Windows.Forms.BindingSource(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.pbScene)).BeginInit();
            this.tbUI.SuspendLayout();
            this.tbSettings.SuspendLayout();
            this.panelDetalisation.SuspendLayout();
            this.panelSurface.SuspendLayout();
            this.tbAdvanced.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.tbMoistureLevel)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.tbOceanicRate)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.tbHeatLevel)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.tbTectonicPlateCount)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.tbDistorionLevel)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.tbDetailLevel)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.bindingSource1)).BeginInit();
            this.SuspendLayout();
            // 
            // pbScene
            // 
            this.pbScene.Location = new System.Drawing.Point(0, -2);
            this.pbScene.Name = "pbScene";
            this.pbScene.Size = new System.Drawing.Size(1217, 785);
            this.pbScene.TabIndex = 0;
            this.pbScene.TabStop = false;
            this.pbScene.MouseDown += new System.Windows.Forms.MouseEventHandler(this.pbScene_MouseDown);
            this.pbScene.MouseMove += new System.Windows.Forms.MouseEventHandler(this.pbScene_MouseMove);
            this.pbScene.MouseUp += new System.Windows.Forms.MouseEventHandler(this.pbScene_MouseUp);
            this.pbScene.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this.pbScene_PreviewKeyDown);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(164, 18);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(35, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "label1";
            this.label1.Visible = false;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(452, 12);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(35, 13);
            this.label2.TabIndex = 3;
            this.label2.Text = "label2";
            this.label2.Visible = false;
            // 
            // tbUI
            // 
            this.tbUI.Controls.Add(this.tbSettings);
            this.tbUI.Controls.Add(this.tbAdvanced);
            this.tbUI.Location = new System.Drawing.Point(849, 74);
            this.tbUI.Name = "tbUI";
            this.tbUI.SelectedIndex = 0;
            this.tbUI.Size = new System.Drawing.Size(424, 365);
            this.tbUI.TabIndex = 18;
            // 
            // tbSettings
            // 
            this.tbSettings.BackColor = System.Drawing.Color.Black;
            this.tbSettings.Controls.Add(this.panelDetalisation);
            this.tbSettings.Controls.Add(this.panelSurface);
            this.tbSettings.Controls.Add(this.btGenerate);
            this.tbSettings.Controls.Add(this.pbLoading);
            this.tbSettings.ForeColor = System.Drawing.SystemColors.ButtonFace;
            this.tbSettings.Location = new System.Drawing.Point(4, 22);
            this.tbSettings.Name = "tbSettings";
            this.tbSettings.Padding = new System.Windows.Forms.Padding(3);
            this.tbSettings.Size = new System.Drawing.Size(416, 339);
            this.tbSettings.TabIndex = 0;
            this.tbSettings.Text = "Настройки";
            // 
            // panelDetalisation
            // 
            this.panelDetalisation.Controls.Add(this.label7);
            this.panelDetalisation.Controls.Add(this.rbHigh);
            this.panelDetalisation.Controls.Add(this.rbMedium);
            this.panelDetalisation.Controls.Add(this.rbLow);
            this.panelDetalisation.Location = new System.Drawing.Point(30, 133);
            this.panelDetalisation.Name = "panelDetalisation";
            this.panelDetalisation.Size = new System.Drawing.Size(200, 100);
            this.panelDetalisation.TabIndex = 9;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(3, 0);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(78, 13);
            this.label7.TabIndex = 4;
            this.label7.Text = "Детализация:";
            // 
            // rbHigh
            // 
            this.rbHigh.AutoSize = true;
            this.rbHigh.Location = new System.Drawing.Point(6, 16);
            this.rbHigh.Name = "rbHigh";
            this.rbHigh.Size = new System.Drawing.Size(70, 17);
            this.rbHigh.TabIndex = 5;
            this.rbHigh.TabStop = true;
            this.rbHigh.Text = "Высокая";
            this.rbHigh.UseVisualStyleBackColor = true;
            this.rbHigh.CheckedChanged += new System.EventHandler(this.rbHigh_CheckedChanged);
            // 
            // rbMedium
            // 
            this.rbMedium.AutoSize = true;
            this.rbMedium.Location = new System.Drawing.Point(6, 39);
            this.rbMedium.Name = "rbMedium";
            this.rbMedium.Size = new System.Drawing.Size(68, 17);
            this.rbMedium.TabIndex = 5;
            this.rbMedium.TabStop = true;
            this.rbMedium.Text = "Средняя";
            this.rbMedium.UseVisualStyleBackColor = true;
            this.rbMedium.CheckedChanged += new System.EventHandler(this.rbMedium_CheckedChanged);
            // 
            // rbLow
            // 
            this.rbLow.AutoSize = true;
            this.rbLow.Location = new System.Drawing.Point(6, 62);
            this.rbLow.Name = "rbLow";
            this.rbLow.Size = new System.Drawing.Size(63, 17);
            this.rbLow.TabIndex = 5;
            this.rbLow.TabStop = true;
            this.rbLow.Text = "Низкая";
            this.rbLow.UseVisualStyleBackColor = true;
            this.rbLow.CheckedChanged += new System.EventHandler(this.rbLow_CheckedChanged);
            // 
            // panelSurface
            // 
            this.panelSurface.Controls.Add(this.rbBiomes);
            this.panelSurface.Controls.Add(this.rbHeightMap);
            this.panelSurface.Controls.Add(this.rbPlates);
            this.panelSurface.Controls.Add(this.lbSurfaceChoice);
            this.panelSurface.Location = new System.Drawing.Point(30, 6);
            this.panelSurface.Name = "panelSurface";
            this.panelSurface.Size = new System.Drawing.Size(357, 121);
            this.panelSurface.TabIndex = 8;
            // 
            // rbBiomes
            // 
            this.rbBiomes.AutoSize = true;
            this.rbBiomes.Checked = true;
            this.rbBiomes.ForeColor = System.Drawing.SystemColors.ButtonFace;
            this.rbBiomes.Location = new System.Drawing.Point(3, 42);
            this.rbBiomes.Name = "rbBiomes";
            this.rbBiomes.Size = new System.Drawing.Size(80, 17);
            this.rbBiomes.TabIndex = 0;
            this.rbBiomes.TabStop = true;
            this.rbBiomes.Text = "Местность";
            this.rbBiomes.UseVisualStyleBackColor = true;
            this.rbBiomes.CheckedChanged += new System.EventHandler(this.rbBiomes_CheckedChanged);
            // 
            // rbHeightMap
            // 
            this.rbHeightMap.AutoSize = true;
            this.rbHeightMap.ForeColor = System.Drawing.SystemColors.ButtonFace;
            this.rbHeightMap.Location = new System.Drawing.Point(3, 65);
            this.rbHeightMap.Name = "rbHeightMap";
            this.rbHeightMap.Size = new System.Drawing.Size(65, 17);
            this.rbHeightMap.TabIndex = 1;
            this.rbHeightMap.Text = "Высоты";
            this.rbHeightMap.UseVisualStyleBackColor = true;
            this.rbHeightMap.CheckedChanged += new System.EventHandler(this.rbHeightMap_CheckedChanged);
            // 
            // rbPlates
            // 
            this.rbPlates.AutoSize = true;
            this.rbPlates.ForeColor = System.Drawing.SystemColors.ButtonFace;
            this.rbPlates.Location = new System.Drawing.Point(3, 88);
            this.rbPlates.Name = "rbPlates";
            this.rbPlates.Size = new System.Drawing.Size(58, 17);
            this.rbPlates.TabIndex = 2;
            this.rbPlates.Text = "Плиты";
            this.rbPlates.UseVisualStyleBackColor = true;
            this.rbPlates.CheckedChanged += new System.EventHandler(this.rbPlates_CheckedChanged);
            // 
            // lbSurfaceChoice
            // 
            this.lbSurfaceChoice.AutoSize = true;
            this.lbSurfaceChoice.ForeColor = System.Drawing.SystemColors.ButtonHighlight;
            this.lbSurfaceChoice.Location = new System.Drawing.Point(3, 12);
            this.lbSurfaceChoice.Name = "lbSurfaceChoice";
            this.lbSurfaceChoice.Size = new System.Drawing.Size(206, 13);
            this.lbSurfaceChoice.TabIndex = 3;
            this.lbSurfaceChoice.Text = "Параметры отображения поверхности:";
            // 
            // btGenerate
            // 
            this.btGenerate.BackColor = System.Drawing.Color.Black;
            this.btGenerate.Location = new System.Drawing.Point(30, 239);
            this.btGenerate.Name = "btGenerate";
            this.btGenerate.Size = new System.Drawing.Size(99, 23);
            this.btGenerate.TabIndex = 7;
            this.btGenerate.Text = "Генерировать";
            this.btGenerate.UseVisualStyleBackColor = false;
            this.btGenerate.Click += new System.EventHandler(this.btGenerate_Click);
            // 
            // pbLoading
            // 
            this.pbLoading.Location = new System.Drawing.Point(30, 296);
            this.pbLoading.Name = "pbLoading";
            this.pbLoading.Size = new System.Drawing.Size(357, 23);
            this.pbLoading.Style = System.Windows.Forms.ProgressBarStyle.Continuous;
            this.pbLoading.TabIndex = 6;
            // 
            // tbAdvanced
            // 
            this.tbAdvanced.BackColor = System.Drawing.Color.MidnightBlue;
            this.tbAdvanced.Controls.Add(this.lbHeatLevel);
            this.tbAdvanced.Controls.Add(this.lbMoistureLevel);
            this.tbAdvanced.Controls.Add(this.lbTectonicPlateCount);
            this.tbAdvanced.Controls.Add(this.lbOceanicRate);
            this.tbAdvanced.Controls.Add(this.lbDistortionLevel);
            this.tbAdvanced.Controls.Add(this.lbDetailLevel);
            this.tbAdvanced.Controls.Add(this.lbSeed);
            this.tbAdvanced.Controls.Add(this.tbSeed);
            this.tbAdvanced.Controls.Add(this.tbMoistureLevel);
            this.tbAdvanced.Controls.Add(this.tbOceanicRate);
            this.tbAdvanced.Controls.Add(this.tbHeatLevel);
            this.tbAdvanced.Controls.Add(this.tbTectonicPlateCount);
            this.tbAdvanced.Controls.Add(this.tbDistorionLevel);
            this.tbAdvanced.Controls.Add(this.tbDetailLevel);
            this.tbAdvanced.ForeColor = System.Drawing.Color.White;
            this.tbAdvanced.Location = new System.Drawing.Point(4, 22);
            this.tbAdvanced.Name = "tbAdvanced";
            this.tbAdvanced.Padding = new System.Windows.Forms.Padding(3);
            this.tbAdvanced.Size = new System.Drawing.Size(416, 339);
            this.tbAdvanced.TabIndex = 1;
            this.tbAdvanced.Text = "Дополнительные параметры";
            // 
            // lbHeatLevel
            // 
            this.lbHeatLevel.AutoSize = true;
            this.lbHeatLevel.Location = new System.Drawing.Point(37, 191);
            this.lbHeatLevel.Name = "lbHeatLevel";
            this.lbHeatLevel.Size = new System.Drawing.Size(106, 13);
            this.lbHeatLevel.TabIndex = 7;
            this.lbHeatLevel.Text = "Уровень тепла (0%)";
            // 
            // lbMoistureLevel
            // 
            this.lbMoistureLevel.AutoSize = true;
            this.lbMoistureLevel.Location = new System.Drawing.Point(237, 191);
            this.lbMoistureLevel.Name = "lbMoistureLevel";
            this.lbMoistureLevel.Size = new System.Drawing.Size(132, 13);
            this.lbMoistureLevel.TabIndex = 7;
            this.lbMoistureLevel.Text = "Уровень влажности (0%)";
            // 
            // lbTectonicPlateCount
            // 
            this.lbTectonicPlateCount.AutoSize = true;
            this.lbTectonicPlateCount.Location = new System.Drawing.Point(37, 114);
            this.lbTectonicPlateCount.Name = "lbTectonicPlateCount";
            this.lbTectonicPlateCount.Size = new System.Drawing.Size(113, 13);
            this.lbTectonicPlateCount.TabIndex = 7;
            this.lbTectonicPlateCount.Text = "Количество плит (50)";
            // 
            // lbOceanicRate
            // 
            this.lbOceanicRate.AutoSize = true;
            this.lbOceanicRate.Location = new System.Drawing.Point(237, 115);
            this.lbOceanicRate.Name = "lbOceanicRate";
            this.lbOceanicRate.Size = new System.Drawing.Size(109, 13);
            this.lbOceanicRate.TabIndex = 7;
            this.lbOceanicRate.Text = "Уровень воды (70%)";
            // 
            // lbDistortionLevel
            // 
            this.lbDistortionLevel.AutoSize = true;
            this.lbDistortionLevel.Location = new System.Drawing.Point(237, 37);
            this.lbDistortionLevel.Name = "lbDistortionLevel";
            this.lbDistortionLevel.Size = new System.Drawing.Size(139, 13);
            this.lbDistortionLevel.TabIndex = 7;
            this.lbDistortionLevel.Text = "Уровень искажения (50%)";
            // 
            // lbDetailLevel
            // 
            this.lbDetailLevel.AutoSize = true;
            this.lbDetailLevel.Location = new System.Drawing.Point(37, 37);
            this.lbDetailLevel.Name = "lbDetailLevel";
            this.lbDetailLevel.Size = new System.Drawing.Size(96, 13);
            this.lbDetailLevel.TabIndex = 7;
            this.lbDetailLevel.Text = "Детализация (50)";
            // 
            // lbSeed
            // 
            this.lbSeed.AutoSize = true;
            this.lbSeed.Location = new System.Drawing.Point(61, 255);
            this.lbSeed.Name = "lbSeed";
            this.lbSeed.Size = new System.Drawing.Size(105, 13);
            this.lbSeed.TabIndex = 7;
            this.lbSeed.Text = "Источник энтропии";
            // 
            // tbSeed
            // 
            this.tbSeed.Location = new System.Drawing.Point(65, 284);
            this.tbSeed.Name = "tbSeed";
            this.tbSeed.Size = new System.Drawing.Size(233, 20);
            this.tbSeed.TabIndex = 6;
            // 
            // tbMoistureLevel
            // 
            this.tbMoistureLevel.Location = new System.Drawing.Point(226, 207);
            this.tbMoistureLevel.Maximum = 100;
            this.tbMoistureLevel.Minimum = -100;
            this.tbMoistureLevel.Name = "tbMoistureLevel";
            this.tbMoistureLevel.Size = new System.Drawing.Size(142, 45);
            this.tbMoistureLevel.TabIndex = 5;
            this.tbMoistureLevel.ValueChanged += new System.EventHandler(this.tbMoistureLevel_ValueChanged);
            // 
            // tbOceanicRate
            // 
            this.tbOceanicRate.Location = new System.Drawing.Point(226, 131);
            this.tbOceanicRate.Maximum = 100;
            this.tbOceanicRate.Name = "tbOceanicRate";
            this.tbOceanicRate.Size = new System.Drawing.Size(142, 45);
            this.tbOceanicRate.TabIndex = 4;
            this.tbOceanicRate.Value = 70;
            this.tbOceanicRate.ValueChanged += new System.EventHandler(this.tbOceanicRate_ValueChanged);
            // 
            // tbHeatLevel
            // 
            this.tbHeatLevel.Location = new System.Drawing.Point(30, 207);
            this.tbHeatLevel.Maximum = 100;
            this.tbHeatLevel.Minimum = -100;
            this.tbHeatLevel.Name = "tbHeatLevel";
            this.tbHeatLevel.Size = new System.Drawing.Size(142, 45);
            this.tbHeatLevel.TabIndex = 3;
            this.tbHeatLevel.ValueChanged += new System.EventHandler(this.tbHeatLevel_ValueChanged);
            // 
            // tbTectonicPlateCount
            // 
            this.tbTectonicPlateCount.Location = new System.Drawing.Point(30, 131);
            this.tbTectonicPlateCount.Maximum = 100;
            this.tbTectonicPlateCount.Minimum = 1;
            this.tbTectonicPlateCount.Name = "tbTectonicPlateCount";
            this.tbTectonicPlateCount.Size = new System.Drawing.Size(142, 45);
            this.tbTectonicPlateCount.TabIndex = 2;
            this.tbTectonicPlateCount.Value = 50;
            this.tbTectonicPlateCount.ValueChanged += new System.EventHandler(this.tbTectonicPlateCount_ValueChanged);
            // 
            // tbDistorionLevel
            // 
            this.tbDistorionLevel.Location = new System.Drawing.Point(226, 53);
            this.tbDistorionLevel.Maximum = 100;
            this.tbDistorionLevel.Name = "tbDistorionLevel";
            this.tbDistorionLevel.Size = new System.Drawing.Size(142, 45);
            this.tbDistorionLevel.TabIndex = 1;
            this.tbDistorionLevel.Value = 50;
            this.tbDistorionLevel.ValueChanged += new System.EventHandler(this.tbDistorionLevel_ValueChanged);
            // 
            // tbDetailLevel
            // 
            this.tbDetailLevel.Location = new System.Drawing.Point(30, 53);
            this.tbDetailLevel.Maximum = 100;
            this.tbDetailLevel.Minimum = 4;
            this.tbDetailLevel.Name = "tbDetailLevel";
            this.tbDetailLevel.Size = new System.Drawing.Size(142, 45);
            this.tbDetailLevel.TabIndex = 0;
            this.tbDetailLevel.Value = 50;
            this.tbDetailLevel.ValueChanged += new System.EventHandler(this.tbDetailLevel_ValueChanged);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Black;
            this.ClientSize = new System.Drawing.Size(1370, 750);
            this.Controls.Add(this.tbUI);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.pbScene);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "Form1";
            this.Text = "Генератор планет";
            ((System.ComponentModel.ISupportInitialize)(this.pbScene)).EndInit();
            this.tbUI.ResumeLayout(false);
            this.tbSettings.ResumeLayout(false);
            this.panelDetalisation.ResumeLayout(false);
            this.panelDetalisation.PerformLayout();
            this.panelSurface.ResumeLayout(false);
            this.panelSurface.PerformLayout();
            this.tbAdvanced.ResumeLayout(false);
            this.tbAdvanced.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.tbMoistureLevel)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.tbOceanicRate)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.tbHeatLevel)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.tbTectonicPlateCount)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.tbDistorionLevel)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.tbDetailLevel)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.bindingSource1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox pbScene;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TabControl tbUI;
        private System.Windows.Forms.TabPage tbSettings;
        private System.Windows.Forms.Label lbSurfaceChoice;
        private System.Windows.Forms.RadioButton rbPlates;
        private System.Windows.Forms.RadioButton rbHeightMap;
        private System.Windows.Forms.RadioButton rbBiomes;
        private System.Windows.Forms.TabPage tbAdvanced;
        private System.Windows.Forms.Button btGenerate;
        private System.Windows.Forms.ProgressBar pbLoading;
        private System.Windows.Forms.RadioButton rbLow;
        private System.Windows.Forms.RadioButton rbMedium;
        private System.Windows.Forms.RadioButton rbHigh;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label lbSeed;
        private System.Windows.Forms.TextBox tbSeed;
        private System.Windows.Forms.TrackBar tbMoistureLevel;
        private System.Windows.Forms.TrackBar tbOceanicRate;
        private System.Windows.Forms.TrackBar tbHeatLevel;
        private System.Windows.Forms.TrackBar tbTectonicPlateCount;
        private System.Windows.Forms.TrackBar tbDistorionLevel;
        private System.Windows.Forms.TrackBar tbDetailLevel;
        private System.Windows.Forms.BindingSource bindingSource1;
        private System.Windows.Forms.Label lbHeatLevel;
        private System.Windows.Forms.Label lbMoistureLevel;
        private System.Windows.Forms.Label lbTectonicPlateCount;
        private System.Windows.Forms.Label lbOceanicRate;
        private System.Windows.Forms.Label lbDistortionLevel;
        private System.Windows.Forms.Label lbDetailLevel;
        private System.Windows.Forms.Panel panelDetalisation;
        private System.Windows.Forms.Panel panelSurface;
    }
}

