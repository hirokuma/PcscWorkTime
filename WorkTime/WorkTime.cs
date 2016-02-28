﻿using System;
using System.ComponentModel;
using System.Windows.Forms;

//https://github.com/danm-de/pcsc-sharp
//https://danm.de/docs/pcsc-sharp/index.html
using PCSC;
using PCSC.Iso7816;
using System.Threading;

namespace PcscSample01
{
    public partial class WorkTime : Form
    {
        SCardContext mContext;
        SCardReader mReader;

        //アプリケーション識別子
        byte[][] AID = new byte[][]
        {
            new byte[] { 0xa0, 0x00, 0x00, 0x02, 0x31, 0x01, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 },
            new byte[] { 0xa0, 0x00, 0x00, 0x02, 0x31, 0x02, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 },
            new byte[] { 0xa0, 0x00, 0x00, 0x02, 0x48, 0x03, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 }
        };
        //byte[] AID_DF1 = new byte[] { 0xa0, 0x00, 0x00, 0x02, 0x31, 0x01, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };
        //byte[] AID_DF2 = new byte[] { 0xa0, 0x00, 0x00, 0x02, 0x31, 0x02, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };
        //byte[] AID_DF3 = new byte[] { 0xa0, 0x00, 0x00, 0x02, 0x48, 0x03, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };

        public WorkTime()
        {
            InitializeComponent();

            //Enabled=false
            foreach (Control ctrl in groupControl.Controls)
            {
                ctrl.Enabled = false;
            }

            //リソースマネージャとの接続
            mContext = new SCardContext();
            mContext.Establish(SCardScope.User);
            mReader = new SCardReader(mContext);

            int num = updateComboRw();
            buttonConnect.Enabled = (num != 0);
        }

        private void buttonRwDetect_Click(object sender, EventArgs e)
        {
            int num = updateComboRw();
            buttonConnect.Enabled = (num != 0);
            if (num == 0)
            {
                MessageBox.Show("R/W not found", "NOT FOUND", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void buttonConnect_Click(object sender, EventArgs e)
        {
            if (!mReader.IsConnected)
            {
                //接続
                SCardError err = mReader.Connect((string)comboRw.SelectedItem, SCardShareMode.Shared, SCardProtocol.Any);
                if (err == SCardError.Success)
                {
                    buttonConnect.Text = "Disconnect";
                    buttonRwDetect.Enabled = false;
                    comboRw.Enabled = false;

                    foreach (Control ctrl in groupControl.Controls)
                    {
                        ctrl.Enabled = true;
                    }
                    buttonWatch.Enabled = false;
                }
                else
                {
                    MessageBox.Show("Fail connect.");
                }
            }
            else
            {
                //切断
                SCardError err = mReader.Disconnect(SCardReaderDisposition.Leave);
                if (err == SCardError.Success)
                {
                    buttonConnect.Text = "Connect";
                    buttonRwDetect.Enabled = true;
                    comboRw.Enabled = true;

                    foreach (Control ctrl in groupControl.Controls)
                    {
                        ctrl.Enabled = false;
                    }
                    buttonWatch.Enabled = true;
                }
                else
                {
                    MessageBox.Show("Fail disconnect.");
                }
            }
        }

        private void buttonWatch_Click(object sender, EventArgs e)
        {
            //カードが載っていることを定期監視しよう
            if (!backgroundWorker.IsBusy)
            {
                buttonRwDetect.Enabled = false;
                buttonConnect.Enabled = false;
                comboRw.Enabled = false;
                buttonWatch.Text = "Watching...";

                backgroundWorker.RunWorkerAsync();
            }
            else
            {
                backgroundWorker.CancelAsync();
            }
        }

        private void backgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            while(!backgroundWorker.CancellationPending)
            {
                //
                Thread.Sleep(1000);
                backgroundWorker.ReportProgress(0);
            }
        }

        private void backgroundWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            string btn_str = "Fail connect.";

            //接続
            SCardError err = mReader.Connect((string)comboRw.SelectedItem, SCardShareMode.Shared, SCardProtocol.Any);
            if (err == SCardError.Success)
            {
                bool ret;

                string data = "";
                ret = sendGetData(mReader, ref data);
                if (ret)
                {
                    btn_str = data;
                }

                string stat = "";
                ret = readerStatus(mReader, ref stat);
                textStatus.Text = stat;

                //切断
                err = mReader.Disconnect(SCardReaderDisposition.Leave);
            }
            else
            {
                //エラー
            }

            buttonWatch.Text = btn_str;
        }

        private void backgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            buttonWatch.Text = "Watch";
            buttonRwDetect.Enabled = true;
            buttonConnect.Enabled = true;
            comboRw.Enabled = true;
        }


