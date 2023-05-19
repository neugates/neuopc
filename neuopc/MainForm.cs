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
using System.Threading.Channels;
using Serilog;

namespace neuopc
{
    public partial class MainForm : Form
    {
        private DaClient client;
        private UAServer server;
        private Channel<DaMsg> channel;
        private Task task;

        public MainForm(DaClient client, UAServer server) : this()
        {
            this.client = client;
            this.server = server;
            channel = Channel.CreateUnbounded<DaMsg>();
        }

        public MainForm()
        {
            InitializeComponent();
        }

        private void UpdateListView(List<Item> list)
        {
            try
            {
                Action<List<Item>> action = (data) =>
                {
                    foreach (var i in data)
                    {
                        int index = i.ClientHandle;
                        var items = MainListView.Items;
                        var item = items[index];
                        var subItemValue = item.SubItems[3];
                        var subItemQuality = item.SubItems[4];
                        var subItemError = item.SubItems[5];
                        var subItemTs = item.SubItems[6];

                        subItemValue.Text = Convert.ToString(i.Value);
                        subItemQuality.Text = i.Quality.ToString();
                        subItemError.Text = i.Error.ToString();
                        subItemTs.Text = Convert.ToString(i.Timestamp);
                    }
                };

                Invoke(action, list);
            }
            catch (Exception exception)
            {
                Log.Error($"update list view error: {exception.Message}");
            }
        }

        private void UpdateDAStatusLabel(DaMsg msg)
        {
            try
            {
                Action<DaMsg> action = (data) =>
                {
                    string label = $"UA:{data.Host}/{data.Server}-{data.Status}";
                    DAStatusLabel.Text = label;
                    if ("connected" == data.Status)
                    {
                        DAStatusLabel.ForeColor = Color.Green;
                    }
                    else
                    {
                        DAStatusLabel.ForeColor = Color.Red;
                    }
                };

                Invoke(action, msg);
            }
            catch (Exception exception)
            {
                Log.Error($"update status lable error: {exception.Message}");
            }
        }


        private void ResetListView(List<Item> list)
        {
            try
            {
                Action<List<Item>> action = (data) =>
                {
                    MainListView.BeginUpdate();
                    MainListView.Items.Clear();
                    for (int i = 0; i < data.Count; i++)
                    {
                        ListViewItem lvi = new ListViewItem();
                        lvi.Text = data[i].Name.ToString(); // handle
                        lvi.SubItems.Add(data[i].Type.ToString()); // type
                        lvi.SubItems.Add(data[i].Rights.ToString()); // rights
                        lvi.SubItems.Add(""); // value
                        lvi.SubItems.Add(""); // quality
                        lvi.SubItems.Add(""); // error
                        lvi.SubItems.Add(""); // timestamp
                        lvi.SubItems.Add(data[i].ClientHandle.ToString()); // handle
                        MainListView.Items.Add(lvi);
                    }
                    MainListView.EndUpdate();

                };

                Invoke(action, list);
            }
            catch (Exception exception)
            {
                Log.Error($"reset list view error: {exception.Message}");
            }
        }

        private void ReadButton_Click(object sender, EventArgs e)
        {
            client.Close();
            MainListView.Items.Clear();
            client.Open(DAHostComboBox.Text, DAServerComboBox.Text);
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            UAPortTextBox.Text = "48401";
            UAUserTextBox.Text = "admin";
            UAPasswordTextBox.Text = "123456";
            NotifyIcon.Visible = true;

            client.AddSlowChannel(channel);
            client.AddFastChannel(server.channel);
            task = new Task(async () =>
            {
                while (await channel.Reader.WaitToReadAsync())
                {
                    if (channel.Reader.TryRead(out var msg))
                    {
                        if (MsgType.List == msg.Type)
                        {
                            ResetListView(msg.Items);
                        }
                        else if (MsgType.Data == msg.Type)
                        {
                            UpdateListView(msg.Items);
                            UpdateDAStatusLabel(msg);
                        }
                    }
                }
            });
            task.Start();
        }

        private void DAServerComboBox_DropDown(object sender, EventArgs e)
        {
            DAServerComboBox.Text = string.Empty;
            DAServerComboBox.Items.Clear();
            var list = DaClient.GetServers(DAHostComboBox.Text);
            DAServerComboBox.Items.AddRange(list.ToArray());
            if (0 < DAServerComboBox.Items.Count)
            {
                DAServerComboBox.SelectedIndex = 0;
            }
        }

        private void DAHostComboBox_DropDown(object sender, EventArgs e)
        {
            DAHostComboBox.Text = string.Empty;
            DAHostComboBox.Items.Clear();
            var list = DaClient.GetHosts();
            DAHostComboBox.Items.AddRange(list.ToArray());
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

            client.Close();
            channel.Writer.Complete();
            task.Wait();
            server.Stop();
            NotifyIcon.Dispose();
        }

        private void RunButton_Click(object sender, EventArgs e)
        {
            server.Write += client.Write;
            var items = new List<Item>();
            server.Start(UAPortTextBox.Text, UAUserTextBox.Text, UAPasswordTextBox.Text);

            RunButton.Enabled = false;
            UAPortTextBox.Enabled = false;
            UAUserTextBox.Enabled = false;
            UAPasswordTextBox.Enabled = false;
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
                string info = $"The content [{strText}] has been copied to the clipboard";
                MessageBox.Show(info, "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (System.Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
