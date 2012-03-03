using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

using MBUtilities;
using MBUtilities.Channel;
using MBFunctionInterface;

namespace MoronBot
{
    public partial class formMoronBot : Form
    {
        MoronBot moronBot;

        BindingSource _bindingSourceChannels = new BindingSource();
        BindingSource _bindingSourceUsers = new BindingSource();

        public formMoronBot()
        {
            InitializeComponent();
        }

        void formMoronBot_Load(object sender, EventArgs e)
        {
            txtIRC.Text = "";
            txtProgLog.Text = "";
            txtInput.Text = "";

            moronBot = Program.moronBot;

            _bindingSourceChannels.DataSource = ChannelList.Channels;
            listChannels.DisplayMember = "Name";
            listChannels.DataSource = _bindingSourceChannels;

            listUsers.DisplayMember = "Nick";
            listUsers.DataSource = _bindingSourceUsers;

            FuncInterface.NickChanged += moronBot_NickChanged;
            CwIRC.Interface.NewRawIRC += moronBot_NewRawIRC;
            MBUtilities.Events.NewFormattedIRC += moronBot_NewFormattedIRC;

            MBUtilities.Events.ChannelListModified += channelList_Modified;
            MBUtilities.Events.UserListModified += userList_Modified;
        }

        void btnClear_Click(object sender, EventArgs e)
        {
            txtIRC.Text = "";
            txtProgLog.Text = "";
        }

        void btnExit_Click(object sender, EventArgs e)
        {
            Close();
        }

        void moronBot_NickChanged(object sender, string nick)
        {
            if (InvokeRequired)
            {
                this.BeginInvoke(new Action<object, string>(moronBot_NickChanged), new object[] { sender, nick });
                return;
            }
            Text = nick;
        }

        void moronBot_NewRawIRC(object sender, string text)
        {
            if (InvokeRequired)
            {
                this.BeginInvoke(new Action<object, string>(moronBot_NewRawIRC), new object[] { sender, text });
                return;
            }
            txtProgLog.Text += text + "\r\n";
        }

        void moronBot_NewFormattedIRC(object sender, string text)
        {
            if (InvokeRequired)
            {
                this.BeginInvoke(new Action<object, string>(moronBot_NewFormattedIRC), new object[] { sender, text });
                return;
            }
            txtIRC.Text += text + "\r\n";
        }

        void channelList_Modified(object sender)
        {
            RefreshChannels();
        }

        void userList_Modified(object sender)
        {
            RefreshChannels();
        }

        void txtProgLog_TextChanged(object sender, EventArgs e)
        {
            txtProgLog.SelectionStart = txtProgLog.Text.Length;
            txtProgLog.ScrollToCaret();
        }

        void txtIRC_TextChanged(object sender, EventArgs e)
        {
            txtIRC.SelectionStart = txtIRC.Text.Length;
            txtIRC.ScrollToCaret();
        }

        void txtInput_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                moronBot.Say(txtInput.Text, ChannelList.Channels[listChannels.SelectedIndex].Name);
                txtInput.Text = "";
            }
        }

        void RefreshChannels()
        {
            if (InvokeRequired)
            {
                this.BeginInvoke(new Action(RefreshChannels));
                return;
            }
            _bindingSourceChannels.DataSource = null;
            _bindingSourceChannels.DataSource = ChannelList.Channels;
            RefreshUsers();
        }

        void RefreshUsers()
        {
            if (InvokeRequired)
            {
                this.BeginInvoke(new Action(RefreshUsers));
                return;
            }
            _bindingSourceUsers.DataSource = null;
            if (listChannels.SelectedIndex == -1)
            {
                return;
            }
            _bindingSourceUsers.DataSource = ChannelList.Channels[listChannels.SelectedIndex].Users;
        }

        void listChannels_SelectedIndexChanged(object sender, EventArgs e)
        {
            RefreshUsers();
        }

        void listChannels_DrawItem(object sender, DrawItemEventArgs e)
        {
            if (e.Index == -1 || _bindingSourceChannels.Count == 0)
                return;
            e.DrawBackground();
            e.Graphics.DrawString(
                _bindingSourceChannels[e.Index].ToString(),
                new Font("Segoe UI", 8, FontStyle.Regular),
                new SolidBrush(Color.WhiteSmoke),
                e.Bounds
                );
            e.DrawFocusRectangle();
        }

        void listUsers_DrawItem(object sender, DrawItemEventArgs e)
        {
            if (e.Index == -1 || _bindingSourceUsers.Count == 0)
                return;
            e.DrawBackground();
            e.Graphics.DrawString(
                _bindingSourceUsers[e.Index].ToString(),
                new Font("Segoe UI", 8, FontStyle.Regular),
                new SolidBrush(Color.WhiteSmoke),
                e.Bounds
                );
            e.DrawFocusRectangle();
        }
    }
}
