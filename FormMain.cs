using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace IMGZ_Editor
{
	delegate void Delegate();

	public class MainForm : Form
	{
		private Process PCSX = new Process();

		private byte[] file_bytes;

		private string command_original_filename = "";

		private FileStream file;

		private Form form;

		private string imd_name = "";

		private ImageContainer img;

		private string imgName;

		private int BAR_index = -1;

		private ListBox listBox1;

		private Bitmap Image_Backup;

		public static readonly FileVersionInfo program = FileVersionInfo.GetVersionInfo(Assembly.GetEntryAssembly().Location);

		private Dictionary<string, Type> typeLoaders;

		private bool closeByDoubleCLick;

		private Thread Search;

		private string IndexPreview_mode = "None";

		private int SEQD_address;

		private int IMD_address;

		private int IMD_length;

		private List<string> IMGD_names = new List<string>(0);

		private List<int> IMGD_addresses = new List<int>(0);

		private List<string> IMGZ_names = new List<string>(0);

		private List<int> IMGZ_addresses = new List<int>(0);

		private List<string> SEQD_names = new List<string>(0);

		private List<int> SEQD_addresses = new List<int>(0);

		private List<string> LAYD_names = new List<string>(0);

		private List<int> LAYD_addresses = new List<int>(0);

		private int IMZ_address;

		private int[] SEQD_results;

		private int Image_index;

		private bool search_files;

		private string path = Path.GetDirectoryName(Directory.GetCurrentDirectory());

		private List<string> foundfiles = new List<string>(0);

		private IContainer components;

		private ToolStripMenuItem allToolStripMenuItem1;

		private ToolStripMenuItem toolStripMenuItem3;

        private System.Windows.Forms.Timer starting_timer;

		private ToolStripMenuItem centerTheStretchToolStripMenuItem;

		private ToolStripMenuItem verticalFlipToolStripMenuItem;

		private ToolStripMenuItem horizontalFlipToolStripMenuItem;

		private ContextMenuStrip contextMenuStrip1;

		private PictureBox close;

		private PictureBox open;

		private Button LockStretch;

		private PictureBox StretchBox;

		private Panel panel2;

		private Label label3;

		private Button ResetStrech;

		private TextBox LTRD;

		private NumericUpDown left;

		private NumericUpDown top;

		private NumericUpDown right;

		private NumericUpDown bottom;

		private GroupBox groupBox4;

		private System.Windows.Forms.Timer timer2;

		private PictureBox StretchContainer;

		private PictureBox BrightBack;

		private PictureBox DarkBack;

		private Button BackChange;

		private System.Windows.Forms.Timer timer1;

		private Button IngamePreview;

		private Label ProcessName;

		private GroupBox groupBox3;

		private SaveFileDialog saveFileDialog1;

		private Panel panel1;

		private TextBox XYWH;

		private NumericUpDown x_pos;

		private NumericUpDown y_pos;

		private NumericUpDown width_;

		private NumericUpDown height_;

		private Button SearchSEQD;

		private System.Windows.Forms.Timer StartSearch;

		private NumericUpDown TrimIndexNum;

		private NumericUpDown StretchIndexNum;

		private Button ResetTrim;

		private Button Save2DD;

		private GroupBox groupBox2;

		private GroupBox groupBox1;

		private Button buttonOpen;

		private OpenFileDialog openFileDialog;

		private PictureBox pictureBoxBMP;

		private Button buttonSave;

		private SaveFileDialog saveFileDialog;

		private FolderBrowserDialog folderBrowserDialog;

		private Button buttonReplace;

		private ToolStripMenuItem sizeToolStripMenuItem;

		private CheckBox General;

		private Label label1;

		private Button ResetAllTrims;

		private Button ResetAllStretches;

		private Button LockTrim;

		private TrackBar redTrack;

		private GroupBox groupBox5;

		private ComboBox RGBALoc;

		private TrackBar alphaTrack;

		private TrackBar blueTrack;

		private TrackBar greenTrack;

		private Button Draw;

		private Button ResetAllBorders;

		private Button ResetBorder;

		private CheckBox PartialMode;

		private int file_address_RAM;

		private Rectangle TrimRectangle;

		private List<List<int[]>> stretches = new List<List<int[]>>(0);

		private readonly List<List<int[]>> stretches_backup = new List<List<int[]>>(0);

		private List<List<int>> stretches_addresses = new List<List<int>>(0);

		private List<int[]> trims = new List<int[]>(0);

		private readonly List<int[]> trims_backup = new List<int[]>(0);

		private List<byte[][]> filters = new List<byte[][]>(0);

		private readonly List<byte[][]> filters_backup = new List<byte[][]>(0);

		private bool left_XYWH = true;

		private bool lock_stretch_leftop;

		private bool lock_trim_leftop;

		private int GrabTypeOver = -1;

		private Point mouse_2L;

		private int GrabTypeDown = -1;

		private Point mouse_2LDown;

		private int[] fix_point = new int[2];

		private Pen RedPen = new Pen(Color.Red, 1f);

		public MainForm()
		{
			this.loadClasses();
			this.InitializeComponent();
			#if MDLX_DUMP_RAW_DATA
		     this.Text += " [MDLX_DUMP_RAW_DATA]";
			#endif
			this.folderBrowserDialog.SelectedPath = AppDomain.CurrentDomain.BaseDirectory;
		}

		public static void ShowError(Exception e, string title = "An error has occurred", MessageBoxIcon icon = System.Windows.Forms.MessageBoxIcon.Error)
		{
			if (MessageBox.Show(string.Format("{0}\r\nDo you want to see more detailed debugging information?", e.Message), title, System.Windows.Forms.MessageBoxButtons.YesNo, icon) == System.Windows.Forms.DialogResult.Yes)
			{
				StringBuilder stringBuilder = new StringBuilder();
				do
				{
					stringBuilder.AppendFormat("{0}: {1}\r\n{2}\r\n\r\n", e.GetType().Name, e.Message, e.StackTrace);
					e = e.InnerException;
				}
				while (e != null);
				MessageBox.Show(stringBuilder.ToString(), title, 0, icon);
			}
		}

		private void loadClasses()
		{
			if (this.typeLoaders != null)
			{
				return;
			}
			this.typeLoaders = new Dictionary<string, Type>();
			Type typeFromHandle = typeof(ImageContainer);
			Assembly assembly = typeFromHandle.Assembly;
			Type[] types = assembly.GetTypes();
			for (int i = 0; i < types.Length; i++)
			{
				Type type = types[i];
				try
				{
					if (type.IsSubclassOf(typeFromHandle) && type.IsClass && !type.IsAbstract)
					{
						FieldInfo field = type.GetField("extensions", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static);
						if (field != null)
						{
							IEnumerable<string> enumerable = (IEnumerable<string>)field.GetValue(null);
							using (IEnumerator<string> enumerator = enumerable.GetEnumerator())
							{
								while (enumerator.MoveNext())
								{
									string current = enumerator.Current;
									this.typeLoaders.Add('.' + current, type);
								}
							}
						}
					}
				}
				catch (Exception e)
				{
					MainForm.ShowError(e, "Error checking " + type);
				}
			}
		}

		private void Cleanup(bool sizeReset = true)
		{
			NumericUpDown arg_36_0 = this.left;
			NumericUpDown arg_30_0 = this.top;
			NumericUpDown arg_28_0 = this.right;
			decimal num;
			this.bottom.Value = num = 0m;
			decimal num2;
			arg_28_0.Value = num2 = num;
			decimal value;
			arg_30_0.Value = value = num2;
			arg_36_0.Value = value;
			this.LTRD.Text = "0;0;0;0";
			NumericUpDown arg_85_0 = this.x_pos;
			NumericUpDown arg_7E_0 = this.y_pos;
			NumericUpDown arg_74_0 = this.width_;
			decimal num3;
			this.height_.Value = num3 = 0m;
			decimal num4;
			arg_74_0.Value = num4 = num3;
			decimal value2;
			arg_7E_0.Value = value2 = num4;
			arg_85_0.Value = value2;
			this.XYWH.Text = "0;0;0;0";
			this.Save2DD.Enabled = false;
			Control arg_DD_0 = this.groupBox1;
			Control arg_D6_0 = this.groupBox4;
			Control arg_CC_0 = this.groupBox2;
			bool flag;
			this.groupBox3.Enabled = flag = false;
			bool flag2;
			arg_CC_0.Enabled = flag2 = flag;
			bool enabled;
			arg_D6_0.Enabled = enabled = flag2;
			arg_DD_0.Enabled = enabled;
			this.buttonSave.Enabled = false;
			this.buttonReplace.Enabled = false;
			this.trims.Clear();
			this.stretches.Clear();
			this.Image_Backup = new Bitmap(256, 256);
			this.StretchContainer.BackgroundImage = null;
			this.pictureBoxBMP.Location = new Point(160, 50);
			this.pictureBoxBMP.Size = new Size(256, 256);
			this.buttonReplace.Enabled = this.buttonSave.Enabled;
			if (this.pictureBoxBMP.Image != null)
			{
				this.pictureBoxBMP.Image.Dispose();
				this.pictureBoxBMP.Image = null;
			}
			if (this.img != null)
			{
				this.img.Dispose();
				this.img = null;
				this.imgName = "";
			}
			this.StretchBox.Size = new Size(256, 256);
			this.StretchBox.Location = new Point(160, this.pictureBoxBMP.Location.Y + this.pictureBoxBMP.Height + 20);
			this.StretchContainer.Location = new Point((int)Math.Ceiling((double)this.StretchBox.Width / 2.0), (int)Math.Ceiling((double)this.StretchBox.Height / 2.0));
		}

		public byte[] SubArray(byte[] data, long index, long length)
		{
			byte[] array = new byte[length];
			try
			{
				Array.Copy(data, index, array, 0L, length);
			}
			catch
			{
			}
			return array;
		}

		public byte[] WriteArray(byte[] input, int address, byte[] value_)
		{
			return MainForm.Combine(MainForm.Combine(this.SubArray(input, 0L, (long)address), value_), this.SubArray(input, (long)(address + value_.Length), (long)(input.Length - (address + value_.Length))));
		}

		private static byte[] Combine(byte[] a, byte[] b)
		{
			byte[] array = new byte[a.Length + b.Length];
			try
			{
				Buffer.BlockCopy(a, 0, array, 0, a.Length);
				Buffer.BlockCopy(b, 0, array, a.Length, b.Length);
			}
			catch
			{
			}
			return array;
		}

		private void OpenList(string title, string[] items)
		{
			this.BAR_index = -1;
			base.Focus();
			this.form = new Form();
			this.listBox1 = new ListBox();
			base.SuspendLayout();
			this.listBox1.FormattingEnabled = true;
			this.listBox1.Location = new Point(13, 13);
			this.listBox1.Name = "listBox1";
			this.listBox1.Size = new Size(259, 121);
			this.listBox1.TabIndex = 0;
			this.listBox1.DoubleClick += new EventHandler(this.ListDoubleClick);
			this.listBox1.SelectedIndexChanged += new EventHandler(this.ListPreviewer);
			if (items.Length > 0)
			{
				this.listBox1.Items.AddRange(items);
			}
			this.form.AutoScaleDimensions = new SizeF(6f, 13f);
			this.form.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.form.ClientSize = new Size(284, 150);
			this.form.Controls.Add(this.listBox1);
			this.form.MaximizeBox = false;
			this.form.MaximumSize = new Size(300, 188);
			this.form.MinimizeBox = false;
			this.form.MinimumSize = new Size(300, 188);
			this.form.Name = "form";
			this.form.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.form.ShowIcon = false;
			this.form.Text = title;
			this.form.FormClosing += new FormClosingEventHandler(this.ListFormClosing);
			this.form.Load += new EventHandler(this.ListLoad);
			base.ResumeLayout(false);
			this.closeByDoubleCLick = false;
			this.form.ShowDialog();
		}

		private void ListLoad(object sender, EventArgs e)
		{
			if (this.listBox1.Items.Count > 0)
			{
				this.listBox1.SelectedIndex = 0;
			}
			this.StartSearch.Enabled = this.search_files;
		}

		private void Method()
		{
			this.OpenPathDirectory(this.path);
		}

		private void StartSearchTick(object sender, EventArgs e)
		{
			this.StartSearch.Enabled = false;
			this.Search = new Thread(new ThreadStart(this.Method));
			this.Search.Start();
		}

		private void ListDoubleClick(object sender, EventArgs e)
		{
			if (this.listBox1.SelectedIndex != -1 | this.listBox1.Items.Count == 0)
			{
				this.closeByDoubleCLick = true;
				this.BAR_index = this.listBox1.SelectedIndex;
				if (this.search_files)
				{
					this.search_files = false;
					try
					{
						this.Search.Abort();
					}
					catch
					{
					}
					this.listBox1.Items.Clear();
					this.form.FormClosing -= new FormClosingEventHandler(this.ListFormClosing);
					this.form.Dispose();
					this.form.FormClosing += new FormClosingEventHandler(this.ListFormClosing);
					this.Text = this.foundfiles[this.BAR_index];
					this.openFile(this.foundfiles[this.BAR_index], false);
					return;
				}
				this.form.Close();
			}
		}

		private void ListFormClosing(object sender, FormClosingEventArgs e)
		{
			try
			{
				if (!this.closeByDoubleCLick)
				{
					this.BAR_index = -1;
					this.Cleanup(true);
				}
				this.listBox1.DoubleClick -= new EventHandler(this.ListDoubleClick);
				this.listBox1.SelectedIndexChanged -= new EventHandler(this.ListPreviewer);
				this.form.Load -= new EventHandler(this.ListLoad);
				this.form.FormClosing -= new FormClosingEventHandler(this.ListFormClosing);
				this.form = null;
			}
			catch
			{
			}
		}

		private void ListPreviewer(object sender, EventArgs e)
		{
			this.BAR_index = ((this.form != null) ? ((this.listBox1.SelectedIndex > -1) ? this.listBox1.SelectedIndex : 0) : 0);
			string indexPreview_mode;
			if (!this.search_files && (indexPreview_mode = this.IndexPreview_mode) != null)
			{
				if (!(indexPreview_mode == "IMGZ"))
				{
					if (!(indexPreview_mode == "IMGD"))
					{
						if (!(indexPreview_mode == "LAYD"))
						{
							if (!(indexPreview_mode == "SEQD"))
							{
								return;
							}
						}
						else
						{
							this.SEQD_address = this.SEQD_results[this.BAR_index];
						}
						this.SEQD_address = ((this.IndexPreview_mode == "SEQD") ? this.RI(this.SEQD_addresses[this.BAR_index] + 8) : this.SEQD_address);
						int num = this.RI(this.SEQD_address + 8);
						Bitmap bitmap = new Bitmap(this.Image_Backup);
						Graphics graphics = Graphics.FromImage(bitmap);
						for (int i = 0; i < num; i++)
						{
							int num2 = this.RI(52 + this.SEQD_address + i * 44);
							int num3 = this.RI(56 + this.SEQD_address + i * 44);
							int num4 = this.RI(60 + this.SEQD_address + i * 44);
							int num5 = this.RI(64 + this.SEQD_address + i * 44);
							this.TrimRectangle = new Rectangle(num2, num3, num4 - num2, num5 - num3);
							graphics.DrawRectangle(this.RedPen, this.TrimRectangle);
						}
						this.pictureBoxBMP.Image = bitmap;
						return;
					}
				}
				else
				{
					this.IMD_address = this.IMZ_address + this.RI(this.IMZ_address + 16 + this.BAR_index * 8);
					this.IMD_length = this.RI(this.IMZ_address + 20 + this.BAR_index * 8);
				}
				this.IMD_address = ((this.IndexPreview_mode == "IMGD") ? this.RI(this.IMGD_addresses[this.BAR_index] + 8) : this.IMD_address);
				this.IMD_length = ((this.IndexPreview_mode == "IMGD") ? this.AdjustBy16(this.RI(this.IMGD_addresses[this.BAR_index] + 12)) : this.IMD_length);
				byte[] array = (this.IMD_address + this.IMD_length < this.file_bytes.Length) ? this.SubArray(this.file_bytes, (long)this.IMD_address, (long)this.IMD_length) : this.SubArray(this.file_bytes, (long)this.IMD_address, (long)(this.file_bytes.Length - this.IMD_address));
				string text = Path.ChangeExtension(Path.GetTempFileName(), Guid.NewGuid() + ".imd");
				File.WriteAllBytes(text, array);
				this.file = File.Open(text, System.IO.FileMode.Open, System.IO.FileAccess.ReadWrite, System.IO.FileShare.None);
				string text2 = ".imd";
				Type type;
				this.typeLoaders.TryGetValue(text2, out type);
				this.img = (ImageContainer)Activator.CreateInstance(type, new object[]
				{
					this.file
				});
				this.img.parse();
				Bitmap image = new Bitmap(this.img.getBMP(0));
				this.pictureBoxBMP.Image = image;
				this.Image_Backup = (Bitmap)this.pictureBoxBMP.Image;
				this.pictureBoxBMP.Width = this.pictureBoxBMP.Image.Width;
				this.pictureBoxBMP.Height = this.pictureBoxBMP.Image.Height;
				this.StretchBox.Width = this.pictureBoxBMP.Image.Width;
				this.StretchBox.Height = (this.pictureBoxBMP.Image.Height <= 257) ? this.pictureBoxBMP.Image.Height : 257;
				this.StretchBox.Location = new Point(160, this.pictureBoxBMP.Location.Y + this.pictureBoxBMP.Height + 20);
				this.StretchContainer.Location = new Point((int)Math.Ceiling((double)this.StretchBox.Width / 2.0), (int)Math.Ceiling((double)this.StretchBox.Height / 2.0));
				return;
			}
		}

		public int[] SearchBytes(int from_, int length_, byte[] to_search)
		{
			List<int> list = new List<int>(0);
			byte[] array = this.SubArray(this.file_bytes, (long)from_, (long)length_);
			int num = to_search.Length;
			int num2 = array.Length - num;
			for (int i = 0; i <= num2; i++)
			{
				int num3 = 0;
				while (num3 < num && to_search[num3] == array[i + num3])
				{
					num3++;
				}
				if (num3 == num)
				{
					list.Add(from_ + i);
				}
			}
			if (list.Count > 0)
			{
				return list.ToArray();
			}
			return new int[]
			{
				-1
			};
		}

		private int AdjustBy16(int entry)
		{
			if (entry >= 16)
			{
				return entry + 16 - entry % 16;
			}
			if (entry <= 0)
			{
				return 0;
			}
			return 16;
		}

		private void openFile(string filename, bool reado = false)
		{
			try
			{
				this.Cleanup(true);
				this.file_address_RAM = 0;
				this.command_original_filename = filename;
				this.file_bytes = File.ReadAllBytes(filename);
				if (this.RI(4) > 0)
				{
					this.BAR_index = -1;
					this.Image_index = 0;
					this.IMGD_names.Clear();
					this.IMGD_addresses.Clear();
					this.IMGZ_names.Clear();
					this.IMGZ_addresses.Clear();
					this.SEQD_names.Clear();
					this.SEQD_addresses.Clear();
					this.LAYD_names.Clear();
					this.LAYD_addresses.Clear();
					for (int i = 0; i < this.RI(4); i++)
					{
						if (this.file_bytes[16 + i * 16] == 28)
						{
							this.LAYD_names.Add(Encoding.ASCII.GetString(this.SubArray(this.file_bytes, (long)(20 + i * 16), 4L)));
							this.LAYD_addresses.Add(16 + i * 16);
						}
						if (this.file_bytes[16 + i * 16] == 29)
						{
							this.IMGZ_names.Add(Encoding.ASCII.GetString(this.SubArray(this.file_bytes, (long)(20 + i * 16), 4L)));
							this.IMGZ_addresses.Add(16 + i * 16);
						}
						if (this.file_bytes[16 + i * 16] == 24)
						{
							this.IMGD_names.Add(Encoding.ASCII.GetString(this.SubArray(this.file_bytes, (long)(20 + i * 16), 4L)));
							this.IMGD_addresses.Add(16 + i * 16);
						}
						if (this.file_bytes[16 + i * 16] == 25)
						{
							this.SEQD_names.Add(Encoding.ASCII.GetString(this.SubArray(this.file_bytes, (long)(20 + i * 16), 4L)));
							this.SEQD_addresses.Add(16 + i * 16);
						}
					}
					this.SEQD_address = 0;
					this.IMD_address = 0;
					this.IMD_length = 0;
					bool flag = true;
					if (this.SEQD_addresses.Count > 0)
					{
						this.IndexPreview_mode = "IMGD";
						if (this.IMGD_names.Count > 1)
						{
							this.OpenList("Please select a IMGD index:", this.IMGD_names.ToArray());
						}
						else
						{
							this.ListPreviewer(null, null);
						}
						if (this.BAR_index == -1)
						{
							return;
						}
						this.IMD_address = this.RI(this.IMGD_addresses[this.BAR_index] + 8);
						this.IMD_length = this.AdjustBy16(this.RI(this.IMGD_addresses[this.BAR_index] + 12));
						this.IndexPreview_mode = "SEQD";
						if (this.SEQD_names.Count > 1)
						{
							this.OpenList("Please select a SEQD index:", this.SEQD_names.ToArray());
						}
						else
						{
							this.BAR_index = 0;
							this.ListPreviewer(null, null);
						}
						if (this.BAR_index == -1)
						{
							return;
						}
						this.SEQD_address = this.RI(this.SEQD_addresses[this.BAR_index] + 8);
						flag = false;
					}
					if (flag & this.IMGZ_addresses.Count > 0)
					{
						this.IndexPreview_mode = "None";
						if (this.IMGZ_names.Count > 1)
						{
							this.OpenList("Please select a IMGZ index:", this.IMGZ_names.ToArray());
						}
						else
						{
							this.BAR_index = 0;
						}
						if (this.BAR_index == -1)
						{
							return;
						}
						List<string> list = new List<string>(0);
						int num = 16;
						this.IMZ_address = this.RI(this.IMGZ_addresses[this.BAR_index] + 8);
						while (this.IMZ_address + this.RI(this.IMZ_address + num) < this.file_bytes.Length && this.file_bytes[this.IMZ_address + this.RI(this.IMZ_address + num)] == 73 && this.file_bytes[this.IMZ_address + this.RI(this.IMZ_address + num) + 1] == 77 && this.file_bytes[this.IMZ_address + this.RI(this.IMZ_address + num) + 2] == 71 && this.file_bytes[this.IMZ_address + this.RI(this.IMZ_address + num) + 3] == 68)
						{
							list.Add(list.Count.ToString());
							num += 8;
						}
						this.IndexPreview_mode = "IMGZ";
						if (list.Count > 1)
						{
							this.OpenList("Please select a IMGD index:", list.ToArray());
						}
						else
						{
							this.ListPreviewer(null, null);
						}
						if (this.BAR_index == -1)
						{
							return;
						}
						this.IMD_address = this.IMZ_address + this.RI(this.IMZ_address + 16 + this.BAR_index * 8);
						this.IMD_length = this.RI(this.IMZ_address + 20 + this.BAR_index * 8);
					}
					if (flag & this.LAYD_addresses.Count > 0)
					{
						this.IndexPreview_mode = "None";
						if (this.LAYD_names.Count > 1)
						{
							this.OpenList("Please select a LAYD index:", this.LAYD_names.ToArray());
						}
						else
						{
							this.BAR_index = 0;
						}
						if (this.BAR_index == -1)
						{
							return;
						}
						int from_ = this.RI(this.LAYD_addresses[this.BAR_index] + 8);
						int length_ = this.RI(this.LAYD_addresses[this.BAR_index] + 12);
						this.SEQD_results = this.SearchBytes(from_, length_, new byte[]
						{
							83,
							69,
							81,
							68
						});
						List<string> list2 = new List<string>(0);
						while (list2.Count < this.SEQD_results.Length)
						{
							list2.Add(list2.Count.ToString());
						}
						this.IndexPreview_mode = "LAYD";
						if (list2.Count > 1)
						{
							this.OpenList("Please select a SEQD index:", list2.ToArray());
						}
						else
						{
							this.ListPreviewer(null, null);
						}
						if (this.BAR_index == -1)
						{
							return;
						}
						this.SEQD_address = this.SEQD_results[this.BAR_index];
					}
				}
				if (this.SEQD_address == 0 | this.IMD_address == 0)
				{
					MessageBox.Show("This file does not contain any SEQD.");
					return;
				}
				byte[] array = (this.IMD_address + this.IMD_length < this.file_bytes.Length) ? this.SubArray(this.file_bytes, (long)this.IMD_address, (long)this.IMD_length) : this.SubArray(this.file_bytes, (long)this.IMD_address, (long)(this.file_bytes.Length - this.IMD_address));
				filename = Path.ChangeExtension(Path.GetTempFileName(), Guid.NewGuid() + "imd");
				this.imd_name = filename;
				this.imgName = Path.GetFileNameWithoutExtension(filename);
				File.WriteAllBytes(filename, array);
				try
				{
					this.file.Close();
				}
				catch
				{
				}
				this.file = File.Open(filename, System.IO.FileMode.Open, reado ? System.IO.FileAccess.Read : System.IO.FileAccess.ReadWrite, System.IO.FileShare.None);
				this.Cleanup(true);
			}
			catch
			{
				throw new NotSupportedException("Unable to access the file: " + this.saveFileDialog1.FileName);
			}
			try
			{
				Type type;
				if (!this.typeLoaders.TryGetValue(".imd", out type))
				{
					throw new NotSupportedException("Unknown filetype: .imd");
				}
				this.img = (ImageContainer)Activator.CreateInstance(type, new object[]
				{
					this.file
				});
			}
			catch (Exception e)
			{
				this.file.Close();
				this.img = null;
				MainForm.ShowError(e, "Fatal error opening file");
				return;
			}
			try
			{
				this.img.parse();
			}
			catch (Exception e2)
			{
				this.img.Dispose();
				this.img = null;
				MainForm.ShowError(e2, "Fatal error parsing file");
				return;
			}
			this.TrimIndexNum.Value = 0m;
			this.StretchIndexNum.Value = 0m;
			if (this.img != null)
			{
				this.Save2DD.Enabled = true;
				Control arg_840_0 = this.pictureBoxBMP;
				Control arg_839_0 = this.groupBox1;
				Control arg_82F_0 = this.groupBox4;
				Control arg_825_0 = this.groupBox5;
				Control arg_81B_0 = this.groupBox2;
				bool flag2;
				this.groupBox3.Enabled = flag2 = true;
				bool flag3;
				arg_81B_0.Enabled = flag3 = flag2;
				bool flag4;
				arg_825_0.Enabled = flag4 = flag3;
				bool flag5;
				arg_82F_0.Enabled = flag5 = flag4;
				bool enabled;
				arg_839_0.Enabled = enabled = flag5;
				arg_840_0.Enabled = enabled;
				this.buttonSave.Enabled = true;
				this.buttonReplace.Enabled = this.file.CanWrite;
				if (this.pictureBoxBMP.Image != null)
				{
					this.pictureBoxBMP.Image.Dispose();
				}
				Bitmap bitmap = new Bitmap(this.img.getBMP(0));
				Bitmap bitmap2 = new Bitmap(bitmap.Width + 1, bitmap.Height + 1);
				Graphics graphics = Graphics.FromImage(bitmap2);
				graphics.DrawImage(bitmap, new Point(0, 0));
				this.pictureBoxBMP.Image = bitmap2;
				this.Image_Backup = (Bitmap)this.pictureBoxBMP.Image;
				this.pictureBoxBMP.Width = this.pictureBoxBMP.Image.Width;
				this.pictureBoxBMP.Height = this.pictureBoxBMP.Image.Height;
				this.StretchBox.Width = this.pictureBoxBMP.Image.Width;
				this.StretchBox.Height = (this.pictureBoxBMP.Image.Height <= 257) ? this.pictureBoxBMP.Image.Height : 257;
				this.StretchBox.Location = new Point(160, this.pictureBoxBMP.Location.Y + this.pictureBoxBMP.Height + 20);
				this.StretchContainer.Location = new Point((int)Math.Ceiling((double)this.StretchBox.Width / 2.0), (int)Math.Ceiling((double)this.StretchBox.Height / 2.0));
			}
			this.Srk_Extension_Init();
		}

		private void saveBMP(int index, string filename)
		{
			if (this.img != null)
			{
				this.img.getBMP(0).Save(filename, ImageFormat.Png);
			}
		}

		private void replaceBMP(int index, string filename)
		{
			if (this.img != null)
			{
				this.img.setBMP(0, new Bitmap(new MemoryStream(File.ReadAllBytes(filename))));
			}
		}

		private void buttonOpen_Click(object sender, EventArgs e)
		{
			this.openFileDialog.Filter = "All Supported Files|*.2dd;*.2ld;*.fm;*.map;*.bar";
			this.openFileDialog.ShowReadOnly = true;
			this.openFileDialog.FileName = "";
			this.openFileDialog.DefaultExt = "";
			if (this.openFileDialog.ShowDialog() == DialogResult.OK)
			{
				this.openFile(this.openFileDialog.FileName, this.openFileDialog.ReadOnlyChecked);
			}
		}

		private void buttonOpen_DragEnter(object sender, DragEventArgs e)
		{
			if (e.Data.GetDataPresent(DataFormats.FileDrop))
			{
				e.Effect = DragDropEffects.Copy;
				return;
			}
			e.Effect = 0;
		}

		private void buttonOpen_DragDrop(object sender, DragEventArgs e)
		{
			if (e.Data.GetDataPresent(DataFormats.FileDrop))
			{
				string[] array = (string[])e.Data.GetData(DataFormats.FileDrop);
				string[] array2 = array;
				for (int i = 0; i < array2.Length; i++)
				{
					string text = array2[i];
					if (File.Exists(text))
					{
						this.openFile(text, false);
						return;
					}
				}
			}
		}

		private void buttonSave_Click(object sender, EventArgs e)
		{
			this.saveFileDialog.FileName = Path.GetFileNameWithoutExtension(this.command_original_filename) + "-" + this.Image_index.ToString() + ".png";
			if (this.img != null && this.saveFileDialog.ShowDialog() == DialogResult.OK)
			{
				this.saveBMP(0, this.saveFileDialog.FileName);
			}
		}

		private void buttonReplace_Click(object sender, EventArgs e)
		{
			this.openFileDialog.FileName = "";
			this.openFileDialog.Filter = "PNG image file|*.png";
			this.openFileDialog.DefaultExt = "png";
			this.openFileDialog.ShowReadOnly = false;
			if (this.img != null && this.img.imageCount > 0 && this.openFileDialog.ShowDialog() == DialogResult.OK)
			{
				this.replaceBMP(0, this.openFileDialog.FileName);
				this.file.Close();
				byte[] value_ = File.ReadAllBytes(this.imd_name);
				this.file_bytes = this.WriteArray(this.file_bytes, this.IMD_address, value_);
				this.file = File.Open(this.imd_name, System.IO.FileMode.Open, false ? System.IO.FileAccess.Read : System.IO.FileAccess.ReadWrite, System.IO.FileShare.None);
				if (this.pictureBoxBMP.Image != null)
				{
					this.pictureBoxBMP.Image.Dispose();
				}
				Bitmap bitmap = new Bitmap(this.img.getBMP(0));
				Bitmap bitmap2 = new Bitmap(bitmap.Width + 1, bitmap.Height + 1);
				Graphics graphics = Graphics.FromImage(bitmap2);
				graphics.DrawImage(bitmap, new Point(0, 0));
				this.pictureBoxBMP.Image = bitmap2;
				this.Image_Backup = (Bitmap)this.pictureBoxBMP.Image;
				this.pictureBoxBMP.Width = this.pictureBoxBMP.Image.Width;
				this.pictureBoxBMP.Height = this.pictureBoxBMP.Image.Height;
				this.StretchBox.Width = this.pictureBoxBMP.Image.Width;
				this.StretchBox.Height = (this.pictureBoxBMP.Image.Height <= 257) ? this.pictureBoxBMP.Image.Height : 257;
				this.StretchBox.Location = new Point(160, this.pictureBoxBMP.Location.Y + this.pictureBoxBMP.Height + 20);
				this.StretchContainer.Location = new Point((int)Math.Ceiling((double)this.StretchBox.Width / 2.0), (int)Math.Ceiling((double)this.StretchBox.Height / 2.0));
				this.TrimIndexNumValueChanged(null, null);
				this.StretchIndexNumValueChanged(null, null);
			}
		}

		private void PictureBoxBMPResize(object sender, EventArgs e)
		{
			base.Size = new Size(this.pictureBoxBMP.Location.X + this.pictureBoxBMP.Width + 30, base.Height);
		}

		private void SearchSEQDClick(object sender, EventArgs e)
		{
			this.folderBrowserDialog.SelectedPath = this.path;
			this.folderBrowserDialog.Description = "Select a folder containing file(s) to scan BAR header(s)\r\nand press OK";
			if (this.folderBrowserDialog.ShowDialog() == DialogResult.OK)
			{
				this.foundfiles.Clear();
				this.path = this.folderBrowserDialog.SelectedPath;
				this.search_files = true;
				this.OpenList("Select the file you want to open:", new string[0]);
			}
		}

		private void OpenPathDirectory(string folder)
		{
			string[] files = Directory.GetFiles(folder);
			string[] array = files;
			for (int i = 0; i < array.Length; i++)
			{
				string curr = array[i];
				try
				{
					byte[] array2 = this.SubArray(File.ReadAllBytes(curr), 0L, 16L);
					bool flag = false;
					bool flag2 = false;
					if (array2[0] == 66 && array2[1] == 65 && array2[2] == 82 && array2[3] == 1)
					{
						ushort num = BitConverter.ToUInt16(new byte[]
						{
							array2[4],
							array2[5]
						}, 0);
						array2 = this.SubArray(File.ReadAllBytes(curr), 0L, (long)(16 + 16 * num));
						for (ushort num2 = 0; num2 < num; num2 += 1)
						{
							flag2 |= (array2[(int)(16 + 16 * num2)] == 28 | array2[(int)(16 + 16 * num2)] == 25);
							flag |= (array2[(int)(16 + 16 * num2)] == 24 | array2[(int)(16 + 16 * num2)] == 29);
							if (flag2 && flag)
							{
								this.foundfiles.Add(curr);
								this.listBox1.Invoke((Delegate)delegate
								{
									this.listBox1.Items.Add(Path.GetFileName(curr));
								});
								num2 = num;
							}
						}
					}
				}
				catch
				{
				}
			}
			string[] directories = Directory.GetDirectories(folder);
			string[] array3 = directories;
			for (int j = 0; j < array3.Length; j++)
			{
				string folder2 = array3[j];
				this.OpenPathDirectory(folder2);
			}
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				this.Cleanup(true);
				if (this.components != null)
				{
					this.components.Dispose();
				}
			}
			base.Dispose(disposing);
		}

		private void InitializeComponent()
		{
			this.components = new Container();
			ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof(MainForm));
			this.buttonOpen = new Button();
			this.openFileDialog = new OpenFileDialog();
			this.pictureBoxBMP = new PictureBox();
			this.buttonSave = new Button();
			this.saveFileDialog = new SaveFileDialog();
			this.folderBrowserDialog = new FolderBrowserDialog();
			this.buttonReplace = new Button();
			this.groupBox1 = new GroupBox();
			this.groupBox2 = new GroupBox();
			this.LockTrim = new Button();
			this.label1 = new Label();
			this.ResetAllTrims = new Button();
			this.height_ = new NumericUpDown();
			this.width_ = new NumericUpDown();
			this.y_pos = new NumericUpDown();
			this.x_pos = new NumericUpDown();
			this.XYWH = new TextBox();
			this.ResetTrim = new Button();
			this.TrimIndexNum = new NumericUpDown();
			this.panel1 = new Panel();
			this.redTrack = new TrackBar();
			this.Save2DD = new Button();
			this.saveFileDialog1 = new SaveFileDialog();
			this.groupBox3 = new GroupBox();
			this.IngamePreview = new Button();
			this.ProcessName = new Label();
			this.timer1 = new System.Windows.Forms.Timer(this.components);
			this.BackChange = new Button();
			this.DarkBack = new PictureBox();
			this.BrightBack = new PictureBox();
			this.StretchContainer = new PictureBox();
			this.timer2 = new System.Windows.Forms.Timer(this.components);
			this.groupBox4 = new GroupBox();
			this.ResetAllStretches = new Button();
			this.LockStretch = new Button();
			this.bottom = new NumericUpDown();
			this.right = new NumericUpDown();
			this.top = new NumericUpDown();
			this.left = new NumericUpDown();
			this.LTRD = new TextBox();
			this.ResetStrech = new Button();
			this.label3 = new Label();
			this.StretchIndexNum = new NumericUpDown();
			this.panel2 = new Panel();
			this.General = new CheckBox();
			this.StretchBox = new PictureBox();
			this.contextMenuStrip1 = new ContextMenuStrip(this.components);
			this.horizontalFlipToolStripMenuItem = new ToolStripMenuItem();
			this.verticalFlipToolStripMenuItem = new ToolStripMenuItem();
			this.centerTheStretchToolStripMenuItem = new ToolStripMenuItem();
			this.sizeToolStripMenuItem = new ToolStripMenuItem();
			this.toolStripMenuItem3 = new ToolStripMenuItem();
			this.allToolStripMenuItem1 = new ToolStripMenuItem();
			this.open = new PictureBox();
			this.close = new PictureBox();
			this.starting_timer = new System.Windows.Forms.Timer(this.components);
			this.SearchSEQD = new Button();
			this.StartSearch = new System.Windows.Forms.Timer(this.components);
			this.groupBox5 = new GroupBox();
			this.ResetAllBorders = new Button();
			this.Draw = new Button();
			this.ResetBorder = new Button();
			this.RGBALoc = new ComboBox();
			this.alphaTrack = new TrackBar();
			this.blueTrack = new TrackBar();
			this.greenTrack = new TrackBar();
			this.PartialMode = new CheckBox();
			((ISupportInitialize)this.pictureBoxBMP).BeginInit();
			this.groupBox1.SuspendLayout();
			this.groupBox2.SuspendLayout();
			this.height_.BeginInit();
			this.width_.BeginInit();
			this.y_pos.BeginInit();
			this.x_pos.BeginInit();
			this.TrimIndexNum.BeginInit();
			this.redTrack.BeginInit();
			this.groupBox3.SuspendLayout();
			((ISupportInitialize)this.DarkBack).BeginInit();
			((ISupportInitialize)this.BrightBack).BeginInit();
			((ISupportInitialize)this.StretchContainer).BeginInit();
			this.groupBox4.SuspendLayout();
			this.bottom.BeginInit();
			this.right.BeginInit();
			this.top.BeginInit();
			this.left.BeginInit();
			this.StretchIndexNum.BeginInit();
			((ISupportInitialize)this.StretchBox).BeginInit();
			this.StretchBox.SuspendLayout();
			this.contextMenuStrip1.SuspendLayout();
			((ISupportInitialize)this.open).BeginInit();
			((ISupportInitialize)this.close).BeginInit();
			this.groupBox5.SuspendLayout();
			this.alphaTrack.BeginInit();
			this.blueTrack.BeginInit();
			this.greenTrack.BeginInit();
			base.SuspendLayout();
			this.buttonOpen.AllowDrop = true;
			this.buttonOpen.Font = new Font("Microsoft Sans Serif", 10f, FontStyle.Regular);
			this.buttonOpen.Location = new Point(8, 8);
			this.buttonOpen.Name = "buttonOpen";
			this.buttonOpen.Size = new Size(81, 32);
			this.buttonOpen.TabIndex = 0;
			this.buttonOpen.Text = "Open SEQD";
			this.buttonOpen.UseVisualStyleBackColor = true;
			this.buttonOpen.Click += new EventHandler(this.buttonOpen_Click);
			this.buttonOpen.DragDrop += new DragEventHandler(this.buttonOpen_DragDrop);
			this.buttonOpen.DragEnter += new DragEventHandler(this.buttonOpen_DragEnter);
			this.openFileDialog.ShowReadOnly = true;
			this.pictureBoxBMP.BackgroundImage = (Image)componentResourceManager.GetObject("pictureBoxBMP.BackgroundImage");
			this.pictureBoxBMP.Cursor = Cursors.Cross;
			this.pictureBoxBMP.Enabled = false;
			this.pictureBoxBMP.Location = new Point(160, 50);
			this.pictureBoxBMP.Name = "pictureBoxBMP";
			this.pictureBoxBMP.Size = new Size(256, 256);
			this.pictureBoxBMP.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
			this.pictureBoxBMP.TabIndex = 2;
			this.pictureBoxBMP.TabStop = false;
			this.pictureBoxBMP.MouseDown += new MouseEventHandler(this.PictureBoxBMPMouseDown);
			this.pictureBoxBMP.MouseMove += new MouseEventHandler(this.PictureBoxBMPMouseMove);
			this.pictureBoxBMP.MouseUp += new MouseEventHandler(this.PictureBoxBMPMouseUp);
			this.buttonSave.Enabled = false;
			this.buttonSave.Location = new Point(32, 12);
			this.buttonSave.Name = "buttonSave";
			this.buttonSave.Size = new Size(75, 23);
			this.buttonSave.TabIndex = 4;
			this.buttonSave.Text = "Save";
			this.buttonSave.UseVisualStyleBackColor = true;
			this.buttonSave.Click += new EventHandler(this.buttonSave_Click);
			this.saveFileDialog.DefaultExt = "png";
			this.saveFileDialog.Filter = "PNG image file|*.png";
			this.buttonReplace.Enabled = false;
			this.buttonReplace.Location = new Point(32, 38);
			this.buttonReplace.Name = "buttonReplace";
			this.buttonReplace.Size = new Size(75, 23);
			this.buttonReplace.TabIndex = 5;
			this.buttonReplace.Text = "Replace";
			this.buttonReplace.UseVisualStyleBackColor = true;
			this.buttonReplace.Click += new EventHandler(this.buttonReplace_Click);
			this.groupBox1.Controls.Add(this.buttonReplace);
			this.groupBox1.Controls.Add(this.buttonSave);
			this.groupBox1.Enabled = false;
			this.groupBox1.Location = new Point(8, 46);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new Size(141, 70);
			this.groupBox1.TabIndex = 6;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "Image";
			this.groupBox2.Controls.Add(this.LockTrim);
			this.groupBox2.Controls.Add(this.label1);
			this.groupBox2.Controls.Add(this.ResetAllTrims);
			this.groupBox2.Controls.Add(this.height_);
			this.groupBox2.Controls.Add(this.width_);
			this.groupBox2.Controls.Add(this.y_pos);
			this.groupBox2.Controls.Add(this.x_pos);
			this.groupBox2.Controls.Add(this.XYWH);
			this.groupBox2.Controls.Add(this.ResetTrim);
			this.groupBox2.Controls.Add(this.TrimIndexNum);
			this.groupBox2.Controls.Add(this.panel1);
			this.groupBox2.Enabled = false;
			this.groupBox2.Location = new Point(8, 114);
			this.groupBox2.Name = "groupBox2";
			this.groupBox2.Size = new Size(140, 175);
			this.groupBox2.TabIndex = 7;
			this.groupBox2.TabStop = false;
			this.groupBox2.Text = "Trims";
			this.LockTrim.BackgroundImageLayout = ImageLayout.Center;
			this.LockTrim.FlatStyle = 0;
			this.LockTrim.Location = new Point(2, 77);
			this.LockTrim.Name = "LockTrim";
			this.LockTrim.Size = new Size(16, 17);
			this.LockTrim.TabIndex = 14;
			this.LockTrim.UseVisualStyleBackColor = true;
			this.LockTrim.Click += new EventHandler(this.LockTrimClick);
			this.label1.Location = new Point(15, 19);
			this.label1.Name = "label1";
			this.label1.Size = new Size(50, 20);
			this.label1.TabIndex = 12;
			this.label1.Text = "Index";
			this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			this.ResetAllTrims.Location = new Point(13, 149);
			this.ResetAllTrims.Name = "ResetAllTrims";
			this.ResetAllTrims.Size = new Size(114, 22);
			this.ResetAllTrims.TabIndex = 10;
			this.ResetAllTrims.Text = "Reset All";
			this.ResetAllTrims.UseVisualStyleBackColor = true;
			this.ResetAllTrims.Click += new EventHandler(this.ResetAllTrimsClick);
			this.height_.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.height_.Location = new Point(73, 75);
			NumericUpDown arg_AC7_0 = this.height_;
			int[] array = new int[4];
			array[0] = 511;
			arg_AC7_0.Maximum = new decimal(array);
			this.height_.Name = "height_";
			this.height_.Size = new Size(43, 20);
			this.height_.TabIndex = 8;
			this.height_.ValueChanged += new EventHandler(this.Height_ValueChanged);
			this.width_.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.width_.Location = new Point(24, 75);
			NumericUpDown arg_B4E_0 = this.width_;
			int[] array2 = new int[4];
			array2[0] = 255;
			arg_B4E_0.Maximum = new decimal(array2);
			this.width_.Name = "width_";
			this.width_.Size = new Size(43, 20);
			this.width_.TabIndex = 7;
			this.width_.ValueChanged += new EventHandler(this.Width_ValueChanged);
			this.y_pos.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.y_pos.Location = new Point(73, 53);
			NumericUpDown arg_BD5_0 = this.y_pos;
			int[] array3 = new int[4];
			array3[0] = 511;
			arg_BD5_0.Maximum = new decimal(array3);
			this.y_pos.Name = "y_pos";
			this.y_pos.Size = new Size(43, 20);
			this.y_pos.TabIndex = 6;
			this.y_pos.ValueChanged += new EventHandler(this.Y_posValueChanged);
			this.x_pos.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.x_pos.Location = new Point(24, 53);
			NumericUpDown arg_C5F_0 = this.x_pos;
			int[] array4 = new int[4];
			array4[0] = 511;
			arg_C5F_0.Maximum = new decimal(array4);
			this.x_pos.Name = "x_pos";
			this.x_pos.Size = new Size(43, 20);
			this.x_pos.TabIndex = 5;
			this.x_pos.ValueChanged += new EventHandler(this.X_posValueChanged);
			this.XYWH.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.XYWH.Location = new Point(21, 102);
			this.XYWH.Name = "XYWH";
			this.XYWH.Size = new Size(98, 20);
			this.XYWH.TabIndex = 4;
			this.XYWH.Text = "0;0;0;0";
			this.XYWH.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			this.XYWH.Click += new EventHandler(this.XYWHClick);
			this.XYWH.TextChanged += new EventHandler(this.XYWH_TextChanged);
			this.XYWH.Leave += new EventHandler(this.XYWHLeave);
			this.ResetTrim.Location = new Point(13, 125);
			this.ResetTrim.Name = "ResetTrim";
			this.ResetTrim.Size = new Size(114, 22);
			this.ResetTrim.TabIndex = 3;
			this.ResetTrim.Text = "Reset Current";
			this.ResetTrim.UseVisualStyleBackColor = true;
			this.ResetTrim.Click += new EventHandler(this.ResetTrimClick);
			this.TrimIndexNum.Location = new Point(68, 19);
			NumericUpDown arg_E05_0 = this.TrimIndexNum;
			int[] array5 = new int[4];
			array5[0] = 1000;
			arg_E05_0.Maximum = new decimal(array5);
			this.TrimIndexNum.Name = "TrimIndexNum";
			this.TrimIndexNum.Size = new Size(51, 20);
			this.TrimIndexNum.TabIndex = 1;
			this.TrimIndexNum.ValueChanged += new EventHandler(this.TrimIndexNumValueChanged);
			this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.panel1.Location = new Point(22, 51);
			this.panel1.Name = "panel1";
			this.panel1.Size = new Size(96, 46);
			this.panel1.TabIndex = 9;
			this.redTrack.BackColor = Color.FromArgb(255, 80, 80);
			this.redTrack.LargeChange = 1;
			this.redTrack.Location = new Point(6, 15);
			this.redTrack.Maximum = 255;
			this.redTrack.Name = "redTrack";
			this.redTrack.Orientation = Orientation.Vertical;
			this.redTrack.Size = new Size(45, 69);
			this.redTrack.TabIndex = 19;
			this.redTrack.TickStyle = 0;
			this.redTrack.Value = 128;
			this.redTrack.Scroll += new EventHandler(this.RedTrackScroll);
			this.Save2DD.Enabled = false;
			this.Save2DD.Font = new Font("Microsoft Sans Serif", 10f, FontStyle.Regular);
			this.Save2DD.Location = new Point(95, 8);
			this.Save2DD.Name = "Save2DD";
			this.Save2DD.Size = new Size(81, 32);
			this.Save2DD.TabIndex = 8;
			this.Save2DD.Text = "Save SEQD";
			this.Save2DD.UseVisualStyleBackColor = true;
			this.Save2DD.Click += new EventHandler(this.Save2DDClick);
			this.saveFileDialog1.DefaultExt = "png";
			this.saveFileDialog1.Filter = "All Supported Files|*.2dd;*.2ld;*.fm;*.map;*.bar";
			this.groupBox3.Controls.Add(this.IngamePreview);
			this.groupBox3.Controls.Add(this.ProcessName);
			this.groupBox3.Enabled = false;
			this.groupBox3.Location = new Point(8, 630);
			this.groupBox3.Name = "groupBox3";
			this.groupBox3.Size = new Size(140, 56);
			this.groupBox3.TabIndex = 9;
			this.groupBox3.TabStop = false;
			this.IngamePreview.Enabled = false;
			this.IngamePreview.Location = new Point(17, 26);
			this.IngamePreview.Name = "IngamePreview";
			this.IngamePreview.Size = new Size(103, 23);
			this.IngamePreview.TabIndex = 0;
			this.IngamePreview.Text = "Preview ingame";
			this.IngamePreview.UseVisualStyleBackColor = true;
			this.IngamePreview.Click += new EventHandler(this.IngamePreviewClick);
			this.ProcessName.Location = new Point(2, 9);
			this.ProcessName.Name = "ProcessName";
			this.ProcessName.Size = new Size(136, 19);
			this.ProcessName.TabIndex = 1;
			this.ProcessName.Text = "Please open PCSX2...";
			this.ProcessName.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			this.timer1.Enabled = true;
			this.timer1.Interval = 500;
			this.timer1.Tick += new EventHandler(this.PCSX2_Search);
			this.BackChange.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.BackChange.Location = new Point(398, 6);
			this.BackChange.Name = "BackChange";
			this.BackChange.Size = new Size(110, 23);
			this.BackChange.TabIndex = 10;
			this.BackChange.Text = "Dark background";
			this.BackChange.UseVisualStyleBackColor = true;
			this.BackChange.Click += new EventHandler(this.BackChangeClick);
			this.DarkBack.BackgroundImage = (Image)componentResourceManager.GetObject("DarkBack.BackgroundImage");
			this.DarkBack.Location = new Point(307, 299);
			this.DarkBack.Name = "DarkBack";
			this.DarkBack.Size = new Size(34, 21);
			this.DarkBack.TabIndex = 11;
			this.DarkBack.TabStop = false;
			this.DarkBack.Visible = false;
			this.BrightBack.BackgroundImage = (Image)componentResourceManager.GetObject("BrightBack.BackgroundImage");
			this.BrightBack.Location = new Point(347, 299);
			this.BrightBack.Name = "BrightBack";
			this.BrightBack.Size = new Size(34, 21);
			this.BrightBack.TabIndex = 12;
			this.BrightBack.TabStop = false;
			this.BrightBack.Visible = false;
			this.StretchContainer.BackColor = Color.Transparent;
			this.StretchContainer.BackgroundImageLayout = ImageLayout.Center;
			this.StretchContainer.Location = new Point(129, 129);
			this.StretchContainer.Name = "StretchContainer";
			this.StretchContainer.Size = new Size(50, 50);
			this.StretchContainer.SizeMode = PictureBoxSizeMode.CenterImage;
			this.StretchContainer.TabIndex = 14;
			this.StretchContainer.TabStop = false;
			this.timer2.Enabled = true;
			this.timer2.Interval = 1;
			this.groupBox4.Controls.Add(this.ResetAllStretches);
			this.groupBox4.Controls.Add(this.LockStretch);
			this.groupBox4.Controls.Add(this.bottom);
			this.groupBox4.Controls.Add(this.right);
			this.groupBox4.Controls.Add(this.top);
			this.groupBox4.Controls.Add(this.left);
			this.groupBox4.Controls.Add(this.LTRD);
			this.groupBox4.Controls.Add(this.ResetStrech);
			this.groupBox4.Controls.Add(this.label3);
			this.groupBox4.Controls.Add(this.StretchIndexNum);
			this.groupBox4.Controls.Add(this.panel2);
			this.groupBox4.Controls.Add(this.General);
			this.groupBox4.Enabled = false;
			this.groupBox4.Location = new Point(8, 451);
			this.groupBox4.Name = "groupBox4";
			this.groupBox4.Size = new Size(140, 181);
			this.groupBox4.TabIndex = 10;
			this.groupBox4.TabStop = false;
			this.groupBox4.Text = "Stretches";
			this.ResetAllStretches.Location = new Point(13, 152);
			this.ResetAllStretches.Name = "ResetAllStretches";
			this.ResetAllStretches.Size = new Size(114, 22);
			this.ResetAllStretches.TabIndex = 13;
			this.ResetAllStretches.Text = "Reset All";
			this.ResetAllStretches.UseVisualStyleBackColor = true;
			this.ResetAllStretches.Click += new EventHandler(this.ResetAllStretchesClick);
			this.LockStretch.BackgroundImage = (Image)componentResourceManager.GetObject("LockStretch.BackgroundImage");
			this.LockStretch.BackgroundImageLayout = ImageLayout.Center;
			this.LockStretch.FlatStyle = 0;
			this.LockStretch.Location = new Point(2, 81);
			this.LockStretch.Name = "LockStretch";
			this.LockStretch.Size = new Size(16, 17);
			this.LockStretch.TabIndex = 10;
			this.LockStretch.UseVisualStyleBackColor = true;
			this.LockStretch.Click += new EventHandler(this.LockStretchClick);
			this.bottom.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.bottom.Location = new Point(72, 79);
			this.bottom.Name = "bottom";
			this.bottom.Size = new Size(43, 20);
			this.bottom.TabIndex = 8;
			this.bottom.ValueChanged += new EventHandler(this.BottomValueChanged);
			this.right.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.right.Location = new Point(23, 79);
			this.right.Name = "right";
			this.right.Size = new Size(43, 20);
			this.right.TabIndex = 7;
			this.right.ValueChanged += new EventHandler(this.RightValueChanged);
			this.top.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.top.Location = new Point(72, 57);
			this.top.Name = "top";
			this.top.Size = new Size(43, 20);
			this.top.TabIndex = 6;
			this.top.ValueChanged += new EventHandler(this.TopValueChanged);
			this.left.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.left.Location = new Point(23, 57);
			this.left.Name = "left";
			this.left.Size = new Size(43, 20);
			this.left.TabIndex = 5;
			this.left.ValueChanged += new EventHandler(this.LeftValueChanged);
			this.LTRD.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.LTRD.Location = new Point(21, 105);
			this.LTRD.Name = "LTRD";
			this.LTRD.Size = new Size(98, 20);
			this.LTRD.TabIndex = 4;
			this.LTRD.Text = "0;0;0;0";
			this.LTRD.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			this.LTRD.TextChanged += new EventHandler(this.LTRDTextChanged);
			this.ResetStrech.Location = new Point(13, 128);
			this.ResetStrech.Name = "ResetStrech";
			this.ResetStrech.Size = new Size(114, 22);
			this.ResetStrech.TabIndex = 3;
			this.ResetStrech.Text = "Reset Current";
			this.ResetStrech.UseVisualStyleBackColor = true;
			this.ResetStrech.Click += new EventHandler(this.ResetStrechClick);
			this.label3.Location = new Point(13, 16);
			this.label3.Name = "label3";
			this.label3.Size = new Size(50, 20);
			this.label3.TabIndex = 2;
			this.label3.Text = "Index";
			this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			this.StretchIndexNum.Location = new Point(69, 16);
			NumericUpDown arg_1998_0 = this.StretchIndexNum;
			int[] array6 = new int[4];
			array6[0] = 1000;
			arg_1998_0.Maximum = new decimal(array6);
			this.StretchIndexNum.Name = "StretchIndexNum";
			this.StretchIndexNum.Size = new Size(51, 20);
			this.StretchIndexNum.TabIndex = 1;
			this.StretchIndexNum.ValueChanged += new EventHandler(this.StretchIndexNumValueChanged);
			this.panel2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.panel2.Location = new Point(21, 55);
			this.panel2.Name = "panel2";
			this.panel2.Size = new Size(96, 46);
			this.panel2.TabIndex = 9;
			this.General.Font = new Font("Microsoft Sans Serif", 10f, FontStyle.Regular);
			this.General.Location = new Point(13, 38);
			this.General.Name = "General";
			this.General.Size = new Size(106, 17);
			this.General.TabIndex = 11;
			this.General.Text = "Global changes";
			this.General.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			this.General.UseVisualStyleBackColor = true;
			this.StretchBox.BackgroundImage = (Image)componentResourceManager.GetObject("StretchBox.BackgroundImage");
			this.StretchBox.ContextMenuStrip = this.contextMenuStrip1;
			this.StretchBox.Controls.Add(this.StretchContainer);
			this.StretchBox.Cursor = Cursors.Default;
			this.StretchBox.Location = new Point(160, 326);
			this.StretchBox.Name = "StretchBox";
			this.StretchBox.Size = new Size(256, 256);
			this.StretchBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
			this.StretchBox.TabIndex = 15;
			this.StretchBox.TabStop = false;
			this.contextMenuStrip1.Items.AddRange(new ToolStripItem[]
			{
				this.horizontalFlipToolStripMenuItem,
				this.verticalFlipToolStripMenuItem,
				this.centerTheStretchToolStripMenuItem,
				this.sizeToolStripMenuItem,
				this.toolStripMenuItem3,
				this.allToolStripMenuItem1
			});
			this.contextMenuStrip1.Name = "contextMenuStrip1";
			this.contextMenuStrip1.Size = new Size(169, 136);
			this.horizontalFlipToolStripMenuItem.Name = "horizontalFlipToolStripMenuItem";
			this.horizontalFlipToolStripMenuItem.Size = new Size(168, 22);
			this.horizontalFlipToolStripMenuItem.Text = "Horizontal flip";
			this.horizontalFlipToolStripMenuItem.Click += new EventHandler(this.HorizontalFlipToolStripMenuItemClick);
			this.verticalFlipToolStripMenuItem.Name = "verticalFlipToolStripMenuItem";
			this.verticalFlipToolStripMenuItem.Size = new Size(168, 22);
			this.verticalFlipToolStripMenuItem.Text = "Vertical flip";
			this.verticalFlipToolStripMenuItem.Click += new EventHandler(this.VerticalFlipToolStripMenuItemClick);
			this.centerTheStretchToolStripMenuItem.Name = "centerTheStretchToolStripMenuItem";
			this.centerTheStretchToolStripMenuItem.Size = new Size(168, 22);
			this.centerTheStretchToolStripMenuItem.Text = "Center the stretch";
			this.centerTheStretchToolStripMenuItem.Click += new EventHandler(this.CenterTheStretchToolStripMenuItemClick);
			this.sizeToolStripMenuItem.Name = "sizeToolStripMenuItem";
			this.sizeToolStripMenuItem.Size = new Size(168, 22);
			this.sizeToolStripMenuItem.Text = "100% Size";
			this.sizeToolStripMenuItem.Click += new EventHandler(this.SizeToolStripMenuItemClick);
			this.toolStripMenuItem3.Name = "toolStripMenuItem3";
			this.toolStripMenuItem3.Size = new Size(168, 22);
			this.toolStripMenuItem3.Text = "16:9";
			this.toolStripMenuItem3.Click += new EventHandler(this.ToolStripMenuItem3Click);
			this.allToolStripMenuItem1.Name = "allToolStripMenuItem1";
			this.allToolStripMenuItem1.Size = new Size(168, 22);
			this.allToolStripMenuItem1.Text = "16:9 All";
			this.allToolStripMenuItem1.Click += new EventHandler(this.AllToolStripMenuItem1Click);
			this.open.BackgroundImage = (Image)componentResourceManager.GetObject("open.BackgroundImage");
			this.open.Location = new Point(398, 285);
			this.open.Name = "open";
			this.open.Size = new Size(34, 21);
			this.open.TabIndex = 16;
			this.open.TabStop = false;
			this.open.Visible = false;
			this.close.BackgroundImage = (Image)componentResourceManager.GetObject("close.BackgroundImage");
			this.close.Location = new Point(438, 285);
			this.close.Name = "close";
			this.close.Size = new Size(34, 21);
			this.close.TabIndex = 17;
			this.close.TabStop = false;
			this.close.Visible = false;
			this.starting_timer.Interval = 10;
			this.SearchSEQD.Font = new Font("Microsoft Sans Serif", 10f, FontStyle.Regular);
			this.SearchSEQD.Location = new Point(182, 8);
			this.SearchSEQD.Name = "SearchSEQD";
			this.SearchSEQD.Size = new Size(81, 32);
			this.SearchSEQD.TabIndex = 18;
			this.SearchSEQD.Text = "Search SEQD";
			this.SearchSEQD.UseVisualStyleBackColor = true;
			this.SearchSEQD.Click += new EventHandler(this.SearchSEQDClick);
			this.StartSearch.Tick += new EventHandler(this.StartSearchTick);
			this.groupBox5.Controls.Add(this.ResetAllBorders);
			this.groupBox5.Controls.Add(this.Draw);
			this.groupBox5.Controls.Add(this.ResetBorder);
			this.groupBox5.Controls.Add(this.RGBALoc);
			this.groupBox5.Controls.Add(this.alphaTrack);
			this.groupBox5.Controls.Add(this.blueTrack);
			this.groupBox5.Controls.Add(this.greenTrack);
			this.groupBox5.Controls.Add(this.redTrack);
			this.groupBox5.Enabled = false;
			this.groupBox5.Location = new Point(8, 287);
			this.groupBox5.Name = "groupBox5";
			this.groupBox5.Size = new Size(140, 166);
			this.groupBox5.TabIndex = 20;
			this.groupBox5.TabStop = false;
			this.groupBox5.Text = "Channels";
			this.ResetAllBorders.Location = new Point(13, 138);
			this.ResetAllBorders.Name = "ResetAllBorders";
			this.ResetAllBorders.Size = new Size(114, 22);
			this.ResetAllBorders.TabIndex = 15;
			this.ResetAllBorders.Text = "Reset All";
			this.ResetAllBorders.UseVisualStyleBackColor = true;
			this.ResetAllBorders.Click += new EventHandler(this.ResetAllBordersClick);
			this.Draw.Font = new Font("Microsoft Sans Serif", 9f, FontStyle.Regular);
			this.Draw.Location = new Point(87, 16);
			this.Draw.Name = "Draw";
			this.Draw.Size = new Size(49, 34);
			this.Draw.TabIndex = 25;
			this.Draw.Text = "Preview";
			this.Draw.UseVisualStyleBackColor = true;
			this.Draw.Click += new EventHandler(this.DrawClick);
			this.ResetBorder.Location = new Point(13, 114);
			this.ResetBorder.Name = "ResetBorder";
			this.ResetBorder.Size = new Size(114, 22);
			this.ResetBorder.TabIndex = 14;
			this.ResetBorder.Text = "Reset Current";
			this.ResetBorder.UseVisualStyleBackColor = true;
			this.ResetBorder.Click += new EventHandler(this.ResetBorderClick);
			this.RGBALoc.DropDownStyle = ComboBoxStyle.DropDown;
			this.RGBALoc.FormattingEnabled = true;
			this.RGBALoc.Items.AddRange(new object[]
			{
				"Top-Left",
				"Top-Right",
				"Bottom-Left",
				"Bottom-Right"
			});
			this.RGBALoc.Location = new Point(21, 88);
			this.RGBALoc.Name = "RGBALoc";
			this.RGBALoc.Size = new Size(95, 21);
			this.RGBALoc.TabIndex = 24;
			this.RGBALoc.SelectedIndexChanged += new EventHandler(this.RGBALocSelectedIndexChanged);
			this.alphaTrack.LargeChange = 1;
			this.alphaTrack.Location = new Point(68, 15);
			this.alphaTrack.Maximum = 255;
			this.alphaTrack.Name = "alphaTrack";
			this.alphaTrack.Orientation = Orientation.Vertical;
			this.alphaTrack.Size = new Size(45, 69);
			this.alphaTrack.TabIndex = 22;
			this.alphaTrack.TickStyle = 0;
			this.alphaTrack.Value = 128;
			this.alphaTrack.Scroll += new EventHandler(this.AlphaTrackScroll);
			this.blueTrack.BackColor = Color.FromArgb(80, 80, 255);
			this.blueTrack.LargeChange = 1;
			this.blueTrack.Location = new Point(47, 15);
			this.blueTrack.Maximum = 255;
			this.blueTrack.Name = "blueTrack";
			this.blueTrack.Orientation = Orientation.Vertical;
			this.blueTrack.Size = new Size(45, 69);
			this.blueTrack.TabIndex = 21;
			this.blueTrack.TickStyle = 0;
			this.blueTrack.Value = 128;
			this.blueTrack.Scroll += new EventHandler(this.BlueTrackScroll);
			this.greenTrack.BackColor = Color.FromArgb(80, 255, 80);
			this.greenTrack.LargeChange = 1;
			this.greenTrack.Location = new Point(27, 15);
			this.greenTrack.Maximum = 255;
			this.greenTrack.Name = "greenTrack";
			this.greenTrack.Orientation = Orientation.Vertical;
			this.greenTrack.Size = new Size(45, 69);
			this.greenTrack.TabIndex = 20;
			this.greenTrack.TickStyle = 0;
			this.greenTrack.Value = 128;
			this.greenTrack.Scroll += new EventHandler(this.GreenTrackScroll);
			this.PartialMode.Font = new Font("Microsoft Sans Serif", 9f, FontStyle.Regular);
			this.PartialMode.Location = new Point(95, 337);
			this.PartialMode.Name = "PartialMode";
			this.PartialMode.Size = new Size(52, 32);
			this.PartialMode.TabIndex = 26;
			this.PartialMode.Text = "Partial mode";
			this.PartialMode.TextAlign = ContentAlignment.MiddleCenter;
			this.PartialMode.UseVisualStyleBackColor = true;
			base.AutoScaleDimensions = new SizeF(6f, 13f);
			base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.AutoSize = true;
			base.AutoSizeMode = 0;
			base.ClientSize = new Size(514, 694);
			base.Controls.Add(this.PartialMode);
			base.Controls.Add(this.SearchSEQD);
			base.Controls.Add(this.close);
			base.Controls.Add(this.open);
			base.Controls.Add(this.StretchBox);
			base.Controls.Add(this.pictureBoxBMP);
			base.Controls.Add(this.BrightBack);
			base.Controls.Add(this.DarkBack);
			base.Controls.Add(this.groupBox1);
			base.Controls.Add(this.BackChange);
			base.Controls.Add(this.Save2DD);
			base.Controls.Add(this.buttonOpen);
			base.Controls.Add(this.groupBox2);
			base.Controls.Add(this.groupBox5);
			base.Controls.Add(this.groupBox4);
			base.Controls.Add(this.groupBox3);
			base.MaximizeBox = false;
			base.MinimizeBox = false;
			this.MinimumSize = new Size(370, 310);
			base.Name = "MainForm";
			base.Padding = new Padding(3);
			this.Text = "BIEN";
			((ISupportInitialize)this.pictureBoxBMP).EndInit();
			this.groupBox1.ResumeLayout(false);
			this.groupBox2.ResumeLayout(false);
			this.groupBox2.PerformLayout();
			this.height_.EndInit();
			this.width_.EndInit();
			this.y_pos.EndInit();
			this.x_pos.EndInit();
			this.TrimIndexNum.EndInit();
			this.redTrack.EndInit();
			this.groupBox3.ResumeLayout(false);
			((ISupportInitialize)this.DarkBack).EndInit();
			((ISupportInitialize)this.BrightBack).EndInit();
			((ISupportInitialize)this.StretchContainer).EndInit();
			this.groupBox4.ResumeLayout(false);
			this.groupBox4.PerformLayout();
			this.bottom.EndInit();
			this.right.EndInit();
			this.top.EndInit();
			this.left.EndInit();
			this.StretchIndexNum.EndInit();
			((ISupportInitialize)this.StretchBox).EndInit();
			this.StretchBox.ResumeLayout(false);
			this.contextMenuStrip1.ResumeLayout(false);
			((ISupportInitialize)this.open).EndInit();
			((ISupportInitialize)this.close).EndInit();
			this.groupBox5.ResumeLayout(false);
			this.groupBox5.PerformLayout();
			this.alphaTrack.EndInit();
			this.blueTrack.EndInit();
			this.greenTrack.EndInit();
			base.ResumeLayout(false);
			base.PerformLayout();
		}

		[DllImport("kernel32.dll")]
		private static extern void WriteProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, byte[] lpBuffer, int nSize, [Out] int lpNumberOfBytesWritten);

		[DllImport("kernel32.dll")]
		private static extern IntPtr OpenProcess(int dwDesiredAccess, bool bInheritHandle, int dwProcessId);

		[DllImport("kernel32.dll")]
		private static extern bool ReadProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, [Out] byte[] lpBuffer, int dwSize, out int lpNumberOfBytesRead);

		public void WriteBytes(int address, byte[] ValueToWrite, int numberOfBytesToWrite)
		{
			try
			{
				MainForm.WriteProcessMemory(MainForm.OpenProcess(2035711, false, this.PCSX.Id), (IntPtr)address, ValueToWrite, numberOfBytesToWrite, 0);
			}
			catch
			{
			}
		}

		public byte[] ReadBytesRAM(int address, int numberOfBytesToRead)
		{
			byte[] array = new byte[numberOfBytesToRead];
			try
			{
				int num;
				MainForm.ReadProcessMemory(MainForm.OpenProcess(2035711, false, this.PCSX.Id), new IntPtr(address), array, numberOfBytesToRead, out num);
			}
			catch
			{
			}
			return array;
		}

		private void PCSX2_Search(object sender, EventArgs e)
		{
			try
			{
				Process.GetProcessById(this.PCSX.Id);
				this.IngamePreview.Enabled = true;
			}
			catch
			{
				this.IngamePreview.Enabled = false;
				bool flag = false;
				Process[] processes = Process.GetProcesses();
				for (int i = 0; i < processes.Length; i++)
				{
					Process process = processes[i];
					Application.DoEvents();
					try{
						if (process.ProcessName.Contains("pcsx2"))
						{
							this.IngamePreview.Enabled = true;
							flag = true;
							this.ProcessName.Text = "Linked to [" + process.ProcessName + "]";
							this.PCSX = process;
						}
					}
					catch(Exception er)
					{
						Debug.Print("Can't check if pcsx2 is there! Might be running on mono linux {0}", er);
					}
				}
				if (!flag)
				{
					this.ProcessName.Text = "Please open PCSX2...";
				}
			}
		}

		private void UpdateCommandAddress()
		{
			if (this.file_address_RAM == 0 || BitConverter.ToString(this.ReadBytesRAM(this.file_address_RAM, 4)) != "42-41-52-01")
			{
				byte[] array = this.ReadBytesRAM(540016640, 268435456);
				int num = array.Length;
				byte[] array2 = new byte[]
				{
					this.file_bytes[16],
					this.file_bytes[28],
					this.file_bytes[29],
					this.file_bytes[30],
					this.file_bytes[31]
				};
				for (int i = 0; i <= num - 48; i += 16)
				{
					if (array[i + 16] == array2[0] && array[i + 28] == array2[1] && array[i + 29] == array2[2] && array[i + 30] == array2[3] && array[i + 31] == array2[4])
					{
						this.file_address_RAM = 540016640 + i;
						return;
					}
				}
			}
		}

		private void IngamePreviewClick(object sender, EventArgs e)
		{
			this.UpdateFile();
			this.UpdateCommandAddress();
			if (BitConverter.ToString(this.ReadBytesRAM(this.file_address_RAM, 4)) == "42-41-52-01")
			{
				this.WriteBytes(this.file_address_RAM + this.IMD_address, this.SubArray(this.file_bytes, (long)this.IMD_address, (long)this.IMD_length), this.IMD_length);
				int num = this.RI(this.SEQD_address + 8);
				for (int i = 0; i < num; i++)
				{
					this.WriteBytes(this.file_address_RAM + 76 + this.SEQD_address + i * 44, this.filters[i][0], 4);
					this.WriteBytes(this.file_address_RAM + 76 + 4 + this.SEQD_address + i * 44, this.filters[i][1], 4);
					this.WriteBytes(this.file_address_RAM + 76 + 8 + this.SEQD_address + i * 44, this.filters[i][2], 4);
					this.WriteBytes(this.file_address_RAM + 76 + 12 + this.SEQD_address + i * 44, this.filters[i][3], 4);
					this.WriteBytes(this.file_address_RAM + 52 + this.SEQD_address + i * 44, MainForm.Combine(MainForm.Combine(MainForm.Combine(BitConverter.GetBytes(this.trims[i][0]), BitConverter.GetBytes(this.trims[i][1])), BitConverter.GetBytes(this.trims[i][2])), BitConverter.GetBytes(this.trims[i][3])), 16);
				}
				for (int j = 0; j < this.stretches_addresses.Count; j++)
				{
					for (int k = 0; k < this.stretches_addresses[j].Count; k++)
					{
						this.WriteBytes(this.file_address_RAM + this.stretches_addresses[j][k], MainForm.Combine(MainForm.Combine(MainForm.Combine(BitConverter.GetBytes(this.stretches[j][k][0]), BitConverter.GetBytes(this.stretches[j][k][1])), BitConverter.GetBytes(this.stretches[j][k][2])), BitConverter.GetBytes(this.stretches[j][k][3])), 16);
					}
				}
			}
		}

		public void Srk_Extension_Init()
		{
			this.left.Maximum = 1000m;
			this.left.Minimum = -1000m;
			this.top.Maximum = 1000m;
			this.top.Minimum = -1000m;
			this.right.Maximum = 1000m;
			this.right.Minimum = -1000m;
			this.bottom.Maximum = 1000m;
			this.bottom.Minimum = -1000m;
			this.InitTrimsAndStretches();
		}

		private Bitmap ApplyChannels(Bitmap input, int position, byte a, byte r, byte g, byte b)
		{
			if (a == 128 && r == 128 && g == 128 && b == 128)
			{
				return input;
			}
			Bitmap bitmap = new Bitmap(input.Width, input.Height);
			Graphics graphics = Graphics.FromImage(bitmap);
			graphics.InterpolationMode = InterpolationMode.Default;
			graphics.CompositingQuality = CompositingQuality.Default;
			graphics.SmoothingMode = SmoothingMode.Default;
			LinearGradientBrush linearGradientBrush = new LinearGradientBrush((new Point[]
			{
				new Point(0, 0),
				new Point(bitmap.Width, 0),
				new Point(0, bitmap.Height),
				new Point(bitmap.Width, bitmap.Height)
			})[position], (new Point[]
			{
				new Point(bitmap.Width, bitmap.Height),
				new Point(0, bitmap.Height),
				new Point(bitmap.Width, 0),
				new Point(0, 0)
			})[position], Color.Transparent, Color.Red);
			if (this.PartialMode.Checked)
			{
				ColorBlend colorBlend = new ColorBlend();
				colorBlend.Colors = new Color[]
				{
					Color.Transparent,
					Color.Red,
					Color.Red
				};
				colorBlend.Positions = new float[]
				{
					default(float),
					0.6f,
					1f
				};
				linearGradientBrush.InterpolationColors = colorBlend;
			}
			graphics.FillRectangle(linearGradientBrush, new Rectangle(0, 0, bitmap.Width, bitmap.Height));
			Bitmap bitmap2 = new Bitmap(input);
			Bitmap bitmap3 = new Bitmap(input.Width, input.Height);
			bitmap3.MakeTransparent();
			bitmap2.MakeTransparent();
			Graphics graphics2 = Graphics.FromImage(bitmap3);
			ImageAttributes imageAttributes = new ImageAttributes();
			float[][] array = new float[5][];
			float[][] arg_24C_0 = array;
			int arg_24C_1 = 0;
			float[] array2 = new float[5];
			array2[0] = (float)r / 128f;
			arg_24C_0[arg_24C_1] = array2;
			float[][] arg_267_0 = array;
			int arg_267_1 = 1;
			float[] array3 = new float[5];
			array3[1] = (float)g / 128f;
			arg_267_0[arg_267_1] = array3;
			float[][] arg_282_0 = array;
			int arg_282_1 = 2;
			float[] array4 = new float[5];
			array4[2] = (float)b / 128f;
			arg_282_0[arg_282_1] = array4;
			float[][] arg_29C_0 = array;
			int arg_29C_1 = 3;
			float[] array5 = new float[5];
			array5[3] = (float)a / 128f;
			arg_29C_0[arg_29C_1] = array5;
			float[][] arg_2AA_0 = array;
			int arg_2AA_1 = 4;
			float[] array6 = new float[5];
			arg_2AA_0[arg_2AA_1] = array6;
			float[][] array7 = array;
			ColorMatrix colorMatrix = new ColorMatrix(array7);
			imageAttributes.SetColorMatrix(colorMatrix, 0, ColorAdjustType.Bitmap);
			graphics2.DrawImage(bitmap2, new Rectangle(0, 0, bitmap2.Width, bitmap2.Height), 0, 0, bitmap2.Width, bitmap2.Height, GraphicsUnit.Pixel, imageAttributes);
			bitmap3.Save("A:\\text.png");
			for (int i = 0; i < bitmap3.Width; i++)
			{
				for (int j = 0; j < bitmap3.Height; j++)
				{
					if (bitmap.GetPixel(i, j).A < 240)
					{
						double num = (double)bitmap.GetPixel(i, j).A / 255.0;
						bitmap2.SetPixel(i, j, Color.FromArgb((int)((double)bitmap2.GetPixel(i, j).A * num), bitmap2.GetPixel(i, j)));
					}
				}
			}
			graphics2.DrawImage(bitmap2, 0, 0);
			return bitmap3;
		}

		private void InitTrimsAndStretches()
		{
			int num = this.RI(this.SEQD_address + 8);
			int num2 = this.SEQD_address + 48 + 44 * num;
			int num3 = this.RI(this.SEQD_address + 16);
			this.TrimIndexNum.Maximum = num - 1;
			this.trims.Clear();
			this.trims_backup.Clear();
			this.stretches.Clear();
			this.stretches_backup.Clear();
			this.stretches_addresses.Clear();
			this.filters.Clear();
			this.filters_backup.Clear();
			for (int i = 0; i < num; i++)
			{
				int[] array = new int[]
				{
					this.RI(52 + this.SEQD_address + i * 44),
					this.RI(56 + this.SEQD_address + i * 44),
					this.RI(60 + this.SEQD_address + i * 44),
					this.RI(64 + this.SEQD_address + i * 44)
				};
				this.trims.Add(new int[]
				{
					array[0],
					array[1],
					array[2],
					array[3]
				});
				this.trims_backup.Add(new int[]
				{
					array[0],
					array[1],
					array[2],
					array[3]
				});
				byte[][] array2 = new byte[][]
				{
					new byte[]
					{
						this.file_bytes[76 + this.SEQD_address + i * 44],
						this.file_bytes[77 + this.SEQD_address + i * 44],
						this.file_bytes[78 + this.SEQD_address + i * 44],
						this.file_bytes[79 + this.SEQD_address + i * 44]
					},
					new byte[]
					{
						this.file_bytes[80 + this.SEQD_address + i * 44],
						this.file_bytes[81 + this.SEQD_address + i * 44],
						this.file_bytes[82 + this.SEQD_address + i * 44],
						this.file_bytes[83 + this.SEQD_address + i * 44]
					},
					new byte[]
					{
						this.file_bytes[84 + this.SEQD_address + i * 44],
						this.file_bytes[85 + this.SEQD_address + i * 44],
						this.file_bytes[86 + this.SEQD_address + i * 44],
						this.file_bytes[87 + this.SEQD_address + i * 44]
					},
					new byte[]
					{
						this.file_bytes[88 + this.SEQD_address + i * 44],
						this.file_bytes[89 + this.SEQD_address + i * 44],
						this.file_bytes[90 + this.SEQD_address + i * 44],
						this.file_bytes[91 + this.SEQD_address + i * 44]
					}
				};
				this.filters.Add(new byte[][]
				{
					new byte[]
					{
						array2[0][0],
						array2[0][1],
						array2[0][2],
						array2[0][3]
					},
					new byte[]
					{
						array2[1][0],
						array2[1][1],
						array2[1][2],
						array2[1][3]
					},
					new byte[]
					{
						array2[2][0],
						array2[2][1],
						array2[2][2],
						array2[2][3]
					},
					new byte[]
					{
						array2[3][0],
						array2[3][1],
						array2[3][2],
						array2[3][3]
					}
				});
				this.filters_backup.Add(new byte[][]
				{
					new byte[]
					{
						array2[0][0],
						array2[0][1],
						array2[0][2],
						array2[0][3]
					},
					new byte[]
					{
						array2[1][0],
						array2[1][1],
						array2[1][2],
						array2[1][3]
					},
					new byte[]
					{
						array2[2][0],
						array2[2][1],
						array2[2][2],
						array2[2][3]
					},
					new byte[]
					{
						array2[3][0],
						array2[3][1],
						array2[3][2],
						array2[3][3]
					}
				});
			}
			while (this.stretches.Count < num)
			{
				this.stretches.Add(new List<int[]>(0));
				this.stretches_backup.Add(new List<int[]>(0));
				this.stretches_addresses.Add(new List<int>(0));
			}
			for (int j = 0; j < num3; j++)
			{
				int[] array3 = new int[]
				{
					this.RI(num2 + 20 * j),
					this.RI(4 + num2 + 20 * j),
					this.RI(8 + num2 + 20 * j),
					this.RI(12 + num2 + 20 * j)
				};
				this.stretches[this.RI(16 + num2 + 20 * j)].Add(new int[]
				{
					array3[0],
					array3[1],
					array3[2],
					array3[3]
				});
				this.stretches_backup[this.RI(16 + num2 + 20 * j)].Add(new int[]
				{
					array3[0],
					array3[1],
					array3[2],
					array3[3]
				});
				this.stretches_addresses[this.RI(16 + num2 + 20 * j)].Add(num2 + 20 * j);
			}
			this.TrimIndexNumValueChanged(null, null);
			this.StretchIndexNumValueChanged(null, null);
		}

		public void UpdateFile()
		{
			int num = this.RI(this.SEQD_address + 8);
			for (int i = 0; i < num; i++)
			{
				this.file_bytes[76 + this.SEQD_address + i * 44] = (new byte[]
				{
					this.filters[i][0][0]
				})[0];
				this.file_bytes[77 + this.SEQD_address + i * 44] = (new byte[]
				{
					this.filters[i][0][1]
				})[0];
				this.file_bytes[78 + this.SEQD_address + i * 44] = (new byte[]
				{
					this.filters[i][0][2]
				})[0];
				this.file_bytes[79 + this.SEQD_address + i * 44] = (new byte[]
				{
					this.filters[i][0][3]
				})[0];
				this.file_bytes[80 + this.SEQD_address + i * 44] = (new byte[]
				{
					this.filters[i][1][0]
				})[0];
				this.file_bytes[81 + this.SEQD_address + i * 44] = (new byte[]
				{
					this.filters[i][1][1]
				})[0];
				this.file_bytes[82 + this.SEQD_address + i * 44] = (new byte[]
				{
					this.filters[i][1][2]
				})[0];
				this.file_bytes[83 + this.SEQD_address + i * 44] = (new byte[]
				{
					this.filters[i][1][3]
				})[0];
				this.file_bytes[84 + this.SEQD_address + i * 44] = (new byte[]
				{
					this.filters[i][2][0]
				})[0];
				this.file_bytes[85 + this.SEQD_address + i * 44] = (new byte[]
				{
					this.filters[i][2][1]
				})[0];
				this.file_bytes[86 + this.SEQD_address + i * 44] = (new byte[]
				{
					this.filters[i][2][1]
				})[0];
				this.file_bytes[87 + this.SEQD_address + i * 44] = (new byte[]
				{
					this.filters[i][2][3]
				})[0];
				this.file_bytes[88 + this.SEQD_address + i * 44] = (new byte[]
				{
					this.filters[i][3][0]
				})[0];
				this.file_bytes[89 + this.SEQD_address + i * 44] = (new byte[]
				{
					this.filters[i][3][1]
				})[0];
				this.file_bytes[90 + this.SEQD_address + i * 44] = (new byte[]
				{
					this.filters[i][3][2]
				})[0];
				this.file_bytes[91 + this.SEQD_address + i * 44] = (new byte[]
				{
					this.filters[i][3][3]
				})[0];
				this.WI(52 + this.SEQD_address + i * 44, this.trims[i][0]);
				this.WI(56 + this.SEQD_address + i * 44, this.trims[i][1]);
				this.WI(60 + this.SEQD_address + i * 44, this.trims[i][2]);
				this.WI(64 + this.SEQD_address + i * 44, this.trims[i][3]);
			}
			for (int j = 0; j < this.stretches_addresses.Count; j++)
			{
				for (int k = 0; k < this.stretches_addresses[j].Count; k++)
				{
					this.WI(this.stretches_addresses[j][k], this.stretches[j][k][0]);
					this.WI(this.stretches_addresses[j][k] + 4, this.stretches[j][k][1]);
					this.WI(this.stretches_addresses[j][k] + 8, this.stretches[j][k][2]);
					this.WI(this.stretches_addresses[j][k] + 12, this.stretches[j][k][3]);
				}
			}
		}

		private void TrimIndexNumValueChanged(object sender, EventArgs e)
		{
			if (this.trims.Count == 0)
			{
				return;
			}
			this.StretchIndexNum.Value = 0m;
			this.UpdateTrimValues();
			this.UpdateTrimRectangle();
			this.StretchIndexNum.Maximum = (this.stretches[(int)this.TrimIndexNum.Value].Count > 0) ? (this.stretches[(int)this.TrimIndexNum.Value].Count - 1) : 0;
			Control arg_F4_0 = this.StretchIndexNum;
			Control arg_ED_0 = this.left;
			Control arg_E4_0 = this.top;
			Control arg_DC_0 = this.right;
			Control arg_D4_0 = this.bottom;
			bool flag;
			this.LTRD.Enabled = flag = (this.stretches[(int)this.TrimIndexNum.Value].Count > 0);
			bool flag2;
			arg_D4_0.Enabled = flag2 = flag;
			bool flag3;
			arg_DC_0.Enabled = flag3 = flag2;
			bool flag4;
			arg_E4_0.Enabled = flag4 = flag3;
			bool enabled;
			arg_ED_0.Enabled = enabled = flag4;
			arg_F4_0.Enabled = enabled;
			this.UpdateStretchesValues();
			this.UpdateStretchRectangle(false);
			this.RGBALoc.SelectedIndex = 0;
			this.RGBALocSelectedIndexChanged(null, null);
		}

		private void RGBALocSelectedIndexChanged(object sender, EventArgs e)
		{
			this.redTrack.Value = (int)this.filters[(int)this.TrimIndexNum.Value][this.RGBALoc.SelectedIndex][0];
			this.greenTrack.Value = (int)this.filters[(int)this.TrimIndexNum.Value][this.RGBALoc.SelectedIndex][1];
			this.blueTrack.Value = (int)this.filters[(int)this.TrimIndexNum.Value][this.RGBALoc.SelectedIndex][2];
			this.alphaTrack.Value = (int)this.filters[(int)this.TrimIndexNum.Value][this.RGBALoc.SelectedIndex][3];
		}

		private void StretchIndexNumValueChanged(object sender, EventArgs e)
		{
			if (this.trims.Count == 0)
			{
				return;
			}
			this.UpdateStretchesValues();
			this.UpdateStretchRectangle(false);
		}

		public int RI(int address)
		{
			return BitConverter.ToInt32(this.SubArray(this.file_bytes, (long)address, 4L), 0);
		}

		public void WI(int address, int valeur)
		{
			byte[] bytes = BitConverter.GetBytes(valeur);
			for (int i = 0; i < 4; i++)
			{
				this.file_bytes[address + i] = bytes[i];
			}
		}

		private void UpdateTrimValues()
		{
			this.RemoveXYWHEvents();
			this.UpdateMaximums();
			try
			{
				this.x_pos.Value = this.trims[(int)this.TrimIndexNum.Value][0];
				this.y_pos.Value = this.trims[(int)this.TrimIndexNum.Value][1];
				this.width_.Value = this.trims[(int)this.TrimIndexNum.Value][2];
				this.height_.Value = this.trims[(int)this.TrimIndexNum.Value][3];
				this.XYWH.Text = string.Concat(new string[]
				{
					this.x_pos.Value.ToString(),
					";",
					this.y_pos.Value.ToString(),
					";",
					this.width_.Value.ToString(),
					";",
					this.height_.Value.ToString()
				});
			}
			catch
			{
			}
			this.AddXYWHEvents();
			this.UpdateStretchRectangle(false);
			this.LockTrimClick(null, null);
		}

		private void UpdateStretchesValues()
		{
			this.RemoveLTRDEvents();
			try
			{
				this.left.Value = this.stretches[(int)this.TrimIndexNum.Value][(int)this.StretchIndexNum.Value][0];
				this.top.Value = this.stretches[(int)this.TrimIndexNum.Value][(int)this.StretchIndexNum.Value][1];
				this.right.Value = this.stretches[(int)this.TrimIndexNum.Value][(int)this.StretchIndexNum.Value][2];
				this.bottom.Value = this.stretches[(int)this.TrimIndexNum.Value][(int)this.StretchIndexNum.Value][3];
				this.LTRD.Text = string.Concat(new string[]
				{
					this.left.Value.ToString(),
					";",
					this.top.Value.ToString(),
					";",
					this.right.Value.ToString(),
					";",
					this.bottom.Value.ToString()
				});
			}
			catch
			{
				NumericUpDown arg_1D6_0 = this.left;
				NumericUpDown arg_1CF_0 = this.top;
				NumericUpDown arg_1C5_0 = this.right;
				decimal num;
				this.bottom.Value = num = 0m;
				decimal num2;
				arg_1C5_0.Value = num2 = num;
				decimal value;
				arg_1CF_0.Value = value = num2;
				arg_1D6_0.Value = value;
				this.LTRD.Text = "0;0;0;0";
			}
			this.AddLTRDEvents();
			this.UpdateStretchRectangle(false);
			this.LockStretchClick(null, null);
		}

		private void RemoveXYWHEvents()
		{
			this.XYWH.TextChanged -= new EventHandler(this.XYWH_TextChanged);
			this.x_pos.ValueChanged -= new EventHandler(this.X_posValueChanged);
			this.y_pos.ValueChanged -= new EventHandler(this.Y_posValueChanged);
			this.height_.ValueChanged -= new EventHandler(this.Height_ValueChanged);
			this.width_.ValueChanged -= new EventHandler(this.Width_ValueChanged);
		}

		private void AddXYWHEvents()
		{
			this.XYWH.TextChanged += new EventHandler(this.XYWH_TextChanged);
			this.x_pos.ValueChanged += new EventHandler(this.X_posValueChanged);
			this.y_pos.ValueChanged += new EventHandler(this.Y_posValueChanged);
			this.height_.ValueChanged += new EventHandler(this.Height_ValueChanged);
			this.width_.ValueChanged += new EventHandler(this.Width_ValueChanged);
		}

		private void RemoveLTRDEvents()
		{
			this.LTRD.TextChanged -= new EventHandler(this.LTRDTextChanged);
			this.left.ValueChanged -= new EventHandler(this.LeftValueChanged);
			this.top.ValueChanged -= new EventHandler(this.TopValueChanged);
			this.right.ValueChanged -= new EventHandler(this.RightValueChanged);
			this.bottom.ValueChanged -= new EventHandler(this.BottomValueChanged);
		}

		private void AddLTRDEvents()
		{
			this.LTRD.TextChanged += new EventHandler(this.LTRDTextChanged);
			this.left.ValueChanged += new EventHandler(this.LeftValueChanged);
			this.top.ValueChanged += new EventHandler(this.TopValueChanged);
			this.right.ValueChanged += new EventHandler(this.RightValueChanged);
			this.bottom.ValueChanged += new EventHandler(this.BottomValueChanged);
		}

		private void UpdateMaximums()
		{
			this.y_pos.Maximum = this.trims[(int)this.TrimIndexNum.Value][3];
			this.x_pos.Maximum = this.trims[(int)this.TrimIndexNum.Value][2];
			this.width_.Maximum = this.pictureBoxBMP.Width - 1;
			this.height_.Maximum = this.pictureBoxBMP.Height - 1;
		}

		private void X_posValueChanged(object sender, EventArgs e)
		{
			if (this.trims.Count == 0)
			{
				return;
			}
			int num = this.trims[(int)this.TrimIndexNum.Value][2] - this.trims[(int)this.TrimIndexNum.Value][0];
			if (this.lock_trim_leftop)
			{
				int num2 = this.trims[(int)this.TrimIndexNum.Value][0];
				int num3 = (int)this.x_pos.Value - num2;
				if (this.trims[(int)this.TrimIndexNum.Value][2] + num3 < this.Image_Backup.Width - 1)
				{
					this.trims[(int)this.TrimIndexNum.Value][2] += num3;
				}
				else
				{
					this.trims[(int)this.TrimIndexNum.Value][2] = this.Image_Backup.Width - 1;
				}
			}
			if (this.trims[(int)this.TrimIndexNum.Value][2] < this.Image_Backup.Width - 1)
			{
				this.trims[(int)this.TrimIndexNum.Value][0] = (int)this.x_pos.Value;
			}
			else
			{
				this.trims[(int)this.TrimIndexNum.Value][0] = this.Image_Backup.Width - 1 - num;
			}
			this.UpdateTrimValues();
			this.UpdateTrimRectangle();
			this.UpdateMaximums();
		}

		private void Y_posValueChanged(object sender, EventArgs e)
		{
			if (this.trims.Count == 0)
			{
				return;
			}
			int num = this.trims[(int)this.TrimIndexNum.Value][3] - this.trims[(int)this.TrimIndexNum.Value][1];
			if (this.lock_trim_leftop)
			{
				int num2 = this.trims[(int)this.TrimIndexNum.Value][1];
				int num3 = (int)this.y_pos.Value - num2;
				if (this.trims[(int)this.TrimIndexNum.Value][3] + num3 < this.Image_Backup.Height - 1)
				{
					this.trims[(int)this.TrimIndexNum.Value][3] += num3;
				}
				else
				{
					this.trims[(int)this.TrimIndexNum.Value][3] = this.Image_Backup.Height - 1;
				}
			}
			if (this.trims[(int)this.TrimIndexNum.Value][3] < this.Image_Backup.Height - 1)
			{
				this.trims[(int)this.TrimIndexNum.Value][1] = (int)this.y_pos.Value;
			}
			else
			{
				this.trims[(int)this.TrimIndexNum.Value][1] = this.Image_Backup.Height - 1 - num;
			}
			this.UpdateTrimValues();
			this.UpdateTrimRectangle();
			this.UpdateMaximums();
		}

		private void Width_ValueChanged(object sender, EventArgs e)
		{
			if (this.trims.Count == 0)
			{
				return;
			}
			this.trims[(int)this.TrimIndexNum.Value][2] = (int)this.width_.Value;
			this.UpdateTrimValues();
			this.UpdateTrimRectangle();
			this.UpdateMaximums();
		}

		private void Height_ValueChanged(object sender, EventArgs e)
		{
			if (this.trims.Count == 0)
			{
				return;
			}
			this.trims[(int)this.TrimIndexNum.Value][3] = (int)this.height_.Value;
			this.UpdateTrimValues();
			this.UpdateTrimRectangle();
			this.UpdateMaximums();
		}

		private void XYWH_TextChanged(object sender, EventArgs e)
		{
			if (this.trims.Count == 0)
			{
				return;
			}
			int selectionStart = this.XYWH.SelectionStart;
			string[] array = this.XYWH.Text.Split(new char[]
			{
				';'
			});
			try
			{
				if (int.Parse(array[0]) <= this.x_pos.Maximum)
				{
					this.trims[(int)this.TrimIndexNum.Value][0] = int.Parse(array[0]);
				}
				if (int.Parse(array[1]) <= this.y_pos.Maximum)
				{
					this.trims[(int)this.TrimIndexNum.Value][1] = int.Parse(array[1]);
				}
				if (int.Parse(array[2]) <= this.width_.Maximum)
				{
					this.trims[(int)this.TrimIndexNum.Value][2] = int.Parse(array[2]);
				}
				if (int.Parse(array[3]) <= this.height_.Maximum)
				{
					this.trims[(int)this.TrimIndexNum.Value][3] = int.Parse(array[3]);
				}
			}
			catch
			{
			}
			this.UpdateTrimValues();
			this.UpdateTrimRectangle();
			try
			{
				this.XYWH.Select(selectionStart, 0);
			}
			catch
			{
			}
		}

		private void XYWHLeave(object sender, EventArgs e)
		{
			this.UpdateTrimValues();
			this.left_XYWH = true;
		}

		private void XYWHClick(object sender, EventArgs e)
		{
			if (this.left_XYWH)
			{
				this.XYWH.Select(0, this.XYWH.Text.Length);
			}
			this.left_XYWH = false;
		}

		private void ResetTrimClick(object sender, EventArgs e)
		{
			if (this.trims.Count == 0)
			{
				return;
			}
			this.trims[(int)this.TrimIndexNum.Value] = new int[]
			{
				this.trims_backup[(int)this.TrimIndexNum.Value][0],
				this.trims_backup[(int)this.TrimIndexNum.Value][1],
				this.trims_backup[(int)this.TrimIndexNum.Value][2],
				this.trims_backup[(int)this.TrimIndexNum.Value][3]
			};
			this.UpdateTrimValues();
			this.UpdateTrimRectangle();
			this.UpdateStretchRectangle(false);
		}

		private void LeftValueChanged(object sender, EventArgs e)
		{
			if (this.trims.Count == 0)
			{
				return;
			}
			if (this.General.Checked)
			{
				int num = (int)this.left.Value - this.stretches[(int)this.TrimIndexNum.Value][(int)this.StretchIndexNum.Value][0];
				for (int i = 0; i < this.stretches.Count; i++)
				{
					for (int j = 0; j < this.stretches[i].Count; j++)
					{
						if (this.lock_stretch_leftop)
						{
							this.stretches[i][j][2] += num;
						}
						this.stretches[i][j][0] += num;
					}
				}
			}
			else
			{
				if (this.lock_stretch_leftop)
				{
					int num2 = this.stretches[(int)this.TrimIndexNum.Value][(int)this.StretchIndexNum.Value][0];
					int num3 = (int)this.left.Value - num2;
					this.stretches[(int)this.TrimIndexNum.Value][(int)this.StretchIndexNum.Value][2] += num3;
				}
				this.stretches[(int)this.TrimIndexNum.Value][(int)this.StretchIndexNum.Value][0] = (int)this.left.Value;
			}
			this.UpdateStretchesValues();
			this.UpdateStretchRectangle(false);
		}

		private void TopValueChanged(object sender, EventArgs e)
		{
			if (this.trims.Count == 0)
			{
				return;
			}
			if (this.General.Checked)
			{
				int num = (int)this.top.Value - this.stretches[(int)this.TrimIndexNum.Value][(int)this.StretchIndexNum.Value][1];
				for (int i = 0; i < this.stretches.Count; i++)
				{
					for (int j = 0; j < this.stretches[i].Count; j++)
					{
						if (this.lock_stretch_leftop)
						{
							this.stretches[i][j][3] += num;
						}
						this.stretches[i][j][1] += num;
					}
				}
			}
			else
			{
				if (this.lock_stretch_leftop)
				{
					int num2 = this.stretches[(int)this.TrimIndexNum.Value][(int)this.StretchIndexNum.Value][1];
					int num3 = (int)this.top.Value - num2;
					this.stretches[(int)this.TrimIndexNum.Value][(int)this.StretchIndexNum.Value][3] += num3;
				}
				this.stretches[(int)this.TrimIndexNum.Value][(int)this.StretchIndexNum.Value][1] = (int)this.top.Value;
			}
			this.UpdateStretchesValues();
			this.UpdateStretchRectangle(false);
		}

		private void RightValueChanged(object sender, EventArgs e)
		{
			if (this.trims.Count == 0)
			{
				return;
			}
			if (this.General.Checked)
			{
				int num = (int)this.right.Value - this.stretches[(int)this.TrimIndexNum.Value][(int)this.StretchIndexNum.Value][2];
				for (int i = 0; i < this.stretches.Count; i++)
				{
					for (int j = 0; j < this.stretches[i].Count; j++)
					{
						this.stretches[i][j][2] += num;
					}
				}
			}
			else
			{
				this.stretches[(int)this.TrimIndexNum.Value][(int)this.StretchIndexNum.Value][2] = (int)this.right.Value;
			}
			this.UpdateStretchesValues();
			this.UpdateStretchRectangle(false);
		}

		private void BottomValueChanged(object sender, EventArgs e)
		{
			if (this.trims.Count == 0)
			{
				return;
			}
			if (this.General.Checked)
			{
				int num = (int)this.bottom.Value - this.stretches[(int)this.TrimIndexNum.Value][(int)this.StretchIndexNum.Value][3];
				for (int i = 0; i < this.stretches.Count; i++)
				{
					for (int j = 0; j < this.stretches[i].Count; j++)
					{
						this.stretches[i][j][3] += num;
					}
				}
			}
			else
			{
				this.stretches[(int)this.TrimIndexNum.Value][(int)this.StretchIndexNum.Value][3] = (int)this.bottom.Value;
			}
			this.UpdateStretchesValues();
			this.UpdateStretchRectangle(false);
		}

		private void LTRDTextChanged(object sender, EventArgs e)
		{
			if (this.trims.Count == 0)
			{
				return;
			}
			int selectionStart = this.LTRD.SelectionStart;
			string[] array = this.LTRD.Text.Split(new char[]
			{
				';'
			});
			try
			{
				this.stretches[(int)this.TrimIndexNum.Value][(int)this.StretchIndexNum.Value][0] = int.Parse(array[0]);
				this.stretches[(int)this.TrimIndexNum.Value][(int)this.StretchIndexNum.Value][1] = int.Parse(array[1]);
				this.stretches[(int)this.TrimIndexNum.Value][(int)this.StretchIndexNum.Value][2] = int.Parse(array[2]);
				this.stretches[(int)this.TrimIndexNum.Value][(int)this.StretchIndexNum.Value][3] = int.Parse(array[3]);
			}
			catch
			{
			}
			try
			{
				this.LTRD.Select(selectionStart, 0);
			}
			catch
			{
			}
			this.UpdateStretchesValues();
			this.UpdateStretchRectangle(false);
		}

		private void ResetStrechClick(object sender, EventArgs e)
		{
			this.stretches[(int)this.TrimIndexNum.Value][(int)this.StretchIndexNum.Value] = new int[]
			{
				this.stretches_backup[(int)this.TrimIndexNum.Value][(int)this.StretchIndexNum.Value][0],
				this.stretches_backup[(int)this.TrimIndexNum.Value][(int)this.StretchIndexNum.Value][1],
				this.stretches_backup[(int)this.TrimIndexNum.Value][(int)this.StretchIndexNum.Value][2],
				this.stretches_backup[(int)this.TrimIndexNum.Value][(int)this.StretchIndexNum.Value][3]
			};
			this.UpdateStretchesValues();
			this.UpdateStretchRectangle(false);
		}

		public void UpdateStretchRectangle(bool colors_refresh)
		{
			try
			{
				int arg_1D_0 = this.trims[(int)this.TrimIndexNum.Value][0];
				int arg_3B_0 = this.trims[(int)this.TrimIndexNum.Value][1];
				int arg_59_0 = this.trims[(int)this.TrimIndexNum.Value][2];
				int arg_77_0 = this.trims[(int)this.TrimIndexNum.Value][3];
				Bitmap bitmap = new Bitmap(this.Image_Backup);
				bitmap = bitmap.Clone(this.TrimRectangle, bitmap.PixelFormat);
				if (colors_refresh)
				{
					this.filters[(int)this.TrimIndexNum.Value][this.RGBALoc.SelectedIndex][0] = (byte)this.redTrack.Value;
					this.filters[(int)this.TrimIndexNum.Value][this.RGBALoc.SelectedIndex][1] = (byte)this.greenTrack.Value;
					this.filters[(int)this.TrimIndexNum.Value][this.RGBALoc.SelectedIndex][2] = (byte)this.blueTrack.Value;
					this.filters[(int)this.TrimIndexNum.Value][this.RGBALoc.SelectedIndex][3] = (byte)this.alphaTrack.Value;
					for (int i = 0; i < 4; i++)
					{
						bitmap = this.ApplyChannels(bitmap, i, this.filters[(int)this.TrimIndexNum.Value][i][3], this.filters[(int)this.TrimIndexNum.Value][i][0], this.filters[(int)this.TrimIndexNum.Value][i][1], this.filters[(int)this.TrimIndexNum.Value][i][2]);
					}
				}
				int num = this.stretches[(int)this.TrimIndexNum.Value][(int)this.StretchIndexNum.Value][0];
				int num2 = this.stretches[(int)this.TrimIndexNum.Value][(int)this.StretchIndexNum.Value][1];
				int num3 = this.stretches[(int)this.TrimIndexNum.Value][(int)this.StretchIndexNum.Value][2];
				int num4 = this.stretches[(int)this.TrimIndexNum.Value][(int)this.StretchIndexNum.Value][3];
				if (num > num3)
				{
					bitmap.RotateFlip(RotateFlipType.Rotate180FlipY);
					int num5 = num;
					num = num3;
					num3 = num5;
				}
				if (num2 > num4)
				{
					bitmap.RotateFlip(RotateFlipType.Rotate180FlipX);
					int num6 = num2;
					num2 = num4;
					num4 = num6;
				}
				this.StretchContainer.BackgroundImage = null;
				this.StretchContainer.Location = new Point((int)Math.Ceiling((double)this.StretchBox.Width / 2.0) + num, (int)Math.Ceiling((double)this.StretchBox.Height / 2.0) + num2);
				this.StretchContainer.Size = new Size(num3 - num, num4 - num2);
				this.StretchContainer.BackgroundImage = bitmap;
			}
			catch
			{
				this.StretchContainer.Location = new Point((int)Math.Ceiling((double)this.StretchBox.Width / 2.0), (int)Math.Ceiling((double)this.StretchBox.Height / 2.0));
				this.StretchContainer.Size = new Size(50, 50);
				this.StretchContainer.BackgroundImage = null;
			}
		}

		private void DrawClick(object sender, EventArgs e)
		{
			this.UpdateStretchRectangle(true);
		}

		private void RedTrackScroll(object sender, EventArgs e)
		{
			this.filters[(int)this.TrimIndexNum.Value][this.RGBALoc.SelectedIndex][0] = (byte)this.redTrack.Value;
		}

		private void GreenTrackScroll(object sender, EventArgs e)
		{
			this.filters[(int)this.TrimIndexNum.Value][this.RGBALoc.SelectedIndex][1] = (byte)this.greenTrack.Value;
		}

		private void BlueTrackScroll(object sender, EventArgs e)
		{
			this.filters[(int)this.TrimIndexNum.Value][this.RGBALoc.SelectedIndex][2] = (byte)this.blueTrack.Value;
		}

		private void AlphaTrackScroll(object sender, EventArgs e)
		{
			this.filters[(int)this.TrimIndexNum.Value][this.RGBALoc.SelectedIndex][3] = (byte)this.alphaTrack.Value;
		}

		private void ResetBorderClick(object sender, EventArgs e)
		{
			this.filters[(int)this.TrimIndexNum.Value][this.RGBALoc.SelectedIndex][0] = this.filters_backup[(int)this.TrimIndexNum.Value][this.RGBALoc.SelectedIndex][0];
			this.filters[(int)this.TrimIndexNum.Value][this.RGBALoc.SelectedIndex][1] = this.filters_backup[(int)this.TrimIndexNum.Value][this.RGBALoc.SelectedIndex][1];
			this.filters[(int)this.TrimIndexNum.Value][this.RGBALoc.SelectedIndex][2] = this.filters_backup[(int)this.TrimIndexNum.Value][this.RGBALoc.SelectedIndex][2];
			this.filters[(int)this.TrimIndexNum.Value][this.RGBALoc.SelectedIndex][3] = this.filters_backup[(int)this.TrimIndexNum.Value][this.RGBALoc.SelectedIndex][3];
			this.RGBALocSelectedIndexChanged(null, null);
		}

		private void ResetAllBordersClick(object sender, EventArgs e)
		{
			for (int i = 0; i < 4; i++)
			{
				this.filters[(int)this.TrimIndexNum.Value][i][0] = this.filters_backup[(int)this.TrimIndexNum.Value][i][0];
				this.filters[(int)this.TrimIndexNum.Value][i][1] = this.filters_backup[(int)this.TrimIndexNum.Value][i][1];
				this.filters[(int)this.TrimIndexNum.Value][i][2] = this.filters_backup[(int)this.TrimIndexNum.Value][i][2];
				this.filters[(int)this.TrimIndexNum.Value][i][3] = this.filters_backup[(int)this.TrimIndexNum.Value][i][3];
			}
			this.RGBALocSelectedIndexChanged(null, null);
		}

		private void LockStretchClick(object sender, EventArgs e)
		{
			if (sender != null)
			{
				this.lock_stretch_leftop = !this.lock_stretch_leftop;
			}
			this.right.Enabled = !this.lock_stretch_leftop & this.left.Enabled;
			this.bottom.Enabled = !this.lock_stretch_leftop & this.top.Enabled;
			this.LTRD.Enabled = !this.lock_stretch_leftop & this.left.Enabled;
			this.LockStretch.BackgroundImage = this.lock_stretch_leftop ? this.close.BackgroundImage : this.open.BackgroundImage;
		}

		private void LockTrimClick(object sender, EventArgs e)
		{
			if (sender != null)
			{
				this.lock_trim_leftop = !this.lock_trim_leftop;
			}
			this.width_.Enabled = !this.lock_trim_leftop;
			this.height_.Enabled = !this.lock_trim_leftop;
			this.XYWH.Enabled = !this.lock_stretch_leftop;
			this.LockTrim.BackgroundImage = this.lock_trim_leftop ? this.close.BackgroundImage : this.open.BackgroundImage;
		}

		private void BackChangeClick(object sender, EventArgs e)
		{
			this.pictureBoxBMP.BackgroundImage = (this.pictureBoxBMP.BackgroundImage == this.DarkBack.BackgroundImage) ? this.BrightBack.BackgroundImage : this.DarkBack.BackgroundImage;
			this.StretchBox.BackgroundImage = this.pictureBoxBMP.BackgroundImage;
			this.BackChange.Text = (this.BackChange.Text == "Dark background") ? "Bright background" : "Dark background";
		}

		private void HorizontalFlipToolStripMenuItemClick(object sender, EventArgs e)
		{
			int num = this.stretches[(int)this.TrimIndexNum.Value][(int)this.StretchIndexNum.Value][1];
			this.stretches[(int)this.TrimIndexNum.Value][(int)this.StretchIndexNum.Value][1] = this.stretches[(int)this.TrimIndexNum.Value][(int)this.StretchIndexNum.Value][3];
			this.stretches[(int)this.TrimIndexNum.Value][(int)this.StretchIndexNum.Value][3] = num;
			this.UpdateStretchesValues();
			this.UpdateStretchRectangle(false);
		}

		private void VerticalFlipToolStripMenuItemClick(object sender, EventArgs e)
		{
			int num = this.stretches[(int)this.TrimIndexNum.Value][(int)this.StretchIndexNum.Value][0];
			this.stretches[(int)this.TrimIndexNum.Value][(int)this.StretchIndexNum.Value][0] = this.stretches[(int)this.TrimIndexNum.Value][(int)this.StretchIndexNum.Value][2];
			this.stretches[(int)this.TrimIndexNum.Value][(int)this.StretchIndexNum.Value][2] = num;
			this.UpdateStretchesValues();
			this.UpdateStretchRectangle(false);
		}

		private void CenterTheStretchToolStripMenuItemClick(object sender, EventArgs e)
		{
			int num = this.stretches[(int)this.TrimIndexNum.Value][(int)this.StretchIndexNum.Value][2] - this.stretches[(int)this.TrimIndexNum.Value][(int)this.StretchIndexNum.Value][0];
			int num2 = this.stretches[(int)this.TrimIndexNum.Value][(int)this.StretchIndexNum.Value][3] - this.stretches[(int)this.TrimIndexNum.Value][(int)this.StretchIndexNum.Value][1];
			this.stretches[(int)this.TrimIndexNum.Value][(int)this.StretchIndexNum.Value][0] = -num / 2;
			num -= num / 2;
			this.stretches[(int)this.TrimIndexNum.Value][(int)this.StretchIndexNum.Value][2] = num;
			this.stretches[(int)this.TrimIndexNum.Value][(int)this.StretchIndexNum.Value][1] = -num2 / 2;
			num2 -= num2 / 2;
			this.stretches[(int)this.TrimIndexNum.Value][(int)this.StretchIndexNum.Value][3] = num2;
			this.UpdateStretchesValues();
			this.UpdateStretchRectangle(false);
		}

		private void SizeToolStripMenuItemClick(object sender, EventArgs e)
		{
			int num = this.stretches[(int)this.TrimIndexNum.Value][(int)this.StretchIndexNum.Value][2] - this.stretches[(int)this.TrimIndexNum.Value][(int)this.StretchIndexNum.Value][0];
			int num2 = this.stretches[(int)this.TrimIndexNum.Value][(int)this.StretchIndexNum.Value][3] - this.stretches[(int)this.TrimIndexNum.Value][(int)this.StretchIndexNum.Value][1];
			this.stretches[(int)this.TrimIndexNum.Value][(int)this.StretchIndexNum.Value][0] = this.stretches[(int)this.TrimIndexNum.Value][(int)this.StretchIndexNum.Value][0] + num / 2 - (this.trims[(int)this.TrimIndexNum.Value][2] - this.trims[(int)this.TrimIndexNum.Value][0]) / 2;
			this.stretches[(int)this.TrimIndexNum.Value][(int)this.StretchIndexNum.Value][1] = this.stretches[(int)this.TrimIndexNum.Value][(int)this.StretchIndexNum.Value][1] + num2 / 2 - (this.trims[(int)this.TrimIndexNum.Value][3] - this.trims[(int)this.TrimIndexNum.Value][1]) / 2;
			this.stretches[(int)this.TrimIndexNum.Value][(int)this.StretchIndexNum.Value][2] = this.stretches[(int)this.TrimIndexNum.Value][(int)this.StretchIndexNum.Value][2] - num / 2 + (this.trims[(int)this.TrimIndexNum.Value][2] - this.trims[(int)this.TrimIndexNum.Value][0]) / 2;
			this.stretches[(int)this.TrimIndexNum.Value][(int)this.StretchIndexNum.Value][3] = this.stretches[(int)this.TrimIndexNum.Value][(int)this.StretchIndexNum.Value][3] - num2 / 2 + (this.trims[(int)this.TrimIndexNum.Value][3] - this.trims[(int)this.TrimIndexNum.Value][1]) / 2;
			this.UpdateStretchesValues();
			this.UpdateStretchRectangle(false);
		}

		private void ToolStripMenuItem3Click(object sender, EventArgs e)
		{
			this.stretches[(int)this.TrimIndexNum.Value][(int)this.StretchIndexNum.Value][0] = (int)((double)this.stretches[(int)this.TrimIndexNum.Value][(int)this.StretchIndexNum.Value][0] * 0.75);
			this.stretches[(int)this.TrimIndexNum.Value][(int)this.StretchIndexNum.Value][2] = (int)((double)this.stretches[(int)this.TrimIndexNum.Value][(int)this.StretchIndexNum.Value][2] * 0.75);
			this.UpdateStretchesValues();
			this.UpdateStretchRectangle(false);
		}

		private void AllToolStripMenuItem1Click(object sender, EventArgs e)
		{
			for (int i = 0; i < this.trims.Count; i++)
			{
				for (int j = 0; j < this.stretches[i].Count; j++)
				{
					this.stretches[i][j][0] = (int)((double)this.stretches[i][j][0] * 0.75);
					this.stretches[i][j][2] = (int)((double)this.stretches[i][j][2] * 0.75);
				}
			}
			this.UpdateStretchesValues();
			this.UpdateStretchRectangle(false);
		}

		private void ResetAllTrimsClick(object sender, EventArgs e)
		{
			for (int i = 0; i < this.trims.Count; i++)
			{
				this.trims[i] = new int[]
				{
					this.trims_backup[i][0],
					this.trims_backup[i][1],
					this.trims_backup[i][2],
					this.trims_backup[i][3]
				};
			}
			this.UpdateTrimValues();
			this.UpdateTrimRectangle();
			this.UpdateStretchRectangle(false);
		}

		private void ResetAllStretchesClick(object sender, EventArgs e)
		{
			for (int i = 0; i < this.trims.Count; i++)
			{
				for (int j = 0; j < this.stretches[i].Count; j++)
				{
					this.stretches[i][j] = new int[]
					{
						this.stretches_backup[i][j][0],
						this.stretches_backup[i][j][1],
						this.stretches_backup[i][j][2],
						this.stretches_backup[i][j][3]
					};
				}
			}
			this.UpdateStretchesValues();
			this.UpdateStretchRectangle(false);
		}

		private void Save2DDClick(object sender, EventArgs e)
		{
			this.saveFileDialog1.FileName = Path.GetFileName(this.command_original_filename);
			this.UpdateFile();
			if (this.saveFileDialog1.ShowDialog() == DialogResult.OK)
			{
				try
				{
					File.WriteAllBytes(this.saveFileDialog1.FileName, this.file_bytes);
				}
				catch
				{
					throw new NotSupportedException("Unable to access the file: " + this.saveFileDialog1.FileName);
				}
			}
		}

		private void PictureBoxBMPMouseMove(object sender, MouseEventArgs e)
		{
			if (this.trims.Count == 0)
			{
				return;
			}
			this.mouse_2L = this.pictureBoxBMP.PointToClient(Cursor.Position);
			if (this.GrabTypeDown == 8)
			{
				if (this.mouse_2L.X - this.mouse_2LDown.X < 0)
				{
					this.mouse_2L = new Point(this.mouse_2LDown.X, this.mouse_2L.Y);
				}
				if (this.mouse_2L.Y - this.mouse_2LDown.Y < 0)
				{
					this.mouse_2L = new Point(this.mouse_2L.X, this.mouse_2LDown.Y);
				}
				int num = this.trims[(int)this.TrimIndexNum.Value][2] - this.trims[(int)this.TrimIndexNum.Value][0];
				int num2 = this.trims[(int)this.TrimIndexNum.Value][3] - this.trims[(int)this.TrimIndexNum.Value][1];
				if (this.mouse_2L.X + num - this.mouse_2LDown.X > this.pictureBoxBMP.Width - 1)
				{
					this.mouse_2L = new Point(this.pictureBoxBMP.Width - 1 - num + this.mouse_2LDown.X, this.mouse_2L.Y);
				}
				if (this.mouse_2L.Y + num2 - this.mouse_2LDown.Y > this.pictureBoxBMP.Height - 1)
				{
					this.mouse_2L = new Point(this.mouse_2L.X, this.pictureBoxBMP.Height - 1 - num2 + this.mouse_2LDown.Y);
				}
			}
			else
			{
				if (this.mouse_2L.X < 0)
				{
					this.mouse_2L = new Point(0, this.mouse_2L.Y);
				}
				if (this.mouse_2L.Y < 0)
				{
					this.mouse_2L = new Point(this.mouse_2L.X, 0);
				}
				if (this.mouse_2L.X > this.pictureBoxBMP.Width - 1)
				{
					this.mouse_2L = new Point(this.pictureBoxBMP.Width - 1, this.mouse_2L.Y);
				}
				if (this.mouse_2L.Y > this.pictureBoxBMP.Height - 1)
				{
					this.mouse_2L = new Point(this.mouse_2L.X, this.pictureBoxBMP.Height - 1);
				}
			}
			if (this.GrabTypeDown == -1)
			{
				if (this.CursorPointAverage(this.mouse_2L, new Point(this.trims[(int)this.TrimIndexNum.Value][0], this.trims[(int)this.TrimIndexNum.Value][1]), 3))
				{
					this.GrabTypeOver = 0;
				}
				else if (this.CursorPointAverage(this.mouse_2L, new Point(this.trims[(int)this.TrimIndexNum.Value][2], this.trims[(int)this.TrimIndexNum.Value][1]), 3))
				{
					this.GrabTypeOver = 1;
				}
				else if (this.CursorPointAverage(this.mouse_2L, new Point(this.trims[(int)this.TrimIndexNum.Value][2], this.trims[(int)this.TrimIndexNum.Value][3]), 3))
				{
					this.GrabTypeOver = 2;
				}
				else if (this.CursorPointAverage(this.mouse_2L, new Point(this.trims[(int)this.TrimIndexNum.Value][0], this.trims[(int)this.TrimIndexNum.Value][3]), 3))
				{
					this.GrabTypeOver = 3;
				}
				else if (this.mouse_2L.X > this.trims[(int)this.TrimIndexNum.Value][0] - 3 & this.mouse_2L.X < this.trims[(int)this.TrimIndexNum.Value][2] + 3 & this.mouse_2L.Y > this.trims[(int)this.TrimIndexNum.Value][1] - 3 & this.mouse_2L.Y < this.trims[(int)this.TrimIndexNum.Value][1] + 3)
				{
					this.GrabTypeOver = 4;
				}
				else if (this.mouse_2L.X > this.trims[(int)this.TrimIndexNum.Value][2] - 3 & this.mouse_2L.X < this.trims[(int)this.TrimIndexNum.Value][2] + 3 & this.mouse_2L.Y > this.trims[(int)this.TrimIndexNum.Value][1] - 3 & this.mouse_2L.Y < this.trims[(int)this.TrimIndexNum.Value][3] + 3)
				{
					this.GrabTypeOver = 5;
				}
				else if (this.mouse_2L.X > this.trims[(int)this.TrimIndexNum.Value][0] - 3 & this.mouse_2L.X < this.trims[(int)this.TrimIndexNum.Value][2] + 3 & this.mouse_2L.Y > this.trims[(int)this.TrimIndexNum.Value][3] - 3 & this.mouse_2L.Y < this.trims[(int)this.TrimIndexNum.Value][3] + 3)
				{
					this.GrabTypeOver = 6;
				}
				else if (this.mouse_2L.X > this.trims[(int)this.TrimIndexNum.Value][0] - 3 & this.mouse_2L.X < this.trims[(int)this.TrimIndexNum.Value][0] + 3 & this.mouse_2L.Y > this.trims[(int)this.TrimIndexNum.Value][1] - 3 & this.mouse_2L.Y < this.trims[(int)this.TrimIndexNum.Value][3] + 3)
				{
					this.GrabTypeOver = 7;
				}
				else if (this.mouse_2L.X > this.trims[(int)this.TrimIndexNum.Value][0] & this.mouse_2L.X < this.trims[(int)this.TrimIndexNum.Value][2] & this.mouse_2L.Y > this.trims[(int)this.TrimIndexNum.Value][1] & this.mouse_2L.Y < this.trims[(int)this.TrimIndexNum.Value][3])
				{
					this.GrabTypeOver = 8;
				}
				else
				{
					this.GrabTypeOver = -1;
				}
			}
			switch (this.GrabTypeDown)
			{
			case -2:
				if (this.mouse_2LDown.X < this.mouse_2L.X)
				{
					this.trims[(int)this.TrimIndexNum.Value][0] = this.mouse_2LDown.X;
					this.trims[(int)this.TrimIndexNum.Value][2] = this.mouse_2L.X;
				}
				else
				{
					this.trims[(int)this.TrimIndexNum.Value][2] = this.mouse_2LDown.X;
					this.trims[(int)this.TrimIndexNum.Value][0] = this.mouse_2L.X;
				}
				if (this.mouse_2LDown.Y < this.mouse_2L.Y)
				{
					this.trims[(int)this.TrimIndexNum.Value][3] = this.mouse_2L.Y;
					this.trims[(int)this.TrimIndexNum.Value][1] = this.mouse_2LDown.Y;
				}
				else
				{
					this.trims[(int)this.TrimIndexNum.Value][3] = this.mouse_2LDown.Y;
					this.trims[(int)this.TrimIndexNum.Value][1] = this.mouse_2L.Y;
				}
				break;
			case 0:
				if (this.mouse_2L.X > this.trims[(int)this.TrimIndexNum.Value][2])
				{
					this.trims[(int)this.TrimIndexNum.Value][0] = this.trims[(int)this.TrimIndexNum.Value][2];
					this.trims[(int)this.TrimIndexNum.Value][2] = this.mouse_2L.X;
					this.GrabTypeDown = 1;
					return;
				}
				if (this.mouse_2L.Y > this.trims[(int)this.TrimIndexNum.Value][3])
				{
					this.trims[(int)this.TrimIndexNum.Value][1] = this.trims[(int)this.TrimIndexNum.Value][3];
					this.trims[(int)this.TrimIndexNum.Value][3] = this.mouse_2L.Y;
					this.GrabTypeDown = 3;
					return;
				}
				this.trims[(int)this.TrimIndexNum.Value][0] = this.mouse_2L.X;
				this.trims[(int)this.TrimIndexNum.Value][1] = this.mouse_2L.Y;
				break;
			case 1:
				if (this.mouse_2L.X < this.trims[(int)this.TrimIndexNum.Value][0])
				{
					this.trims[(int)this.TrimIndexNum.Value][2] = this.trims[(int)this.TrimIndexNum.Value][0];
					this.trims[(int)this.TrimIndexNum.Value][0] = this.mouse_2L.X;
					this.GrabTypeDown = 0;
					return;
				}
				if (this.mouse_2L.Y > this.trims[(int)this.TrimIndexNum.Value][3])
				{
					this.trims[(int)this.TrimIndexNum.Value][1] = this.trims[(int)this.TrimIndexNum.Value][3];
					this.trims[(int)this.TrimIndexNum.Value][3] = this.mouse_2L.Y;
					this.GrabTypeDown = 2;
					return;
				}
				this.trims[(int)this.TrimIndexNum.Value][2] = this.mouse_2L.X;
				this.trims[(int)this.TrimIndexNum.Value][1] = this.mouse_2L.Y;
				break;
			case 2:
				if (this.mouse_2L.X < this.trims[(int)this.TrimIndexNum.Value][0])
				{
					this.trims[(int)this.TrimIndexNum.Value][2] = this.trims[(int)this.TrimIndexNum.Value][0];
					this.trims[(int)this.TrimIndexNum.Value][0] = this.mouse_2L.X;
					this.GrabTypeDown = 3;
					return;
				}
				if (this.mouse_2L.Y < this.trims[(int)this.TrimIndexNum.Value][1])
				{
					this.trims[(int)this.TrimIndexNum.Value][3] = this.trims[(int)this.TrimIndexNum.Value][1];
					this.trims[(int)this.TrimIndexNum.Value][1] = this.mouse_2L.Y;
					this.GrabTypeDown = 1;
					return;
				}
				this.trims[(int)this.TrimIndexNum.Value][2] = this.mouse_2L.X;
				this.trims[(int)this.TrimIndexNum.Value][3] = this.mouse_2L.Y;
				break;
			case 3:
				if (this.mouse_2L.X > this.trims[(int)this.TrimIndexNum.Value][2])
				{
					this.trims[(int)this.TrimIndexNum.Value][0] = this.trims[(int)this.TrimIndexNum.Value][2];
					this.trims[(int)this.TrimIndexNum.Value][2] = this.mouse_2L.X;
					this.GrabTypeDown = 2;
					return;
				}
				if (this.mouse_2L.Y < this.trims[(int)this.TrimIndexNum.Value][1])
				{
					this.trims[(int)this.TrimIndexNum.Value][3] = this.trims[(int)this.TrimIndexNum.Value][1];
					this.trims[(int)this.TrimIndexNum.Value][1] = this.mouse_2L.Y;
					this.GrabTypeDown = 0;
					return;
				}
				this.trims[(int)this.TrimIndexNum.Value][0] = this.mouse_2L.X;
				this.trims[(int)this.TrimIndexNum.Value][3] = this.mouse_2L.Y;
				break;
			case 4:
				if (this.mouse_2L.Y > this.trims[(int)this.TrimIndexNum.Value][3])
				{
					this.trims[(int)this.TrimIndexNum.Value][1] = this.trims[(int)this.TrimIndexNum.Value][3];
					this.trims[(int)this.TrimIndexNum.Value][3] = this.mouse_2L.Y;
					this.GrabTypeDown = 6;
				}
				else
				{
					this.trims[(int)this.TrimIndexNum.Value][1] = this.mouse_2L.Y;
				}
				break;
			case 5:
				if (this.mouse_2L.X < this.trims[(int)this.TrimIndexNum.Value][0])
				{
					this.trims[(int)this.TrimIndexNum.Value][2] = this.trims[(int)this.TrimIndexNum.Value][0];
					this.trims[(int)this.TrimIndexNum.Value][0] = this.mouse_2L.X;
					this.GrabTypeDown = 7;
				}
				else
				{
					this.trims[(int)this.TrimIndexNum.Value][2] = this.mouse_2L.X;
				}
				break;
			case 6:
				if (this.mouse_2L.Y < this.trims[(int)this.TrimIndexNum.Value][1])
				{
					this.trims[(int)this.TrimIndexNum.Value][3] = this.trims[(int)this.TrimIndexNum.Value][1];
					this.trims[(int)this.TrimIndexNum.Value][1] = this.mouse_2L.Y;
					this.GrabTypeDown = 4;
					return;
				}
				this.trims[(int)this.TrimIndexNum.Value][3] = this.mouse_2L.Y;
				break;
			case 7:
				if (this.mouse_2L.X > this.trims[(int)this.TrimIndexNum.Value][2])
				{
					this.trims[(int)this.TrimIndexNum.Value][0] = this.trims[(int)this.TrimIndexNum.Value][2];
					this.trims[(int)this.TrimIndexNum.Value][2] = this.mouse_2L.X;
					this.GrabTypeDown = 5;
				}
				else
				{
					this.trims[(int)this.TrimIndexNum.Value][0] = this.mouse_2L.X;
				}
				break;
			case 8:
			{
				int num3 = this.trims[(int)this.TrimIndexNum.Value][2] - this.trims[(int)this.TrimIndexNum.Value][0];
				int num4 = this.trims[(int)this.TrimIndexNum.Value][3] - this.trims[(int)this.TrimIndexNum.Value][1];
				this.trims[(int)this.TrimIndexNum.Value][0] = this.mouse_2L.X - this.mouse_2LDown.X;
				this.trims[(int)this.TrimIndexNum.Value][1] = this.mouse_2L.Y - this.mouse_2LDown.Y;
				this.trims[(int)this.TrimIndexNum.Value][2] = this.trims[(int)this.TrimIndexNum.Value][0] + num3;
				this.trims[(int)this.TrimIndexNum.Value][3] = this.trims[(int)this.TrimIndexNum.Value][1] + num4;
				break;
			}
			}
			switch (this.GrabTypeOver)
			{
			case -1:
				this.pictureBoxBMP.Cursor = Cursors.Cross;
				break;
			case 0:
			case 2:
				this.pictureBoxBMP.Cursor = Cursors.SizeNWSE;
				break;
			case 1:
			case 3:
				this.pictureBoxBMP.Cursor = Cursors.SizeNESW;
				break;
			case 4:
			case 6:
				this.pictureBoxBMP.Cursor = Cursors.SizeNS;
				break;
			case 5:
			case 7:
				this.pictureBoxBMP.Cursor = Cursors.SizeWE;
				break;
			case 8:
				this.pictureBoxBMP.Cursor = Cursors.SizeAll;
				break;
			}
			if (this.GrabTypeDown != -1)
			{
				this.UpdateTrimValues();
				this.UpdateTrimRectangle();
				this.UpdateStretchRectangle(false);
			}
		}

		private bool CursorPointAverage(Point input, Point output, int perimeter)
		{
			return input.X > output.X - perimeter & input.X < output.X + perimeter & input.Y > output.Y - perimeter & input.Y < output.Y + perimeter;
		}

		private void PictureBoxBMPMouseDown(object sender, MouseEventArgs e)
		{
			this.GrabTypeDown = this.GrabTypeOver;
			if (this.GrabTypeDown == -1)
			{
				this.GrabTypeDown = -2;
				this.mouse_2LDown = this.mouse_2L;
				return;
			}
			this.mouse_2LDown = new Point(this.mouse_2L.X - this.trims[(int)this.TrimIndexNum.Value][0], this.mouse_2L.Y - this.trims[(int)this.TrimIndexNum.Value][1]);
		}

		private void PictureBoxBMPMouseUp(object sender, MouseEventArgs e)
		{
			this.GrabTypeDown = -1;
		}

		public void UpdateTrimRectangle()
		{
			Bitmap bitmap = new Bitmap(this.Image_Backup);
			Graphics graphics = Graphics.FromImage(bitmap);
			int num = this.trims[(int)this.TrimIndexNum.Value][0];
			int num2 = this.trims[(int)this.TrimIndexNum.Value][1];
			int num3 = this.trims[(int)this.TrimIndexNum.Value][2];
			int num4 = this.trims[(int)this.TrimIndexNum.Value][3];
			this.TrimRectangle = new Rectangle(num, num2, num3 - num, num4 - num2);
			graphics.DrawRectangle(this.RedPen, this.TrimRectangle);
			this.pictureBoxBMP.Image = bitmap;
		}
	}
}
