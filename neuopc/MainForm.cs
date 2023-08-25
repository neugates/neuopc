using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Windows.Forms;
using System.Text.Json;
using System.Threading.Channels;
using System.IO;
using System.Diagnostics;
using Serilog;
using neulib;

namespace neuopc
{
    public partial class MainForm : Form
    {
        private Task task;
        private SubProcess subProcess;
        private bool running;

        public MainForm()
        {
            InitializeComponent();
            subProcess = new SubProcess();
            running = true;
        }

        //private void UpdateDAStatusLabel(DaMsg msg)
        //{
        //    try
        //    {
        //        Action<DaMsg> action = (data) =>
        //        {
        //            string label = $"UA:{data.Host}/{data.Server}-{data.Status}";
        //            DAStatusLabel.Text = label;
        //            if ("connected" == data.Status)
        //            {
        //                DAStatusLabel.ForeColor = Color.Green;
        //            }
        //            else
        //            {
        //                DAStatusLabel.ForeColor = Color.Red;
        //            }
        //        };

        //        Invoke(action, msg);
        //    }
        //    catch (Exception exception)
        //    {
        //        Log.Error($"update status lable error: {exception.Message}");
        //    }
        //}

        private void ResetListView(List<DataItem> list)
        {
            try
            {
                Action<List<DataItem>> action = (data) =>
                {
                    var items = MainListView.Items;
                    foreach (var item in data)
                    {
                        ListViewItem li = MainListView.Items.Cast<ListViewItem>().FirstOrDefault(x => x.Text == item.Name);
                        if (null == li)
                        {
                            MainListView.BeginUpdate();
                            ListViewItem lvi = new ListViewItem();
                            lvi.Text = item.Name;
                            lvi.SubItems.Add(item.Type); // type
                            lvi.SubItems.Add(item.Right); // rights
                            lvi.SubItems.Add(item.Value); // value
                            lvi.SubItems.Add(item.Quality); // quality
                            lvi.SubItems.Add(item.Error); // error
                            lvi.SubItems.Add(item.Timestamp); // timestamp
                            lvi.SubItems.Add(item.ClientHandle); // handle
                            MainListView.Items.Add(lvi);
                            MainListView.EndUpdate();
                        }
                        else
                        {
                            li.SubItems[3].Text = item.Value;
                            li.SubItems[4].Text = item.Quality;
                            li.SubItems[5].Text = item.Error;
                            li.SubItems[6].Text = item.Timestamp;
                        }
                    }
                };

                Invoke(action, list);
            }
            catch (Exception exception)
            {
                Log.Error($"reset list view error: {exception.Message}");
            }
        }

        private void TestGetDatas()
        {
            while (running)
            {
                Thread.Sleep(3000);

                var req = new DataReqMsg();
                req.Type = neulib.MsgType.DADataReq;
                var buff = Serializer.Serialize<DataReqMsg>(req);
                subProcess.Request(in buff, out byte[] result);

                if (null != result)
                {
                    try
                    {
                        var requestMsg = Serializer.Deserialize<DataResMsg>(result);
                        ResetListView(requestMsg.Items);
                    }
                    catch (Exception ex)
                    {
                        Log.Error($"get datas error:{ex.Message}");
                    }
                }
            }
        }


        private void MainForm_Load(object sender, EventArgs e)
        {
            NotifyIcon.Visible = true;
            var config = ConfigUtil.LoadConfig("neuopc.json");
            DAHostComboBox.Text = config.DAHost;
            DAServerComboBox.Text = config.DAServer;

            UAPortTextBox.Text = config.UAUrl;
            UAUserTextBox.Text = config.UAUser;
            UAPasswordTextBox.Text = config.UAPassword;
            CheckBox.Checked = config.AutoConnect;

            if (string.IsNullOrEmpty(UAPortTextBox.Text))
            {
                UAPortTextBox.Text = "opc.tcp://localhost:48401";
            }

            if (string.IsNullOrEmpty(UAUserTextBox.Text))
            {
                UAUserTextBox.Text = "admin";
            }

            if (string.IsNullOrEmpty(UAPasswordTextBox.Text))
            {
                UAPasswordTextBox.Text = "123456";
            }

            if (CheckBox.Checked)
            {
                subProcess.SetDAArguments(DAHostComboBox.Text, DAServerComboBox.Text);
                subProcess.SetUAArguments(UAPortTextBox.Text, UAUserTextBox.Text, UAPasswordTextBox.Text);

                SwitchButton.Text = "Stop";

                DAHostComboBox.Enabled = false;
                DAServerComboBox.Enabled = false;
                TestButton.Enabled = false;

                UAPortTextBox.Enabled = false;
                UAUserTextBox.Enabled = false;
                UAPasswordTextBox.Enabled = false;
            }
            else
            {
                subProcess.SetDAArguments("", "");
                subProcess.SetUAArguments("", "", "");

                SwitchButton.Text = "Start";
            }

            subProcess.Daemon();
            var ts = new ThreadStart(TestGetDatas);
            var thread = new Thread(ts);
            thread.Start();
        }

