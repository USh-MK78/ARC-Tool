using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Diagnostics;
using Microsoft.VisualBasic.FileIO;

namespace ARC_Tools_For_USh_MK78
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        public string ARCPackDirPath;

        public class ReadHeader
        {
            public char[] Header;
        }

        private void arcPack_Exited(object sender, EventArgs e)
        {

        }
        private void arc_yaz0_Pack_Exited(object sender, EventArgs e)
        {

        }
        private void arcExt_Exited(object sender, EventArgs e)
        {

        }
        private void arc_yaz0_Ext_Exited(object sender, EventArgs e)
        {

        }

        private void Form1_Load(object sender, EventArgs e)
        {
            string CD = System.IO.Directory.GetCurrentDirectory();
            //フォルダの存在確認
            string PathCheck = CD + "\\tools";
            if (Directory.Exists(PathCheck))
            {
                //既に該当フォルダが存在する場合は何もしない
            }
            else
            {
                DirectoryInfo di1 = new DirectoryInfo(CD + "\\tools");
                di1.Create();

                var yaz0d = Properties.Resources.yaz0dec;
                var yaz0e = Properties.Resources.yaz0enc;
                var arcExt = Properties.Resources.arcExtract;
                var arcPck = Properties.Resources.arcPack;
                FileStream fs1 = new FileStream(CD + "\\tools\\yaz0dec.exe", FileMode.Create, FileAccess.Write);
                FileStream fs2 = new FileStream(CD + "\\tools\\yaz0enc.exe", FileMode.Create, FileAccess.Write);
                FileStream fs3 = new FileStream(CD + "\\tools\\arcExtract.exe", FileMode.Create, FileAccess.Write);
                FileStream fs4 = new FileStream(CD + "\\tools\\arcPack.exe", FileMode.Create, FileAccess.Write);
                fs1.Write(yaz0d, 0, yaz0d.Length);
                fs2.Write(yaz0e, 0, yaz0e.Length);
                fs3.Write(arcExt, 0, arcExt.Length);
                fs4.Write(arcPck, 0, arcPck.Length);
                fs1.Close();
                fs2.Close();
                fs3.Close();
                fs4.Close();
                fs1.Dispose();
                fs2.Dispose();
                fs3.Dispose();
                fs4.Dispose();
            }
        }

        private async void button1_Click(object sender, EventArgs e)
        {
            //ファイルを開く
            OpenFileDialog Open_ARC = new OpenFileDialog()
            {
                Title = "Open",
                InitialDirectory = @"C:\Users\User\Desktop",
                Filter = "arc file|*.arc"
            };

            if (Open_ARC.ShowDialog() != DialogResult.OK) return;

            //ファイル名(拡張子なし)を取得
            string FileName = System.IO.Path.GetFileNameWithoutExtension(Open_ARC.FileName);

            //ファイル名(拡張子付き)を取得
            string FileName2 = System.IO.Path.GetFileName(Open_ARC.FileName);

            //ディレクトリを取得
            string PathDir = System.IO.Path.GetDirectoryName(Open_ARC.FileName);

            //カレントディレクトリを取得
            string CDir = System.IO.Directory.GetCurrentDirectory();

            //ヘッダーの読み取り
            System.IO.FileStream fs1 = new FileStream(Open_ARC.FileName, FileMode.Open, FileAccess.Read);
            BinaryReader br1 = new BinaryReader(fs1);
            ReadHeader rh = new ReadHeader();

            //ヘッダーの情報からRARCかYAZ0か判定
            rh.Header = br1.ReadChars(4);
            if (new string(rh.Header) == "RARC")
            {
                br1.Close();
                fs1.Close();

                //フォルダを生成
                FileSystem.CreateDirectory(PathDir + "\\" + FileName);

                //ファイルをコピー
                System.IO.File.Copy(Open_ARC.FileName, PathDir + "\\" + FileName + "\\" + FileName2, true);

                //arcファイルを解凍

                //プロセス(arc)
                System.Diagnostics.Process arcExt = new System.Diagnostics.Process();
                arcExt.StartInfo.FileName = CDir + "\\tools\\arcExtract.exe";
                arcExt.StartInfo.Arguments = "\"" + PathDir + "\\" + FileName + "\\" + FileName2 + "\"";
                arcExt.StartInfo.UseShellExecute = true;
                //イベントハンドラがフォームを作成したスレッドで実行されるようにする
                arcExt.SynchronizingObject = this;
                arcExt.Exited += new EventHandler(arcExt_Exited);
                //プロセス終了時にイベントを発生させる
                arcExt.EnableRaisingEvents = true;
                arcExt.Start();

                //ファイルの出力が完了するまでTask.Delayで待つ
                await Task.Delay(2000);

                //コピー先のファイル(arc)を削除
                System.IO.File.Delete(PathDir + "\\" + FileName + "\\" + FileName2);
            }
            else if (new string(rh.Header) == "Yaz0")
            {
                br1.Close();
                fs1.Close();

                //フォルダを生成
                FileSystem.CreateDirectory(PathDir + "\\" + FileName);

                //ファイルをコピー
                System.IO.File.Copy(Open_ARC.FileName, PathDir + "\\" + FileName + "\\" + FileName2, true);

                System.Diagnostics.Process arc_yaz0_Ext = new System.Diagnostics.Process();
                arc_yaz0_Ext.StartInfo.FileName = CDir + "\\tools\\yaz0dec.exe";
                arc_yaz0_Ext.StartInfo.Arguments = "\"" + PathDir + "\\" + FileName + "\\" + FileName2 + "\"";
                arc_yaz0_Ext.StartInfo.UseShellExecute = true;
                arc_yaz0_Ext.SynchronizingObject = this;
                arc_yaz0_Ext.Exited += new EventHandler(arc_yaz0_Ext_Exited);
                arc_yaz0_Ext.EnableRaisingEvents = true;
                arc_yaz0_Ext.Start();

                //Yaz0の解凍が完了するまでTask.Delayで待つ
                await Task.Delay(2000);

                System.Diagnostics.Process arcExt = new System.Diagnostics.Process();
                arcExt.StartInfo.FileName = CDir + "\\tools\\arcExtract.exe";
                arcExt.StartInfo.Arguments = "\"" + PathDir + "\\" + FileName + "\\" + FileName2 + "\"";
                arcExt.StartInfo.UseShellExecute = true;
                arcExt.SynchronizingObject = this;
                arcExt.Exited += new EventHandler(arcExt_Exited);
                arcExt.EnableRaisingEvents = true;
                arcExt.Start();

                //ファイルの出力が完了するまでTask.Delayで待つ
                await Task.Delay(2000);

                //コピー先のファイル(arc)を削除
                System.IO.File.Delete(PathDir + "\\" + FileName + "\\" + FileName2);
                System.IO.File.Delete(PathDir + "\\" + FileName + "\\" + FileName2 + " 0.rarc");
            }
        }

        private async void button2_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog fbd1 = new FolderBrowserDialog();
            fbd1.RootFolder = Environment.SpecialFolder.Desktop;
            fbd1.ShowNewFolderButton = true;
            if(fbd1.ShowDialog(this) == DialogResult.OK)
            {
                ARCPackDirPath = fbd1.SelectedPath;
            }
            else
            {
                //フォルダ選択ダイアログでキャンセルボタンが選択された場合はreturnで処理を終了
                return;
            }

            string CDir = System.IO.Directory.GetCurrentDirectory();

            //プロセス(arc)
            System.Diagnostics.Process arcPack = new System.Diagnostics.Process();
            arcPack.StartInfo.FileName = CDir + "\\tools\\arcPack.exe";
            arcPack.StartInfo.Arguments = "\"" + ARCPackDirPath + "\"";
            arcPack.StartInfo.UseShellExecute = true;
            arcPack.SynchronizingObject = this;
            arcPack.Exited += new EventHandler(arcPack_Exited);
            arcPack.EnableRaisingEvents = true;
            arcPack.Start();

            //ファイルが作成されるまでTask.Delayで待つ
            await Task.Delay(2000);

            if (checkBox1.Checked)
            {
                //プロセス(yaz0)
                System.Diagnostics.Process arc_yaz0_Pack = new System.Diagnostics.Process();
                arc_yaz0_Pack.StartInfo.FileName = CDir + "\\tools\\yaz0enc.exe";
                arc_yaz0_Pack.StartInfo.Arguments = "\"" + ARCPackDirPath + ".arc" + "\"";
                arc_yaz0_Pack.StartInfo.UseShellExecute = true;
                arc_yaz0_Pack.SynchronizingObject = this;
                arc_yaz0_Pack.Exited += new EventHandler(arc_yaz0_Pack_Exited);
                arc_yaz0_Pack.EnableRaisingEvents = true;
                arc_yaz0_Pack.Start();

                //ファイルが作成されるまでTask.Delayで待つ
                await Task.Delay(2000);

                //既にファイルが存在する場合は一度削除
                System.IO.File.Delete(ARCPackDirPath + ".arc");

                //ファイルが作成されるまでTask.Delayで待つ
                await Task.Delay(2000);

                //ファイルのリネーム
                System.IO.FileInfo FIren = new System.IO.FileInfo(ARCPackDirPath + ".arc.yaz0");
                FIren.MoveTo(ARCPackDirPath + ".arc");

                MessageBox.Show("Done!\r\n" + "arc(yaz0) File location : " + ARCPackDirPath + ".arc");
            }
            else
            {
                //yaz0圧縮しなかった場合はそのまま終了
                MessageBox.Show("Done!\r\n" + "arc File location : " + ARCPackDirPath + ".arc");
            }
        }
    }
}
