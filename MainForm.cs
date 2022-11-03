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
using Opc;

namespace neuopc
{
    public partial class MainForm : Form
    {
        public OPCServer server;
        public MainForm()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //var server = new OPCServer();
            try
            {
                server.Connect(DAServerComboBox.Text, DAHostTextBox.Text);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"connect to opc server:{DAHostTextBox.Text} failed：{ex.Message}");
            }

            if (server.ServerState == (int)OPCServerState.OPCRunning)
            {
                System.Diagnostics.Debug.WriteLine("connected：{0}", server.ServerName);
            }
            else
            {
                System.Diagnostics.Debug.WriteLine("状态：{0}", server.ServerState.ToString());
            }

            // Browser
            var brower = server.CreateBrowser();
            brower.ShowBranches();
            brower.ShowLeafs(true);

            var groups = server.OPCGroups;
            groups.DefaultGroupIsActive = true;
            groups.DefaultGroupDeadband = 0;
            groups.DefaultGroupUpdateRate = 200;
            var group = groups.Add("all_data");

            group.IsActive = true;
            group.IsSubscribed = true;
            group.UpdateRate = 200;
            group.AsyncReadComplete += Group_AsyncReadComplete;

            int i = 0;
            foreach (var item in brower)
            {
                System.Diagnostics.Debug.WriteLine($"node：{item}");
                group.OPCItems.AddItem(item.ToString(), i++);
            }

            group.DataChange += new DIOPCGroupEvent_DataChangeEventHandler(GroupDataChange);
            group.AsyncWriteComplete += new DIOPCGroupEvent_AsyncWriteCompleteEventHandler(GroupAsyncWriteComplete);
            group.AsyncReadComplete += new DIOPCGroupEvent_AsyncReadCompleteEventHandler(GroupAsyncReadComplete);
            group.AsyncWriteComplete += new DIOPCGroupEvent_AsyncWriteCompleteEventHandler(GroupAsyncWriteComplete);


        }

        void GroupDataChange(int TransactionID, int NumItems, ref Array ClientHandles, ref Array ItemValues, ref Array Qualities, ref Array TimeStamps)
        {
            try
            {
                System.Diagnostics.Debug.WriteLine("====================DataChanged");
                for (int i = 1; i <= NumItems; i++)
                {
                    System.Diagnostics.Debug.WriteLine($"item {ClientHandles.GetValue(i)?.ToString()} value：{ItemValues.GetValue(i)?.ToString()}");
                    //Console.WriteLine("item句柄：{0}", ClientHandles.GetValue(i).ToString());
                    //Console.WriteLine("item质量：{0}", Qualities.GetValue(i).ToString());
                    //Console.WriteLine("item时间戳：{0}", TimeStamps.GetValue(i).ToString());
                    //Console.WriteLine("item类型：{0}", ItemValues.GetValue(i).GetType().FullName);
                }
            }
            catch (Exception ex) { 
                System.Diagnostics.Debug.WriteLine($"data changed handle error：{ex.Message}");
            }
        }

        void GroupAsyncWriteComplete(int TransactionID, int NumItems, ref Array ClientHandles, ref Array Errors)
        {
            System.Diagnostics.Debug.WriteLine("====================AsyncWriteComplete");
            /*for (int i = 1; i <= NumItems; i++)
            {
                Console.WriteLine("Tran：{0}   ClientHandles：{1}   Error：{2}", TransactionID.ToString(), ClientHandles.GetValue(i).ToString(), Errors.GetValue(i).ToString());
            }*/
        }

        void GroupAsyncReadComplete(int TransactionID, int NumItems, ref System.Array ClientHandles, ref System.Array ItemValues, ref System.Array Qualities, ref System.Array TimeStamps, ref System.Array Errors)
        {
            System.Diagnostics.Debug.WriteLine("====================GroupAsyncReadComplete");
            for (int i = 1; i <= NumItems; i++)
            {
                //Console.WriteLine("Tran：{0}   ClientHandles：{1}   Error：{2}", TransactionID.ToString(), ClientHandles.GetValue(i).ToString(), Errors.GetValue(i).ToString());
                //System.Diagnostics.Debug.WriteLine("Vaule：{0}", Convert.ToString(ItemValues.GetValue(i)));
            }
        }

        private void Group_AsyncReadComplete(int TransactionID, int NumItems, ref Array ClientHandles, ref Array ItemValues, ref Array Qualities, ref Array TimeStamps, ref Array Errors)
        {
            throw new NotImplementedException();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            DAHostTextBox.Text = "192.168.241.133";
            UAPortTextBox.Text = "48401";

            server = new OPCServer();
        }

        private void DAServerComboBox_DropDown(object sender, EventArgs e)
        {
            DAServerComboBox.Text = String.Empty;
            DAServerComboBox.Items.Clear();
            var server = new OPCServer();
            try
            {
                object servers = server.GetOPCServers(DAHostTextBox.Text);
                foreach (string s in (Array)servers)
                {
                    DAServerComboBox.Items.Add(s);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"get opc servers failed：{ex.Message}");
            }
        }
    }
}