        ///////////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// R/W一覧
        /// </summary>
        /// <returns></returns>
        private int updateComboRw()
        {
            comboRw.Items.Clear();
            string[] readerNames = mContext.GetReaders();
            if ((readerNames == null) || (readerNames.Length == 0))
            {
                return 0;
            }

            foreach (string reader in readerNames)
            {
                comboRw.Items.Add(reader);
            }
            comboRw.SelectedIndex = 0;

            return readerNames.Length;
        }


        private bool readerStatus(ISCardReader reader, ref string str_atr)
        {
            string[] readerNames;
            SCardProtocol proto;
            SCardState state;
            byte[] atr;
            SCardError err;

            err = reader.Status(out readerNames, out state, out proto, out atr);
            if (err == SCardError.Success)
            {
                str_atr = "[A]" + BitConverter.ToString(atr) + " [P]" + proto + " [S]" + state;
            }
            else
            {
                //MessageBox.Show("Fail Status.", "STATUS");
            }

            return err == SCardError.Success;
        }

        private bool sendGetData(ISCardReader reader, ref string data)
        {
            SCardError err;

            err = reader.BeginTransaction();
            if (err != SCardError.Success)
            {
                MessageBox.Show("Fail BeginTransaction.", "BEGIN TRANSACTION");
                return false;
            }

            // pcsc3_v2.01.09.pdf
            // p.31
            // "3.2.2.1.3  Get Data Command"
            CommandApdu apdu = new CommandApdu(IsoCase.Case2Short, reader.ActiveProtocol)
            {
                CLA = 0xff,
                Instruction = InstructionCode.GetData,
                P1 = 0x00,
                P2 = 0x00,
                Le = 0
            };
            SCardPCI recvPci = new SCardPCI();      //protocol control information
            byte[] res = new byte[10];
            err = reader.Transmit(
                SCardPCI.GetPci(reader.ActiveProtocol),
                apdu.ToArray(),
                recvPci,
                ref res);
            reader.EndTransaction(SCardReaderDisposition.Leave);

            if (err == SCardError.Success)
            {
                ResponseApdu resApdu = new ResponseApdu(res, IsoCase.Case2Short, reader.ActiveProtocol);
                if ((resApdu.SW1 == (byte)SW1Code.Normal) && (resApdu.SW2 == 0x00))
                {
                    data = BitConverter.ToString(resApdu.GetData());
                }
            }
            else
            {
                //
            }

            return err == SCardError.Success;
        }

        private void sendSelectFile(ISCardReader reader, int ef)
        {
            byte[] efid = new byte[2];

            efid[0] = (byte)((ef & 0xff00) >> 8);
            efid[1] = (byte)( ef & 0x00ff);
            sendSelectFile(reader, FileType.Elementary, efid);
        }

