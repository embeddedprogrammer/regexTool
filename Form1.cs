using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Text.RegularExpressions;
using System.IO;

namespace RegexTool
{
	public partial class Form1 : Form
	{
		public Form1()
		{
			InitializeComponent();
		}

		string replaceExp;
		string extractExp;
		string equalExp;
		string startComment;
		bool unescape;

		RegexOptions regexOptions;

		private void button1_Click(object sender, EventArgs e)
		{
			string text = richTextBox2.Text;
			regexOptions = RegexOptions.None;
			unescape = true;
			replaceExp = "->";
			extractExp = "=>";
			equalExp = "=";
			startComment = @"//";
			for(int i = 0; i < richTextBox1.Lines.Length; i++)
			{
				string line = richTextBox1.Lines[i];
				if (line.Contains(startComment))
				{
					line = line.Split(new string[] { startComment }, StringSplitOptions.None)[0];
				}
				if (line.Contains(replaceExp))
				{
					string[] parms = line.Split(new string[] { replaceExp }, StringSplitOptions.None);
					if (parms.Length != 2)
					{
						MessageBox.Show("Must contain only one instance of " + replaceExp + ". If need be, change it with replaceExp = \"*\"");
						return;
					}
					Regex rgx = new Regex(parms[0], regexOptions);
					text = rgx.Replace(text, unescape ? Regex.Unescape(parms[1]) : parms[1]);
				}
				if (line.Contains(extractExp))
				{
					string[] parms = line.Split(new string[] { extractExp }, StringSplitOptions.None);
					if (parms.Length != 2)
					{
						MessageBox.Show("Must contain only one instance of " + extractExp + ". If need be, change it with replaceExp = \"*\"");
						return;
					}
					Regex rgx = new Regex(parms[0], regexOptions);
					MatchCollection mc = rgx.Matches(text);
					text = "";
					foreach (Match m in mc)
					{
						text += ((text.Length > 0) ? "\n" : "") + 
							m.Result(unescape ? Regex.Unescape(parms[1]) : parms[1]);
					}
				}
				else if (line.Contains(equalExp))
				{
					string[] parms = line.Split(new string[] { equalExp }, StringSplitOptions.None);
					if (parms.Length != 2)
					{
						MessageBox.Show("Line must contain only one \"=\"");
						return;
					}
					SetParameter(parms[0].Trim(), parms[1].Trim());
				}
			}
			richTextBox3.Text = text;
		}

		private void SetParameter(string p, string o)
		{
			try
			{
				p = p.ToLowerInvariant();
				if (p == "replaceexp")
					replaceExp = o;
				else if (p == "extractexp")
					extractExp = o;
				else if (p == "equalexp")
					equalExp = o;
				else if (p == "startcomment")
					startComment = o;
				else if (p == "unescape")
					unescape = Convert.ToBoolean(o);
				else if (p == "compiled")
					SetRegexOption(RegexOptions.Compiled, Convert.ToBoolean(o));
				else if (p == "cultureinvariant")
					SetRegexOption(RegexOptions.CultureInvariant, Convert.ToBoolean(o));
				else if (p == "ecmascript")
					SetRegexOption(RegexOptions.ECMAScript, Convert.ToBoolean(o));
				else if (p == "explicitcapture")
					SetRegexOption(RegexOptions.ExplicitCapture, Convert.ToBoolean(o));
				else if (p == "ignorecase")
					SetRegexOption(RegexOptions.IgnoreCase, Convert.ToBoolean(o));
				else if (p == "ignorepatternwhitespace")
					SetRegexOption(RegexOptions.IgnorePatternWhitespace, Convert.ToBoolean(o));
				else if (p == "multiline")
					SetRegexOption(RegexOptions.Multiline, Convert.ToBoolean(o));
				else if (p == "righttoleft")
					SetRegexOption(RegexOptions.RightToLeft, Convert.ToBoolean(o));
				else if (p == "singleline")
					SetRegexOption(RegexOptions.Singleline, Convert.ToBoolean(o));
				else
				{

					MessageBox.Show(
					"The parameter you attempted to set is invalid. " +
					"Valid options are: StartComment, ReplaceExp, Unescape, Compiled, CultureInvariant, ECMAScript, ExplicitCapture," +
								"IgnoreCase, IgnorePatternWhitespace, Multiline, RightToLeft, Singleline");
				}
			}
			catch(Exception e)
			{
				MessageBox.Show(e.Message);
			}
		}
		
		private void SetRegexOption(RegexOptions op, bool b)
		{
			regexOptions = b ? (regexOptions | op) : (regexOptions & ~op);
		}

		Dictionary<RichTextBox, string> fileNames = new Dictionary<RichTextBox,string>();
		SaveFileDialog saveFileDialog1;
		OpenFileDialog openFileDialog1;

		private void richTextBox_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.Control && e.KeyCode == Keys.S)
			{
				if(e.Shift || !fileNames.ContainsKey((RichTextBox)sender))
				{
					// Save as
					if (saveFileDialog1 == null)
					{
						saveFileDialog1 = new SaveFileDialog();
						saveFileDialog1.Filter = "Text File (*.txt)|*.txt|All Files (*.*)|*.*";
						saveFileDialog1.FilterIndex = 1;
						saveFileDialog1.InitialDirectory = @"E:\";
					}

					if (saveFileDialog1.ShowDialog() == DialogResult.OK)
					{
						File.WriteAllText(saveFileDialog1.FileName, ((RichTextBox)sender).Text);
						fileNames[(RichTextBox)sender] = saveFileDialog1.FileName;
					}
				}
				else
				{
					File.WriteAllText(fileNames[(RichTextBox)sender], ((RichTextBox)sender).Text);
				}
			}
			else if (e.Control && e.KeyCode == Keys.O)
			{
				if (openFileDialog1 == null)
				{
					openFileDialog1 = new OpenFileDialog();
					openFileDialog1.Filter = "Text File (*.txt)|*.txt|All Files (*.*)|*.*";
					openFileDialog1.FilterIndex = 1;
					openFileDialog1.InitialDirectory = @"E:\"; //Application.ExecutablePath;
				}
				if (openFileDialog1.ShowDialog() == DialogResult.OK)
				{
					((RichTextBox)sender).Text = File.ReadAllText(openFileDialog1.FileName);
					fileNames[(RichTextBox)sender] = openFileDialog1.FileName;
				}
			}
			else if (e.Control && e.KeyCode == Keys.V)
			{
				((RichTextBox)sender).SelectedText = Clipboard.GetText();
				e.SuppressKeyPress = true;
			}
			else if (e.Control && e.KeyCode == Keys.N)
			{
				((RichTextBox)sender).Text = "";
				fileNames.Remove((RichTextBox)sender);
			}
		}
	}
}