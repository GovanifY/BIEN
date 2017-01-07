using System;
using System.Diagnostics;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace IMGZ_Editor
{
    public partial class FormMain : Form
    {
        public static readonly System.Diagnostics.FileVersionInfo program = System.Diagnostics.FileVersionInfo.GetVersionInfo(System.Reflection.Assembly.GetEntryAssembly().Location);

        public static void ShowError(Exception e, string title = "An error has occurred", System.Windows.Forms.MessageBoxIcon icon = System.Windows.Forms.MessageBoxIcon.Error)
        {
            if (System.Windows.Forms.MessageBox.Show(string.Format("{0}\r\nDo you want to see more detailed debugging information?", e.Message), title, System.Windows.Forms.MessageBoxButtons.YesNo, icon) == System.Windows.Forms.DialogResult.Yes)
            {
                System.Text.StringBuilder str = new System.Text.StringBuilder();
                do
                {
                    str.AppendFormat("{0}: {1}\r\n{2}\r\n\r\n", e.GetType().Name, e.Message, e.StackTrace);
                    e = e.InnerException;
                } while (e != null);
                System.Windows.Forms.MessageBox.Show(str.ToString(), title, System.Windows.Forms.MessageBoxButtons.OK, icon);
            }
        }
		
		[DllImport("kernel32.dll", EntryPoint = "WriteProcessMemory")]
			//Definition of the function arguments for "WriteProcessMemory"
			static extern void WriteProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, byte[] lpBuffer, int nSize, [Out] int lpNumberOfBytesWritten);
		
		[DllImport("kernel32.dll", EntryPoint = "OpenProcess")]
			//Definition of the function arguments for "OpenProcess"
			static extern IntPtr OpenProcess(int dwDesiredAccess, bool bInheritHandle, int dwProcessId);
			public void WriteBytes(Process process, int address, byte[] ValueToWrite, int numberOfBytesToWrite)
		{
			try
			{
				WriteProcessMemory(OpenProcess(0x001F0FFF, false,process.Id), (IntPtr)address, ValueToWrite, numberOfBytesToWrite, 0);
			}
			catch {}
		}
        ImageContainer img;
        string imgName;

        System.Collections.Generic.Dictionary<string, Type> typeLoaders;

        public FormMain()
        {
            loadClasses();
            InitializeComponent();
#if MDLX_DUMP_RAW_DATA
            this.Text += " [MDLX_DUMP_RAW_DATA]";
#endif
            this.folderBrowserDialog.SelectedPath = AppDomain.CurrentDomain.BaseDirectory;
        }
        
		[DllImport("kernel32.dll", EntryPoint = "ReadProcessMemory")]
			static extern bool ReadProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, [Out] byte[] lpBuffer, int dwSize, out int lpNumberOfBytesRead);
        public byte[] ReadBytes(Process process, int address, int numberOfBytesToRead)
		{
			try
			{
				byte[] bytes_to_read = new byte[numberOfBytesToRead];
				int ReturnBytesRead;
				ReadProcessMemory(OpenProcess(0x001F0FFF, false, process.Id), new IntPtr(address), bytes_to_read, numberOfBytesToRead, out ReturnBytesRead);
				return bytes_to_read;
			}
			catch
			{
				return new byte[] {0};
			}
		}
        
        private void Cleanup(bool sizeReset = true)
        {
            pictureBoxBMP.Enabled = groupBox1.Enabled = groupBox4.Enabled = groupBox2.Enabled = groupBox3.Enabled = false; 
            if (this.pictureBoxBMP.Image != null) { this.pictureBoxBMP.Image.Dispose(); this.pictureBoxBMP.Image = null; }
            if (img != null) { img.Dispose(); img = null; imgName = ""; }
            this.pictureBoxBMP.Height = this.pictureBoxBMP.Width = 200;
        }
        private void loadClasses()
        {
            if (this.typeLoaders != null) { return; }
            this.typeLoaders = new System.Collections.Generic.Dictionary<string, Type>();
            var baseType = typeof(ImageContainer);
            var assembly = baseType.Assembly;
            foreach (Type type in assembly.GetTypes())
            {
                try
                {
                    if (type.IsSubclassOf(baseType) && type.IsClass && !type.IsAbstract)
                    {
                        var field = type.GetField("extensions", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static);
                        if (field != null)
                        {
                            var exts = (System.Collections.Generic.IEnumerable<string>)field.GetValue(null);
                            foreach (string ext in exts)
                            {
                                this.typeLoaders.Add('.' + ext, type);
                                System.Diagnostics.Debug.WriteLine(string.Format("Loaded ext {0} for {1}", ext, type));
                            }
                        }
                    }
                }
                catch (Exception e) { ShowError(e, "Error checking " + type); }
            }
        }
		public byte[] SubArray(byte[] data, long index, long length)
        {
            byte[] result = new byte[length];
            try
            {
                Array.Copy(data, index, result, 0, length);
            }
            catch { }
            return result;
        }

        public byte[] WriteArray(byte[] input, int address, byte[] value_)
        {
            return Combine(Combine(SubArray(input, 0, address), value_), SubArray(input, address + value_.Length, input.Length - (address + value_.Length)));
        }

        private static byte[] Combine(byte[] a, byte[] b)
        {
            byte[] c = new byte[a.Length + b.Length];
            try
            {
                System.Buffer.BlockCopy(a, 0, c, 0, a.Length);
                System.Buffer.BlockCopy(b, 0, c, a.Length, b.Length);
            }
            catch { }
            return c;
        }
        
		private System.Windows.Forms.ListBox listBox1;
		private System.Windows.Forms.Form form;
		void OpenList(string[] items)
		{
			form = new Form();
			listBox1 = new System.Windows.Forms.ListBox();
			SuspendLayout();
			// 
			// listBox1
			// 
			listBox1.FormattingEnabled = true;
			listBox1.Location = new System.Drawing.Point(13, 13);
			listBox1.Name = "listBox1";
			listBox1.Size = new System.Drawing.Size(259, 121);
			listBox1.TabIndex = 0;
			listBox1.DoubleClick += new System.EventHandler(ListBox1DoubleClick);
			// 
			// MainForm
			// 
			listBox1.Items.AddRange(items);
			form.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			form.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			form.ClientSize = new System.Drawing.Size(284, 150);
			form.Controls.Add(listBox1);
			form.MaximizeBox = false;
			form.MaximumSize = new System.Drawing.Size(300, 188);
			form.MinimizeBox = false;
			form.MinimumSize = new System.Drawing.Size(300, 188);
			form.Name = "form";
			this.form.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			form.ShowIcon = false;
			form.Text = "BAR index";
			//form.Location = new Point(this.Location.X+(this.Width/2)-(form.ClientSize.Width/2),this.Location.Y+(this.Height/2)-(form.ClientSize.Height/2));
			this.ResumeLayout(false);
			this.form.ShowDialog();
		}
        byte[] command_bytes;
        string imd_name = "";
            System.IO.FileStream file = null;
            string command_original_filename = "";
            
		void ListBox1DoubleClick(object sender, EventArgs e)
		{
			index=listBox1.SelectedIndex;
			form.Close();
		}
		int index=-1;
        private void openFile(string filename, bool reado = false)
        {
            num_index = -1;
            try
            {
            		command_original_filename = filename;
	                command_bytes = System.IO.File.ReadAllBytes(filename);
            	if (BitConverter.ToInt32(SubArray(command_bytes,4,4),0)>0)
            	{
            		index = -1;
            		string[] names = new string[BitConverter.ToInt32(SubArray(command_bytes,4,4),0)/2];
            		for (int i=0;i<BitConverter.ToInt32(SubArray(command_bytes,4,4),0)/2;i++)
            		{
            			names[i]=System.Text.Encoding.UTF8.GetString(SubArray(command_bytes,0x14+(i*32),4))+"\r\n";
            		}
            		if (BitConverter.ToInt32(SubArray(command_bytes,4,4),0)>2)
            		{
            			OpenList(names);
            		}
            		else
            		{
            			index=0;
            		}
            		if (index>-1)
            		{
	            	   	byte[] byte_stream;
						if (index<(BitConverter.ToInt32(SubArray(command_bytes,4,4),0)/2))
						{
							byte_stream	= SubArray(command_bytes, BitConverter.ToInt32(SubArray(command_bytes, 0x18+index*0x20, 4), 0), BitConverter.ToInt32(SubArray(command_bytes, 0x28+index*0x20, 4), 0) - BitConverter.ToInt32(SubArray(command_bytes, 0x18+index*0x20, 4), 0));
						}
						else
						{
							byte_stream	= SubArray(command_bytes, BitConverter.ToInt32(SubArray(command_bytes, 0x18+index*0x20, 4), 0), command_bytes.Length - BitConverter.ToInt32(SubArray(command_bytes, 0x18+index*0x20, 4), 0));
						}
	
		                //filename = System.IO.Path.GetDirectoryName(filename) + @"\" + System.IO.Path.GetFileNameWithoutExtension(filename) + ".imd";
		                filename = System.IO.Path.ChangeExtension(System.IO.Path.GetTempFileName(), Guid.NewGuid().ToString() + "imd");
		                imd_name = filename;
		                System.IO.File.WriteAllBytes(filename, byte_stream);
		            	file = System.IO.File.Open(filename, System.IO.FileMode.Open, reado ? System.IO.FileAccess.Read : System.IO.FileAccess.ReadWrite, System.IO.FileShare.None);
            		}
	            }
            Cleanup();
            ResetCrops();
            }
            catch
            {
                throw new NotSupportedException("Unable to access the file: " + System.IO.Path.GetFileName(filename));
            }
            
            try
            {
                Type type;
                string ext = ".imd";
                if (!this.typeLoaders.TryGetValue(ext, out type))
                {
                    throw new NotSupportedException("Unknown filetype: " + ext);
                }
                img = (ImageContainer)Activator.CreateInstance(type, file);
            }
            catch (Exception e)
            {
                file.Close();
                img = null;
                ShowError(e, "Fatal error opening file");
                return;
            }
            try { img.parse(); }
            catch (Exception e)
            {
                img.Dispose();
                img = null;
                ShowError(e, "Fatal error parsing file");
                return;
            }
            imgName = System.IO.Path.GetFileNameWithoutExtension(command_original_filename);
            if (img.imageCount > 0)
            {
            	button1.Enabled = true;
            	 pictureBoxBMP.Enabled = groupBox1.Enabled = groupBox4.Enabled = groupBox2.Enabled = groupBox3.Enabled = true; 
                this.buttonSave.Enabled = true;
                this.buttonReplace.Enabled = !reado && file.CanWrite;if (img != null && img.imageCount>0)
	            if (this.pictureBoxBMP.Image != null) { this.pictureBoxBMP.Image.Dispose(); }
                Bitmap b = img.getBMP(0);
                // GDI+ doesn't like RAW 4\8-bit palettes, but converts them to 32-bit on load. Make sure ours is 32-bit (since we generate them ourself)
                this.pictureBoxBMP.Image = new Bitmap(b);
                this.pictureBoxBMP.Width = b.Width;
                this.pictureBoxBMP.Height = b.Height;
                base_crops.Clear();
                base_crops_backup.Clear();
                stretches_indexes.Clear();
                int start = BitConverter.ToInt32(SubArray(command_bytes, 0x28+index*0x20, 4), 0);
                for (int i = start+0x34;i<start+0x34+0x2c*BitConverter.ToInt32(SubArray(command_bytes, start+8, 4), 0);i=i+0x2c)
                {
                	base_crops.Add(new int[] {BitConverter.ToInt32(SubArray(command_bytes, i, 4), 0),BitConverter.ToInt32(SubArray(command_bytes, i+4, 4), 0),BitConverter.ToInt32(SubArray(command_bytes, i+8, 4), 0),BitConverter.ToInt32(SubArray(command_bytes, i+12, 4), 0)});
                	base_crops_backup.Add(new int[] {BitConverter.ToInt32(SubArray(command_bytes, i, 4), 0),BitConverter.ToInt32(SubArray(command_bytes, i+4, 4), 0),BitConverter.ToInt32(SubArray(command_bytes, i+8, 4), 0),BitConverter.ToInt32(SubArray(command_bytes, i+12, 4), 0)});
               	}
                numericUpDown1.Maximum = base_crops.Count-1;
                x_pos.Maximum = width_.Maximum = this.pictureBoxBMP.Width;
                y_pos.Maximum = height_.Maximum = this.pictureBoxBMP.Height;
                num_index = (int)numericUpDown1.Value;
	        	this.pictureBoxBMP.Image = DrawRectangle_(new Bitmap(img.getBMP(0)),
	        	                                          base_crops[num_index][0]
	        	                                          ,base_crops[num_index][1]
	        	                                          ,base_crops[num_index][2]-base_crops[num_index][0]
	        	                                          ,base_crops[num_index][3]-base_crops[num_index][1]);
        		x_pos.Value = base_crops[num_index][0];
	        	y_pos.Value = base_crops[num_index][1];
	        	width_.Value = base_crops[num_index][2]-base_crops[num_index][0];
	        	height_.Value = base_crops[num_index][3]-base_crops[num_index][1];
                int start_stretches = start + 48+0x2C*BitConverter.ToInt32(SubArray(command_bytes, start+8, 4), 0);
                int start_stretches_number = BitConverter.ToInt32(SubArray(command_bytes, start+0x10, 4), 0);
                stretches.Clear();
                stretches_backup.Clear();
                
                for (int i = start_stretches;i<start_stretches+(start_stretches_number*20);i=i+20)
                {
                	int index__ = BitConverter.ToInt32(SubArray(command_bytes,i+16, 4), 0);
                	
                	if (index__ >= 0 & index__ <=start_stretches_number)
                	{
                		stretches_indexes.Add(index__);
                		stretches.Add(new int[] {BitConverter.ToInt32(SubArray(command_bytes,i, 4),0),
            			BitConverter.ToInt32(SubArray(command_bytes,i+4, 4),0),
            			BitConverter.ToInt32(SubArray(command_bytes,i+8, 4),0),
            			BitConverter.ToInt32(SubArray(command_bytes,i+12, 4),0)});
            			stretches_backup.Add(new int[] {BitConverter.ToInt32(SubArray(command_bytes,i, 4),0),
            			BitConverter.ToInt32(SubArray(command_bytes,i+4, 4),0),
            			BitConverter.ToInt32(SubArray(command_bytes,i+8, 4),0),
            			BitConverter.ToInt32(SubArray(command_bytes,i+12, 4),0)});
               		}
                }
                
	        	numericUpDown6.Value =0;
				int stretchcount = 0;
				for (int i=0;i<stretches_indexes.Count;i++)
				{
					if (stretches_indexes[i]==num_index)
					{
						stretchcount++;
					}
				}
				if (stretchcount>0)
				{
	        		numericUpDown6.Maximum = stretchcount-1;
				}
				else
				{
					numericUpDown6.Maximum = 0;
				}
	        	starting_timer.Enabled =true;
            }
        }
        
        List<int[]> stretches = new List<int[]>(0);
        List<int> stretches_indexes = new List<int>(0);
        List<int[]> stretches_backup = new List<int[]>(0);
        void ResetCrops()
        {
        	numericUpDown1.Value = x_pos.Value = y_pos.Value = width_.Value = height_.Value =0;
        	textBox1.Text = "0;0;0;0";
        }
        List<int[]> base_crops = new List<int[]>(0);
        List<int[]> base_crops_backup = new List<int[]>(0);
        private void saveBMP(int index, string filename)
        {
            if (img != null && index < img.imageCount)
            {
                img.getBMP(index).Save(filename, System.Drawing.Imaging.ImageFormat.Png);
            }
        }
        private void replaceBMP(int index, string filename)
        {
            if (img != null && index < img.imageCount)
            {
                //This way we don't tie up the image file.
                //You must keep the stream open for the lifetime of the Bitmap.
                img.setBMP(0, new Bitmap(new System.IO.MemoryStream(System.IO.File.ReadAllBytes(filename))));
            }
        }


        /// <summary>Open file button</summary>
        private void buttonOpen_Click(object sender, EventArgs e) { try {file.Close();} catch {}openFileDialog.Filter = "2DD Files|*.2dd"; openFileDialog.ShowReadOnly = true; openFileDialog.FileName = ""; openFileDialog.DefaultExt = ""; if (openFileDialog.ShowDialog() == DialogResult.OK) { openFile(openFileDialog.FileName, openFileDialog.ReadOnlyChecked); } }
        /// <summary>Change mouse icon on drag+drop</summary>
        private void buttonOpen_DragEnter(object sender, DragEventArgs e) { if (e.Data.GetDataPresent(DataFormats.FileDrop)) { e.Effect = DragDropEffects.Copy; } else { e.Effect = DragDropEffects.None; } }
        /// <summary>Change mouse icon on drag+drop</summary>
        private void buttonOpen_DragDrop(object sender, DragEventArgs e) { if (e.Data.GetDataPresent(DataFormats.FileDrop)) { string[] filePaths = (string[])(e.Data.GetData(DataFormats.FileDrop)); foreach (string fileLoc in filePaths) { if (System.IO.File.Exists(fileLoc)) { openFile(fileLoc); break; } } } }
        /// <summary>Save all loaded images</summary>
        
        /// <summary>Save single selected image</summary>
        private void buttonSave_Click(object sender, EventArgs e)
        {
            int index = 0;
            saveFileDialog.FileName = imgName +  ".png";
            if (index >= 0 && img != null && index < img.imageCount && saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                saveBMP(index, saveFileDialog.FileName);
            }
        }

        private void buttonReplace_Click(object sender, EventArgs e)
        {
            openFileDialog.FileName = ""; openFileDialog.Filter = "PNG image file|*.png"; openFileDialog.DefaultExt = "png"; openFileDialog.ShowReadOnly = false;
            if (img != null && img.imageCount>0 && openFileDialog.ShowDialog() == DialogResult.OK)
            {
                replaceBMP(0, openFileDialog.FileName);
                file.Close();
                byte [] new_imd = System.IO.File.ReadAllBytes(imd_name);
                command_bytes = WriteArray(command_bytes,BitConverter.ToInt32(SubArray(command_bytes, 0x18, 4), 0),new_imd);
                System.IO.File.WriteAllBytes(command_original_filename,command_bytes);
            	file = System.IO.File.Open(imd_name, System.IO.FileMode.Open, false ? System.IO.FileAccess.Read : System.IO.FileAccess.ReadWrite, System.IO.FileShare.None);
            	if (this.pictureBoxBMP.Image != null) { this.pictureBoxBMP.Image.Dispose(); }
                Bitmap b = img.getBMP(0);
                this.pictureBoxBMP.Image = new Bitmap(b);
                this.pictureBoxBMP.Width = b.Width;
                this.pictureBoxBMP.Height = b.Height;
                num_index = -1;
            }
        }
        bool mouse_down = false;
        int[] poses = new int[] {0,0,0,0,0,0,0,0};
        int type = 0;
        int[] resize_all_diff = new int[] {0,0};
        int count= 0;
        void PictureBoxBMPMouseDown(object sender, MouseEventArgs e)
        {
        	count++;
        	if (mouse_down==false)
        	{
        		if (this.pictureBoxBMP.Cursor == System.Windows.Forms.Cursors.Default)
        		{
        			poses[0] = pictureBoxBMP.PointToClient(Control.MousePosition).X;
        			poses[1] = pictureBoxBMP.PointToClient(Control.MousePosition).Y;
        		}
        		else
        		{
        			if (type==9)
        			{
        				resize_all_diff[0]=pictureBoxBMP.PointToClient(Control.MousePosition).X-(int)x_pos.Value;
        				resize_all_diff[1]=pictureBoxBMP.PointToClient(Control.MousePosition).Y-(int)y_pos.Value;
        			}
        				if (type==4|type==6|type==8)
    					{
    						poses[0] = (int)x_pos.Value;
        					poses[1] = (int)y_pos.Value;
	        				if (type==6)
	        				{
        						Cursor.Position = new Point(pictureBoxBMP.PointToScreen(Point.Empty).X+(int)x_pos.Value+(int)width_.Value,Cursor.Position.Y);
	        				}
	        				else if (type==8)
	        				{
        						Cursor.Position = new Point(Cursor.Position.X,pictureBoxBMP.PointToScreen(Point.Empty).Y+(int)y_pos.Value+(int)height_.Value);
	        				}
	        				else
	        				{
        						Cursor.Position = new Point(pictureBoxBMP.PointToScreen(Point.Empty).X+(int)x_pos.Value+(int)width_.Value,pictureBoxBMP.PointToScreen(Point.Empty).Y+(int)y_pos.Value+(int)height_.Value);
	        				}
    					}
	        			if (type==5|type==3)
        				{
	        				poses[0] = (int)x_pos.Value+(int)width_.Value;
	        				poses[1] = (int)y_pos.Value;
	        				if (type==5)
	        				{
	        					Cursor.Position = new Point(pictureBoxBMP.PointToScreen(Point.Empty).X+(int)x_pos.Value,Cursor.Position.Y);        				
	        				}
	        				else
	        				{
	        					Cursor.Position = new Point(pictureBoxBMP.PointToScreen(Point.Empty).X+(int)x_pos.Value,pictureBoxBMP.PointToScreen(Point.Empty).Y+(int)y_pos.Value+(int)height_.Value);        				
	        				}
        					
						}
	        			if (type==2|type==7)
        				{
	        				poses[0] = (int)x_pos.Value;
	        				poses[1] = (int)y_pos.Value+(int)height_.Value;
	        				if (type==7)
	        				{
	        					Cursor.Position = new Point(Cursor.Position.X,pictureBoxBMP.PointToScreen(Point.Empty).Y+(int)y_pos.Value);
	        				}
	        				else
	        				{
	        					Cursor.Position = new Point(pictureBoxBMP.PointToScreen(Point.Empty).X+(int)x_pos.Value+(int)width_.Value,pictureBoxBMP.PointToScreen(Point.Empty).Y+(int)y_pos.Value);
	        				}
	        				
	        			}
	        			if (type==1)
        				{
	        				poses[0] = (int)x_pos.Value+(int)width_.Value;
	        				poses[1] = (int)y_pos.Value+(int)height_.Value;
	        				Cursor.Position = new Point(pictureBoxBMP.PointToScreen(Point.Empty).X+(int)x_pos.Value,pictureBoxBMP.PointToScreen(Point.Empty).Y+(int)y_pos.Value);
	        			}
        			
        		}
        		mouse_down = true;
        	}
        }
        
        void FormMainDeactivate(object sender, EventArgs e)
        {
        	mouse_down = false;
        }
        Pen RedPen= new Pen(Color.Red, 1);
        Bitmap DrawRectangle_(Bitmap input ,int x,int y,int width,int height)
        {
        	Bitmap to_return = input;
        	Graphics g = Graphics.FromImage(to_return);
        	if (width==1|height==1)
        	{
        		if (width==1)
        		{
        			g.DrawLine(RedPen, x, y, x, y+height);
        		}
        		else
        		{
        			g.DrawLine(RedPen, x, y, x+width, y);
        		}
        	}
        	else
        	{
        		g.DrawRectangle(RedPen, x, y, width-1, height-1);
        	}
           
            return to_return;
        }
        
        void PictureBoxBMPMouseUp(object sender, MouseEventArgs e)
        {
        	mouse_down = false;
        }
        
        void PictureBoxBMPMouseMove(object sender, MouseEventArgs e)
        {
        	if (mouse_down)
        	{
        		if (type!=9)
        		{
        			if (type!=5&type!=6)
	        		{
	        			poses[3] = pictureBoxBMP.PointToClient(Control.MousePosition).Y;
	        		}
	        		else
	        		{
	        			poses[3] = (int)y_pos.Value+(int)height_.Value;
	        		}
	        		if (type!=7&type!=8)
	        		{
	        			poses[2] = pictureBoxBMP.PointToClient(Control.MousePosition).X;
	        		}
	        		else
	        		{
	        			poses[2] = (int)x_pos.Value+(int)width_.Value;
	        		}
		        	if (poses[2]>pictureBoxBMP.Width) {poses[2] = pictureBoxBMP.Width;}
		        	if (poses[2]<0) {poses[2] = 0;}
		        	if (poses[3]>pictureBoxBMP.Height) {poses[3] = pictureBoxBMP.Height;}
		        	if (poses[3]<0) {poses[3] = 0;}
		        	
	        		if (poses[2]>poses[0])
	        		{
	        			poses[6] = poses[2]-poses[0];
	        			poses[4] = poses[0];
	        		}
	        		else
	        		{
	        			poses[6] = poses[0]-poses[2];
	        			poses[4] = poses[2];
	        		}
	        		if (poses[3]>poses[1])
	        		{
	        			poses[7] = poses[3]-poses[1];
	        			poses[5] = poses[1];
	        		}
	        		else
	        		{
	        			poses[7] = poses[1]-poses[3];
	        			poses[5] = poses[3];
	        		}
		        	if (poses[4]+poses[6]>pictureBoxBMP.Width) {poses[6]=pictureBoxBMP.Width-poses[4];}
		        	if (poses[5]+poses[7]>pictureBoxBMP.Height) {poses[7]=pictureBoxBMP.Height-poses[5];}
	        		x_pos.Value = poses[4];
	        		y_pos.Value = poses[5];
	        		width_.Value = poses[6];
	        		height_.Value = poses[7];
	        		Refreshstretch(poses[4],poses[5],poses[6],poses[7]);
        		
	        		this.pictureBoxBMP.Image = DrawRectangle_(new Bitmap(img.getBMP(0)),poses[4],poses[5],poses[6],poses[7]);
        		}
        		else
        		{
        			int x_resizeall = pictureBoxBMP.PointToClient(Control.MousePosition).X-resize_all_diff[0];
        			int y_resizeall = pictureBoxBMP.PointToClient(Control.MousePosition).Y-resize_all_diff[1];
		        	if (x_resizeall+(int)width_.Value>pictureBoxBMP.Width) {x_resizeall = pictureBoxBMP.Width-(int)width_.Value;}
		        	if (x_resizeall<0) {x_resizeall = 0;}
		        	if (y_resizeall+(int)height_.Value>pictureBoxBMP.Height) {y_resizeall = pictureBoxBMP.Height-(int)height_.Value;}
		        	if (y_resizeall<0) {y_resizeall = 0;}
        			x_pos.Value = x_resizeall;
	        		y_pos.Value = y_resizeall;
        			this.pictureBoxBMP.Image = DrawRectangle_(new Bitmap(img.getBMP(0)),x_resizeall,y_resizeall,(int)width_.Value,(int)height_.Value);
	        		Refreshstretch(x_resizeall,y_resizeall,(int)width_.Value,(int)height_.Value);
        		}
        		
        	}
        	else if (groupBox2.Enabled)
        	{
        		if (TestAverage(pictureBoxBMP.PointToClient(Control.MousePosition).X,pictureBoxBMP.PointToClient(Control.MousePosition).Y,x_pos.Value,y_pos.Value,4))
        		{
        			this.pictureBoxBMP.Cursor = System.Windows.Forms.Cursors.SizeNWSE;
        			type = 1;
        		}
        		else if (TestAverage(pictureBoxBMP.PointToClient(Control.MousePosition).X,pictureBoxBMP.PointToClient(Control.MousePosition).Y,x_pos.Value+width_.Value,y_pos.Value,4))
        		{
        			this.pictureBoxBMP.Cursor = System.Windows.Forms.Cursors.SizeNESW;
        			type = 2;
        		}
        		else if (TestAverage(pictureBoxBMP.PointToClient(Control.MousePosition).X,pictureBoxBMP.PointToClient(Control.MousePosition).Y,x_pos.Value,y_pos.Value+height_.Value,4))
        		{
        			this.pictureBoxBMP.Cursor = System.Windows.Forms.Cursors.SizeNESW;
        			type = 3;
        		}
        		else if (TestAverage(pictureBoxBMP.PointToClient(Control.MousePosition).X,pictureBoxBMP.PointToClient(Control.MousePosition).Y,x_pos.Value+width_.Value,y_pos.Value+height_.Value,4))
        		{
        			this.pictureBoxBMP.Cursor = System.Windows.Forms.Cursors.SizeNWSE;
        			type = 4;
        		}
        		else if (pictureBoxBMP.PointToClient(Control.MousePosition).Y > y_pos.Value&pictureBoxBMP.PointToClient(Control.MousePosition).Y < y_pos.Value+height_.Value&TestAverage(pictureBoxBMP.PointToClient(Control.MousePosition).X,(int)y_pos.Value+(int)height_.Value,x_pos.Value,y_pos.Value+height_.Value,4))
        		{
        			this.pictureBoxBMP.Cursor = System.Windows.Forms.Cursors.SizeWE;
        			type = 5;
        		}
        		else if (pictureBoxBMP.PointToClient(Control.MousePosition).Y > y_pos.Value&pictureBoxBMP.PointToClient(Control.MousePosition).Y < y_pos.Value+height_.Value&TestAverage(pictureBoxBMP.PointToClient(Control.MousePosition).X,(int)y_pos.Value+(int)height_.Value,x_pos.Value+width_.Value,y_pos.Value+height_.Value,4))
        		{
        			this.pictureBoxBMP.Cursor = System.Windows.Forms.Cursors.SizeWE;
        			type = 6;
        		}
        		else if (pictureBoxBMP.PointToClient(Control.MousePosition).X > x_pos.Value&pictureBoxBMP.PointToClient(Control.MousePosition).X < x_pos.Value+width_.Value&TestAverage((int)x_pos.Value,pictureBoxBMP.PointToClient(Control.MousePosition).Y,x_pos.Value,y_pos.Value,4))
        		{
        			this.pictureBoxBMP.Cursor = System.Windows.Forms.Cursors.SizeNS;
        			type = 7;
        		}
        		else if (pictureBoxBMP.PointToClient(Control.MousePosition).X > x_pos.Value&pictureBoxBMP.PointToClient(Control.MousePosition).X < x_pos.Value+width_.Value&TestAverage((int)x_pos.Value,pictureBoxBMP.PointToClient(Control.MousePosition).Y,x_pos.Value,y_pos.Value+height_.Value,4))
        		{
        			this.pictureBoxBMP.Cursor = System.Windows.Forms.Cursors.SizeNS;
        			type = 8;
        		}
        		else if (pictureBoxBMP.PointToClient(Control.MousePosition).X > x_pos.Value
        		        & pictureBoxBMP.PointToClient(Control.MousePosition).X < x_pos.Value+width_.Value
        		       &pictureBoxBMP.PointToClient(Control.MousePosition).Y > y_pos.Value
        		        & pictureBoxBMP.PointToClient(Control.MousePosition).Y < y_pos.Value+height_.Value)
        		{
        			this.pictureBoxBMP.Cursor = System.Windows.Forms.Cursors.SizeAll;
        			type = 9;
        		}
        		else
        		{
        			this.pictureBoxBMP.Cursor = System.Windows.Forms.Cursors.Default;
        			type = 0;
        		}
        	}
        }
        
        bool TestAverage(int x_match, int y_match, decimal x, decimal y,int accuracy)
        {
        	return (x_match>(int)x-accuracy&x_match<(int)x+accuracy&y_match>(int)y-accuracy&y_match<(int)y+accuracy);
        }
        
        int num_index = -1;
        
        void NumericUpDown1ValueChanged(object sender, EventArgs e)
        {
        	try
        	{
	        	if (num_index>-1)
	        	{
		        	base_crops[num_index][0] = (int)x_pos.Value;
				    base_crops[num_index][1] = (int)y_pos.Value;
				    base_crops[num_index][2] = (int)width_.Value+(int)x_pos.Value;
				    base_crops[num_index][3] = (int)height_.Value+(int)y_pos.Value;
	        	}
	        	num_index = (int)numericUpDown1.Value;
	        	this.pictureBoxBMP.Image = DrawRectangle_(new Bitmap(img.getBMP(0)),
	        	                                          base_crops[num_index][0]
	        	                                          ,base_crops[num_index][1]
	        	                                          ,base_crops[num_index][2]-base_crops[num_index][0]
	        	                                          ,base_crops[num_index][3]-base_crops[num_index][1]);
	        	if (sender!=null)
	        	{
	        		numericUpDown6.Value =0;
		        	Refreshstretch(base_crops[num_index][0],base_crops[num_index][1],base_crops[num_index][2]-base_crops[num_index][0],base_crops[num_index][3]-base_crops[num_index][1]);
		        	x_pos.Value = base_crops[num_index][0];
		        	y_pos.Value = base_crops[num_index][1];
		        	width_.Value = base_crops[num_index][2]-base_crops[num_index][0];
		        	height_.Value = base_crops[num_index][3]-base_crops[num_index][1];
				int stretchcount = 0;
				for (int i=0;i<stretches_indexes.Count;i++)
				{
					if (stretches_indexes[i]==num_index)
					{
						stretchcount++;
					}
				}
				if (stretchcount>0)
				{
	        		numericUpDown6.Maximum = stretchcount-1;
				}
				else
				{
					numericUpDown6.Maximum = 0;
				}
	        	NumericUpDown6ValueChanged(null,null);
	        	}
        	}
        	catch
        	{
        		try
	        	{
		        	if (num_index>-1)
		        	{
			        	base_crops[num_index][0] = (int)x_pos.Value;
					    base_crops[num_index][1] = (int)y_pos.Value;
					    base_crops[num_index][2] = (int)width_.Value+(int)x_pos.Value;
					    base_crops[num_index][3] = (int)height_.Value+(int)y_pos.Value;
		        	}
		        	num_index = (int)numericUpDown1.Value;
		        	this.pictureBoxBMP.Image = DrawRectangle_(new Bitmap(img.getBMP(0)),
		        	                                          base_crops[num_index][0]
		        	                                          ,base_crops[num_index][1]
		        	                                          ,base_crops[num_index][2]-base_crops[num_index][0]
		        	                                          ,base_crops[num_index][3]-base_crops[num_index][1]);
		        	Refreshstretch(base_crops[num_index][0],base_crops[num_index][1],base_crops[num_index][2]-base_crops[num_index][0],base_crops[num_index][3]-base_crops[num_index][1]);
		        	x_pos.Value = base_crops[num_index][0];
		        	y_pos.Value = base_crops[num_index][1];
		        	width_.Value = base_crops[num_index][2]-base_crops[num_index][0];
		        	height_.Value = base_crops[num_index][3]-base_crops[num_index][1];
	        	}
	        	catch
	        	{
	        		
	        	}
	        }
        }
        
        void Refreshstretch(int a_,int b_,int c_, int d_)
        {
        	try
        	{
        		Rectangle cropRect = new Rectangle(a_,b_,c_,d_);
				Bitmap target = new Bitmap(cropRect.Width, cropRect.Height);
				Graphics.FromImage(target).DrawImage(new Bitmap(img.getBMP(0)), new Rectangle(0, 0, target.Width, target.Height), 
				                    cropRect,                        
				                    GraphicsUnit.Pixel);
				int stretch_location = 0;
				int first_stretchcount = 0;
				for (int i=0;i<stretches_indexes.Count;i++)
				{
					if (stretches_indexes[i]==num_index)
					{
						if (first_stretchcount==(int)numericUpDown6.Value)
						{
							stretch_location=i;
							i=5000;
						}
						first_stretchcount++;
					}
				}
        		pictureBox3.BackgroundImage = target;
        		
        		int a__=0;
        		int b__=0;
        		int c__=0;
        		int d__=0;
        		
				if (stretches[stretch_location][2]<stretches[stretch_location][0])
				{
					pictureBox3.BackgroundImage.RotateFlip(RotateFlipType.Rotate180FlipY);
					a__=stretches[stretch_location][2];
					c__=(int)(stretches[stretch_location][0]-stretches[stretch_location][2]);
				}
				else
				{
					c__=(int)(stretches[stretch_location][2]-stretches[stretch_location][0]);
					a__=stretches[stretch_location][0];
				}
				if (stretches[stretch_location][3]<stretches[stretch_location][1])
				{
					pictureBox3.BackgroundImage.RotateFlip(RotateFlipType.Rotate180FlipX);
					b__=stretches[stretch_location][3];
					d__=(int)(stretches[stretch_location][1]-stretches[stretch_location][3]);
				}
				else
				{
					d__=(int)(stretches[stretch_location][3]-stretches[stretch_location][1]);
					b__=stretches[stretch_location][1];
				}
				
				pictureBox3.Size = new Size(c__,d__);
        		pictureBox3.Location=new Point((int)(pictureBox4.Width/2)+a__,(int)(pictureBox4.Height/2)+b__);
        	}
        	catch
        	{
        		pictureBox3.Size = new Size(0,0);
        	}
        }
        void RefreshRect()
        {
        	try
	        {
        		if (mouse_down==false)
	        	{
	        		int[] poses_ =new int[] {(int)x_pos.Value,(int)y_pos.Value,(int)width_.Value,(int)height_.Value};
		        	if (poses_[2]+poses_[0]>pictureBoxBMP.Width)
		        	{
		        		poses_[2] = pictureBoxBMP.Width-poses_[0];
		        	}
		        	if (poses_[3]+poses_[1]>pictureBoxBMP.Height)
		        	{
		        		poses_[3] = pictureBoxBMP.Height-poses_[1];
		        	}
		        	x_pos.Value =poses_[0];
	        		y_pos.Value =poses_[1];
	        		width_.Value =poses_[2];
	        		height_.Value =poses_[3];
	        		this.pictureBoxBMP.Image = DrawRectangle_(new Bitmap(img.getBMP(0)),poses_[0],poses_[1],poses_[2],poses_[3]);
	        		Refreshstretch(poses_[0],poses_[1],poses_[2],poses_[3]);
        		}
        			textBox1.Text=((int)x_pos.Value).ToString()+";"+((int)y_pos.Value).ToString()+";"+((int)width_.Value).ToString()+";"+((int)height_.Value).ToString();
        	}
        	catch
        	{
        		ResetCrops();
        	}
        }
        void X_posValueChanged(object sender, EventArgs e)
        {
        	RefreshRect();
        }
        
        void Y_posValueChanged(object sender, EventArgs e)
        {
        	RefreshRect();
        }
        
        void Width_ValueChanged(object sender, EventArgs e)
        {
        	RefreshRect();
        }
        
        void Height_ValueChanged(object sender, EventArgs e)
        {
        	RefreshRect();
        }
        string old_quick_fill="0;0;0;0";
        string old_quick_fill2="0;0;0;0";
        void TextBox1TextChanged(object sender, EventArgs e)
        {
        	try
        	{
        		int[] capts = new int[] {0,0,0,0};
        		string[] values = textBox1.Text.Split(';');
        		if (values.Length > 4)
        		{
        			int.Parse("ifjnzrf");
        		}
        		x_pos.Value = int.Parse(values[0]);
        		y_pos.Value = int.Parse(values[1]);
        		width_.Value = int.Parse(values[2]);
        		height_.Value = int.Parse(values[3]);
        		old_quick_fill = textBox1.Text;
        	}
        	catch
        	{
        		
        	}
        }
        
        void TextBox1Leave(object sender, EventArgs e)
        {
        	textBox1.Text = old_quick_fill;
        }
        
        void Button2Click(object sender, EventArgs e)
        {
        	try
        	{
        		int stretch_location = 0;
				int first_stretchcount = 0;
				for (int i=0;i<stretches_indexes.Count;i++)
				{
					if (stretches_indexes[i]==num_index)
					{
						if (first_stretchcount==(int)numericUpDown6.Value)
						{
							stretch_location=i;
							i=5000;
						}
						first_stretchcount++;
					}
				}
        		stretches[stretch_location]=new int[] {stretches_backup[stretch_location][0],stretches_backup[stretch_location][1],stretches_backup[stretch_location][2],stretches_backup[stretch_location][3]};
	    	}
        	catch
        	{
        		
        	}
        	try
        	{
        		Refreshstretch(base_crops[num_index][0]
	        	                                          ,base_crops[num_index][1]
	        	                                          ,base_crops[num_index][2]-base_crops[num_index][0]
	        	                                          ,base_crops[num_index][3]-base_crops[num_index][1]);
        	}
        	catch
        	{
        		
        	}
	        	
        	x_pos.Value = base_crops_backup[num_index][0];
        	y_pos.Value = base_crops_backup[num_index][1];
        	width_.Value = base_crops_backup[num_index][2]-base_crops_backup[num_index][0];
        	height_.Value = base_crops_backup[num_index][3]-base_crops_backup[num_index][1];
        }
        
        void Button1Click(object sender, EventArgs e)
        {
        	try
        	{
	        	NumericUpDown1ValueChanged(null,null);
	        	int index_ = 0;
	        	int start = BitConverter.ToInt32(SubArray(command_bytes, 0x28+index*0x20, 4), 0);
	            for (int i = start+0x34;i<start+0x34+0x2c*BitConverter.ToInt32(SubArray(command_bytes, start+8, 4), 0);i=i+0x2c)
	            {
        			command_bytes = WriteArray(command_bytes,i,BitConverter.GetBytes(base_crops[index_][0]));
	        		command_bytes = WriteArray(command_bytes,i+4,BitConverter.GetBytes(base_crops[index_][1]));
	        		command_bytes = WriteArray(command_bytes,i+8,BitConverter.GetBytes(base_crops[index_][2]));
	        		command_bytes = WriteArray(command_bytes,i+12,BitConverter.GetBytes(base_crops[index_][3]));
	        		index_++;
	           }
	            
	            int start_stretches = start + 48+0x2C*BitConverter.ToInt32(SubArray(command_bytes, start+8, 4), 0);
                int start_stretches_number = BitConverter.ToInt32(SubArray(command_bytes, start+0x10, 4), 0);
                int simple_index =0;
                for (int i = start_stretches;i<start_stretches+start_stretches_number*20;i=i+20)
                {
                	int index__ = BitConverter.ToInt32(SubArray(command_bytes,i+16, 4), 0);
                	
                	if (index__ >= 0 & index__ <=start_stretches_number)
                	{
	        			byte[] to_write_in_file = Combine(Combine(BitConverter.GetBytes(stretches[simple_index][0]),BitConverter.GetBytes(stretches[simple_index][1])),Combine(BitConverter.GetBytes(stretches[simple_index][2]),BitConverter.GetBytes(stretches[simple_index][3])));
                		command_bytes = WriteArray(command_bytes,i,to_write_in_file);
	    				simple_index++;
               		}
                }
	       }
        	catch
        	{
        		
        	}
        	
        	saveFileDialog1.FileName = imgName +  ".2dd";
        	if (saveFileDialog1.ShowDialog() == DialogResult.OK)
        	{
        		try
        		{
        			System.IO.File.WriteAllBytes(saveFileDialog1.FileName,command_bytes);
        		}
        		catch
        		{
        			MessageBox.Show("Unable to access the file.\r\nIt's maybe used by another process.");
        		}
        	}
        }
        
        void Button3Click(object sender, EventArgs e)
        {
        	//
        	int command_address = 0;
        	if (imgName.Contains("field"))
    		{
    			command_address=0x203B8BC0;
    		}
    		else
    		{
    			command_address=0x20000000+BitConverter.ToInt32(ReadBytes(PCSX,0x20347FD8,4),0);
    		}
        	try
        	{
	        	NumericUpDown1ValueChanged(null,null);
	        	int index_ = 0;
	        	int start = BitConverter.ToInt32(SubArray(command_bytes, 0x28+index*0x20, 4), 0);
	            for (int i = start+0x34;i<start+0x34+0x2c*BitConverter.ToInt32(SubArray(command_bytes, start+8, 4), 0);i=i+0x2c)
	            {
	             	WriteBytes(PCSX,i+command_address,BitConverter.GetBytes(base_crops[index_][0]),BitConverter.GetBytes(base_crops[index_][0]).Length);
        			command_bytes = WriteArray(command_bytes,i,BitConverter.GetBytes(base_crops[index_][0]));
	             	WriteBytes(PCSX,i+4+command_address,BitConverter.GetBytes(base_crops[index_][1]),BitConverter.GetBytes(base_crops[index_][1]).Length);
	        		command_bytes = WriteArray(command_bytes,i+4,BitConverter.GetBytes(base_crops[index_][1]));
	             	WriteBytes(PCSX,i+8+command_address,BitConverter.GetBytes(base_crops[index_][2]),BitConverter.GetBytes(base_crops[index_][2]).Length);
	        		command_bytes = WriteArray(command_bytes,i+8,BitConverter.GetBytes(base_crops[index_][2]));
	             	WriteBytes(PCSX,i+12+command_address,BitConverter.GetBytes(base_crops[index_][3]),BitConverter.GetBytes(base_crops[index_][3]).Length);
	        		command_bytes = WriteArray(command_bytes,i+12,BitConverter.GetBytes(base_crops[index_][3]));
	        		index_++;
	           }
	            
	            int start_stretches = start + 48+0x2C*BitConverter.ToInt32(SubArray(command_bytes, start+8, 4), 0);
                int start_stretches_number = BitConverter.ToInt32(SubArray(command_bytes, start+0x10, 4), 0);
                int simple_index =0;
                for (int i = start_stretches;i<start_stretches+start_stretches_number*20;i=i+20)
                {
                	int index__ = BitConverter.ToInt32(SubArray(command_bytes,i+16, 4), 0);
                	
                	if (index__ >= 0 & index__ <=start_stretches_number)
                	{
	        			byte[] to_write_in_file = Combine(Combine(BitConverter.GetBytes(stretches[simple_index][0]),BitConverter.GetBytes(stretches[simple_index][1])),Combine(BitConverter.GetBytes(stretches[simple_index][2]),BitConverter.GetBytes(stretches[simple_index][3])));
                		command_bytes = WriteArray(command_bytes,i,to_write_in_file);
	    				WriteBytes(PCSX,i+command_address,to_write_in_file,to_write_in_file.Length);
	    				simple_index++;
               		}
                }
	            WriteBytes(PCSX,0x20000000+BitConverter.ToInt32(ReadBytes(PCSX,(command_address+0x18+index*0x20),4),0),SubArray(command_bytes,BitConverter.ToInt32(SubArray(command_bytes, 0x18+index*0x20, 4), 0),BitConverter.ToInt32(SubArray(command_bytes, 0x1C+index*0x20, 4), 0)),BitConverter.ToInt32(SubArray(command_bytes, 0x1C+index*0x20, 4), 0));
        	}
        	catch
        	{
        		
        	}
        }
        
        Process PCSX = new Process();
        void Timer1Tick(object sender, EventArgs e)
        {
			try
			{
				Process.GetProcessById(PCSX.Id);
			}
			catch
			{
				button3.Enabled = false;
				bool ok_ = false;
				foreach (Process pcsx_search in Process.GetProcesses())
				{
					Application.DoEvents();
					try
					{

						if (pcsx_search.ProcessName.ToString().Contains("pcsx2"))
						{
							button3.Enabled = true;
							ok_ = true;
							label2.Text = "Linked to [" + pcsx_search.ProcessName.ToString() + "]";
							PCSX = pcsx_search;
						}
					}
					catch(Exception er)
					{
						Debug.Print("Can't check if pcsx2 is there! Might be running on mono linux {0}", er);
					}
				}
				if (ok_ ==false)
				{
					label2.Text = "Please open PCSX2...";
				}
			}
        }
        bool switch_=false;
        void Button4Click(object sender, EventArgs e)
        {
        	if (switch_)
        	{
        		pictureBoxBMP.BackgroundImage = pictureBox1.BackgroundImage;
        		pictureBox4.BackgroundImage = pictureBox1.BackgroundImage;
        		RedPen = new Pen(Color.Red, 1);
        		button4.Text = "Dark background";
        	}
        	else
        	{
        		pictureBoxBMP.BackgroundImage = pictureBox2.BackgroundImage;
        		pictureBox4.BackgroundImage = pictureBox2.BackgroundImage;
        		RedPen = new Pen(Color.White, 1);
        		button4.Text = "Bright background";
        	}
        	switch_=switch_==false;
                NumericUpDown1ValueChanged(null,null);
        }
        
        
        void Timer2Tick(object sender, EventArgs e)
        {
        	try
        	{
        	}
        	catch
        	{
        		
        	}
        	pictureBox4.Location = new Point(160,pictureBoxBMP.Location.Y+pictureBoxBMP.Height+12);
        	pictureBox4.Size = pictureBoxBMP.Size;
        }
        
        void NumericUpDown6ValueChanged(object sender, EventArgs e)
        {
        	
        	int stretch_location = 0;
			int first_stretchcount = 0;
			for (int i=0;i<stretches_indexes.Count;i++)
			{
				if (stretches_indexes[i]==num_index)
				{
					if (first_stretchcount==(int)numericUpDown6.Value)
					{
						stretch_location=i;
						i=5000;
					}
					first_stretchcount++;
				}
			}
			textBox2.Text = stretches[stretch_location][0].ToString()+";"+
			stretches[stretch_location][1].ToString()+";"+
			stretches[stretch_location][2].ToString()+";"+
			stretches[stretch_location][3].ToString();
			
        	if (left.Value<right.Value)
        	{
        		left_right=(int)(Math.Abs(left.Value)+Math.Abs(right.Value));
        	}
        	else
        	{
        		left_right=-((int)(Math.Abs(left.Value)+Math.Abs(right.Value)));
        	}
        	if (up.Value<down.Value)
        	{
        		up_down=(int)(Math.Abs(up.Value)+Math.Abs(down.Value));
        	}
        	else
        	{
        		up_down=-((int)(Math.Abs(up.Value)+Math.Abs(down.Value)));
        	}
        }
        
        void TextBox2TextChanged(object sender, EventArgs e)
        {
        	try
        	{
    		int[] capts = new int[] {0,0,0,0};
    		string[] values = textBox2.Text.Split(';');
    		if (values.Length > 4)
    		{
    			int.Parse("ifjnzrf");
    		}
        	int stretch_location = 0;
			int first_stretchcount = 0;
			for (int i=0;i<stretches_indexes.Count;i++)
			{
				if (stretches_indexes[i]==num_index)
				{
					if (first_stretchcount==(int)numericUpDown6.Value)
					{
						stretch_location=i;
						i=5000;
					}
					first_stretchcount++;
				}
			}
        		stretches[stretch_location][0] = int.Parse(values[0]);
        		stretches[stretch_location][1] = int.Parse(values[1]);
        		stretches[stretch_location][2] = int.Parse(values[2]);
        		stretches[stretch_location][3] = int.Parse(values[3]);
        		left.Value = int.Parse(values[0]);
        		up.Value = int.Parse(values[1]);
        		right.Value = int.Parse(values[2]);
        		down.Value = int.Parse(values[3]);
        		old_quick_fill2 = textBox2.Text;
	        	Refreshstretch(base_crops[num_index][0]
	        	                                          ,base_crops[num_index][1]
	        	                                          ,base_crops[num_index][2]-base_crops[num_index][0]
	        	                                          ,base_crops[num_index][3]-base_crops[num_index][1]);
        		RefreshRect();
            }
        	catch
        	{
        		
        	}
        }
        
        void TextBox2Leave(object sender, EventArgs e)
        {
        	textBox2.Text = old_quick_fill2;
        }
        
        void Button6Click(object sender, EventArgs e)
        {
        	int stretch_location = 0;
			int first_stretchcount = 0;
			for (int i=0;i<stretches_indexes.Count;i++)
			{
				if (stretches_indexes[i]==num_index)
				{
					if (first_stretchcount==(int)numericUpDown6.Value)
					{
						stretch_location=i;
						i=5000;
					}
					first_stretchcount++;
				}
			}
        	textBox2.Text = stretches_backup[stretch_location][0].ToString()+";"+
			stretches_backup[stretch_location][1].ToString()+";"+
			stretches_backup[stretch_location][2].ToString()+";"+
			stretches_backup[stretch_location][3].ToString();
        }
        
        void LeftValueChanged(object sender, EventArgs e)
        {
        	int stretch_location = 0;
			int first_stretchcount = 0;
			for (int i=0;i<stretches_indexes.Count;i++)
			{
				if (stretches_indexes[i]==num_index)
				{
					if (first_stretchcount==(int)numericUpDown6.Value)
					{
						stretch_location=i;
						i=5000;
					}
					first_stretchcount++;
				}
			}
        	textBox2.Text = left.Value.ToString()+";"+
			stretches[stretch_location][1].ToString()+";"+
			stretches[stretch_location][2].ToString()+";"+
			stretches[stretch_location][3].ToString();
        	if (cadenas==-1)
        	{
        		right.Value=left.Value+left_right;
        	}
        }
        
        void UpValueChanged(object sender, EventArgs e)
        {
        	int stretch_location = 0;
			int first_stretchcount = 0;
			for (int i=0;i<stretches_indexes.Count;i++)
			{
				if (stretches_indexes[i]==num_index)
				{
					if (first_stretchcount==(int)numericUpDown6.Value)
					{
						stretch_location=i;
						i=5000;
					}
					first_stretchcount++;
				}
			}
        	textBox2.Text = stretches[stretch_location][0].ToString()+";"+
			up.Value.ToString()+";"+
			stretches[stretch_location][2].ToString()+";"+
			stretches[stretch_location][3].ToString();
        	if (cadenas==-1)
        	{
        		down.Value=up.Value+up_down;
        	}
        }
        
        void RightValueChanged(object sender, EventArgs e)
        {
        	int stretch_location = 0;
			int first_stretchcount = 0;
			for (int i=0;i<stretches_indexes.Count;i++)
			{
				if (stretches_indexes[i]==num_index)
				{
					if (first_stretchcount==(int)numericUpDown6.Value)
					{
						stretch_location=i;
						i=5000;
					}
					first_stretchcount++;
				}
			}
			
        	textBox2.Text = stretches[stretch_location][0].ToString()+";"+
			stretches[stretch_location][1].ToString()+";"+
			right.Value.ToString()+";"+
			stretches[stretch_location][3].ToString();
        }
        
        void DownValueChanged(object sender, EventArgs e)
        {
        	
        	int stretch_location = 0;
			int first_stretchcount = 0;
			for (int i=0;i<stretches_indexes.Count;i++)
			{
				if (stretches_indexes[i]==num_index)
				{
					if (first_stretchcount==(int)numericUpDown6.Value)
					{
						stretch_location=i;
						i=5000;
					}
					first_stretchcount++;
				}
			}
			
        	textBox2.Text = stretches[stretch_location][0].ToString()+";"+
			stretches[stretch_location][1].ToString()+";"+
			stretches[stretch_location][2].ToString()+";"+
			down.Value.ToString();
        }
        int cadenas = 1;
        
        int left_right= 0;
        int up_down = 0;
        void Button5Click(object sender, EventArgs e)
        {
        	cadenas=-cadenas;
        	button5.BackgroundImage=(new Image[] {open.BackgroundImage,close.BackgroundImage})[(cadenas.ToString().Length)-(int)Math.Abs(cadenas).ToString().Length];
        	right.Enabled = cadenas==1;
        	down.Enabled = cadenas==1;
        	if (left.Value<right.Value)
        	{
        		left_right=(int)(Math.Abs(left.Value)+Math.Abs(right.Value));
        	}
        	else
        	{
        		left_right=-((int)(Math.Abs(left.Value)+Math.Abs(right.Value)));
        	}
        	if (up.Value<down.Value)
        	{
        		up_down=(int)(Math.Abs(up.Value)+Math.Abs(down.Value));
        	}
        	else
        	{
        		up_down=-((int)(Math.Abs(up.Value)+Math.Abs(down.Value)));
        	}
        }
        
        void Starting_timerTick(object sender, EventArgs e)
        {
        	NumericUpDown6ValueChanged(null,null);
	        	Refreshstretch(base_crops[num_index][0]
	        	                                          ,base_crops[num_index][1]
	        	                                          ,base_crops[num_index][2]-base_crops[num_index][0]
	        	                                          ,base_crops[num_index][3]-base_crops[num_index][1]);
        	starting_timer.Enabled =false;
        }
        
        void HorizontalFlipToolStripMenuItemClick(object sender, EventArgs e)
        {
        	decimal old_left=left.Value;
        	left.Value=right.Value;
        	right.Value =old_left;
        }
        
        void VerticalFlipToolStripMenuItemClick(object sender, EventArgs e)
        {
        	decimal old_up=up.Value;
        	up.Value=down.Value;
        	down.Value =old_up;
        }
        
        void CenterTheStretchToolStripMenuItemClick(object sender, EventArgs e)
        {
        	left.Value=-(width_.Value/2);
        	up.Value=-(height_.Value/2);
        	right.Value=(width_.Value/2);
        	down.Value=(height_.Value/2);
        }
        
        void ToolStripMenuItem3Click(object sender, EventArgs e)
        {
        	left.Value = (decimal)Math.Floor(((((9*left.Value)/16)*4)/3));
        	right.Value = (decimal)Math.Floor(((((9*right.Value)/16)*4)/3));
        }
        
        void ToolStripMenuItem2Click(object sender, EventArgs e)
        {
        	left.Value = (decimal)Math.Floor(((((3*left.Value)/4)*16)/9));
        	right.Value = (decimal)Math.Floor(((((3*right.Value)/4)*16)/9));
        }
        
        void AllToolStripMenuItem1Click(object sender, EventArgs e)
        {
        	for (int i=0;i<stretches.Count;i++)
    		{
        		stretches[i][0] = (int)Math.Floor((decimal)((((9*stretches[i][0])/16)*4)/3));
        		stretches[i][2] = (int)Math.Floor((decimal)((((9*stretches[i][2])/16)*4)/3));
    		}
        	
        }
        
        void AllToolStripMenuItemClick(object sender, EventArgs e)
        {
        	
        }
    }
}
