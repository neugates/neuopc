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

namespace neuopc
{
    public partial class MainForm : Form
    {
        private DaClient client;
        private UAServer server;
        private List<Item> items;
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

        public void UpdateListView(List<Item> list)
        {
            foreach (var i in list)
            {
                Action<Item> action = (data) =>
                {
                    int index = data.ClientHandle;
                    var items = MainListView.Items;
                    var item = items[index];
                    var subItemValue = item.SubItems[4];
                    var subItemQuality = item.SubItems[5];
                    var subItemError = item.SubItems[6];
                    var subItemTs = item.SubItems[7];

                    subItemValue.Text = Convert.ToString(data.Value);
                    subItemQuality.Text = data.Quality.ToString();
                    subItemError.Text = data.Error.ToString();
                    subItemTs.Text = Convert.ToString(data.Timestamp);
                };

                try
                {
                    Invoke(action, i);
                }
                catch
                {
                }
            }
        }

        public void ResetListView(List<Item> list)
        {
            Action<List<Item>> action = (data) =>
            {
                MainListView.BeginUpdate();
                MainListView.Items.Clear();
                for (int i = 0; i < data.Count; i++)
                {
                    ListViewItem lvi = new ListViewItem();
                    lvi.Text = data[i].ClientHandle.ToString(); // handle
                    lvi.SubItems.Add(data[i].Name.ToString()); // name
                    lvi.SubItems.Add(data[i].Type.ToString()); // type
                    lvi.SubItems.Add(data[i].Rights.ToString()); // rights
                    lvi.SubItems.Add(""); // value
                    lvi.SubItems.Add(""); // quality
                    lvi.SubItems.Add(""); // error
                    lvi.SubItems.Add(""); // timestamp
                    MainListView.Items.Add(lvi);
                }
                MainListView.EndUpdate();

            };


            try
            {
                Invoke(action, list);
            }
            catch
            {
            }
        }

        private void ReadButton_Click(object sender, EventArgs e)
        {
            client.Close();
            client.Open(DAHostComboBox.Text, DAServerComboBox.Text);
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            UAPortTextBox.Text = "48401";

            client.AddSlowChannel(channel);
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
            client.Close();
            channel.Writer.Complete();
            task.Wait();
            server.Stop();
            NotifyIcon.Dispose();
        }

        private void RunButton_Click(object sender, EventArgs e)
        {
            server.Write += client.Write;
            server.Start(UAPortTextBox.Text, items);
        }
    }
}