        private void DAServerComboBox_DropDown(object sender, EventArgs e)
        {
            DAServerComboBox.Text = string.Empty;
            DAServerComboBox.Items.Clear();
            var host = DAHostComboBox.Text;

            try
            {
                DAServerComboBox.Items.AddRange(neuclient.DaDiscovery.GetServers(host, 2).ToArray());
            }
            catch (Exception ex)
            {
                Log.Error($"GetServer, msg: ${ex.Message}");
            }
        }

        private void DAHostComboBox_DropDown(object sender, EventArgs e)
        {
            DAHostComboBox.Text = string.Empty;
            DAHostComboBox.Items.Clear();

            try
            {
                DAHostComboBox.Items.AddRange(neuclient.DaDiscovery.GetHosts().ToArray());
            }
            catch (Exception ex)
            {
                Log.Error($"GetHosts falied, msg ${ex.Message}");
            }

            if (0 < DAHostComboBox.Items.Count)
            {
                DAHostComboBox.SelectedIndex = 0;
            }
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            var result = MessageBox.Show("Do you want to exit the program?", "Warning", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning);
            if (DialogResult.Cancel == result)
            {
                e.Cancel = true;
                return;
            }

            //this.Hide();
            //e.Cancel = true;

            //client.Close();
            //channel.Writer.Complete();
            //task.Wait();
            //server.Stop();
            //NotifyIcon.Dispose();

            Log.Information("exit neuopc");

            running = false;
            subProcess.Stop();

            NotifyIcon.Dispose();
            Environment.Exit(0);
        }

        private void MainListView_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            ListView listView = (ListView)sender;
            ListViewItem row = listView.GetItemAt(e.X, e.Y);
            ListViewItem.ListViewSubItem col = row.GetSubItemAt(e.X, e.Y);
            string strText = col.Text;
            try
            {
                Clipboard.SetDataObject(strText);
            }
            catch (System.Exception ex)
            {
                Log.Error($"clipboard error:{ex.Message}");
            }
        }

        private void NotifyIcon_Click(object sender, EventArgs e)
        {
            this.Show();
            this.WindowState = FormWindowState.Normal;
            this.Activate();
        }

        private void ExitButton_Click(object sender, EventArgs e)
        {
            Log.Information("exit neuopc");

            var result = MessageBox.Show("Do you want to exit the program?", "Warning", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning);
            if (DialogResult.Cancel == result)
            {
                return;
            }


            running = false;
            subProcess.Stop();
            Environment.Exit(0);
        }

        private void TestButton_Click(object sender, EventArgs e)
        {
            //var req = new ConnectTestReqMsg
            //{
            //    Type = neulib.MsgType.DAConnectTestReq,
            //    Host = DAHostComboBox.Text,
            //    Server = DAServerComboBox.Text
            //};
            //var buff = Serializer.Serialize<ConnectTestReqMsg>(req);
            //subProcess.Request(in buff, out byte[] result);
            //if (null != result)
            //{
            //    var res = Serializer.Deserialize<ConnectTestResMsg>(result);
            //    if (res.Result)
            //    {
            //        UALabel.Text = "Connection tested successfully";
            //        UALabel.ForeColor = Color.Green;
            //    }
            //    else
            //    {
            //        UALabel.Text = "Connection tested failed";
            //        UALabel.ForeColor = Color.Red;
            //    }
            //}

            var str = DAServerComboBox.Text;
            var uri = new Uri(str);
            var user = DAUserTextBox.Text;
            var password = DAPasswordTextBox.Text;
            var domain = DADomainTextBox.Text;
            var client = new neuclient.DaClient(uri, user, password, domain);

            try
            {
                client.Connect();
            }
            catch (Exception ex)
            {
                Log.Error($"connect to server failed, msg:${ex.Message}");
            }

            try
            {
                var nodes = neuclient.DaBrowse.AllNode(client.Server);
            }
            catch (Exception ex)
            {
                Log.Error($"connect to server failed, msg:${ex.Message}");
            }
        }

