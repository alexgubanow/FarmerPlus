using System.Diagnostics;
using System;
using System.Collections.Generic;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Xml;
using System.Windows.Forms;
using Awesomium.Core;
using System.Web.Script.Serialization;
using Awesomium;
using System.Drawing.Printing;
using PdfSharp.Drawing;
using PdfSharp.Forms;
using MigraDoc.DocumentObjectModel;
using MigraDoc.DocumentObjectModel.IO;
using MigraDoc.Rendering;
using MigraDoc.Rendering.Printing;
using MigraDoc.Rendering.Forms;
using MigraDoc.RtfRendering;
using MigraDoc.DocumentObjectModel.Tables;
using FirebirdSql.Data.FirebirdClient;
using System.IO;

namespace farmer
{
    public partial class Form8 : Form
    {
        public Form8()
        {
            InitializeComponent();
            toolStrip3.Items.Add(new ToolStripControlHost(maskedTextBox1));
            toolStrip3.Items.Insert(1, toolStrip3.Items[toolStrip3.Items.Count - 1]);
            toolStrip3.Items[1].Visible = false;
            toolStrip3.Visible = false;
            toolStrip2.Visible = false;
        }
        int prevobl = -1;
        int prevray = -1;
        private void Form8_Load(object sender, EventArgs e)
        {
            webControl1.Source = new System.Uri("file:///" + Application.StartupPath + "/index.html");
            bdfunc.select("SELECT ALL NUM FROM FIELDS");
            toolStripComboBox9.Items.AddRange(variable.select.ToArray());
            {
                bdfunc.select("SELECT SQUARE FROM FARMER");
                string cumsum = 0.ToString();
                for (int i = 0; i < variable.select.Count; i++)
                {
                    cumsum = (Convert.ToDouble(cumsum) + Convert.ToDouble(variable.select[i])).ToString();
                }
                toolStripStatusLabel1.Text = main.sumsq + cumsum;
            }
        }
        
        private void maskedTextBox1_TextChanged(object sender, EventArgs e)
        {
            if (toolStripButton6.Checked != false && maskedTextBox1.Text.Length == 22 && webControl1.IsDocumentReady != false)
            {
                try
                {
                    toolStripButton1.Checked = false;
                    toolStripButton3.Checked = false;
                    toolStripButton4.Checked = false;
                    toolStripButton8.Checked = false;
                    toolStripButton10.Checked = false;
                    toolStripButton11.Checked = false;
                    toolStripButton12.Checked = false;
                    toolStripButton13.Checked = false;
                    toolStripDropDownButton1.Text = toolStripDropDownButton1.Text.Replace("\u2714", "");
                    foreach (ToolStripMenuItem item in toolStripDropDownButton1.DropDownItems)
                    {
                        if (item.Checked == true)
                        {
                            item.Checked = false;
                        }
                    }
                    bdfunc.select("SELECT ID,CONTRACT_NUMBER FROM FARMER WHERE CADASTRAL_NUMBER='" + maskedTextBox1.Text + "'");
                    variable.id = Convert.ToInt32(variable.select[0]);
                    bdfunc.select("SELECT X,Y FROM WGS84 WHERE ID='" + variable.id + "' AND RANK=0");
                    double[][] coords = new double[variable.select.Count / 2][];
                    int j = 0;
                    for (int i = 0; i < variable.select.Count / 2; i++)
                    {
                        coords[i] = new double[2];
                        coords[i][0] = Convert.ToDouble(variable.select[j].ToString().Replace('.', ','));
                        coords[i][1] = Convert.ToDouble(variable.select[j + 1].ToString().Replace('.', ','));
                        j = j + 2;
                    }
                    double[][][] temp = new double[1][][];
                    temp[0] = coords;
                    var json = new JavaScriptSerializer().Serialize(temp);
                    webControl1.ExecuteJavascript("lights('" + json + "',true)");
                }
                catch
                {
                    MessageBox.Show(main.cadnumless,main.Error);
                    maskedTextBox1.Text = "";
                }
            }
            else if (toolStripButton6.Checked != false && maskedTextBox1.Text != "          :  :   :" && webControl1.IsDocumentReady != false)
            {
                filterid();
            }
        }
        private void maskedTextBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if ((e.KeyChar <= 47 || e.KeyChar >= 58) && e.KeyChar != 8 && e.KeyChar != 22)
                e.Handled = true;
        }
        
        private void Awesomium_Windows_Forms_WebControl_LoadingFrameComplete(object sender, FrameEventArgs e)
        {
            if (e.IsMainFrame)
            {
                if (this.Text != "Карта")
                {
                    bdfunc.select("SELECT CADASTRAL_NUMBER FROM FARMER WHERE ID='" + variable.id + "'");
                    toolStripButton6.Checked = true;
                    toolStripButton2.PerformClick();
                    maskedTextBox1.Text = variable.select[0].ToString();
                }
            }
        }

