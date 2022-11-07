using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using OPCAutomation;

namespace neuopc
{
    public partial class MainForm : Form
    {
        private DAClient client = null;

        public MainForm(DAClient client) : this()
        {
            this.client = client;
        }

        public OPCServer server;

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
                    var subItemValue = item.SubItems[2];
                    var subItemRights = item.SubItems[3];
                    var subItemQualitie = item.SubItems[4];
                    var subItemError = item.SubItems[5];
                    var subItemTs = item.SubItems[6];

                    subItemValue.Text = Convert.ToString(data.Value);
                    subItemRights.Text = data.Rights.ToString();
                    subItemQualitie.Text = data.Quality.ToString();
                    subItemTs.Text = data.Timestamp;
                    subItemError.Text = data.Error.ToString();
                };

                Invoke(action, i);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            client.Connect(DAHostComboBox.Text, DAServerComboBox.Text);
            var list = client.BuildGroup();
            MainListView.BeginUpdate();
            MainListView.Items.Clear();
            for (int i = 0; i < list.Count; i++)
            {
                ListViewItem lvi = new ListViewItem();
                lvi.Text = list[i].ClientHandle.ToString();
                lvi.SubItems.Add(list[i].Name.ToString());
                lvi.SubItems.Add(Convert.ToString(list[i].Value));
                lvi.SubItems.Add("");
                lvi.SubItems.Add("");
                lvi.SubItems.Add("");
                lvi.SubItems.Add("");
                MainListView.Items.Add(lvi);
            }
            MainListView.EndUpdate();

            client.update += UpdateListView;
            client.Read();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            UAPortTextBox.Text = "48401";
            server = new OPCServer();
        }

        private void DAServerComboBox_DropDown(object sender, EventArgs e)
        {
            DAServerComboBox.Text = string.Empty;
            DAServerComboBox.Items.Clear();
            var list = client.GetServers(DAHostComboBox.Text);
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
            var list = client.GetHosts();
            DAHostComboBox.Items.AddRange(list.ToArray());
            if (0 < DAHostComboBox.Items.Count)
            {
                DAHostComboBox.SelectedIndex = 0;
            }
        }
    }
}
