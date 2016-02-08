using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using SlavaGu.ConsoleAppLauncher;
using System.IO;

namespace webPGUI
{
    public partial class frmMain : Form
    {

        public frmMain()
        {
            InitializeComponent();
            this.AllowDrop = true;
            this.DragEnter += new DragEventHandler(frmMain_DragEnter);
            this.DragDrop += new DragEventHandler(frmMain_DragDrop);
            tabControl1.SelectedIndexChanged += new EventHandler(TabControl1_SelectedIndexChanged);
        }

        void frmMain_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop)) e.Effect = DragDropEffects.Copy;
        }

        void frmMain_DragDrop(object sender, DragEventArgs e)
        {

            string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);

            string extension = Path.GetExtension(files[0]);
            //Console.WriteLine(extension);

            if (extension == ".jpg" || extension == ".jpeg" || extension == ".tif" || extension == ".png" || extension == ".gif" || extension == ".webp")
            {
                openFileDialog1.FileName = files[0];
                textBox_input.Text = files[0];
                processArgs();
            }

            //foreach (string file in files) Console.WriteLine(file);
        }

        private void TabControl1_SelectedIndexChanged(Object sender, EventArgs e)
        {
            if (tabControl1.SelectedIndex == 0)
                this.AllowDrop = true;
            else
                this.AllowDrop = false;
        }

        private void trackBar1_Scroll(object sender, EventArgs e)
        {
            numericUpDown_q.Value = trackBar_quality.Value;
            processArgs();
        }


        private void button2_Click(object sender, EventArgs e)
        {
            // ========================= RUN COMMAND ===============================
            Cursor.Current = Cursors.WaitCursor;

            SaveSettings();

            frmOutput frm = new frmOutput();
            frm.ShowDialog();
        }


        private void button3_Click(object sender, EventArgs e)
        {

            openFileDialog1.Filter = "Image files (*.jpg, *.jpeg, *.tif, *.gif, *.png, *.webp) | *.jpg; *.jpeg; *.tif; *.gif; *.png; *.webp|JPEG Files (*.jpeg)|*.jpeg|PNG Files (*.png)|*.png|JPG Files (*.jpg)|*.jpg|GIF Files (*.gif)|*.gif|TIFF Files (*.tif)|*.tif|WebP Files (*.webp)|*.webp";
            openFileDialog1.FilterIndex = 1;

            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                textBox_input.Text = openFileDialog1.FileName;
                textBox_outputfile.Text = Path.GetDirectoryName(openFileDialog1.FileName) + "\\" +
                Path.GetFileNameWithoutExtension(openFileDialog1.FileName) + ".webp";
                processArgs();
            }
        }

        /// <summary>
        /// Generate command line arguments
        /// </summary>
        public void processArgs()
        {
            Globals.args = "cwebp.exe"; //  cwebp [-preset <...>] [options] in_file [-o out_file]

            if (cboPreset.SelectedIndex>0) // preset setting, one of: default, photo, picture, drawing, icon, text
                Globals.args += " -preset "+ cboPreset.Text; // -preset must come first, as it overwrites other parameters

            Globals.args += " -q " + trackBar_quality.Value.ToString(); // -q <float> quality factor (0:small..100:big)
            Globals.args += " -alpha_q " + trackBar_alpha_q.Value.ToString(); // -alpha_q <int> transparency-compression quality (0..100)
            Globals.args += " -z " + trackBar_z.Value.ToString(); // activates lossless preset with given level in [0:fast, ..., 9:slowest]
            Globals.args += " -m " + trackBar_m.Value.ToString(); // compression method (0=fast, 6=slowest)
            Globals.args += " -segments " + trackBar_segments.Value.ToString(); // number of segments to use (1..4)

            uint sizetemp = Convert.ToUInt32(numericUpDown_Size.Value); 
            if (radioLossySize.Checked && sizetemp > 0) {
                switch (comboUnitSize.SelectedIndex) {
                    case 1:
                        sizetemp *= 1024;
                        break;
                    case 2:
                        sizetemp *= 1024 * 1024;
                        break;
                }
                Globals.args += " -size " + sizetemp.ToString(); // target size (in bytes)
            }

            if (checkBox_psnr.Checked)
                Globals.args += " -psnr " + numericUpDown_psnr.Value.ToString();  // target PSNR (in dB.typically: 42)
                // Specify a target PSNR (in dB) to try and reach for the compressed output. Compressor will make several pass of partial encoding in order to get as close as possible to this target.

            // TODO -s <int> <int> ......... input size (width x height) for YUV

            Globals.args += " -sns " + trackBar_sns.Value.ToString(); // spatial noise shaping (0:off, 100:max)
            Globals.args += " -f " + trackBar_f.Value.ToString(); // deblocking filter strength (0=off..100)
            // Specify the strength of the deblocking filter, between 0 (no filtering) and 100 (maximum filtering). A value of 0 will turn off any filtering. Higher value will increase the strength of the filtering process applied after decoding the picture. The higher the value the smoother the picture will appear. Typical values are usually in the range of 20 to 50.
            Globals.args += " -sharpness " + trackBar_sharpness.Value.ToString(); // filter sharpness (0:most .. 7:least sharp)

            if (checkBox_strong.Checked) {
                Globals.args += " -strong"; // use strong filter instead of simple (default)
            } else {
                Globals.args += " -nostrong"; // use simple filter instead of strong
            }

            // Globals.args += " -partition_limit " + trackBar10.Value.ToString(); // limit quality to fit the 512k limit on the first partition(0 = no degradation... 100 = full)
            Globals.args += " -pass " + trackBar_pass.Value.ToString(); // analysis pass number (1..10)

            /* TODO -crop <x> <y> <w> <h> .. crop picture with the given rectangle
                    -resize <w> <h> ........ resize picture (after any cropping) */

            if (checkBox_mt.Checked)
                Globals.args += " -mt"; // use multi-threading if available

            if (checkBox_low_memory.Checked)
                Globals.args += " -low_memory"; // reduce memory usage (slower encoding)


            if (checkBox_map.Checked)
                Globals.args += " -map " + numericUpDown_map.Value.ToString(); // print map of extra info

            if (checkBox_print_psnr.Checked)
                Globals.args += " -print_psnr"; // print map of extra info

            if (checkBox_print_ssim.Checked)
                Globals.args += " -print_ssim"; // prints averaged SSIM distortion

            if (checkBox_print_lsim.Checked)
                Globals.args += " -print_lsim"; // prints local-similarity distortion

            if (checkBox_d.Checked)
                Globals.args += " -d file.pgm"; // dump the compressed output (PGM file)

            if (checkBox_alpha_method.Checked)
                Globals.args += " -alpha_method 1"; // transparency-compression method (0..1)
            else
                Globals.args += " -alpha_method 0"; // Specify the algorithm used for alpha compression. Off denotes no compression, On uses WebP lossless format for compression.

            switch (cbo_alpha_filter.SelectedIndex)
            { // predictive filtering for alpha plane, one of: none, fast(default) or best
                case 0:
                    Globals.args += " -alpha_filter none";
                    break;
                case 1:
                    Globals.args += " -alpha_filter fast";
                    break;
                case 2:
                    Globals.args += " -alpha_filter best";
                    break;
            }

            if (checkBox_exact.Checked)
                Globals.args += " -exact"; // preserve RGB values in transparent area

            if (checkBox_blend_alpha.Checked) // blend colors against background color expressed as RGB values written in hexadecimal, e.g. 0xc0e0d0 for red = 0xc0 green = 0xe0 and blue = 0xd0
                Globals.args += " -blend_alpha " + HexConverter(colorDialog1.Color);

            if (checkBox_noalpha.Checked)
                Globals.args += " -noalpha"; // discard any transparency information

            if (radioLossless.Checked)
                Globals.args += " -lossless";// encode image losslessly

            Globals.args += " -near_lossless " + trackBar_near_lossless.Value.ToString(); // use near-lossless image preprocessing(0..100 = off)

            if (cboHint.SelectedIndex>0)
                Globals.args += " -hint " + cboHint.SelectedItem.ToString(); // specify image characteristics hint, one of: photo, picture or graph

            /* -metadata <string> ..... comma separated list of metadata to
                           copy from the input to the output if present.
                           Valid values: all, none (default), exif, icc, xmp

                  -version ............... print version number and exit */

            if (checkBox_short.Checked)
                Globals.args += " -short"; // condense printed message

            if (checkBox_quiet.Checked)
                Globals.args += " -quiet"; // don't print anything

            if (checkBox_noasm.Checked)
                Globals.args += " -noasm"; // disable all assembly optimizations

            if (checkBox_v.Checked)
                Globals.args += " -v"; // verbose, e.g. print encoding/decoding times

            if (checkBox_progress.Checked)
                Globals.args += " -progress"; // report encoding progres

            // ============================== experimental features =================================
            if (checkBox_jpeg_like.Checked)
                Globals.args += " -jpeg_like"; // roughly match expected JPEG size

            if (checkBox_af.Checked)
                Globals.args += " -af"; // auto-adjust filter strength

            // -pre <int> ............. pre-processing filter Specify some pre-processing steps. Using a value of 2 will trigger quality-dependent pseudo-random dithering during RGBA->YUVA conversion (lossy compression only).

            // ============================== output =================================
            if (openFileDialog1.FileName != "")
            {
                Globals.args += " \"" + openFileDialog1.FileName + "\" ";
                Globals.args += "-o \"" + textBox_outputfile.Text + "\" ";
                button_encode.Enabled = true;
            } else {
                Globals.args += " SPECIFY A FILE NAME";
                button_encode.Enabled = false;
            }
            
            // show commands on text box
            textBox_console.Text = Globals.args;
        }

        private void trackBar1_Scroll_1(object sender, EventArgs e)
        {
            numericUpDown_alpha_q.Value = trackBar_alpha_q.Value;
            processArgs();
        }

        private void trackBar2_Scroll(object sender, EventArgs e)
        {
            numericUpDown_z.Value = trackBar_z.Value;
            processArgs();
        }

        private void trackBar3_Scroll(object sender, EventArgs e)
        {
            numericUpDown_m.Value = trackBar_m.Value;
            processArgs();
        }

        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {
            trackBar_quality.Value = Decimal.ToInt32(numericUpDown_q.Value);
            processArgs();
        }



        private void button1_Click(object sender, EventArgs e)
        {
            saveFileDialog1.Filter = "webP Image (*.webp) | *.webp";
            saveFileDialog1.FilterIndex = 1;

            // Process input if the user clicked OK.
            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                // Open the selected file to read.
                textBox_outputfile.Text = saveFileDialog1.FileName;
                processArgs();
            }
        }

 

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            processArgs();
        }

        private void numericUpDown4_ValueChanged(object sender, EventArgs e)
        {
            processArgs();
        }

        private void comboBox3_SelectedIndexChanged(object sender, EventArgs e)
        {
            processArgs();
        }

        private void checkBox8_CheckedChanged(object sender, EventArgs e)
        {
            processArgs();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            if (colorDialog1.ShowDialog() == DialogResult.OK)
            {
                label_color.BackColor = colorDialog1.Color;
                processArgs();
            }
        }

        private void checkBox9_CheckedChanged(object sender, EventArgs e)
        {
            processArgs();
        }

        private void checkBox5_CheckedChanged(object sender, EventArgs e)
        {
            btn_color.Enabled = checkBox_jpeg_like.Checked;
            processArgs();
        }

        /// <summary>
        /// Convert Color to HEX value like 0xFFFFFF
        /// </summary>
        /// <param name="c">Color var type</param>
        /// <returns></returns>
        private static String HexConverter(System.Drawing.Color c)
        {
            return "0x" + c.R.ToString("X2") + c.G.ToString("X2") + c.B.ToString("X2");
        }

        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {
            if (radioLossless.Checked)
            {
                label16.Text = "faster (larger file size)";
                label17.Text = "slower";
            } else
            {
                label16.Text = "lower quality (smaller file size)";
                label17.Text = "better";
            }
            processArgs();
        }

        private void checkBox11_CheckedChanged(object sender, EventArgs e)
        {
            processArgs();
        }

        private void trackBar9_Scroll(object sender, EventArgs e)
        {
            numericUpDown_sns.Value = trackBar_sns.Value;
            processArgs();
        }

        private void trackBar10_Scroll(object sender, EventArgs e)
        {
            numericUpDown_partition_limit.Value = trackBar_partition_limit.Value;
            processArgs();
        }

        private void trackBar7_Scroll(object sender, EventArgs e)
        {
            numericUpDown_f.Value = trackBar_f.Value;
            processArgs();
        }

        private void numericUpDown3_ValueChanged(object sender, EventArgs e)
        {
            trackBar_f.Value = decimal.ToInt32(numericUpDown_f.Value);
            processArgs();
        }

        /// <summary>
        /// Read saved settings
        /// </summary>
        private void ReadSettings()
        {
            // prevent loose settings on version upgrade
            if (webPGUI.Properties.Settings.Default.MustUpgrade)
            {
                webPGUI.Properties.Settings.Default.Upgrade();
                webPGUI.Properties.Settings.Default.MustUpgrade = false;
                webPGUI.Properties.Settings.Default.Save();
            }

            trackBar_quality.Value = webPGUI.Properties.Settings.Default.q;
            cboPreset.SelectedIndex = webPGUI.Properties.Settings.Default.preset;
            trackBar_alpha_q.Value = webPGUI.Properties.Settings.Default.alpha_q;

            switch (webPGUI.Properties.Settings.Default.lossless)
            {
                case 0: radioLossy.Checked = true; // lossy
                    break;
                case 1: radioLossless.Checked = true; // lossless
                    break;
                case 2: radioLossySize.Checked = true; // lossy size
                    break;
            }
            
            trackBar_z.Value=webPGUI.Properties.Settings.Default.z;
            numericUpDown_z.Value= webPGUI.Properties.Settings.Default.z;

            trackBar_m.Value = webPGUI.Properties.Settings.Default.m;
            numericUpDown_m.Value = webPGUI.Properties.Settings.Default.m;

            trackBar_segments.Value = webPGUI.Properties.Settings.Default.segments;
            numericUpDown_segments.Value = webPGUI.Properties.Settings.Default.segments;

            numericUpDown_Size.Value = Convert.ToDecimal(webPGUI.Properties.Settings.Default.size);

            numericUpDown_psnr.Value = Convert.ToDecimal(webPGUI.Properties.Settings.Default.psnrval);
            checkBox_psnr.Checked = webPGUI.Properties.Settings.Default.psnr;

            trackBar_f.Value = webPGUI.Properties.Settings.Default.f;
            numericUpDown_f.Value = webPGUI.Properties.Settings.Default.f;

            trackBar_sharpness.Value = webPGUI.Properties.Settings.Default.sharpness;
            numericUpDown_sharpness.Value = webPGUI.Properties.Settings.Default.sharpness;

            checkBox_strong.Checked = webPGUI.Properties.Settings.Default.strong;

            trackBar_partition_limit.Value = webPGUI.Properties.Settings.Default.partition_limit;
            numericUpDown_partition_limit.Value = webPGUI.Properties.Settings.Default.partition_limit;

            trackBar_pass.Value = webPGUI.Properties.Settings.Default.pass;
            numericUpDown_pass.Value = webPGUI.Properties.Settings.Default.pass;

            //TODOresize CROP
            checkBox_mt.Checked = webPGUI.Properties.Settings.Default.mt;
            checkBox_low_memory.Checked = webPGUI.Properties.Settings.Default.low_memory;
            numericUpDown_map.Value = Convert.ToDecimal(webPGUI.Properties.Settings.Default.map);
            checkBox_print_psnr.Checked = webPGUI.Properties.Settings.Default.print_psnr;
            checkBox_print_ssim.Checked = webPGUI.Properties.Settings.Default.print_ssim;
            checkBox_print_lsim.Checked = webPGUI.Properties.Settings.Default.print_lsim;
            //TODOdump
            checkBox_alpha_method.Checked = webPGUI.Properties.Settings.Default.alpha_method;
            cbo_alpha_filter.SelectedIndex = webPGUI.Properties.Settings.Default.alpha_filter;
            checkBox_exact.Checked = webPGUI.Properties.Settings.Default.exact;
            checkBox_blend_alpha.Checked = webPGUI.Properties.Settings.Default.blend_alpha;
            colorDialog1.Color = webPGUI.Properties.Settings.Default.blend_alpha_color;
            checkBox_noalpha.Checked = webPGUI.Properties.Settings.Default.noalpha;

            trackBar_near_lossless.Value = webPGUI.Properties.Settings.Default.near_lossless;
            numericUpDown_near_lossless.Value = webPGUI.Properties.Settings.Default.near_lossless;

            cboHint.SelectedIndex = webPGUI.Properties.Settings.Default.hint;
            checkBox_short.Checked = webPGUI.Properties.Settings.Default.shortshort;
            checkBox_quiet.Checked = webPGUI.Properties.Settings.Default.quiet;
            checkBox_v.Checked = webPGUI.Properties.Settings.Default.v;
            checkBox_progress.Checked = webPGUI.Properties.Settings.Default.progress;
            checkBox_jpeg_like.Checked = webPGUI.Properties.Settings.Default.jpeg_like;
            checkBox_af.Checked = webPGUI.Properties.Settings.Default.af;

        }

        /// <summary>
        /// Save Settings
        /// </summary>
        private void SaveSettings()
        {
            webPGUI.Properties.Settings.Default.q = trackBar_quality.Value;
            webPGUI.Properties.Settings.Default.preset = cboPreset.SelectedIndex;
            webPGUI.Properties.Settings.Default.alpha_q = trackBar_alpha_q.Value;

            if (radioLossy.Checked)
                webPGUI.Properties.Settings.Default.lossless = 0;
            else if (radioLossless.Checked)
                webPGUI.Properties.Settings.Default.lossless = 1;
            else if (radioLossySize.Checked)
                webPGUI.Properties.Settings.Default.lossless = 2;

            webPGUI.Properties.Settings.Default.z = trackBar_z.Value;

            webPGUI.Properties.Settings.Default.m = trackBar_m.Value;
            webPGUI.Properties.Settings.Default.segments = trackBar_segments.Value;
            webPGUI.Properties.Settings.Default.size = Convert.ToInt32(numericUpDown_Size.Value);
            webPGUI.Properties.Settings.Default.psnrval = Convert.ToInt32(numericUpDown_psnr.Value);
            webPGUI.Properties.Settings.Default.psnr = checkBox_psnr.Checked;
            webPGUI.Properties.Settings.Default.f = trackBar_f.Value;
            webPGUI.Properties.Settings.Default.sharpness = trackBar_sharpness.Value;
            webPGUI.Properties.Settings.Default.strong = checkBox_strong.Checked;

            webPGUI.Properties.Settings.Default.pass = trackBar_pass.Value;

            webPGUI.Properties.Settings.Default.mt = checkBox_mt.Checked;
            webPGUI.Properties.Settings.Default.low_memory = checkBox_low_memory.Checked;
            webPGUI.Properties.Settings.Default.map = Convert.ToInt32(numericUpDown_map.Value);
            webPGUI.Properties.Settings.Default.print_psnr = checkBox_print_psnr.Checked;
            webPGUI.Properties.Settings.Default.print_ssim = checkBox_print_ssim.Checked;
            webPGUI.Properties.Settings.Default.print_lsim = checkBox_print_lsim.Checked;

            webPGUI.Properties.Settings.Default.partition_limit= trackBar_partition_limit.Value;

            webPGUI.Properties.Settings.Default.alpha_method = checkBox_alpha_method.Checked;
            webPGUI.Properties.Settings.Default.alpha_filter = cbo_alpha_filter.SelectedIndex;
            webPGUI.Properties.Settings.Default.exact = checkBox_exact.Checked;
            webPGUI.Properties.Settings.Default.blend_alpha = checkBox_blend_alpha.Checked;
            webPGUI.Properties.Settings.Default.blend_alpha_color = colorDialog1.Color;
            webPGUI.Properties.Settings.Default.noalpha = checkBox_noalpha.Checked;
            webPGUI.Properties.Settings.Default.near_lossless = trackBar_near_lossless.Value;
            webPGUI.Properties.Settings.Default.hint = cboHint.SelectedIndex;
            webPGUI.Properties.Settings.Default.shortshort = checkBox_short.Checked;
            webPGUI.Properties.Settings.Default.quiet = checkBox_quiet.Checked;
            webPGUI.Properties.Settings.Default.v = checkBox_v.Checked;
            webPGUI.Properties.Settings.Default.progress = checkBox_progress.Checked;
            webPGUI.Properties.Settings.Default.jpeg_like = checkBox_jpeg_like.Checked;
            webPGUI.Properties.Settings.Default.af = checkBox_af.Checked;


            webPGUI.Properties.Settings.Default.Save();
        }



        private void checkBox7_CheckedChanged(object sender, EventArgs e)
        {
            numericUpDown_psnr.Enabled = checkBox_psnr.Checked;
            processArgs();
        }

        private void numericUpDown5_ValueChanged(object sender, EventArgs e)
        {
            processArgs();
            processArgs();
        }

        private void numericUpDown7_ValueChanged(object sender, EventArgs e)
        {
            trackBar_partition_limit.Value = decimal.ToInt32(numericUpDown_partition_limit.Value);
            processArgs();
        }

        private void numericUpDown13_ValueChanged(object sender, EventArgs e)
        {
            trackBar_pass.Value = decimal.ToInt32(numericUpDown_pass.Value);
            processArgs();
        }

        private void numericUpDown8_ValueChanged(object sender, EventArgs e)
        {
            trackBar_near_lossless.Value = decimal.ToInt32(numericUpDown_near_lossless.Value);
            processArgs();
        }

        private void numericUpDown10_ValueChanged(object sender, EventArgs e)
        {
            trackBar_m.Value = decimal.ToInt32(numericUpDown_m.Value);
            processArgs();
        }

        private void numericUpDown11_ValueChanged(object sender, EventArgs e)
        {
            trackBar_segments.Value = decimal.ToInt32(numericUpDown_segments.Value);
            processArgs();
        }

        private void numericUpDown12_ValueChanged(object sender, EventArgs e)
        {
            trackBar_sharpness.Value = decimal.ToInt32(numericUpDown_sharpness.Value);
            processArgs();
        }

        private void numericUpDown9_ValueChanged(object sender, EventArgs e)
        {
            trackBar_z.Value = decimal.ToInt32(numericUpDown_z.Value);
            processArgs();
        }

        private void numericUpDown6_ValueChanged(object sender, EventArgs e)
        {
            trackBar_sns.Value = decimal.ToInt32(numericUpDown_sns.Value);
            processArgs();
        }

        private void trackBar6_Scroll(object sender, EventArgs e)
        {
            numericUpDown_segments.Value = trackBar_segments.Value;
            processArgs();
        }

        private void trackBar11_Scroll(object sender, EventArgs e)
        {
            numericUpDown_segments.Value = trackBar_near_lossless.Value;
            processArgs();
        }

        private void trackBar4_Scroll(object sender, EventArgs e)
        {
            numericUpDown_sharpness.Value = trackBar_sharpness.Value;
            processArgs();
        }

        private void trackBar5_Scroll(object sender, EventArgs e)
        {
            numericUpDown_pass.Value = trackBar_pass.Value;
            processArgs();
        }

        private void numericUpDown2_ValueChanged(object sender, EventArgs e)
        {
            trackBar_alpha_q.Value = decimal.ToInt32(numericUpDown_alpha_q.Value);
            processArgs();
        }

        private void checkBox4_CheckedChanged(object sender, EventArgs e)
        {
            processArgs();
        }

        private void checkBox3_CheckedChanged(object sender, EventArgs e)
        {
            processArgs();
        }

        private void checkBox10_CheckedChanged(object sender, EventArgs e)
        {
            processArgs();
        }

        private void checkBox13_CheckedChanged(object sender, EventArgs e)
        {
            processArgs();
        }

        private void checkBox14_CheckedChanged(object sender, EventArgs e)
        {
            processArgs();
        }

        private void checkBox15_CheckedChanged(object sender, EventArgs e)
        {
            processArgs();
        }

        private void checkBox17_CheckedChanged(object sender, EventArgs e)
        {
            processArgs();
        }

        private void checkBox18_CheckedChanged(object sender, EventArgs e)
        {
            processArgs();
        }

        private void checkBox19_CheckedChanged(object sender, EventArgs e)
        {
            processArgs();
        }

        private void checkBox20_CheckedChanged(object sender, EventArgs e)
        {
            processArgs();
        }

        private void checkBox21_CheckedChanged(object sender, EventArgs e)
        {
            processArgs();
        }

        private void checkBox12_CheckedChanged(object sender, EventArgs e)
        {
            numericUpDown_map.Enabled = checkBox_map.Checked;
            processArgs();
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            processArgs();
        }

        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            processArgs();
        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            processArgs();
        }

        private void radioButton3_CheckedChanged(object sender, EventArgs e)
        {
            if (radioLossySize.Checked)
            {
                numericUpDown_Size.Visible = true;
                comboUnitSize.Visible = true;
            } else
            {
                numericUpDown_Size.Visible = false;
                comboUnitSize.Visible = false;
            }
            processArgs();
        }


        private void frmMain_Load(object sender, EventArgs e)
        {
            label_header.Width = 710;
            ReadSettings();
            comboUnitSize.SelectedIndex = 1;
            cboHint.SelectedIndex = 0;
            cbo_alpha_filter.SelectedIndex = 1;
        }

        private void button9_Click(object sender, EventArgs e)
        {
            tabControl1.SelectedIndex = 7;
            button_1.BackColor = Color.FromArgb(218, 218, 218);
            button_2.BackColor = Color.FromArgb(218, 218, 218);
            button_3.BackColor = Color.FromArgb(218, 218, 218);
            button_4.BackColor = Color.FromArgb(15, 157, 88);

            button_1.Enabled = true;
            button_2.Enabled = true;
            button_3.Enabled = true;
            button_4.Enabled = false;

            hideSubMenu(false);
        }

        private void button7_Click_1(object sender, EventArgs e)
        {
            tabControl1.SelectedIndex = 1;
            button_1.BackColor = Color.FromArgb(218, 218, 218);
            button_2.BackColor = Color.FromArgb(15, 157, 88);
            button_3.BackColor= Color.FromArgb(218, 218, 218);
            button_4.BackColor = Color.FromArgb(218, 218, 218);

            button_1.Enabled = true;
            button_2.Enabled = false;
            button_3.Enabled = true;
            button_4.Enabled = true;

            hideSubMenu(false);
        }

        private void button8_Click(object sender, EventArgs e)
        {
            // advanced options
            tabControl1.SelectedIndex = 2;
            button_1.BackColor = Color.FromArgb(218, 218, 218);
            button_2.BackColor = Color.FromArgb(218, 218, 218);
            button_3.BackColor = Color.FromArgb(15, 157, 88); 
            button_4.BackColor = Color.FromArgb(218, 218, 218);
            button_31.BackColor = Color.FromArgb(218, 218, 218);
            button_32.BackColor = Color.FromArgb(218, 218, 218);
            button_33.BackColor = Color.FromArgb(218, 218, 218);
            button_34.BackColor = Color.FromArgb(218, 218, 218);

            button_1.Enabled = true;
            button_2.Enabled = true;
            button_3.Enabled = false;
            button_4.Enabled = true;

            button_31.Enabled = true;
            button_32.Enabled = true;
            button_33.Enabled = true;
            button_34.Enabled = true;


            hideSubMenu(true);
        }

        private void hideSubMenu(Boolean menustatus)
        {
            button_31.Visible = menustatus;
            button_32.Visible = menustatus;
            button_33.Visible = menustatus;
            button_34.Visible = menustatus;
        }

        private void button6_Click_1(object sender, EventArgs e)
        {
            tabControl1.SelectedIndex = 0;
            button_1.BackColor = Color.FromArgb(15, 157, 88);
            button_2.BackColor = Color.FromArgb(218, 218, 218);
            button_3.BackColor = Color.FromArgb(218, 218, 218);
            button_4.BackColor = Color.FromArgb(218, 218, 218);

            button_1.Enabled = false;
            button_2.Enabled = true;
            button_3.Enabled = true;
            button_4.Enabled = true;

            hideSubMenu(false);
        }


        private void tabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {
            
        }

        private void button11_Click(object sender, EventArgs e)
        {
            tabControl1.SelectedIndex = 3;
            button_31.BackColor = Color.FromArgb(15, 157, 88);
            button_32.BackColor = Color.FromArgb(218, 218, 218);
            button_33.BackColor = Color.FromArgb(218, 218, 218);
            button_34.BackColor = Color.FromArgb(218, 218, 218);
            button_3.BackColor = Color.FromArgb(218, 218, 218);

            button_31.Enabled = false;
            button_32.Enabled = true;
            button_33.Enabled = true;
            button_34.Enabled = true;
            button_3.Enabled = true;
        }

        private void button12_Click(object sender, EventArgs e)
        {
            tabControl1.SelectedIndex = 4;
            button_31.BackColor = Color.FromArgb(218, 218, 218); 
            button_32.BackColor = Color.FromArgb(15, 157, 88);
            button_33.BackColor = Color.FromArgb(218, 218, 218);
            button_34.BackColor = Color.FromArgb(218, 218, 218);
            button_3.BackColor = Color.FromArgb(218, 218, 218);

            button_31.Enabled = true;
            button_32.Enabled = false;
            button_33.Enabled = true;
            button_34.Enabled = true;
            button_3.Enabled = true;
        }

        private void button13_Click(object sender, EventArgs e)
        {
            tabControl1.SelectedIndex = 5;
            button_31.BackColor = Color.FromArgb(218, 218, 218);
            button_32.BackColor = Color.FromArgb(218, 218, 218);
            button_33.BackColor = Color.FromArgb(15, 157, 88);
            button_34.BackColor = Color.FromArgb(218, 218, 218);
            button_3.BackColor = Color.FromArgb(218, 218, 218);

            button_31.Enabled = true;
            button_32.Enabled = true;
            button_33.Enabled = false;
            button_34.Enabled = true;
            button_3.Enabled = true;
        }

        private void button14_Click(object sender, EventArgs e)
        {
            tabControl1.SelectedIndex = 6;
            button_31.BackColor = Color.FromArgb(218, 218, 218);
            button_32.BackColor = Color.FromArgb(218, 218, 218);
            button_33.BackColor = Color.FromArgb(218, 218, 218);
            button_34.BackColor = Color.FromArgb(15, 157, 88);
            button_3.BackColor = Color.FromArgb(218, 218, 218);

            button_31.Enabled = true;
            button_32.Enabled = true;
            button_33.Enabled = true;
            button_34.Enabled = false;
            button_3.Enabled = true;
        }


        private void button10_Click(object sender, EventArgs e)
        {
            Clipboard.SetText(textBox_console.Text);
        }

        private void button15_Click(object sender, EventArgs e)
        {
            SaveSettings();

            AboutBox frm2 = new AboutBox();
            frm2.ShowDialog();
        }

        private void numericUpDown_sharpness_ValueChanged(object sender, EventArgs e)
        {
            trackBar_sharpness.Value = decimal.ToInt32(numericUpDown_sharpness.Value);
            processArgs();
        }

        private void trackBar_sharpness_Scroll(object sender, EventArgs e)
        {
            numericUpDown_sharpness.Value = trackBar_sharpness.Value;
            processArgs();
        }

        private void trackBar_f_Scroll(object sender, EventArgs e)
        {
            numericUpDown_f.Value = trackBar_f.Value;
            processArgs();
        }

        private void numericUpDown_f_ValueChanged(object sender, EventArgs e)
        {
            trackBar_f.Value = decimal.ToInt32(numericUpDown_f.Value);
            processArgs();
        }

        private void trackBar_m_Scroll(object sender, EventArgs e)
        {
            numericUpDown_m.Value = trackBar_m.Value;
            processArgs();
        }

        private void numericUpDown_m_ValueChanged(object sender, EventArgs e)
        {
            trackBar_m.Value = decimal.ToInt32(numericUpDown_m.Value);    
            processArgs();
        }

        private void cboPreset_SelectedIndexChanged(object sender, EventArgs e)
        {
            processArgs();
        }

        private void radioLossy_CheckedChanged(object sender, EventArgs e)
        {
            processArgs();
        }

        private void numericUpDown_z_ValueChanged(object sender, EventArgs e)
        {
            trackBar_z.Value = decimal.ToInt32(numericUpDown_z.Value);
            processArgs();
        }

        private void trackBar_z_Scroll(object sender, EventArgs e)
        {
            numericUpDown_z.Value = trackBar_z.Value;
            processArgs();
        }

        private void trackBar_segments_Scroll(object sender, EventArgs e)
        {
            numericUpDown_segments.Value = trackBar_segments.Value;
            processArgs();
        }

        private void numericUpDown_segments_ValueChanged(object sender, EventArgs e)
        {
            trackBar_segments.Value = decimal.ToInt32(numericUpDown_segments.Value);
            processArgs();
        }

        private void trackBar_sns_Scroll(object sender, EventArgs e)
        {
            numericUpDown_sns.Value = trackBar_sns.Value;
            processArgs();
        }

        private void numericUpDown_sns_ValueChanged(object sender, EventArgs e)
        {
            trackBar_sns.Value = decimal.ToInt32(numericUpDown_sns.Value);
            processArgs();
        }

        private void trackBar_near_lossless_Scroll(object sender, EventArgs e)
        {
            numericUpDown_near_lossless.Value = trackBar_near_lossless.Value;
            processArgs();
        }

        private void numericUpDown_near_lossless_ValueChanged(object sender, EventArgs e)
        {
            trackBar_near_lossless.Value = decimal.ToInt32(numericUpDown_near_lossless.Value);
            processArgs();
        }

        private void trackBar_pass_Scroll(object sender, EventArgs e)
        {
            numericUpDown_pass.Value = trackBar_pass.Value;
            processArgs();
        }

        private void numericUpDown_pass_ValueChanged(object sender, EventArgs e)
        {
            trackBar_pass.Value = decimal.ToInt32(numericUpDown_pass.Value);
            processArgs();
        }
    }

    public static class Globals
    {
        public static String args;
    }
}