        private JSValue update(JSValue[] result)
        {
            if (!result[0].IsNull | result[0].IsArray)
            {
                bdfunc.update("UPDATE FARMER SET AREA_NUMBER='" + result[0][1].ToString() + "' WHERE ID='" + result[0][0].ToString() + "'");
                filterid();
                return null;
            }
            else
            {
                MessageBox.Show(main.jsvalless, main.Error, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return null;
            }
        }
        private JSValue loadareas(JSValue[] result)
        {
            bdfunc.select("SELECT ID FROM FARMER");
            int[] listid = new int[variable.select.Count];
            variable.select.CopyTo(listid, 0);
            
            for (int k = 0; k < listid.Length; k++)
            {
                string cvet = "blue";
                bdfunc.select("SELECT CADASTRAL_NUMBER,OWNER,TYPE_CONTRACT,SQUARE,AREA_NUMBER,UNIT_NUMBER,EXCHANGE FROM FARMER WHERE ID='" + listid[k] + "'");
                if (variable.select[2].ToString() != "4" && variable.select[6].ToString() != "0")
                {
                    cvet = "black";
                }
                else if (variable.select[2].ToString() == "4")
                {
                    cvet = "cyan";
                }
                string descript = @"<p id=" + '"' + "index" + '"' + " style=" + '"' + "display:none" + '"' + ">" + listid[k] + "</p><p id=" + '"' + "cadnum" + '"' + ">" + main.cadnum + ": " + variable.select[0].ToString() + "</p><p id=" + '"' + "owner" + '"' + ">" + main.owner + ": " + variable.select[1].ToString() + "</p><p id=" + '"' + "square" + '"' + ">" + main.square + ": " + variable.select[3].ToString() + "</p><p id=" + '"' + "field" + '"' + ">" + main.field + ": <input id=" + '"' + "area_input" + '"' + " onchange=" + '"' + "enblbutton()" + '"' + " type=" + '"' + "text" + '"' + " style=" + '"' + "width: 60px" + '"' + " value=" + '"' + variable.select[4].ToString() + '"' + "/> <button id=" + '"' + "savebtn" + '"' + " style=" + '"' + "visibility: hidden" + '"' + " onclick=" + '"' + "updatefdb()" + '"' + ">" + main.save + "</button></p><p id=" + '"' + "unit_number" + '"' + ">" + main.unit_number + ": " + variable.select[5].ToString() + "</p>";
                bdfunc.select("SELECT RANK FROM WGS84 WHERE ID='" + listid[k] + "'");
                object[] listrank = variable.select.ToArray().Distinct().ToArray(); 
                //bdfunc.select("SELECT X,Y FROM WGS84 WHERE ID='" + listid[k] + "' AND RANK=0");
                double[][][] coords = new double[listrank.Length][][];
                
                for (int l = 0; l < listrank.Length; l++)
                {
                    bdfunc.select("SELECT X,Y FROM WGS84 WHERE ID='" + listid[k] + "' AND RANK='" + l + "'");
                    coords[l] = new double[variable.select.Count / 2][];
                    int j = 0;
                    for (int i = 0; i < variable.select.Count / 2; i++)
                    {
                        coords[l][i] = new double[2];
                        coords[l][i][0] = Convert.ToDouble(variable.select[j].ToString().Replace('.', ','));
                        coords[l][i][1] = Convert.ToDouble(variable.select[j + 1].ToString().Replace('.', ','));
                        j = j + 2;
                    }
                }
                int opt = 2;
                var latlng = new JavaScriptSerializer().Serialize(coords);
                webControl1.ExecuteJavascript("loadarea('" + latlng + "'" + "," + "'" + opt + "'" + "," + "'" + cvet + "'" + "," + "'" + descript + "')");
            }
            webControl1.ExecuteJavascript("init()");
            return null;
        }

        private void обновитьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            maskedTextBox1.Clear();
            webControl1.Reload(false);
        }

        private void Awesomium_Windows_Forms_WebControl_ShowContextMenu(object sender, ContextMenuEventArgs e)
        {
            contextMenuStrip1.Show(this, e.Info.MouseX, e.Info.MouseY);
        }

