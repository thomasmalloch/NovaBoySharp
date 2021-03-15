using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using ScintillaNET;

namespace NovaBoySharp
{
    
    public partial class FormMain : Form
    {
        // Variables
        private byte[] Rom;
        private byte[] Bus;
        private CPU CPU;
        private int MaxLineNumberCharLength = 4;
        private volatile bool IsHalt = false;

        // Constant Strings
        private const string StartProgram =
@"; This program multiplies 10 by 3
; A is only 8 bits. Max value is $FF (255)
start:
			ld a,10			; load 10 into a
			ld b,3				; load 3 into b
loop:	dec b				; decrement b (subtract 1)
			jp z, end		; if b is 0, jump to the end
			add a,10			; add 10 to a
			jp loop			; jump to the top of the multiply loop
end:		halt					; end. a should be equal to 30
";
        private const string ASMHeader = "SECTION \"Header\", ROM0[$100]\nSECTION \"Game code\", ROM0\n";

        public FormMain()
        {
            this.InitializeComponent();
            this.Bus = new byte[0xFFFF + 1];

            // set up scintilla
            this.txtCode.Lexer = Lexer.Asm;

            Console.WriteLine(this.txtCode.DescribeKeywordSets());

            // cpu instructions
            this.txtCode.SetKeywords(0, string.Join(" ", Keywords.Instructions()));
            this.txtCode.SetKeywords(2, string.Join(" ", Keywords.Reserved()));

            for (int i = Style.Asm.Default; i < Style.Asm.CommentDirective; i++)
            {
                this.txtCode.Styles[i].Font = "Consolas";
                this.txtCode.Styles[i].Size = 10;
            }

            this.txtCode.Styles[Style.Asm.CpuInstruction].ForeColor = Color.Blue;
            this.txtCode.Styles[Style.Asm.ExtInstruction].ForeColor = Color.Blue;
            this.txtCode.Styles[Style.Asm.Register].ForeColor = Color.FromArgb(50,50,200);
            this.txtCode.Styles[Style.Asm.Directive].ForeColor = Color.DarkCyan;            
            this.txtCode.Styles[Style.Asm.Comment].ForeColor = Color.FromArgb(0, 128, 0);
            this.txtCode.Styles[Style.Asm.CommentBlock].ForeColor = Color.FromArgb(0, 128, 0);
            this.txtCode.Styles[Style.Asm.CommentDirective].ForeColor = Color.FromArgb(128, 128, 128);
            this.txtCode.Styles[Style.LineNumber].Font = "Consolas";

            this.txtCode.TabWidth = 4;
            this.txtCode.UseTabs = true;

            this.txtCode.Margins[0].Width = 16;
            this.txtCode.TextChanged += this.TxtCode_TextChanged;
            this.txtCode.Text = StartProgram;

        }
        
        private void TxtCode_TextChanged(object sender, EventArgs e)
        {
            // Did the number of characters in the line number display change?
            // i.e. nnn VS nn, or nnnn VS nn, etc...
            var maxLineNumberCharLength = this.txtCode.Lines.Count.ToString().Length;
            if (maxLineNumberCharLength == this.MaxLineNumberCharLength)
                return;

            // Calculate the width required to display the last line number
            // and include some padding for good measure.
            const int padding = 2;
            this.txtCode.Margins[0].Width = this.txtCode.TextWidth(Style.LineNumber, new string('9', maxLineNumberCharLength + 1)) + padding;
            this.MaxLineNumberCharLength = maxLineNumberCharLength;
        }

        private void OpenFile(string filename)
        {
            this.IsHalt = false;
            using (FileStream file = File.OpenRead(filename))
            {
                this.Rom = new byte[file.Length];
                file.Read(this.Rom, 0, (int)file.Length);
            }

            this.Bus = new byte[0xFFFF + 1];
            const long compare = -1;
            Queue<byte> check = new Queue<byte>();
            int index = 0;
            bool start = false;
            for (int i = 0; i < this.Rom.Length; i++)
            {
                if (!start)
                {
                    if (check.Count == 8)
                        check.Dequeue();

                    check.Enqueue(this.Rom[i]);
                    if (check.Count != 8)
                        continue;

                    if (compare == BitConverter.ToInt64(check.ToArray(), 0))
                    {
                        start = true;
                        i += 4;
                        continue;
                    }
                }

                if (!start)
                    continue;

                this.Bus[index++] = this.Rom[i];
            }

            this.CPU = new CPU(this.Bus);
            this.CPU.InstructionExecuted += this.CPU_CPUInstructionExecuted;
            this.lblStatus.Text = this.CPU.ToString();
            this.txtOutput.Text = "";
            this.CPU.CPUHalt += this.CPU_CPUHalt;
        }

        private void CPU_CPUInstructionExecuted(int code, int? data, string name, string status)
        {
            this.lblStatus.Text = status;
            string dataString = "Data: ";
            if (data == null)
                dataString += "--";
            else if (data < 256)
                dataString += string.Format("${0:X2}   ({0})", data);
            else
                dataString += string.Format("${0:X4} ({0})", data);

            this.txtOutput.Text += string.Format("{0,-20}{1}", name, dataString) + Environment.NewLine;
            this.txtOutput.SelectionStart = this.txtOutput.TextLength;
            this.txtOutput.ScrollToCaret();
        }

        private void BtnRun_Click(object sender, EventArgs e)
        {
            while (!this.IsHalt)
            {
                this.CPU.Run();
                Application.DoEvents();
            }
        }
        
        private void CPU_CPUHalt()
        {
            this.IsHalt = true;
        }