        private void SwitchButton_Click(object sender, EventArgs e)
        {
            if (SwitchButton.Text.Equals("Start"))
            {
                // DA start
                var req1 = new ConnectReqMsg
                {
                    Type = neulib.MsgType.DAConnectReq,
                    Host = DAHostComboBox.Text,
                    Server = DAServerComboBox.Text
                };
                var buff1 = Serializer.Serialize<ConnectReqMsg>(req1);
                subProcess.Request(in buff1, out byte[] result1);
                if (null != result1)
                {
                    var res1 = Serializer.Deserialize<ConnectResMsg>(result1);
                    Log.Information($"connect to da, code:{res1.Code}, msg:{res1.Msg}");
                }

                // UA start
                var req2 = new UAStartReqMsg
                {
                    Type = neulib.MsgType.UAStartReq,
                    Url = UAPortTextBox.Text,
                    User = UAUserTextBox.Text,
                    Password = UAPasswordTextBox.Text
                };
                var buff2 = Serializer.Serialize<UAStartReqMsg>(req2);
                subProcess.Request(in buff2, out byte[] result2);
                if (null != result2)
                {
                    var res2 = Serializer.Deserialize<UAStartResMsg>(result2);
                }

                SwitchButton.Text = "Stop";

                DAHostComboBox.Enabled = false;
                DAServerComboBox.Enabled = false;
                TestButton.Enabled = false;

                UAPortTextBox.Enabled = false;
                UAUserTextBox.Enabled = false;
                UAPasswordTextBox.Enabled = false;
            }
            else
            {
                // DA start
                var req1 = new DisconnectReqMsg
                {
                    Type = neulib.MsgType.DADisconnectReq,
                };
                var buff1 = Serializer.Serialize<DisconnectReqMsg>(req1);
                subProcess.Request(in buff1, out byte[] result1);
                if (null != result1)
                {
                    var res1 = Serializer.Deserialize<DisconnectResMsg>(result1);
                }


                // UA start
                var req2 = new UAStopReqMsg
                {
                    Type = neulib.MsgType.UAStopReq
                };
                var buff2 = Serializer.Serialize<UAStopReqMsg>(req2);
                subProcess.Request(in buff2, out byte[] result2);
                if (null != result2)
                {
                    var res2 = Serializer.Deserialize<UAStopResMsg>(result2);
                }

                subProcess.SetDAArguments(DAHostComboBox.Text, DAServerComboBox.Text);
                subProcess.SetUAArguments(UAPortTextBox.Text, UAUserTextBox.Text, UAPasswordTextBox.Text);

                SwitchButton.Text = "Start";

                DAHostComboBox.Enabled = true;
                DAServerComboBox.Enabled = true;
                TestButton.Enabled = true;

                UAPortTextBox.Enabled = true;
                UAUserTextBox.Enabled = true;
                UAPasswordTextBox.Enabled = true;
            }
        }

        private void SaveButton_Click(object sender, EventArgs e)
        {
            var config = new Config
            {
                DAHost = DAHostComboBox.Text,
                DAServer = DAServerComboBox.Text,
                UAUrl = UAPortTextBox.Text,
                UAUser = UAUserTextBox.Text,
                UAPassword = UAPasswordTextBox.Text,
                AutoConnect = CheckBox.Checked
            };

            ConfigUtil.SaveConfig("neuopc.json", config);
        }

        private void TabControl_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (1 == TabControl.SelectedIndex)
            {
                try
                {
                    Action action = () =>
                    {
                        MainListView.BeginUpdate();
                        MainListView.Items.Clear();
                        MainListView.EndUpdate();
                    };

                    Invoke(action);
                }
                catch (Exception exception)
                {
                    Log.Error($"clear list view error: {exception.Message}");
                }
            }

            if (2 == TabControl.SelectedIndex)
            {
                try
                {
                    Action action = () =>
                    {
                        DirectoryInfo di = new DirectoryInfo("./log");
                        LogListView.BeginUpdate();
                        LogListView.Items.Clear();

                        foreach (var fi in di.GetFiles())
                        {
                            ListViewItem lvi = new ListViewItem
                            {
                                Text = fi.Name,
                            };

                            lvi.SubItems.Add(fi.LastWriteTime.ToString());
                            lvi.SubItems.Add(fi.Length.ToString());
                            LogListView.Items.Add(lvi);
                        }

                        LogListView.EndUpdate();
                    };

                    Invoke(action);
                }
                catch (Exception exception)
                {
                    Log.Error($"get logs error: {exception.Message}");
                }
            }
        }

        private void LogListView_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            ListView listView = (ListView)sender;
            ListViewItem row = listView.GetItemAt(e.X, e.Y);
            ListViewItem.ListViewSubItem col = row.GetSubItemAt(e.X, e.Y);
            string strText = col.Text;
            try
            {
                Process.Start("notepad.exe", $"./log/{strText}");
            }
            catch (System.Exception ex)
            {
                Log.Error($"clipboard error:{ex.Message}");
            }
        }
    }
}
