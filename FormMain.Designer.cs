namespace IMGZ_Editor
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
            if (disposing)
            {
                Cleanup();
                if (components != null) { components.Dispose(); }
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
        	System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormMain));
        	this.buttonOpen = new System.Windows.Forms.Button();
        	this.openFileDialog = new System.Windows.Forms.OpenFileDialog();
        	this.pictureBoxBMP = new System.Windows.Forms.PictureBox();
        	this.buttonSave = new System.Windows.Forms.Button();
        	this.saveFileDialog = new System.Windows.Forms.SaveFileDialog();
        	this.folderBrowserDialog = new System.Windows.Forms.FolderBrowserDialog();
        	this.buttonReplace = new System.Windows.Forms.Button();
        	this.groupBox1 = new System.Windows.Forms.GroupBox();
        	this.groupBox2 = new System.Windows.Forms.GroupBox();
        	this.height_ = new System.Windows.Forms.NumericUpDown();
        	this.width_ = new System.Windows.Forms.NumericUpDown();
        	this.y_pos = new System.Windows.Forms.NumericUpDown();
        	this.x_pos = new System.Windows.Forms.NumericUpDown();
        	this.textBox1 = new System.Windows.Forms.TextBox();
        	this.button2 = new System.Windows.Forms.Button();
        	this.label1 = new System.Windows.Forms.Label();
        	this.numericUpDown1 = new System.Windows.Forms.NumericUpDown();
        	this.panel1 = new System.Windows.Forms.Panel();
        	this.button1 = new System.Windows.Forms.Button();
        	this.saveFileDialog1 = new System.Windows.Forms.SaveFileDialog();
        	this.groupBox3 = new System.Windows.Forms.GroupBox();
        	this.button3 = new System.Windows.Forms.Button();
        	this.label2 = new System.Windows.Forms.Label();
        	this.timer1 = new System.Windows.Forms.Timer(this.components);
        	this.button4 = new System.Windows.Forms.Button();
        	this.pictureBox1 = new System.Windows.Forms.PictureBox();
        	this.pictureBox2 = new System.Windows.Forms.PictureBox();
        	this.pictureBox3 = new System.Windows.Forms.PictureBox();
        	this.timer2 = new System.Windows.Forms.Timer(this.components);
        	this.groupBox4 = new System.Windows.Forms.GroupBox();
        	this.button5 = new System.Windows.Forms.Button();
        	this.down = new System.Windows.Forms.NumericUpDown();
        	this.right = new System.Windows.Forms.NumericUpDown();
        	this.up = new System.Windows.Forms.NumericUpDown();
        	this.left = new System.Windows.Forms.NumericUpDown();
        	this.textBox2 = new System.Windows.Forms.TextBox();
        	this.button6 = new System.Windows.Forms.Button();
        	this.label3 = new System.Windows.Forms.Label();
        	this.numericUpDown6 = new System.Windows.Forms.NumericUpDown();
        	this.panel2 = new System.Windows.Forms.Panel();
        	this.pictureBox4 = new System.Windows.Forms.PictureBox();
        	this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
        	this.horizontalFlipToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
        	this.verticalFlipToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
        	this.centerTheStretchToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
        	this.toolStripMenuItem2 = new System.Windows.Forms.ToolStripMenuItem();
        	this.toolStripMenuItem3 = new System.Windows.Forms.ToolStripMenuItem();
        	this.allToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
        	this.allToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
        	this.open = new System.Windows.Forms.PictureBox();
        	this.close = new System.Windows.Forms.PictureBox();
        	this.starting_timer = new System.Windows.Forms.Timer(this.components);
        	((System.ComponentModel.ISupportInitialize)(this.pictureBoxBMP)).BeginInit();
        	this.groupBox1.SuspendLayout();
        	this.groupBox2.SuspendLayout();
        	((System.ComponentModel.ISupportInitialize)(this.height_)).BeginInit();
        	((System.ComponentModel.ISupportInitialize)(this.width_)).BeginInit();
        	((System.ComponentModel.ISupportInitialize)(this.y_pos)).BeginInit();
        	((System.ComponentModel.ISupportInitialize)(this.x_pos)).BeginInit();
        	((System.ComponentModel.ISupportInitialize)(this.numericUpDown1)).BeginInit();
        	this.groupBox3.SuspendLayout();
        	((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
        	((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).BeginInit();
        	((System.ComponentModel.ISupportInitialize)(this.pictureBox3)).BeginInit();
        	this.groupBox4.SuspendLayout();
        	((System.ComponentModel.ISupportInitialize)(this.down)).BeginInit();
        	((System.ComponentModel.ISupportInitialize)(this.right)).BeginInit();
        	((System.ComponentModel.ISupportInitialize)(this.up)).BeginInit();
        	((System.ComponentModel.ISupportInitialize)(this.left)).BeginInit();
        	((System.ComponentModel.ISupportInitialize)(this.numericUpDown6)).BeginInit();
        	((System.ComponentModel.ISupportInitialize)(this.pictureBox4)).BeginInit();
        	this.pictureBox4.SuspendLayout();
        	this.contextMenuStrip1.SuspendLayout();
        	((System.ComponentModel.ISupportInitialize)(this.open)).BeginInit();
        	((System.ComponentModel.ISupportInitialize)(this.close)).BeginInit();
        	this.SuspendLayout();
        	// 
        	// buttonOpen
        	// 
        	this.buttonOpen.AllowDrop = true;
        	this.buttonOpen.Location = new System.Drawing.Point(6, 15);
        	this.buttonOpen.Name = "buttonOpen";
        	this.buttonOpen.Size = new System.Drawing.Size(67, 23);
        	this.buttonOpen.TabIndex = 0;
        	this.buttonOpen.Text = "Open 2DD";
        	this.buttonOpen.UseVisualStyleBackColor = true;
        	this.buttonOpen.Click += new System.EventHandler(this.buttonOpen_Click);
        	this.buttonOpen.DragDrop += new System.Windows.Forms.DragEventHandler(this.buttonOpen_DragDrop);
        	this.buttonOpen.DragEnter += new System.Windows.Forms.DragEventHandler(this.buttonOpen_DragEnter);
        	// 
        	// openFileDialog
        	// 
        	this.openFileDialog.ShowReadOnly = true;
        	// 
        	// pictureBoxBMP
        	// 
        	this.pictureBoxBMP.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("pictureBoxBMP.BackgroundImage")));
        	this.pictureBoxBMP.Cursor = System.Windows.Forms.Cursors.Default;
        	this.pictureBoxBMP.Enabled = false;
        	this.pictureBoxBMP.Location = new System.Drawing.Point(160, 50);
        	this.pictureBoxBMP.Name = "pictureBoxBMP";
        	this.pictureBoxBMP.Size = new System.Drawing.Size(256, 256);
        	this.pictureBoxBMP.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
        	this.pictureBoxBMP.TabIndex = 2;
        	this.pictureBoxBMP.TabStop = false;
        	this.pictureBoxBMP.MouseDown += new System.Windows.Forms.MouseEventHandler(this.PictureBoxBMPMouseDown);
        	this.pictureBoxBMP.MouseMove += new System.Windows.Forms.MouseEventHandler(this.PictureBoxBMPMouseMove);
        	this.pictureBoxBMP.MouseUp += new System.Windows.Forms.MouseEventHandler(this.PictureBoxBMPMouseUp);
        	// 
        	// buttonSave
        	// 
        	this.buttonSave.Enabled = false;
        	this.buttonSave.Location = new System.Drawing.Point(32, 19);
        	this.buttonSave.Name = "buttonSave";
        	this.buttonSave.Size = new System.Drawing.Size(75, 23);
        	this.buttonSave.TabIndex = 4;
        	this.buttonSave.Text = "Save";
        	this.buttonSave.UseVisualStyleBackColor = true;
        	this.buttonSave.Click += new System.EventHandler(this.buttonSave_Click);
        	// 
        	// saveFileDialog
        	// 
        	this.saveFileDialog.DefaultExt = "png";
        	this.saveFileDialog.Filter = "PNG image file|*.png";
        	// 
        	// buttonReplace
        	// 
        	this.buttonReplace.Enabled = false;
        	this.buttonReplace.Location = new System.Drawing.Point(32, 48);
        	this.buttonReplace.Name = "buttonReplace";
        	this.buttonReplace.Size = new System.Drawing.Size(75, 23);
        	this.buttonReplace.TabIndex = 5;
        	this.buttonReplace.Text = "Replace";
        	this.buttonReplace.UseVisualStyleBackColor = true;
        	this.buttonReplace.Click += new System.EventHandler(this.buttonReplace_Click);
        	// 
        	// groupBox1
        	// 
        	this.groupBox1.Controls.Add(this.buttonReplace);
        	this.groupBox1.Controls.Add(this.buttonSave);
        	this.groupBox1.Enabled = false;
        	this.groupBox1.Location = new System.Drawing.Point(5, 44);
        	this.groupBox1.Name = "groupBox1";
        	this.groupBox1.Size = new System.Drawing.Size(141, 84);
        	this.groupBox1.TabIndex = 6;
        	this.groupBox1.TabStop = false;
        	this.groupBox1.Text = "Image";
        	// 
        	// groupBox2
        	// 
        	this.groupBox2.Controls.Add(this.height_);
        	this.groupBox2.Controls.Add(this.width_);
        	this.groupBox2.Controls.Add(this.y_pos);
        	this.groupBox2.Controls.Add(this.x_pos);
        	this.groupBox2.Controls.Add(this.textBox1);
        	this.groupBox2.Controls.Add(this.button2);
        	this.groupBox2.Controls.Add(this.label1);
        	this.groupBox2.Controls.Add(this.numericUpDown1);
        	this.groupBox2.Controls.Add(this.panel1);
        	this.groupBox2.Enabled = false;
        	this.groupBox2.Location = new System.Drawing.Point(6, 134);
        	this.groupBox2.Name = "groupBox2";
        	this.groupBox2.Size = new System.Drawing.Size(140, 149);
        	this.groupBox2.TabIndex = 7;
        	this.groupBox2.TabStop = false;
        	this.groupBox2.Text = "Crops";
        	// 
        	// height_
        	// 
        	this.height_.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
        	this.height_.Location = new System.Drawing.Point(73, 71);
        	this.height_.Name = "height_";
        	this.height_.Size = new System.Drawing.Size(43, 20);
        	this.height_.TabIndex = 8;
        	this.height_.ValueChanged += new System.EventHandler(this.Height_ValueChanged);
        	this.height_.Click += new System.EventHandler(this.Height_Click);
        	this.height_.DoubleClick += new System.EventHandler(this.Height_DoubleClick);
        	// 
        	// width_
        	// 
        	this.width_.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
        	this.width_.Location = new System.Drawing.Point(24, 71);
        	this.width_.Name = "width_";
        	this.width_.Size = new System.Drawing.Size(43, 20);
        	this.width_.TabIndex = 7;
        	this.width_.ValueChanged += new System.EventHandler(this.Width_ValueChanged);
        	this.width_.Click += new System.EventHandler(this.Width_Click);
        	this.width_.DoubleClick += new System.EventHandler(this.Width_DoubleClick);
        	// 
        	// y_pos
        	// 
        	this.y_pos.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
        	this.y_pos.Location = new System.Drawing.Point(73, 49);
        	this.y_pos.Name = "y_pos";
        	this.y_pos.Size = new System.Drawing.Size(43, 20);
        	this.y_pos.TabIndex = 6;
        	this.y_pos.ValueChanged += new System.EventHandler(this.Y_posValueChanged);
        	this.y_pos.Click += new System.EventHandler(this.Y_posClick);
        	this.y_pos.DoubleClick += new System.EventHandler(this.Y_posDoubleClick);
        	// 
        	// x_pos
        	// 
        	this.x_pos.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
        	this.x_pos.Location = new System.Drawing.Point(24, 49);
        	this.x_pos.Name = "x_pos";
        	this.x_pos.Size = new System.Drawing.Size(43, 20);
        	this.x_pos.TabIndex = 5;
        	this.x_pos.ValueChanged += new System.EventHandler(this.X_posValueChanged);
        	this.x_pos.Click += new System.EventHandler(this.X_posClick);
        	this.x_pos.DoubleClick += new System.EventHandler(this.X_posDoubleClick);
        	// 
        	// textBox1
        	// 
        	this.textBox1.Location = new System.Drawing.Point(21, 96);
        	this.textBox1.Name = "textBox1";
        	this.textBox1.Size = new System.Drawing.Size(98, 20);
        	this.textBox1.TabIndex = 4;
        	this.textBox1.Text = "0;0;0;0";
        	this.textBox1.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
        	this.textBox1.TextChanged += new System.EventHandler(this.TextBox1TextChanged);
        	this.textBox1.Leave += new System.EventHandler(this.TextBox1Leave);
        	// 
        	// button2
        	// 
        	this.button2.Location = new System.Drawing.Point(13, 122);
        	this.button2.Name = "button2";
        	this.button2.Size = new System.Drawing.Size(114, 22);
        	this.button2.TabIndex = 3;
        	this.button2.Text = "Reset Index";
        	this.button2.UseVisualStyleBackColor = true;
        	this.button2.Click += new System.EventHandler(this.Button2Click);
        	// 
        	// label1
        	// 
        	this.label1.Location = new System.Drawing.Point(13, 16);
        	this.label1.Name = "label1";
        	this.label1.Size = new System.Drawing.Size(50, 20);
        	this.label1.TabIndex = 2;
        	this.label1.Text = "Index";
        	this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
        	// 
        	// numericUpDown1
        	// 
        	this.numericUpDown1.Location = new System.Drawing.Point(69, 16);
        	this.numericUpDown1.Name = "numericUpDown1";
        	this.numericUpDown1.Size = new System.Drawing.Size(51, 20);
        	this.numericUpDown1.TabIndex = 1;
        	this.numericUpDown1.ValueChanged += new System.EventHandler(this.NumericUpDown1ValueChanged);
        	// 
        	// panel1
        	// 
        	this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
        	this.panel1.Location = new System.Drawing.Point(22, 47);
        	this.panel1.Name = "panel1";
        	this.panel1.Size = new System.Drawing.Size(96, 46);
        	this.panel1.TabIndex = 9;
        	// 
        	// button1
        	// 
        	this.button1.Enabled = false;
        	this.button1.Location = new System.Drawing.Point(79, 15);
        	this.button1.Name = "button1";
        	this.button1.Size = new System.Drawing.Size(67, 23);
        	this.button1.TabIndex = 8;
        	this.button1.Text = "Save 2DD";
        	this.button1.UseVisualStyleBackColor = true;
        	this.button1.Click += new System.EventHandler(this.Button1Click);
        	// 
        	// saveFileDialog1
        	// 
        	this.saveFileDialog1.DefaultExt = "png";
        	this.saveFileDialog1.Filter = "2DD Files|*.2dd";
        	// 
        	// groupBox3
        	// 
        	this.groupBox3.Controls.Add(this.button3);
        	this.groupBox3.Controls.Add(this.label2);
        	this.groupBox3.Enabled = false;
        	this.groupBox3.Location = new System.Drawing.Point(6, 450);
        	this.groupBox3.Name = "groupBox3";
        	this.groupBox3.Size = new System.Drawing.Size(140, 65);
        	this.groupBox3.TabIndex = 9;
        	this.groupBox3.TabStop = false;
        	// 
        	// button3
        	// 
        	this.button3.Enabled = false;
        	this.button3.Location = new System.Drawing.Point(17, 30);
        	this.button3.Name = "button3";
        	this.button3.Size = new System.Drawing.Size(103, 23);
        	this.button3.TabIndex = 0;
        	this.button3.Text = "Preview ingame";
        	this.button3.UseVisualStyleBackColor = true;
        	this.button3.Click += new System.EventHandler(this.Button3Click);
        	// 
        	// label2
        	// 
        	this.label2.Location = new System.Drawing.Point(2, 7);
        	this.label2.Name = "label2";
        	this.label2.Size = new System.Drawing.Size(136, 20);
        	this.label2.TabIndex = 1;
        	this.label2.Text = "Please open PCSX2...";
        	this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
        	// 
        	// timer1
        	// 
        	this.timer1.Enabled = true;
        	this.timer1.Interval = 500;
        	this.timer1.Tick += new System.EventHandler(this.Timer1Tick);
        	// 
        	// button4
        	// 
        	this.button4.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
        	this.button4.Location = new System.Drawing.Point(398, 15);
        	this.button4.Name = "button4";
        	this.button4.Size = new System.Drawing.Size(110, 23);
        	this.button4.TabIndex = 10;
        	this.button4.Text = "Dark background";
        	this.button4.UseVisualStyleBackColor = true;
        	this.button4.Click += new System.EventHandler(this.Button4Click);
        	// 
        	// pictureBox1
        	// 
        	this.pictureBox1.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("pictureBox1.BackgroundImage")));
        	this.pictureBox1.Location = new System.Drawing.Point(307, 299);
        	this.pictureBox1.Name = "pictureBox1";
        	this.pictureBox1.Size = new System.Drawing.Size(34, 21);
        	this.pictureBox1.TabIndex = 11;
        	this.pictureBox1.TabStop = false;
        	this.pictureBox1.Visible = false;
        	// 
        	// pictureBox2
        	// 
        	this.pictureBox2.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("pictureBox2.BackgroundImage")));
        	this.pictureBox2.Location = new System.Drawing.Point(347, 299);
        	this.pictureBox2.Name = "pictureBox2";
        	this.pictureBox2.Size = new System.Drawing.Size(34, 21);
        	this.pictureBox2.TabIndex = 12;
        	this.pictureBox2.TabStop = false;
        	this.pictureBox2.Visible = false;
        	// 
        	// pictureBox3
        	// 
        	this.pictureBox3.BackColor = System.Drawing.Color.Transparent;
        	this.pictureBox3.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
        	this.pictureBox3.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
        	this.pictureBox3.Location = new System.Drawing.Point(128, 129);
        	this.pictureBox3.Name = "pictureBox3";
        	this.pictureBox3.Size = new System.Drawing.Size(50, 50);
        	this.pictureBox3.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
        	this.pictureBox3.TabIndex = 14;
        	this.pictureBox3.TabStop = false;
        	// 
        	// timer2
        	// 
        	this.timer2.Enabled = true;
        	this.timer2.Interval = 1;
        	this.timer2.Tick += new System.EventHandler(this.Timer2Tick);
        	// 
        	// groupBox4
        	// 
        	this.groupBox4.Controls.Add(this.button5);
        	this.groupBox4.Controls.Add(this.down);
        	this.groupBox4.Controls.Add(this.right);
        	this.groupBox4.Controls.Add(this.up);
        	this.groupBox4.Controls.Add(this.left);
        	this.groupBox4.Controls.Add(this.textBox2);
        	this.groupBox4.Controls.Add(this.button6);
        	this.groupBox4.Controls.Add(this.label3);
        	this.groupBox4.Controls.Add(this.numericUpDown6);
        	this.groupBox4.Controls.Add(this.panel2);
        	this.groupBox4.Enabled = false;
        	this.groupBox4.Location = new System.Drawing.Point(6, 289);
        	this.groupBox4.Name = "groupBox4";
        	this.groupBox4.Size = new System.Drawing.Size(140, 155);
        	this.groupBox4.TabIndex = 10;
        	this.groupBox4.TabStop = false;
        	this.groupBox4.Text = "Stretches";
        	// 
        	// button5
        	// 
        	this.button5.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("button5.BackgroundImage")));
        	this.button5.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
        	this.button5.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
        	this.button5.Location = new System.Drawing.Point(4, 73);
        	this.button5.Name = "button5";
        	this.button5.Size = new System.Drawing.Size(16, 17);
        	this.button5.TabIndex = 10;
        	this.button5.UseVisualStyleBackColor = true;
        	this.button5.Click += new System.EventHandler(this.Button5Click);
        	// 
        	// down
        	// 
        	this.down.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
        	this.down.Location = new System.Drawing.Point(73, 71);
        	this.down.Maximum = new decimal(new int[] {
        	        	        	5000,
        	        	        	0,
        	        	        	0,
        	        	        	0});
        	this.down.Minimum = new decimal(new int[] {
        	        	        	5000,
        	        	        	0,
        	        	        	0,
        	        	        	-2147483648});
        	this.down.Name = "down";
        	this.down.Size = new System.Drawing.Size(43, 20);
        	this.down.TabIndex = 8;
        	this.down.ValueChanged += new System.EventHandler(this.DownValueChanged);
        	// 
        	// right
        	// 
        	this.right.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
        	this.right.Location = new System.Drawing.Point(24, 71);
        	this.right.Maximum = new decimal(new int[] {
        	        	        	5000,
        	        	        	0,
        	        	        	0,
        	        	        	0});
        	this.right.Minimum = new decimal(new int[] {
        	        	        	5000,
        	        	        	0,
        	        	        	0,
        	        	        	-2147483648});
        	this.right.Name = "right";
        	this.right.Size = new System.Drawing.Size(43, 20);
        	this.right.TabIndex = 7;
        	this.right.ValueChanged += new System.EventHandler(this.RightValueChanged);
        	// 
        	// up
        	// 
        	this.up.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
        	this.up.Location = new System.Drawing.Point(73, 49);
        	this.up.Maximum = new decimal(new int[] {
        	        	        	5000,
        	        	        	0,
        	        	        	0,
        	        	        	0});
        	this.up.Minimum = new decimal(new int[] {
        	        	        	5000,
        	        	        	0,
        	        	        	0,
        	        	        	-2147483648});
        	this.up.Name = "up";
        	this.up.Size = new System.Drawing.Size(43, 20);
        	this.up.TabIndex = 6;
        	this.up.ValueChanged += new System.EventHandler(this.UpValueChanged);
        	// 
        	// left
        	// 
        	this.left.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
        	this.left.Location = new System.Drawing.Point(24, 49);
        	this.left.Maximum = new decimal(new int[] {
        	        	        	5000,
        	        	        	0,
        	        	        	0,
        	        	        	0});
        	this.left.Minimum = new decimal(new int[] {
        	        	        	5000,
        	        	        	0,
        	        	        	0,
        	        	        	-2147483648});
        	this.left.Name = "left";
        	this.left.Size = new System.Drawing.Size(43, 20);
        	this.left.TabIndex = 5;
        	this.left.ValueChanged += new System.EventHandler(this.LeftValueChanged);
        	// 
        	// textBox2
        	// 
        	this.textBox2.Location = new System.Drawing.Point(21, 96);
        	this.textBox2.Name = "textBox2";
        	this.textBox2.Size = new System.Drawing.Size(98, 20);
        	this.textBox2.TabIndex = 4;
        	this.textBox2.Text = "0;0;0;0";
        	this.textBox2.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
        	this.textBox2.TextChanged += new System.EventHandler(this.TextBox2TextChanged);
        	this.textBox2.Leave += new System.EventHandler(this.TextBox2Leave);
        	// 
        	// button6
        	// 
        	this.button6.Location = new System.Drawing.Point(13, 122);
        	this.button6.Name = "button6";
        	this.button6.Size = new System.Drawing.Size(114, 22);
        	this.button6.TabIndex = 3;
        	this.button6.Text = "Reset Stretch";
        	this.button6.UseVisualStyleBackColor = true;
        	this.button6.Click += new System.EventHandler(this.Button6Click);
        	// 
        	// label3
        	// 
        	this.label3.Location = new System.Drawing.Point(13, 16);
        	this.label3.Name = "label3";
        	this.label3.Size = new System.Drawing.Size(50, 20);
        	this.label3.TabIndex = 2;
        	this.label3.Text = "Index";
        	this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
        	// 
        	// numericUpDown6
        	// 
        	this.numericUpDown6.Location = new System.Drawing.Point(69, 16);
        	this.numericUpDown6.Name = "numericUpDown6";
        	this.numericUpDown6.Size = new System.Drawing.Size(51, 20);
        	this.numericUpDown6.TabIndex = 1;
        	this.numericUpDown6.ValueChanged += new System.EventHandler(this.NumericUpDown6ValueChanged);
        	// 
        	// panel2
        	// 
        	this.panel2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
        	this.panel2.Location = new System.Drawing.Point(22, 47);
        	this.panel2.Name = "panel2";
        	this.panel2.Size = new System.Drawing.Size(96, 46);
        	this.panel2.TabIndex = 9;
        	// 
        	// pictureBox4
        	// 
        	this.pictureBox4.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("pictureBox4.BackgroundImage")));
        	this.pictureBox4.ContextMenuStrip = this.contextMenuStrip1;
        	this.pictureBox4.Controls.Add(this.pictureBox3);
        	this.pictureBox4.Cursor = System.Windows.Forms.Cursors.Default;
        	this.pictureBox4.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox4.Image")));
        	this.pictureBox4.Location = new System.Drawing.Point(160, 317);
        	this.pictureBox4.Name = "pictureBox4";
        	this.pictureBox4.Size = new System.Drawing.Size(256, 256);
        	this.pictureBox4.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
        	this.pictureBox4.TabIndex = 15;
        	this.pictureBox4.TabStop = false;
        	// 
        	// contextMenuStrip1
        	// 
        	this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
        	        	        	this.horizontalFlipToolStripMenuItem,
        	        	        	this.verticalFlipToolStripMenuItem,
        	        	        	this.centerTheStretchToolStripMenuItem,
        	        	        	this.toolStripMenuItem2,
        	        	        	this.toolStripMenuItem3,
        	        	        	this.allToolStripMenuItem,
        	        	        	this.allToolStripMenuItem1});
        	this.contextMenuStrip1.Name = "contextMenuStrip1";
        	this.contextMenuStrip1.Size = new System.Drawing.Size(169, 158);
        	// 
        	// horizontalFlipToolStripMenuItem
        	// 
        	this.horizontalFlipToolStripMenuItem.Name = "horizontalFlipToolStripMenuItem";
        	this.horizontalFlipToolStripMenuItem.Size = new System.Drawing.Size(168, 22);
        	this.horizontalFlipToolStripMenuItem.Text = "Horizontal flip";
        	this.horizontalFlipToolStripMenuItem.Click += new System.EventHandler(this.HorizontalFlipToolStripMenuItemClick);
        	// 
        	// verticalFlipToolStripMenuItem
        	// 
        	this.verticalFlipToolStripMenuItem.Name = "verticalFlipToolStripMenuItem";
        	this.verticalFlipToolStripMenuItem.Size = new System.Drawing.Size(168, 22);
        	this.verticalFlipToolStripMenuItem.Text = "Vertical flip";
        	this.verticalFlipToolStripMenuItem.Click += new System.EventHandler(this.VerticalFlipToolStripMenuItemClick);
        	// 
        	// centerTheStretchToolStripMenuItem
        	// 
        	this.centerTheStretchToolStripMenuItem.Name = "centerTheStretchToolStripMenuItem";
        	this.centerTheStretchToolStripMenuItem.Size = new System.Drawing.Size(168, 22);
        	this.centerTheStretchToolStripMenuItem.Text = "Center the stretch";
        	this.centerTheStretchToolStripMenuItem.Click += new System.EventHandler(this.CenterTheStretchToolStripMenuItemClick);
        	// 
        	// toolStripMenuItem2
        	// 
        	this.toolStripMenuItem2.Name = "toolStripMenuItem2";
        	this.toolStripMenuItem2.Size = new System.Drawing.Size(168, 22);
        	this.toolStripMenuItem2.Text = "4:3";
        	this.toolStripMenuItem2.Click += new System.EventHandler(this.ToolStripMenuItem2Click);
        	// 
        	// toolStripMenuItem3
        	// 
        	this.toolStripMenuItem3.Name = "toolStripMenuItem3";
        	this.toolStripMenuItem3.Size = new System.Drawing.Size(168, 22);
        	this.toolStripMenuItem3.Text = "16:9";
        	this.toolStripMenuItem3.Click += new System.EventHandler(this.ToolStripMenuItem3Click);
        	// 
        	// allToolStripMenuItem
        	// 
        	this.allToolStripMenuItem.Name = "allToolStripMenuItem";
        	this.allToolStripMenuItem.Size = new System.Drawing.Size(168, 22);
        	this.allToolStripMenuItem.Text = "4:3 All";
        	this.allToolStripMenuItem.Click += new System.EventHandler(this.AllToolStripMenuItemClick);
        	// 
        	// allToolStripMenuItem1
        	// 
        	this.allToolStripMenuItem1.Name = "allToolStripMenuItem1";
        	this.allToolStripMenuItem1.Size = new System.Drawing.Size(168, 22);
        	this.allToolStripMenuItem1.Text = "16:9 All";
        	this.allToolStripMenuItem1.Click += new System.EventHandler(this.AllToolStripMenuItem1Click);
        	// 
        	// open
        	// 
        	this.open.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("open.BackgroundImage")));
        	this.open.Location = new System.Drawing.Point(398, 285);
        	this.open.Name = "open";
        	this.open.Size = new System.Drawing.Size(34, 21);
        	this.open.TabIndex = 16;
        	this.open.TabStop = false;
        	this.open.Visible = false;
        	// 
        	// close
        	// 
        	this.close.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("close.BackgroundImage")));
        	this.close.Location = new System.Drawing.Point(438, 285);
        	this.close.Name = "close";
        	this.close.Size = new System.Drawing.Size(34, 21);
        	this.close.TabIndex = 17;
        	this.close.TabStop = false;
        	this.close.Visible = false;
        	// 
        	// starting_timer
        	// 
        	this.starting_timer.Interval = 10;
        	this.starting_timer.Tick += new System.EventHandler(this.Starting_timerTick);
        	// 
        	// FormMain
        	// 
        	this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
        	this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
        	this.AutoSize = true;
        	this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
        	this.ClientSize = new System.Drawing.Size(514, 599);
        	this.Controls.Add(this.close);
        	this.Controls.Add(this.open);
        	this.Controls.Add(this.pictureBox4);
        	this.Controls.Add(this.groupBox4);
        	this.Controls.Add(this.pictureBoxBMP);
        	this.Controls.Add(this.pictureBox2);
        	this.Controls.Add(this.pictureBox1);
        	this.Controls.Add(this.button4);
        	this.Controls.Add(this.groupBox3);
        	this.Controls.Add(this.button1);
        	this.Controls.Add(this.buttonOpen);
        	this.Controls.Add(this.groupBox1);
        	this.Controls.Add(this.groupBox2);
        	this.MinimumSize = new System.Drawing.Size(370, 310);
        	this.Name = "FormMain";
        	this.Padding = new System.Windows.Forms.Padding(3);
        	this.Text = "KH2 2DD Command Editor 1.3";
        	this.Deactivate += new System.EventHandler(this.FormMainDeactivate);
        	((System.ComponentModel.ISupportInitialize)(this.pictureBoxBMP)).EndInit();
        	this.groupBox1.ResumeLayout(false);
        	this.groupBox2.ResumeLayout(false);
        	this.groupBox2.PerformLayout();
        	((System.ComponentModel.ISupportInitialize)(this.height_)).EndInit();
        	((System.ComponentModel.ISupportInitialize)(this.width_)).EndInit();
        	((System.ComponentModel.ISupportInitialize)(this.y_pos)).EndInit();
        	((System.ComponentModel.ISupportInitialize)(this.x_pos)).EndInit();
        	((System.ComponentModel.ISupportInitialize)(this.numericUpDown1)).EndInit();
        	this.groupBox3.ResumeLayout(false);
        	((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
        	((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).EndInit();
        	((System.ComponentModel.ISupportInitialize)(this.pictureBox3)).EndInit();
        	this.groupBox4.ResumeLayout(false);
        	this.groupBox4.PerformLayout();
        	((System.ComponentModel.ISupportInitialize)(this.down)).EndInit();
        	((System.ComponentModel.ISupportInitialize)(this.right)).EndInit();
        	((System.ComponentModel.ISupportInitialize)(this.up)).EndInit();
        	((System.ComponentModel.ISupportInitialize)(this.left)).EndInit();
        	((System.ComponentModel.ISupportInitialize)(this.numericUpDown6)).EndInit();
        	((System.ComponentModel.ISupportInitialize)(this.pictureBox4)).EndInit();
        	this.pictureBox4.ResumeLayout(false);
        	this.contextMenuStrip1.ResumeLayout(false);
        	((System.ComponentModel.ISupportInitialize)(this.open)).EndInit();
        	((System.ComponentModel.ISupportInitialize)(this.close)).EndInit();
        	this.ResumeLayout(false);
        	this.PerformLayout();
        }
        private System.Windows.Forms.ToolStripMenuItem allToolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem allToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem3;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem2;
        private System.Windows.Forms.Timer starting_timer;
        private System.Windows.Forms.ToolStripMenuItem centerTheStretchToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem verticalFlipToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem horizontalFlipToolStripMenuItem;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.PictureBox close;
        private System.Windows.Forms.PictureBox open;
        private System.Windows.Forms.Button button5;
        private System.Windows.Forms.PictureBox pictureBox4;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.NumericUpDown numericUpDown6;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button button6;
        private System.Windows.Forms.TextBox textBox2;
        private System.Windows.Forms.NumericUpDown left;
        private System.Windows.Forms.NumericUpDown up;
        private System.Windows.Forms.NumericUpDown right;
        private System.Windows.Forms.NumericUpDown down;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.Timer timer2;
        private System.Windows.Forms.PictureBox pictureBox3;
        private System.Windows.Forms.PictureBox pictureBox2;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Button button4;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.SaveFileDialog saveFileDialog1;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.NumericUpDown x_pos;
        private System.Windows.Forms.NumericUpDown y_pos;
        private System.Windows.Forms.NumericUpDown width_;
        private System.Windows.Forms.NumericUpDown height_;
        private System.Windows.Forms.NumericUpDown numericUpDown1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.GroupBox groupBox1;

        #endregion

        private System.Windows.Forms.Button buttonOpen;
        private System.Windows.Forms.OpenFileDialog openFileDialog;
        private System.Windows.Forms.PictureBox pictureBoxBMP;
        private System.Windows.Forms.Button buttonSave;
        private System.Windows.Forms.SaveFileDialog saveFileDialog;
        private System.Windows.Forms.FolderBrowserDialog folderBrowserDialog;
        private System.Windows.Forms.Button buttonReplace;
        
        void X_posDoubleClick(object sender, System.EventArgs e)
        {
        	if ((int)x_pos.Value==x_)
        	{
        		if (2*x_pos.Value<x_pos.Maximum)
        		{
        			x_pos.Value = 2*x_pos.Value;
        		}
        		else
        		{
        			x_pos.Value = x_pos.Maximum-width_.Value;
        		}
        	}
        }
        
        void Y_posDoubleClick(object sender, System.EventArgs e)
        {
        	if ((int)y_pos.Value==y_)
        	{
        		if (2*y_pos.Value<y_pos.Maximum)
        		{
        			y_pos.Value = 2*y_pos.Value;
        		}
        		else
        		{
        			y_pos.Value = y_pos.Maximum-height_.Value;
        		}
        	}
        }
        
        void Width_DoubleClick(object sender, System.EventArgs e)
        {
        	if ((int)width_.Value==w_)
        	{
        		if (2*width_.Value<width_.Maximum)
        		{
        			width_.Value = 2*width_.Value;
        		}
        		else
        		{
        			width_.Value =width_.Maximum;
        		}
        	}
        }
        
        void Height_DoubleClick(object sender, System.EventArgs e)
        {
        	if ((int)height_.Value==h_)
        	{
        		if (2*height_.Value<height_.Maximum)
        		{
        			height_.Value = 2*height_.Value;
        		}
        		else
        		{
        			height_.Value = height_.Maximum;
        		}
        	}
        }
        int y_=0;
        int x_=0;
        int w_=0;
        int h_=0;
        
        void Y_posClick(object sender, System.EventArgs e)
        {
        	y_= (int)y_pos.Value;
        }
        
        void X_posClick(object sender, System.EventArgs e)
        {
        	x_= (int)x_pos.Value;
        }
        
        
        void Width_Click(object sender, System.EventArgs e)
        {
        	w_= (int)width_.Value;
        }
        
        void Height_Click(object sender, System.EventArgs e)
        {
        	h_= (int)height_.Value;
        }
    }
}