        private void BtnStep_Click(object sender, EventArgs e)
        {
            if(!this.IsHalt)
                this.CPU.Run();
        }

        private void BtnReset_Click(object sender, EventArgs e)
        {
            this.CPU = new CPU(this.Bus);
            this.CPU.InstructionExecuted += this.CPU_CPUInstructionExecuted;
            this.lblStatus.Text = this.CPU.ToString();
            this.txtOutput.Text = "";
            this.IsHalt = false;
            this.CPU.CPUHalt += this.CPU_CPUHalt;
        }

        private void BtnAssemble_Click(object sender, EventArgs e)
        {
            using (FileStream file = File.Open(Application.StartupPath + "\\asm\\temp.asm", FileMode.Create, FileAccess.Write, FileShare.None))
            using (StreamWriter writer = new StreamWriter(file, Encoding.ASCII))
                writer.Write(ASMHeader + this.txtCode.Text + "\nhalt");

            this.txtAssemblerOutput.Text = "";
            Process process = new Process();
            ProcessStartInfo info = new ProcessStartInfo();            
            info.WindowStyle = ProcessWindowStyle.Hidden;
            info.FileName = Application.StartupPath + "\\rgb\\rgbasm.exe";
            info.Arguments = $"-v -o \"{ Application.StartupPath + "\\roms\\rom.o" }\" \"{ Application.StartupPath + "\\asm\\temp.asm" }\"";
            info.RedirectStandardError = true;
            info.UseShellExecute = false;
            process.StartInfo = info;
            process.Start();
            
            process.WaitForExit();

            string output = process.StandardError.ReadToEnd();
            if (output.IndexOf("error", StringComparison.OrdinalIgnoreCase) != -1)
            {
                string line = Regex.Match(output, @"(?<=temp\.asm\()\d+")?.ToString();
                string type = output.Split(new[] {'\r', '\n'}, StringSplitOptions.RemoveEmptyEntries)[1].Trim();
                type = type[0].ToString().ToUpperInvariant()[0] + type.Substring(1);
                if (!int.TryParse(line, out int num))
                    num = 2;

                this.btnReset.Enabled = false;
                this.btnStop.Enabled = false;
                this.btnStep.Enabled = false;
                this.btnRun.Enabled = false;
                this.btnTurbo.Enabled = false;
                MessageBox.Show(this, $"{type} at line {num - 2}", "Assembler Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            this.OpenFile(Application.StartupPath + "\\roms\\rom.o");
            this.tabControls.SelectedTab = this.pageDebug;

            this.btnReset.Enabled = true;
            this.btnStop.Enabled = true;
            this.btnStep.Enabled = true;
            this.btnRun.Enabled = true;
            this.btnTurbo.Enabled = true;
        }

        private void mniOpenROM_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog dialog = new OpenFileDialog())
            {
                dialog.Filter = "BIN Files|*.bin|All Files|*.*";
                dialog.Multiselect = false;
                if (dialog.ShowDialog() != DialogResult.OK)
                    return;

                this.OpenFile(dialog.FileName);
            }

            this.tabControls.SelectedTab = this.pageDebug;
        }

        private void MniOpenASM_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog dialog = new OpenFileDialog())
            {
                dialog.Filter = "ASM Files|*.asm|All Files|*.*";
                dialog.Multiselect = false;
                if (dialog.ShowDialog() != DialogResult.OK)
                    return;

                using (FileStream file = File.Open(dialog.FileName, FileMode.Open, FileAccess.Read, FileShare.Read))
                using (StreamReader reader = new StreamReader(file))
                    this.txtCode.Text = reader.ReadToEnd();
            }

            this.IsHalt = true;
            this.tabControls.SelectedTab = this.pageDevelop;
        }

        private void MniSaveASM_Click(object sender, EventArgs e)
        {
            using (SaveFileDialog dialog = new SaveFileDialog())
            {
                dialog.Filter = "ASM Files|*.asm|All Files|*.*";                
                if (dialog.ShowDialog() != DialogResult.OK)
                    return;

                using (FileStream file = File.Open(dialog.FileName, FileMode.Create, FileAccess.Write, FileShare.None))
                using (StreamWriter writer = new StreamWriter(file, Encoding.ASCII))
                    writer.Write(this.txtCode.Text);
            }
        }

        private void MniClose_Click(object sender, EventArgs e)
        {
            this.IsHalt = true;
            this.Close();
        }

        protected override void OnClosed(EventArgs e)
        {
            this.IsHalt = true;
            base.OnClosed(e);
        }

        private void BtnStop_Click(object sender, EventArgs e)
        {
            this.IsHalt = true;
        }

        private async void BtnTurbo_Click(object sender, EventArgs e)
        {
            if (this.IsHalt)
                return;

            this.CPU.InstructionExecuted -= this.CPU_CPUInstructionExecuted;
            StringBuilder builder = new StringBuilder();
            this.CPU.InstructionExecuted += (code, data, name, status) => 
            {
                string dataString = "Data: ";
                if (data == null)
                    dataString += "--";
                else if (data < 256)
                    dataString += string.Format("${0:X2}   ({0})", data);
                else
                    dataString += string.Format("${0:X4} ({0})", data);

                builder.AppendLine($"{name,-20}{dataString}");
            };

            await Task.Run(() =>
            {
                while (!this.IsHalt)
                    this.CPU.Run();
            });

            this.lblStatus.Text = this.CPU.ToString();
            this.txtOutput.Text = builder.ToString();
            this.txtOutput.SelectionStart = this.txtOutput.TextLength;
            this.txtOutput.ScrollToCaret();
        }
    }
}
