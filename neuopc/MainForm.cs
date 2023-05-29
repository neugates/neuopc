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

        //private void UpdateListView(List<Item> list)
        //{
        //    try
        //    {
        //        Action<List<Item>> action = (data) =>
        //        {
        //            foreach (var i in data)
        //            {
        //                int index = i.ClientHandle;
        //                var items = MainListView.Items;
        //                var item = items[index];
        //                var subItemValue = item.SubItems[3];
        //                var subItemQuality = item.SubItems[4];
        //                var subItemError = item.SubItems[5];
        //                var subItemTs = item.SubItems[6];

        //                subItemValue.Text = Convert.ToString(i.Value);
        //                subItemQuality.Text = i.Quality.ToString();
        //                subItemError.Text = i.Error.ToString();
        //                subItemTs.Text = Convert.ToString(i.Timestamp);
        //            }
        //        };

        //        Invoke(action, list);
        //    }
        //    catch (Exception exception)
        //    {
        //        Log.Error($"update list view error: {exception.Message}");
        //    }
        //}

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


        //private void ResetListView(List<Item> list)
        //{
        //    try
        //    {
        //        Action<List<Item>> action = (data) =>
        //        {
        //            MainListView.BeginUpdate();
        //            MainListView.Items.Clear();
        //            for (int i = 0; i < data.Count; i++)
        //            {
        //                ListViewItem lvi = new ListViewItem();
        //                lvi.Text = data[i].Name.ToString(); // handle
        //                lvi.SubItems.Add(data[i].Type.ToString()); // type
        //                lvi.SubItems.Add(data[i].Rights.ToString()); // rights
        //                lvi.SubItems.Add(""); // value
        //                lvi.SubItems.Add(""); // quality
        //                lvi.SubItems.Add(""); // error
        //                lvi.SubItems.Add(""); // timestamp
        //                lvi.SubItems.Add(data[i].ClientHandle.ToString()); // handle
        //                MainListView.Items.Add(lvi);
        //            }
        //            MainListView.EndUpdate();

        //        };

        //        Invoke(action, list);
        //    }
        //    catch (Exception exception)
        //    {
        //        Log.Error($"reset list view error: {exception.Message}");
        //    }
        //}

        private void ResetListView(List<DataItem> list)
        {
            try
            {
                Action<List<DataItem>> action = (data) =>
                {
                    MainListView.BeginUpdate();
                    MainListView.Items.Clear();
                    for (int i = 0; i < data.Count; i++)
                    {
                        ListViewItem lvi = new ListViewItem();
                        lvi.Text = data[i].Name;
                        lvi.SubItems.Add(data[i].Type); // type
                        lvi.SubItems.Add(data[i].Right); // rights
                        lvi.SubItems.Add(data[i].Value); // value
                        lvi.SubItems.Add(data[i].Quality); // quality
                        lvi.SubItems.Add(data[i].Error); // error
                        lvi.SubItems.Add(data[i].Timestamp); // timestamp
                        lvi.SubItems.Add(data[i].ClientHandle); // handle
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
                        //foreach (var item in requestMsg.Items)
                        //{
                        //    Log.Information($"name:{item.Name}, handle:{item.ClientHandle}, right:{item.Right}, value:{item.Value}, quality:{item.Quality}, error:{item.Error}, timestamp:{item.Timestamp}");


                        //}

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
                UAPortTextBox.Text = "opc.tcp//localhost:48401";
            }

            if (string.IsNullOrEmpty(UAUserTextBox.Text))
            {
                UAUserTextBox.Text = "admin";
            }

            if (string.IsNullOrEmpty(UAPasswordTextBox.Text))
            {
                UAPasswordTextBox.Text = "123456";
            }


            subProcess.Daemon();
            var ts = new ThreadStart(TestGetDatas);
            var thread = new Thread(ts);
            thread.Start();


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
        }

        private void DAServerComboBox_DropDown(object sender, EventArgs e)
        {
            DAServerComboBox.Text = string.Empty;
            DAServerComboBox.Items.Clear();

            var req = new DAServerReqMsg
            {
                Type = neulib.MsgType.DAServersReq,
                Host = DAHostComboBox.Text
            };
            var buff = Serializer.Serialize<DAServerReqMsg>(req);
            subProcess.Request(in buff, out byte[] result);
            if (null != result)
            {
                var res = Serializer.Deserialize<DAServerResMsg>(result);
                var list = res.Servers;
                DAServerComboBox.Items.AddRange(list.ToArray());
                if (0 < DAServerComboBox.Items.Count)
                {
                    DAServerComboBox.SelectedIndex = 0;
                }
            }
        }

        private void DAHostComboBox_DropDown(object sender, EventArgs e)
        {
            DAHostComboBox.Text = string.Empty;
            DAHostComboBox.Items.Clear();

            var req = new DAHostsReqMsg
            {
                Type = neulib.MsgType.DAHostsReq
            };
            var buff = Serializer.Serialize<DAHostsReqMsg>(req);
            subProcess.Request(in buff, out byte[] result);
            if (null != result)
            {
                var res = Serializer.Deserialize<DAHostsResMsg>(result);
                var list = res.Hosts;
                DAHostComboBox.Items.AddRange(list.ToArray());
                if (0 < DAHostComboBox.Items.Count)
                {
                    DAHostComboBox.SelectedIndex = 0;
                }
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

            running = false;
            subProcess.Stop();

            NotifyIcon.Dispose();
            Environment.Exit(0);
        }

        //private void RunButton_Click(object sender, EventArgs e)
        //{
        //    //    server.Write += client.Write;
        //    //    var items = new List<Item>();
        //    //    server.Start(UAPortTextBox.Text, UAUserTextBox.Text, UAPasswordTextBox.Text);

        //    //    RunButton.Enabled = false;
        //    //    UAPortTextBox.Enabled = false;
        //    //    UAUserTextBox.Enabled = false;
        //    //    UAPasswordTextBox.Enabled = false;

        //    var req = new UAStartReqMsg
        //    {
        //        Type = neulib.MsgType.UAStartReq,
        //        Port = UAPortTextBox.Text,
        //        User = UAUserTextBox.Text,
        //        Password = UAPasswordTextBox.Text
        //    };
        //    var buff = Serializer.Serialize<UAStartReqMsg>(req);
        //    subProcess.Request(in buff, out byte[] result);
        //    if (null != result)
        //    {
        //        var res = Serializer.Deserialize<UAStartResMsg>(result);
        //    }

        //    SwitchButton.Enabled = false;
        //    UAPortTextBox.Enabled = false;
        //    UAUserTextBox.Enabled = false;
        //    UAPasswordTextBox.Enabled = false;
        //}

        private void MainListView_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            //ListView listView = (ListView)sender;
            //ListViewItem row = listView.GetItemAt(e.X, e.Y);
            //ListViewItem.ListViewSubItem col = row.GetSubItemAt(e.X, e.Y);
            //string strText = col.Text;
            //try
            //{
            //    Clipboard.SetDataObject(strText);
            //    string info = $"The content [{strText}] has been copied to the clipboard";
            //    MessageBox.Show(info, "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
            //}
            //catch (System.Exception ex)
            //{
            //    MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            //}
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
            var req = new ConnectTestReqMsg
            {
                Type = neulib.MsgType.DAConnectTestReq,
                Host = DAHostComboBox.Text,
                Server = DAServerComboBox.Text
            };
            var buff = Serializer.Serialize<ConnectTestReqMsg>(req);
            subProcess.Request(in buff, out byte[] result);
            if (null != result)
            {
                var res = Serializer.Deserialize<ConnectTestResMsg>(result);
                if (res.Result)
                {
                    UALabel.Text = "Connection tested successfully";
                    UALabel.ForeColor = Color.Green;
                }
                else
                {
                    UALabel.Text = "Connection tested failed";
                    UALabel.ForeColor = Color.Red;
                }
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

                SwitchButton.Text = "Start";

                DAHostComboBox.Enabled = true;
                DAServerComboBox.Enabled = true;
                TestButton.Enabled = true;

                UAPortTextBox.Enabled = true;
                UAUserTextBox.Enabled = true;
                UAPasswordTextBox.Enabled = true;
            }
        }
    }
}
