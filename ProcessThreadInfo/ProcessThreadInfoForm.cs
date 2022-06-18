using ProcessHacker;
using System;
using System.Diagnostics;
using System.Windows.Forms;

namespace ProcessThreadInfo
{
    public partial class ProcessThreadInfoForm : Form
    {
        private static ThreadProvider _threadP = null;
        public ProcessThreadInfoForm()
        {
            InitializeComponent();
            this.MouseDown += ProcessThreadInfo_MouseDown;
        }

        #region Form Event

        private void ProcessThreadInfo_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                NativeStuff.ReleaseCapture();
                NativeStuff.SendMessage(Handle, NativeStuff.WM_NCLBUTTONDOWN, NativeStuff.HT_CAPTION, 0);
            }
        }
        #endregion

        private void buttonWalk_Click(object sender, EventArgs e)
        {
            try
            {
                Process processEx = Process.GetProcessById(int.Parse(TextBox1.Text));

                listViewCallStack.Items.Clear();

                _threadP = new ThreadProvider(processEx.Id);
                _threadP.LoadKernelSymbols();
                _threadP.Run();

                _threadP.BeforeUpdate += _threadP_BeforeUpdate;
                _threadP.ResolveSybol += _threadP_ResolveSybol;
                _threadP.Error += _threadP_Error;
            }
            catch
            {
                MessageBox.Show("Invalid Process ID ?");
            }
        }

        #region Thread Events

        private void _threadP_Error(Exception ex) => MessageBox.Show(ex.ToString());

        private void _threadP_ResolveSybol(object sender, ThreadProvider.ResolveSybolData e)
        {
            if (listViewCallStack.InvokeRequired)
            {
                //MessageBox.Show("OK");
                listViewCallStack.Invoke((MethodInvoker)delegate ()
                {
                    ListViewItem itemN = new ListViewItem();
                    itemN.Text = e.TID.ToString();
                    itemN.SubItems.Add(e.ResolveInfo.Symbol);
                    listViewCallStack.Items.Add(itemN);
                    //listViewCallStack.EnsureVisible(listViewCallStack.Items.Count - 1);
                });
            }
        }

        private void _threadP_BeforeUpdate() => listViewCallStack.Items.Clear();
        #endregion
    }
}