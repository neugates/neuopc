using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;
using Serilog;
using neuclient;

namespace neuopc
{
    public partial class MainForm : Form
    {
        private bool _running;
        private const int _MaxLogLines = 5000;

        public MainForm()
        {
            InitializeComponent();

            _running = true;
            LogTaskRun();
            UAMessageMode.DrawItem += (sender, e) =>
            {
                if (e.Index < 0 || e.Index >= UAMessageMode.Items.Count)
                {
                    return;
                }
                e.DrawBackground();
                var text = UAMessageMode.Items[e.Index].ToString();
                var isDisable = false;
                if (UASecurityPolicy.Text == "None")
                {
                    if (text != "None")
                    {
                        isDisable = true;
                    }
                }
                else
                {
                    if (text == "None")
                    {
                        isDisable = true;
                    }
                }
                e.Graphics.DrawString(
                    text,
                    e.Font,
                    isDisable ? Brushes.Gray : Brushes.Black,
                    e.Bounds
                );

                e.DrawFocusRectangle();
            };
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

        private void LoadMetaInfo()
        {
            AboutRichTextBox.Clear();
            AboutRichTextBox.AppendText($"{MetaInfo.Name} v{MetaInfo.Version}\r\n");
            AboutRichTextBox.AppendText("\r\n");
            AboutRichTextBox.AppendText(MetaInfo.Description);
            AboutRichTextBox.AppendText($"Document {MetaInfo.Documenation}\r\n");
            AboutRichTextBox.AppendText($"License {MetaInfo.License}\r\n");
            AboutRichTextBox.AppendText($"NeuOPC project {MetaInfo.NeuopcProject}\r\n");
            AboutRichTextBox.AppendText($"Neuron project {MetaInfo.NeuronProject}\r\n");
            AboutRichTextBox.AppendText("\r\n");
            AboutRichTextBox.AppendText("\r\n");
            AboutRichTextBox.AppendText($"OPC foundation {MetaInfo.OpcdaProject}\r\n");
            AboutRichTextBox.AppendText($"OPC UA project {MetaInfo.OpcuaProject}\r\n");
            AboutRichTextBox.AppendText($"Serilog project {MetaInfo.SerilogProject}\r\n");
            AboutRichTextBox.AppendText("\r\n");
            AboutRichTextBox.AppendText("\r\n");
            AboutRichTextBox.AppendText(MetaInfo.Disclaimer);
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            Log.Information("neuopc start");

            LoadMetaInfo();

            NotifyIcon.Visible = true;
            var config = ConfigUtil.LoadConfig("neuopc.json");
            DAHostComboBox.Text = config.DAHost;
            DAServerComboBox.Text = config.DAServer;

            UAUrlTextBox.Text = config.UAUrl;
            UAUserTextBox.Text = config.UAUser;
            UAPasswordTextBox.Text = config.UAPassword;
            CheckBox.Checked = config.AutoConnect;
            UAIsAllowAnonymous.Checked = config.AllowAnonymous;
            UASecurityPolicy.Text = config.SecurityPolicy;
            UAMessageMode.Text = config.MessageMode;

            if (string.IsNullOrEmpty(UAUrlTextBox.Text))
            {
                UAUrlTextBox.Text = "opc.tcp://localhost:48401";
            }

            if (string.IsNullOrEmpty(UAUserTextBox.Text))
            {
                UAUserTextBox.Text = "admin";
            }

            if (string.IsNullOrEmpty(UAPasswordTextBox.Text))
            {
                UAPasswordTextBox.Text = "123456";
            }

            SwitchButton.Text = "Start";

            if (CheckBox.Checked)
            {
                SwitchButton.PerformClick();
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
            var result = MessageBox.Show(
                "Do you want to exit the program?",
                "Warning",
                MessageBoxButtons.OKCancel,
                MessageBoxIcon.Warning
            );
            if (DialogResult.Cancel == result)
            {
                e.Cancel = true;
                return;
            }

            Log.Information("exit neuopc");
            _running = false;
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

            var result = MessageBox.Show(
                "Do you want to exit the program?",
                "Warning",
                MessageBoxButtons.OKCancel,
                MessageBoxIcon.Warning
            );
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
                client.Disconnect();
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
        }

        private void SwitchButton_Click(object sender, EventArgs e)
        {
            try
            {
                SwitchButton.Enabled = false;

                if (SwitchButton.Text.Equals("Start"))
                {
                    var url = UAUrlTextBox.Text;
                    var user = UAUserTextBox.Text;
                    var password = UAPasswordTextBox.Text;
                    Server.Start(url, user, password, Client.WriteTag, new neuserver.ServerConfig
                    {
                        IsAllowAnonymous = UAIsAllowAnonymous.Checked,
                        SecurityMode = UAMessageMode.Text,
                        SecurityPolicy = UASecurityPolicy.Text
                    });

                    var uri = DAServerComboBox.Text;
                    Client.Start(uri, Server.DataChannel);

                    SwitchButton.Text = "Stop";
                    DAHostComboBox.Enabled = false;
                    DAServerComboBox.Enabled = false;
                    TestButton.Enabled = false;
                    UAUrlTextBox.Enabled = false;
                    UAUserTextBox.Enabled = false;
                    UAPasswordTextBox.Enabled = false;
                    UAIsAllowAnonymous.Enabled = false;
                    UASecurityPolicy.Enabled = false;
                    UAMessageMode.Enabled = false;

                    Log.Information($"da server {uri} started");
                }
                else
                {
                    Client.Stop();
                    Server.Stop();

                    SwitchButton.Text = "Start";
                    DAHostComboBox.Enabled = true;
                    DAServerComboBox.Enabled = true;
                    TestButton.Enabled = true;
                    UAUrlTextBox.Enabled = true;
                    UAUserTextBox.Enabled = true;
                    UAPasswordTextBox.Enabled = true;
                    UAIsAllowAnonymous.Enabled = true;
                    UASecurityPolicy.Enabled = true;
                    UAMessageMode.Enabled = true;

                    var uri = DAServerComboBox.Text;
                    Log.Information($"da server {uri} server stopped");
                }

                SwitchButton.Enabled = true;
            }
            catch (Exception ex)
            {
                Log.Error(ex, "switch server error");
                SwitchButton.Enabled = true;
                MessageBox.Show(
                    "Switch server failed, please check the log for details.",
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
        }

        private void SaveButton_Click(object sender, EventArgs e)
        {
            var config = new Config
            {
                DAHost = DAHostComboBox.Text,
                DAServer = DAServerComboBox.Text,
                UAUrl = UAUrlTextBox.Text,
                UAUser = UAUserTextBox.Text,
                UAPassword = UAPasswordTextBox.Text,
                AutoConnect = CheckBox.Checked,
                AllowAnonymous = UAIsAllowAnonymous.Checked,
                SecurityPolicy = UASecurityPolicy.Text,
                MessageMode = UAMessageMode.Text,
            };

            ConfigUtil.SaveConfig("neuopc.json", config);
        }

        private void ResetListView(IEnumerable<NodeInfo> nodes)
        {
            Action<IEnumerable<NodeInfo>> action = (data) =>
            {
                var items = MainListView.Items;
                foreach (var info in data)
                {
                    MainListView.BeginUpdate();
                    ListViewItem lvi = new();

                    var itemType = "unknow";
                    if (null != info.Node.Type)
                    {
                        itemType = info.Node.Type.ToString();
                    }

                    var itemValue = "null";
                    if (null != info.Node.Item && null != info.Node.Item.Value)
                    {
                        itemValue = info.Node.Item.Value.ToString();
                    }

                    var itemQuality = "unknow";
                    if (null != info.Node.Item)
                    {
                        itemQuality = info.Node.Item.Quality.ToString();
                    }

                    var itemSourceTimestamp = "unknow";
                    if (null != info.Node.Item)
                    {
                        itemSourceTimestamp = info.Node.Item.SourceTimestamp.ToString();
                    }

                    lvi.Text = info.Node.ItemName;
                    lvi.SubItems.Add(itemType); // type
                    lvi.SubItems.Add(""); // rights
                    lvi.SubItems.Add(itemValue); // value
                    lvi.SubItems.Add(itemQuality); // quality
                    lvi.SubItems.Add(""); // error
                    lvi.SubItems.Add(itemSourceTimestamp); // timestamp
                    lvi.SubItems.Add(""); // handle
                    MainListView.Items.Add(lvi);
                    MainListView.EndUpdate();
                }
            };

            try
            {
                Invoke(action, nodes);
            }
            catch (Exception exception)
            {
                Log.Error(exception, $"reset list view error");
            }
        }

        private void TabControl_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (1 == TabControl.SelectedIndex)
            {
                Action action = () =>
                {
                    MainListView.BeginUpdate();
                    MainListView.Items.Clear();
                    MainListView.EndUpdate();
                };

                try
                {
                    Invoke(action);
                }
                catch (Exception exception)
                {
                    Log.Error($"clear list view error: {exception.Message}");
                }

                var nodes = Client.GetNodes();
                if (nodes != null)
                {
                    ResetListView(nodes);
                }
            }

            if (2 == TabControl.SelectedIndex) { }
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

        private void addTagToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TagForm tagForm = new();
            tagForm.StartPosition = FormStartPosition.CenterParent;
            tagForm.Text = "Add Tag";
            tagForm.ShowDialog();
        }

        private void MainListView_SelectedIndexChanged(object sender, EventArgs e) { }

        private void UASecurityPolicy_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (UASecurityPolicy.Text == "None")
            {
                UAMessageMode.Text = "None";
            }
            else
            {
                UAMessageMode.Text = "Sign & Encrypt";
            }
        }

        private void UAMessageMode_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (UASecurityPolicy.Text == "None")
            {
                if (UAMessageMode.Text != "None")
                {
                    UAMessageMode.Text = "None";
                }
            }
            else
            {
                if (UAMessageMode.Text == "None")
                {
                    UAMessageMode.Text = "Sign & Encrypt";
                }
            }
        }
    }
}
