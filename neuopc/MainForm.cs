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
using neuclient;

namespace neuopc
{
    public partial class MainForm : Form
    {
        private readonly Task _task;
        private DaClient _client;
        private bool _clientRunning;
        private Thread _clientThread;

        public MainForm()
        {
            InitializeComponent();
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

        //private void ResetListView(List<DataItem> list)
        //{
        //    try
        //    {
        //        Action<List<DataItem>> action = (data) =>
        //        {
        //            var items = MainListView.Items;
        //            foreach (var item in data)
        //            {
        //                ListViewItem li = MainListView.Items.Cast<ListViewItem>().FirstOrDefault(x => x.Text == item.Name);
        //                if (null == li)
        //                {
        //                    MainListView.BeginUpdate();
        //                    ListViewItem lvi = new ListViewItem();
        //                    lvi.Text = item.Name;
        //                    lvi.SubItems.Add(item.Type); // type
        //                    lvi.SubItems.Add(item.Right); // rights
        //                    lvi.SubItems.Add(item.Value); // value
        //                    lvi.SubItems.Add(item.Quality); // quality
        //                    lvi.SubItems.Add(item.Error); // error
        //                    lvi.SubItems.Add(item.Timestamp); // timestamp
        //                    lvi.SubItems.Add(item.ClientHandle); // handle
        //                    MainListView.Items.Add(lvi);
        //                    MainListView.EndUpdate();
        //                }
        //                else
        //                {
        //                    li.SubItems[3].Text = item.Value;
        //                    li.SubItems[4].Text = item.Quality;
        //                    li.SubItems[5].Text = item.Error;
        //                    li.SubItems[6].Text = item.Timestamp;
        //                }
        //            }
        //        };

        //        Invoke(action, list);
        //    }
        //    catch (Exception exception)
        //    {
        //        Log.Error($"reset list view error: {exception.Message}");
        //    }
        //}


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
                SwitchButton.Text = "Start";
            }
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


            Environment.Exit(0);
        }

        private void TestButton_Click(object sender, EventArgs e)
        {
            var str = DAServerComboBox.Text;
            Uri uri;
            try
            {
                uri = new Uri(str);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "");
                // TODO: label tips
                return;
            }

            var user = DAUserTextBox.Text;
            var password = DAPasswordTextBox.Text;
            var domain = DADomainTextBox.Text;

            try
            {
                var client = new DaClient(uri, user, password, domain);
                client.Connect();
                client.Disconnect();
            }
            catch (Exception ex)
            {
                Log.Error($"connect to server failed, msg:${ex.Message}");
                // TODO: label tips
                return;
            }

            // TODO: label tips

            //try
            //{
            //    var nodes = neuclient.DaBrowse.AllNode(client.Server);
            //}
            //catch (Exception ex)
            //{
            //    Log.Error($"connect to server failed, msg:${ex.Message}");
            //}
        }

        private void DaClientThread()
        {
            _client.Connect();
            while (_clientRunning)
            {
            }
        }

        private void DaClientStart()
        {
            var urlStr = DAServerComboBox.Text;
            var uri = new Uri(urlStr);
            var user = DAUserTextBox.Text;
            var password = DAPasswordTextBox.Text;
            var domain = DADomainTextBox.Text;
            _client = new DaClient(uri, user, password, domain);
            _clientThread = new Thread(new ThreadStart(DaClientThread));
            _clientRunning = true;
            _clientThread.Start();
        }

        private void DaClientStop()
        {
            _clientRunning = false;
            _clientThread.Join();
        }

        private void SwitchButton_Click(object sender, EventArgs e)
        {
            if (SwitchButton.Text.Equals("Start"))
            {
                DaClientStart();
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
                DaClientStop();
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