        private void sendSelectFile(ISCardReader reader, FileType type, byte[] aid)
        {
            SCardError err;
            string typeName = "";

            err = reader.BeginTransaction();
            if (err != SCardError.Success)
            {
                MessageBox.Show("Fail BeginTransaction.", "BEGIN TRANSACTION");
                return;
            }

            CommandApdu apdu = null;
            switch (type)
            {
                case FileType.Dedicated:
                    if (aid == null)
                    {
                        //MF
                        apdu = new CommandApdu(IsoCase.Case1, reader.ActiveProtocol)
                        {
                            P1 = 0x00,
                            P2 = 0x00       //MFはこれでもFCI無し
                        };
                        typeName = "Dedicated(MF)";
                    }
                    else
                    {
                        //DF
                        apdu = new CommandApdu(IsoCase.Case3Short, reader.ActiveProtocol)
                        {
                            P1 = 0x04,
                            P2 = 0x0c,      //FCI無し
                            Data = aid,
                        };
                        typeName = "Dedicated(DF)";
                    }
                    break;

                case FileType.Elementary:
                    //EF
                    apdu = new CommandApdu(IsoCase.Case3Short, reader.ActiveProtocol)
                    {
                        P1 = 0x02,
                        P2 = 0x0c,          //FCI無し
                        Data = aid
                    };
                    typeName = "Elemantary";
                    break;
            }
            apdu.Instruction = InstructionCode.SelectFile;
            SCardPCI recvPci = new SCardPCI();      //protocol control information
            byte[] res = new byte[2];
            err = reader.Transmit(
                SCardPCI.GetPci(reader.ActiveProtocol),
                apdu.ToArray(),
                recvPci,
                ref res);
            reader.EndTransaction(SCardReaderDisposition.Leave);

            if (err == SCardError.Success)
            {
                ResponseApdu resApdu = new ResponseApdu(res, IsoCase.Case2Short, reader.ActiveProtocol);
                MessageBox.Show(
                    "\nSW1 : " + resApdu.SW1.ToString("x2") +
                    "\nSW2 : " + resApdu.SW2.ToString("x2"),
                    "SELECT FILE : " + typeName);
            }
            else
            {
                MessageBox.Show("Fail Transmit");
            }
        }

        private void sendVerifyNum(ISCardReader reader, int ef)
        {
            SCardError err;

            if ((ef != 1) && (ef != 2))
            {
                MessageBox.Show("Invalid Args.", "err");
                return;
            }

            err = reader.BeginTransaction();
            if (err != SCardError.Success)
            {
                MessageBox.Show("Fail BeginTransaction.", "BEGIN TRANSACTION");
                return;
            }

            CommandApdu apdu = new CommandApdu(IsoCase.Case1, reader.ActiveProtocol)
            {
                CLA = 0x00,
                Instruction = InstructionCode.Verify,
                P1 = 0x00,
                P2 = (byte)(0x80 + ef)          //短縮EF識別子指定
            };

            SCardPCI recvPci = new SCardPCI();      //protocol control information
            byte[] res = new byte[2];
            err = reader.Transmit(
                SCardPCI.GetPci(reader.ActiveProtocol),
                apdu.ToArray(),
                recvPci,
                ref res);
            reader.EndTransaction(SCardReaderDisposition.Leave);

            if (err == SCardError.Success)
            {
                ResponseApdu resApdu = new ResponseApdu(res, IsoCase.Case2Short, reader.ActiveProtocol);
                MessageBox.Show(
                    "SW1 : " + resApdu.SW1.ToString("x2") +
                    "\nSW2 : " + resApdu.SW2.ToString("x2"),
                    "Verify(Num)");
            }
            else
            {
                MessageBox.Show("Fail Transmit");
            }
        }


        private void sendReadBinary(ISCardReader reader)
        {
            SCardError err;
            IsoCase iso_case = IsoCase.Case2Short;

            err = reader.BeginTransaction();
            if (err != SCardError.Success)
            {
                MessageBox.Show("Fail BeginTransaction.", "BEGIN TRANSACTION");
                return;
            }

            CommandApdu apdu = new CommandApdu(iso_case, reader.ActiveProtocol)
            {
                CLA = 0x00,
                Instruction = InstructionCode.ReadBinary,
                //P1 = 0x80,      //相対アドレス8bit指定 + カレントEF指定
                //P2 = 0x00
                P1 = 0x00,      //相対アドレス15bit指定
                P2 = 0x00
            };

            SCardPCI recvPci = new SCardPCI();      //protocol control information
            byte[] res = new byte[880];
            err = reader.Transmit(
                SCardPCI.GetPci(reader.ActiveProtocol),
                apdu.ToArray(),
                recvPci,
                ref res);
            reader.EndTransaction(SCardReaderDisposition.Leave);

            if (err == SCardError.Success)
            {
                ResponseApdu resApdu = new ResponseApdu(res, iso_case, reader.ActiveProtocol);
                string data = "(none)";
                if (resApdu.DataSize > 0)
                {
                    data = BitConverter.ToString(resApdu.GetData());
                }
                MessageBox.Show("result : " + data +
                    "\nSW1 : " + resApdu.SW1.ToString("x2") +
                    "\nSW2 : " + resApdu.SW2.ToString("x2"),
                    "ReadBinary");
            }
            else
            {
                MessageBox.Show("Fail Transmit");
            }
        }
    }
}