        Document document;
        PdfDocumentRenderer renderer;
        private void печатьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (saveFileDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                if (!webControl1.IsLive)
                    return;

                Graphics g = webControl1.CreateGraphics();
                File.Delete(AppDomain.CurrentDomain.BaseDirectory + @"\temp.png");
                int bmpHeight;
                int bmpWidth;
                using (Bitmap bmp = new Bitmap(webControl1.Width, webControl1.Height))
                {
                    webControl1.DrawToBitmap(bmp, new Rectangle(0, 0, webControl1.Width, webControl1.Height));
                    bmp.Save(AppDomain.CurrentDomain.BaseDirectory + @"\temp.png", System.Drawing.Imaging.ImageFormat.Png);
                    bmpHeight = bmp.Height;
                    bmpWidth = bmp.Width;
                }
                document = new Document();
                Style style = document.Styles["Normal"];
                style.Font.Name = "Times New Roman";
                style.Font.Size = 10;
                Section page = document.AddSection();
                PageSetup p = new PageSetup();
                p.OddAndEvenPagesHeaderFooter = true;
                p.StartingNumber = 1;
                p.BottomMargin = "5mm";
                p.LeftMargin = "5mm";
                p.RightMargin = "5mm";
                p.TopMargin = "5mm";
                p.PageFormat = PageFormat.A4;
                if (bmpHeight > 700 | bmpWidth > 1070 && bmpHeight <= 1050 && bmpWidth <= 1530)
                {
                    p.PageFormat = PageFormat.A3;
                }
                else if (bmpHeight > 1050 | bmpWidth > 1530 && bmpHeight <= 1530 && bmpWidth <= 2100)
                {
                    p.PageFormat = PageFormat.A2;
                }
                p.Orientation = MigraDoc.DocumentObjectModel.Orientation.Landscape;
                page.PageSetup = p;
                Paragraph paragraph = page.AddParagraph(this.Text);
                paragraph.Style = "Normal";
                paragraph = page.AddParagraph("");
                paragraph.Style = "Normal";
                MigraDoc.DocumentObjectModel.Shapes.Image map = paragraph.AddImage(@"temp.png");
                paragraph.Style = "Normal";
                paragraph = page.AddParagraph(toolStripStatusLabel1.Text);
                renderer = new PdfDocumentRenderer(true, PdfSharp.Pdf.PdfFontEmbedding.Always);
                renderer.Document = document;
                renderer.RenderDocument();
                renderer.Save(saveFileDialog1.FileName);
                Process.Start(saveFileDialog1.FileName);
            }
        }

        private void toolStripButton2_Click(object sender, EventArgs e)
        {
            toolStrip2.Visible = !toolStrip2.Visible;
            toolStrip3.Visible = !toolStrip3.Visible;
        }

        private void toolStripButton3_CheckedChanged(object sender, EventArgs e)
        {
            if (toolStripButton3.Checked == true)
            {
                toolStripButton3.Text = "\u2714 " + toolStripButton3.Text;
                toolStripComboBox1.Visible = true;
                toolStripComboBox1.Items.Clear();
                toolStripComboBox1.Text = "";
                bdfunc.select("SELECT NAME FROM OBL");
                toolStripComboBox1.Items.AddRange(variable.select.ToArray());
            }
            else
            {
                toolStripButton3.Text = toolStripButton3.Text.Replace("\u2714", "");
                toolStripComboBox1.Visible = false;
                toolStripComboBox1.Items.Clear();
                toolStripButton4.Checked = false;
                toolStripButton4.Text = toolStripButton4.Text.Replace("\u2714", "");
                toolStripComboBox2.Visible = false;
                toolStripComboBox2.Items.Clear();
                toolStripDropDownButton1.DropDownItems.Clear();
                toolStripComboBox1.SelectedIndex = -1;
                filterid();
            }
        }

        private void toolStripButton4_CheckedChanged(object sender, EventArgs e)
        {
            if (toolStripButton3.Checked == true && toolStripComboBox1.SelectedIndex != -1)
            {
                if (toolStripButton4.Checked == true)
                {
                    toolStripButton4.Text = "\u2714 " + toolStripButton4.Text;
                    toolStripComboBox2.Visible = true;
                    toolStripComboBox2.Items.Clear();
                    toolStripComboBox2.Text = "";
                    bdfunc.select("SELECT CODE FROM OBL WHERE NAME='" + toolStripComboBox1.SelectedItem + "'");
                    bdfunc.select("SELECT NAME FROM RAYON WHERE CODE>'" + Convert.ToInt64(variable.select[0]) + "' AND CODE<'" + (Convert.ToInt64(variable.select[0]) + 20000000) + "'");
                    toolStripComboBox2.Items.AddRange(variable.select.ToArray());
                }
                else
                {
                    toolStripButton4.Text = toolStripButton4.Text.Replace("\u2714", "");
                    toolStripComboBox2.Visible = false;
                    toolStripComboBox2.Items.Clear();
                    toolStripDropDownButton1.DropDownItems.Clear();
                    toolStripComboBox2.SelectedIndex = -1;
                    filterid();
                }
            }
            else
            {
                toolStripButton4.Checked = false;
            }
        }

        private void toolStripComboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (toolStripButton3.Checked == true && toolStripComboBox1.SelectedIndex != -1)
            {
                filterid();
            }
        }

        private void toolStripComboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (toolStripButton4.Checked == true && toolStripComboBox2.SelectedIndex != -1)
            {
                filterid();
            }
        }
        private void toolStripMenuItem_CheckedChanged(object sender, EventArgs e)
        {
            filterid();
        }

        private void toolStripButton6_CheckedChanged(object sender, EventArgs e)
        {
            if (toolStripButton6.Checked == true)
            {
                toolStripButton6.Text = "\u2714 " + toolStripButton6.Text;
                toolStrip3.Items[1].Visible = true;
            }
            else
            {
                toolStripButton6.Text = toolStripButton6.Text.Replace("\u2714", "");
                toolStrip3.Items[1].Visible = false;
                maskedTextBox1.Text = "";
                webControl1.ExecuteJavascript("removeLayer()");
                this.Text = "Карта";
            }
        }

        private void toolStripButton8_CheckedChanged(object sender, EventArgs e)
        {
            if (toolStripButton8.Checked == true)
            {
                toolStripButton8.Text = "\u2714 " + toolStripButton8.Text;
                toolStripComboBox4.Visible = true;
            }
            else
            {
                toolStripButton8.Text = toolStripButton8.Text.Replace("\u2714", "");
                toolStripComboBox4.Visible = false;
                toolStripComboBox4.SelectedIndex = -1;
                filterid();
            }
        }

        private void toolStripButton10_CheckedChanged(object sender, EventArgs e)
        {
            if (toolStripButton10.Checked == true)
            {
                toolStripButton10.Text = "\u2714 " + toolStripButton10.Text;
                toolStripComboBox5.Visible = true;
            }
            else
            {
                toolStripButton10.Text = toolStripButton10.Text.Replace("\u2714", "");
                toolStripComboBox5.Visible = false;
                toolStripComboBox5.SelectedIndex = -1;
                filterid();
            }
        }

        private void toolStripButton11_CheckedChanged(object sender, EventArgs e)
        {
            if (toolStripButton11.Checked == true)
            {
                toolStripButton11.Text = "\u2714 " + toolStripButton11.Text;
                toolStripComboBox6.Visible = true;
            }
            else
            {
                toolStripButton11.Text = toolStripButton11.Text.Replace("\u2714", "");
                toolStripComboBox6.Visible = false;
                toolStripComboBox6.SelectedIndex = -1;
                filterid();
            }
        }

        private void toolStripButton12_CheckedChanged(object sender, EventArgs e)
        {
            if (toolStripButton12.Checked == true)
            {
                toolStripButton12.Text = "\u2714 " + toolStripButton12.Text;
                toolStripComboBox7.Visible = true;
            }
            else
            {
                toolStripButton12.Text = toolStripButton12.Text.Replace("\u2714", "");
                toolStripComboBox7.Visible = false;
                toolStripComboBox7.SelectedIndex = -1;
                filterid();
            }
        }

        private void toolStripButton13_CheckedChanged(object sender, EventArgs e)
        {
            if (toolStripButton13.Checked == true)
            {
                toolStripButton13.Text = "\u2714 " + toolStripButton13.Text;
                toolStripComboBox8.Visible = true;
            }
            else
            {
                toolStripButton13.Text = toolStripButton13.Text.Replace("\u2714", "");
                toolStripComboBox8.Visible = false;
                toolStripComboBox8.SelectedIndex = -1;
                filterid();
            }
        }

        private void toolStripComboBox4_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (toolStripComboBox4.SelectedIndex != -1)
            {
                filterid();
            }
        }

        private void toolStripComboBox5_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (toolStripComboBox5.SelectedIndex != -1)
            {
                filterid();
            }
        }

        private void toolStripComboBox6_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (toolStripComboBox6.SelectedIndex != -1)
            {
                filterid();
            }
        }

        private void toolStripComboBox7_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (toolStripComboBox7.SelectedIndex != -1)
            {
                filterid();
            }
        }

        private void toolStripComboBox8_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (toolStripComboBox8.SelectedIndex != -1)
            {
                filterid();
            }
        }

        private void toolStripComboBox9_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (toolStripButton1.Checked == true)
            {
                if (toolStripComboBox9.SelectedIndex != -1)
                {
                    filterid();
                }
            }
        }
        private void toolStripButton1_CheckedChanged(object sender, EventArgs e)
        {
            if (toolStripButton1.Checked == true)
            {
                toolStripButton1.Text = "\u2714 " + toolStripButton1.Text;
                toolStripComboBox9.Visible = true;
            }
            else
            {
                toolStripButton1.Text = toolStripButton1.Text.Replace("\u2714", "");
                toolStripComboBox9.Visible = false;
                toolStripComboBox9.SelectedIndex = -1;
                filterid();
            }
        }
        private void filterid()
        {
            webControl1.ExecuteJavascript("removeLayer()");
            this.Text = "Карта";
            bdfunc.select("SELECT ID FROM FARMER");
            int[] listid = new int[variable.select.Count];
            int countcheck = 0;
            variable.select.CopyTo(listid, 0);
            if (toolStripComboBox9.SelectedIndex != -1)
            {
                if (this.Text != "Карта")
                {
                    this.Text = this.Text + ",  " + toolStripButton1.Text.Replace("\u2714", "") + "=" + toolStripComboBox9.SelectedItem;
                }
                else
                {
                    this.Text = this.Text + main.filtering + toolStripButton1.Text.Replace("\u2714", "") + "=" + toolStripComboBox9.SelectedItem;
                }
                countcheck++;
                bdfunc.select("SELECT ID FROM FARMER WHERE AREA_NUMBER='" + toolStripComboBox9.SelectedItem + "'");
                int[] listidsel = new int[variable.select.Count];
                variable.select.CopyTo(listidsel, 0);
                int[] temp;
                temp = listid.Where(t => listidsel.Contains(t)).ToArray();
                listid = new int[temp.Length];
                listid = temp;
            }
            if (toolStripComboBox8.SelectedIndex != -1)
            {
                if (this.Text != "Карта")
                {
                    this.Text = this.Text + ",  " + toolStripButton13.Text.Replace("\u2714", "") + "=" + toolStripComboBox8.SelectedItem;
                }
                else
                {
                    this.Text = this.Text + main.filtering + toolStripButton13.Text.Replace("\u2714", "") + "=" + toolStripComboBox8.SelectedItem;
                }
                countcheck++;
                bdfunc.select("SELECT ID FROM FARMER WHERE FORM_UNIT='" + toolStripComboBox8.SelectedIndex + "'");
                int[] listidsel = new int[variable.select.Count];
                variable.select.CopyTo(listidsel, 0);
                int[] temp;
                temp = listid.Where(t => listidsel.Contains(t)).ToArray();
                listid = new int[temp.Length];
                listid = temp;
            }
            if (toolStripComboBox7.SelectedIndex != -1)
            {
                if (this.Text != "Карта")
                {
                    this.Text = this.Text + ",  " + toolStripButton12.Text.Replace("\u2714", "") + "=" + toolStripComboBox7.SelectedItem;
                }
                else
                {
                    this.Text = this.Text + main.filtering + toolStripButton12.Text.Replace("\u2714", "") + "=" + toolStripComboBox7.SelectedItem;
                }
                countcheck++;
                bdfunc.select("SELECT ID FROM FARMER WHERE FORM_USE='" + toolStripComboBox7.SelectedIndex + "'");
                int[] listidsel = new int[variable.select.Count];
                variable.select.CopyTo(listidsel, 0);
                int[] temp;
                temp = listid.Where(t => listidsel.Contains(t)).ToArray();
                listid = new int[temp.Length];
                listid = temp;
            }
            if (toolStripComboBox6.SelectedIndex != -1)
            {
                if (this.Text != "Карта")
                {
                    this.Text = this.Text + ",  " + toolStripButton11.Text.Replace("\u2714", "") + "=" + toolStripComboBox6.SelectedItem;
                }
                else
                {
                    this.Text = this.Text + main.filtering + toolStripButton11.Text.Replace("\u2714", "") + "=" + toolStripComboBox6.SelectedItem;
                }
                countcheck++;
                bdfunc.select("SELECT ID FROM FARMER WHERE FORM_OWNER='" + toolStripComboBox6.SelectedIndex + "'");
                int[] listidsel = new int[variable.select.Count];
                variable.select.CopyTo(listidsel, 0);
                int[] temp;
                temp = listid.Where(t => listidsel.Contains(t)).ToArray();
                listid = new int[temp.Length];
                listid = temp;
            }
            if (toolStripComboBox5.SelectedIndex != -1)
            {
                if (this.Text != "Карта")
                {
                    this.Text = this.Text + ",  " + toolStripButton10.Text.Replace("\u2714", "") + "=" + toolStripComboBox5.SelectedItem;
                }
                else
                {
                    this.Text = this.Text + main.filtering + toolStripButton10.Text.Replace("\u2714", "") + "=" + toolStripComboBox5.SelectedItem;
                }
                countcheck++;
                bdfunc.select("SELECT ID FROM FARMER WHERE TYPE_CONTRACT='" + toolStripComboBox5.SelectedIndex + "'");
                int[] listidsel = new int[variable.select.Count];
                variable.select.CopyTo(listidsel, 0);
                int[] temp;
                temp = listid.Where(t => listidsel.Contains(t)).ToArray();
                listid = new int[temp.Length];
                listid = temp;
            }
            if (toolStripComboBox4.SelectedIndex != -1)
            {
                if (this.Text != "Карта")
                {
                    this.Text = this.Text + ",  " + toolStripButton8.Text.Replace("\u2714", "") + "=" + toolStripComboBox4.SelectedItem;
                }
                else
                {
                    this.Text = this.Text + main.filtering + toolStripButton8.Text.Replace("\u2714", "") + "=" + toolStripComboBox4.SelectedItem;
                }
                countcheck++;
                bdfunc.select("SELECT ID FROM FARMER WHERE CONTRACT_STATUS='" + toolStripComboBox4.SelectedIndex + "'");
                int[] listidsel = new int[variable.select.Count];
                variable.select.CopyTo(listidsel, 0);
                int[] temp;
                temp = listid.Where(t => listidsel.Contains(t)).ToArray();
                listid = new int[temp.Length];
                listid = temp;
            }
            if (toolStripButton3.Checked == true && toolStripComboBox1.SelectedIndex != -1 && prevobl != toolStripComboBox1.SelectedIndex)
            {
                if (this.Text != "Карта")
                {
                    this.Text = this.Text + ",  " + toolStripButton3.Text.Replace("\u2714", "") + "=" + toolStripComboBox1.SelectedItem;
                }
                else
                {
                    this.Text = this.Text + main.filtering + toolStripButton3.Text.Replace("\u2714", "") + "=" + toolStripComboBox1.SelectedItem;
                }
                countcheck++;
                toolStripComboBox2.Items.Clear();
                toolStripComboBox2.Text = "";
                bdfunc.select("SELECT CODE FROM OBL WHERE NAME='" + toolStripComboBox1.SelectedItem + "'");
                bdfunc.select("SELECT NAME FROM RAYON WHERE CODE>='" + Convert.ToInt64(variable.select[0]) + "' AND CODE<='" + (Convert.ToInt64(variable.select[0]) + 20000000) + "'");
                toolStripComboBox2.Items.AddRange(variable.select.ToArray());
                webControl1.ExecuteJavascript("find_locat('" + toolStripComboBox1.SelectedItem + " область" + "',8)");
                bdfunc.select("SELECT CODE FROM OBL WHERE NAME='" + toolStripComboBox1.SelectedItem + "'");
                bdfunc.select("SELECT ID FROM FARMER WHERE KOATUU>='" + Convert.ToInt64(variable.select[0]) + "' AND KOATUU<='" + (Convert.ToInt64(variable.select[0]) + 20000000) + "'");
                int[] listidsel = new int[variable.select.Count];
                variable.select.CopyTo(listidsel, 0);
                int[] temp;
                temp = listid.Where(t => listidsel.Contains(t)).ToArray();
                listid = new int[temp.Length];
                listid = temp;
            }
            else if (toolStripButton3.Checked == true && toolStripComboBox1.SelectedIndex != -1)
            {
                if (this.Text != "Карта")
                {
                    this.Text = this.Text + ",  " + toolStripButton3.Text.Replace("\u2714", "") + "=" + toolStripComboBox1.SelectedItem;
                }
                else
                {
                    this.Text = this.Text + main.filtering + toolStripButton3.Text.Replace("\u2714", "") + "=" + toolStripComboBox1.SelectedItem;
                }
                countcheck++;
            }
            prevobl = toolStripComboBox1.SelectedIndex;
            if (toolStripButton4.Checked == true && toolStripComboBox2.SelectedIndex != -1 && prevray != toolStripComboBox2.SelectedIndex)
            {
                if (this.Text != "Карта")
                {
                    this.Text = this.Text + ",  " + toolStripButton4.Text.Replace("\u2714", "") + "=" + toolStripComboBox2.SelectedItem;
                }
                else
                {
                    this.Text = this.Text + main.filtering + toolStripButton4.Text.Replace("\u2714", "") + "=" + toolStripComboBox2.SelectedItem;
                }
                countcheck++;
                toolStripDropDownButton1.DropDownItems.Clear();
                bdfunc.select("SELECT CODE FROM OBL WHERE NAME='" + toolStripComboBox1.SelectedItem + "'");
                bdfunc.select("SELECT CODE FROM RAYON WHERE NAME='" + toolStripComboBox2.SelectedItem + "' AND CODE>='" + Convert.ToInt64(variable.select[0]) + "' AND CODE<='" + (Convert.ToInt64(variable.select[0]) + 20000000) + "'");
                bdfunc.select("SELECT NAME FROM SIL WHERE CODE>='" + Convert.ToInt64(variable.select[0]) + "' AND CODE<='" + (Convert.ToInt64(variable.select[0]) + 100000) + "'");
                ToolStripMenuItem[] items = new ToolStripMenuItem[variable.select.Count];
                string[] tempstr = new string[variable.select.Count];
                variable.select.CopyTo(tempstr, 0);
                for (int i = 0; i < variable.select.Count; i++)
                {
                    items[i] = new ToolStripMenuItem();
                    items[i].Name = "toolStripMenuItem'" + i + "'";
                    items[i].Text = tempstr[i];
                    items[i].CheckOnClick = true;
                    items[i].CheckedChanged += new System.EventHandler(this.toolStripMenuItem_CheckedChanged);
                }
                toolStripDropDownButton1.DropDownItems.AddRange(items);
                webControl1.ExecuteJavascript("find_locat('" + toolStripComboBox1.SelectedItem + " область," + toolStripComboBox2.SelectedItem + " район" + "',11)");
                bdfunc.select("SELECT CODE FROM OBL WHERE NAME='" + toolStripComboBox1.SelectedItem + "'");
                bdfunc.select("SELECT CODE FROM RAYON WHERE NAME='" + toolStripComboBox2.SelectedItem + "' AND CODE>='" + Convert.ToInt64(variable.select[0]) + "' AND CODE<='" + (Convert.ToInt64(variable.select[0]) + 20000000) + "'");
                bdfunc.select("SELECT ID FROM FARMER WHERE KOATUU>='" + Convert.ToInt64(variable.select[0]) + "' AND KOATUU<='" + (Convert.ToInt64(variable.select[0]) + 100000) + "'");
                int[] listidsel = new int[variable.select.Count];
                variable.select.CopyTo(listidsel, 0);
                int[] temp;
                temp = listid.Where(t => listidsel.Contains(t)).ToArray();
                listid = new int[temp.Length];
                listid = temp;
            }
            else if (toolStripButton4.Checked == true && toolStripComboBox2.SelectedIndex != -1)
            {
                if (this.Text != "Карта")
                {
                    this.Text = this.Text + ",  " + toolStripButton4.Text.Replace("\u2714", "") + "=" + toolStripComboBox2.SelectedItem;
                }
                else
                {
                    this.Text = this.Text + main.filtering + toolStripButton4.Text.Replace("\u2714", "") + "=" + toolStripComboBox2.SelectedItem;
                }
                countcheck++;
            }
            prevray = toolStripComboBox2.SelectedIndex;
            string listsil = "";
            foreach (ToolStripMenuItem item in toolStripDropDownButton1.DropDownItems)
            {
                if (item.Checked == true)
                {
                    bdfunc.select("SELECT CODE FROM OBL WHERE NAME='" + toolStripComboBox1.SelectedItem + "'");
                    bdfunc.select("SELECT CODE FROM RAYON WHERE NAME='" + toolStripComboBox2.SelectedItem + "' AND CODE>='" + Convert.ToInt64(variable.select[0]) + "' AND CODE<='" + (Convert.ToInt64(variable.select[0]) + 20000000) + "'");
                    bdfunc.select("SELECT CODE FROM SIL WHERE NAME='" + item.Text + "' AND CODE>='" + Convert.ToInt64(variable.select[0]) + "' AND CODE<='" + (Convert.ToInt64(variable.select[0]) + 100000) + "'");
                    bdfunc.select("SELECT ID FROM FARMER WHERE KOATUU>='" + Convert.ToInt64(variable.select[0]) + "' AND KOATUU<='" + (Convert.ToInt64(variable.select[0]) + 100) + "'");
                    if (variable.select.Count != 0)
                    {
                        toolStripDropDownButton1.Text = "\u2714 " + toolStripDropDownButton1.Text;
                        if (listsil != "")
                        {
                            listsil = listsil + ", " + item.Text;
                        }
                        else
                        {
                            listsil = item.Text;
                        }
                    }
                    else
                    {
                        MessageBox.Show(main.lessunit + item.Text.ToString().ToLower() + " сільська рада");
                        item.Checked = false;
                    }
                }
            }
            if (listsil != "")
            {
                ArrayList listsilid = new ArrayList();
                ArrayList coords = new ArrayList();
                foreach (ToolStripMenuItem item in toolStripDropDownButton1.DropDownItems)
                {
                    if (item.Checked == true)
                    {
                        bdfunc.select("SELECT CODE FROM OBL WHERE NAME='" + toolStripComboBox1.SelectedItem + "'");
                        bdfunc.select("SELECT CODE FROM RAYON WHERE NAME='" + toolStripComboBox2.SelectedItem + "' AND CODE>='" + Convert.ToInt64(variable.select[0]) + "' AND CODE<='" + (Convert.ToInt64(variable.select[0]) + 20000000) + "'");
                        bdfunc.select("SELECT CODE FROM SIL WHERE NAME='" + item.Text + "' AND CODE>='" + Convert.ToInt64(variable.select[0]) + "' AND CODE<='" + (Convert.ToInt64(variable.select[0]) + 100000) + "'");
                        bdfunc.select("SELECT ID FROM FARMER WHERE KOATUU>='" + Convert.ToInt64(variable.select[0]) + "' AND KOATUU<='" + (Convert.ToInt64(variable.select[0]) + 100) + "'");
                        listsilid.AddRange(variable.select.ToArray());
                        if (variable.select.Count != 0)
                        {
                            bdfunc.select("SELECT X,Y FROM WGS84 WHERE ID='" + variable.select[0] + "' AND RANK=0");
                            if (variable.select.Count != 0)
                            {
                                coords.Add(Convert.ToDouble(variable.select[0].ToString().Replace('.', ',')));
                                coords.Add(Convert.ToDouble(variable.select[1].ToString().Replace('.', ',')));
                            }
                            else
                            {
                                MessageBox.Show("В договоре ID='" + variable.select[0] + "' нет координат", main.Error);
                                break;
                            }
                        }
                        else
                        {
                            MessageBox.Show(main.lessunit + item.Text.ToString().ToLower() + " сільська рада");
                            item.Checked = false;
                        }
                    }
                }
                if (coords.Count != 0)
                {
                    double[] lats = new double[coords.Count / 2];
                    double[] lons = new double[coords.Count / 2];
                    int k = 0;
                    for (int i = 0; i < coords.Count; i += 2)
                    {
                        lats[k] = Convert.ToDouble(coords[i]);
                        lons[k] = Convert.ToDouble(coords[i + 1]);
                        k++;
                    }
                    double lat = lats.Average();
                    double lon = lons.Average();
                    double[][] point = new double[1][];
                    point[0] = new double[2];
                    point[0][0] = lat;
                    point[0][1] = lon;
                    var json = new JavaScriptSerializer().Serialize(point);
                    webControl1.ExecuteJavascript("pan_to_sil('" + json + "')");
                }
                if (this.Text != "Карта")
                {
                    this.Text = this.Text + ",  " + toolStripDropDownButton1.Text.Replace("\u2714", "") + "=" + listsil;
                }
                else
                {
                    this.Text = this.Text + main.filtering + toolStripDropDownButton1.Text.Replace("\u2714", "") + "=" + listsil;
                }
                countcheck++;
                int[] listidsel = new int[listsilid.Count];
                listsilid.CopyTo(listidsel, 0);
                int[] temp;
                temp = listid.Where(t => listidsel.Contains(t)).ToArray();
                listid = new int[temp.Length];
                listid = temp;
            }
            else
            {
                toolStripDropDownButton1.Text = toolStripDropDownButton1.Text.Replace("\u2714", "");
            }
            if (toolStripButton6.Checked != false && maskedTextBox1.Text.Length != 22 && maskedTextBox1.Text != "          :  :   :" && webControl1.IsDocumentReady != false)
            {
                string cad = "";
                for (int i = 0; i < maskedTextBox1.Text.Length; i++)
                {
                    if (maskedTextBox1.Text[i] != ' ' && maskedTextBox1.Text[i] != ':')
                    {
                        cad = cad + maskedTextBox1.Text[i];
                    }
                    else if (i + 1 < maskedTextBox1.Text.Length)
                    {
                        if (maskedTextBox1.Text[i + 1] != ' ' && maskedTextBox1.Text[i] == ':')
                        {
                            cad = cad + maskedTextBox1.Text[i];
                        }
                    }
                    else
                    {
                        break;
                    }
                }
                if (this.Text != "Карта")
                {
                    this.Text = this.Text + ",  " + toolStripButton6.Text.Replace("\u2714", "") + "=" + cad;
                }
                else
                {
                    this.Text = this.Text + main.filtering + toolStripButton6.Text.Replace("\u2714", "") + "=" + cad;
                }
                countcheck++;
                bdfunc.select("SELECT ID FROM FARMER WHERE SUBSTRING(CADASTRAL_NUMBER FROM 1 FOR " + cad.Length + ")='" + cad + "'");
                int[] listidsel = new int[variable.select.Count];
                variable.select.CopyTo(listidsel, 0);
                int[] temp;
                temp = listid.Where(t => listidsel.Contains(t)).ToArray();
                listid = new int[temp.Length];
                listid = temp;
            }
            {
                bdfunc.select("SELECT SQUARE FROM FARMER");
                string cumsum = 0.ToString();
                for (int i = 0; i < variable.select.Count; i++)
                {
                    cumsum = (Convert.ToDouble(cumsum) + Convert.ToDouble(variable.select[i])).ToString();
                }
                toolStripStatusLabel1.Text = main.sumsq + cumsum;
            }
            {
                string cumsum = 0.ToString();
                for (int i = 0; i < listid.Length; i++)
                {
                    bdfunc.select("SELECT SQUARE FROM FARMER WHERE ID='" + listid[i] + "'");
                    cumsum = (Convert.ToDouble(cumsum) + Convert.ToDouble(variable.select[0])).ToString();
                }
                toolStripStatusLabel1.Text = toolStripStatusLabel1.Text + main.sumsqfil + cumsum;
            }
            if (countcheck != 0)
            {
                if (listid.Length != 0)
                {
                    double[][][] temp = new double[listid.Length][][];
                    for (int k = 0; k < listid.Length; k++)
                    {
                        temp[k] = new double[1][];
                        bdfunc.select("SELECT X,Y FROM WGS84 WHERE ID='" + listid[k] + "' AND RANK=0");
                        if (variable.select.Count != 0)
                        {
                            double[][] coords = new double[variable.select.Count / 2][];
                            int j = 0;
                            for (int i = 0; i < variable.select.Count / 2; i++)
                            {
                                coords[i] = new double[2];
                                coords[i][0] = Convert.ToDouble(variable.select[j].ToString().Replace('.', ','));
                                coords[i][1] = Convert.ToDouble(variable.select[j + 1].ToString().Replace('.', ','));
                                j = j + 2;
                            }
                            temp[k] = coords;
                        }
                        else
                        {
                            MessageBox.Show("В договоре ID='" + listid[k] + "' нет координат", main.Error);
                            toolStripComboBox4.SelectedIndex = 0;
                            break;
                        }
                    }
                    var json = new JavaScriptSerializer().Serialize(temp);
                    webControl1.ExecuteJavascript("lights('" + json + "',false)");
                }
                else
                {
                    MessageBox.Show(main.contrless, main.Error);
                }
            }
        }

        private void Awesomium_Windows_Forms_WebControl_DocumentReady(object sender, DocumentReadyEventArgs e)
        {
            using (JSObject myGlobalObject = webControl1.CreateGlobalJavascriptObject("myGlobalObject"))
            {
                myGlobalObject.Bind("update", update);
                myGlobalObject.Bind("loadareas", loadareas);
            }
        }
    }
}
