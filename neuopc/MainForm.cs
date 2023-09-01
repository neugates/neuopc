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
using Accessibility;

namespace neuopc
{
    public partial class MainForm : Form
    {
        private bool _running;
        private const int _MaxLogLines = 1000;

        private DaClient _client;
        private bool _clientRunning;
        private Thread _clientThread;

        private readonly Dictionary<string, Node> _nodeMap;

        public MainForm()
        {
            InitializeComponent();

            _running = true;
            _nodeMap = new Dictionary<string, Node>();

            LogTaskRun();
        }

        private void LogTaskRun()
        {
            var _ = Task.Run(async () =>
             {
                 var channel = NeuSinkChannel.GetChannel();
                 Action<string> action = (data) =>
                 {
                     if (LogRichTextBox.Lines.Length > _MaxLogLines)
                     {
                         LogRichTextBox.Clear();
                     }

                     LogRichTextBox.AppendText(data);
                     LogRichTextBox.ScrollToCaret();
                 };

                 while (await channel.Reader.WaitToReadAsync())
                 {
                     if (!_running)
                     {
                         break;
                     }

                     if (!channel.Reader.TryRead(out var msg))
                     {
                         continue;
                     }

                     try
                     {
                         Invoke(action, msg);
                     }
                     catch (Exception)
                     {
                         continue;
                     }
                 }
             });
        }


        private void MainForm_Load(object sender, EventArgs e)
        {
            Log.Information("neuopc start");

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
                DAServerComboBox.Items.AddRange(DaDiscovery.GetServers(host, 2).ToArray());
            }
            catch (Exception ex)
            {
                Log.Error(ex, $"get da servers error, host:{host}");
                return;
            }

            if (0 < DAServerComboBox.Items.Count)
            {
                DAServerComboBox.SelectedIndex = 0;
            }
        }

        private void DAHostComboBox_DropDown(object sender, EventArgs e)
        {
            DAHostComboBox.Text = string.Empty;
            DAHostComboBox.Items.Clear();

            try
            {
                DAHostComboBox.Items.AddRange(DaDiscovery.GetHosts().ToArray());
            }
            catch (Exception ex)
            {
                Log.Error(ex, "get da hosts error");
                return;
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

            Log.Information("exit neuopc");
            _running = false;
            DaClientStop();
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
            Log.Information("neuopc exit");

            var result = MessageBox.Show("Do you want to exit the program?", "Warning", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning);
            if (DialogResult.Cancel == result)
            {
                return;
            }


            Environment.Exit(0);
        }

        private void TestButton_Click(object sender, EventArgs e)
        {
            DALabel.Text = string.Empty;
            var uri = DAServerComboBox.Text;
            var user = string.Empty;
            var password = string.Empty;
            var domain = string.Empty;

            DaClient client;
            try
            {
                client = new DaClient(uri, user, password, domain);
                client.Connect();
                //client.Disconnect();
            }
            catch (Exception ex)
            {
                Log.Error(ex, "connect to server failed");

                DALabel.Text = "Connection tested failed";
                DALabel.ForeColor = Color.Red;
                return;
            }

            DALabel.Text = "Connection tested successfully";
            DALabel.ForeColor = Color.Green;

            try
            {
                var nodes = DaBrowse.AllNode(client.Server);

                foreach (var node in nodes)
                {
                    var item = client.Read(node.Name);
                    Log.Information($"name:{node.ItemName}, value:{item.Value}");
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, $"read server failed");
            }
        }

        private void DaClientThread()
        {
            while (_clientRunning)
            {
                try
                {
                    _client.Connect();
                }
                catch (Exception ex)
                {
                    Log.Error(ex, "connect to server failed");
                    Thread.Sleep(1000);
                    continue;
                }

                Thread.Sleep(1000);

                var nodes = DaBrowse.AllNode(_client.Server);
                foreach (var node in nodes)
                {
                    if (!_nodeMap.ContainsKey(node.Name))
                    {
                        _nodeMap.Add(node.Name, node);
                        Log.Information($"add item:{node.Name}");
                    }
                }

                foreach (var pair in _nodeMap)
                {
                    var name = pair.Key;
                    var readItem = _client.Read("_System._ActiveTagCount");
                    Log.Information($"read item:{name}");
                }
            }
        }

        private void DaClientStart()
        {
            var uri = DAServerComboBox.Text;
            var user = string.Empty;
            var password = string.Empty;
            var domain = string.Empty;
            _client = new DaClient(uri, user, password, domain);
            _clientThread = new Thread(new ThreadStart(DaClientThread));
            _clientRunning = true;
            _clientThread.Start();
        }

        private void DaClientStop()
        {
            if (_clientRunning)
            {
                _clientRunning = false;
                _clientThread.Join();
            }
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
