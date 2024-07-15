using NetMQ;
using NetMQ.Zyre;
using NetMQ.Zyre.ZyreEvents;
using SamplePeer;
using System.Collections.ObjectModel;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace ZyreSamplePeerWPF.App
{

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        #region Properties
        private string _name;
        private readonly Zyre _zyre;
        private readonly ObservableCollection<Peer> _connectedPeers;
        private readonly ObservableCollection<Group> _ownGroups;
        private readonly ObservableCollection<Group> _peerGroups;
        private readonly Guid _uuid;
        private string _endpoint;
        private readonly Dictionary<Guid, ObservableCollection<Header>> _headersByPeerGuid;
        #endregion

        #region Ctor
        public MainWindow()
        {
            InitializeComponent();

            DisplayTitle();

            btnStop.IsEnabled = false;
            _connectedPeers = new ObservableCollection<Peer>();
            peerBindingSource.ItemsSource = _connectedPeers;
            _ownGroups = new ObservableCollection<Group>();
            ownGroupBindingSource.ItemsSource = _ownGroups;
            _peerGroups = new ObservableCollection<Group>();
            peerGroupBindingSource.ItemsSource = _peerGroups;
            _headersByPeerGuid = new Dictionary<Guid, ObservableCollection<Header>>();

            _zyre = new Zyre(_name, true, NodeLogger);
            if (!string.IsNullOrEmpty(nameTb.Text))
            {
                _zyre.SetName(nameTb.Text);
            }
            _name = _zyre.Name();
            _uuid = _zyre.Uuid();

            _zyre.EnterEvent += ZyreEnterEvent;
            _zyre.StopEvent += ZyreStopEvent;
            _zyre.ExitEvent += ZyreExitEvent;
            _zyre.EvasiveEvent += ZyreEvasiveEvent;
            _zyre.JoinEvent += ZyreJoinEvent;
            _zyre.LeaveEvent += ZyreLeaveEvent;
            _zyre.WhisperEvent += ZyreWhisperEvent;
            _zyre.ShoutEvent += ZyreShoutEvent;
        }
        #endregion

        private void ZyreShoutEvent(object sender, ZyreEventShout e)
        {
            Application.Current.Dispatcher.Invoke(new Action(() =>
            {
                var msg = e.Content.Pop().ConvertToString();
                var uuidShort = e.SenderUuid.ToShortString6();
                var str = $"Shout from {uuidShort} to group={e.GroupName}: {msg}";
                UpdateMessageReceived(str);
                EventsLogger($"Shout: {e.SenderName} {e.SenderUuid.ToShortString6()} shouted message {msg} to group:{e.GroupName}");
            }));
        }

        private void ZyreWhisperEvent(object sender, ZyreEventWhisper e)
        {
            Application.Current.Dispatcher.Invoke(new Action(() =>
            {
                var msg = e.Content.Pop().ConvertToString();
                var uuidShort = e.SenderUuid.ToShortString6();
                var str = $"Whisper from {uuidShort}: {msg}";
                UpdateMessageReceived(str);
                EventsLogger($"Whisper: {e.SenderName} {e.SenderUuid.ToShortString6()} whispered message {msg}");
            }));
        }

        private void ZyreJoinEvent(object sender, ZyreEventJoin e)
        {
            Application.Current.Dispatcher.Invoke(new Action(() =>
            {
                EventsLogger($"Join: {e.SenderName} {e.SenderUuid.ToShortString6()} Group:{e.GroupName}");
                UpdateAndShowGroups();
            }));
        }

        private void ZyreLeaveEvent(object sender, ZyreEventLeave e)
        {
            Application.Current.Dispatcher.Invoke(new Action(() =>
            {
                EventsLogger($"Leave: {e.SenderName} {e.SenderUuid.ToShortString6()} Group:{e.GroupName}");
                UpdateAndShowGroups();
            }));
        }

        private void ZyreEvasiveEvent(object sender, ZyreEventEvasive e)
        {
            Application.Current.Dispatcher.Invoke(new Action(() =>
            {
                EventsLogger($"Evasive: {e.SenderName} {e.SenderUuid.ToShortString6()}");
            }));
        }

        private void ZyreExitEvent(object sender, ZyreEventExit e)
        {
            Application.Current.Dispatcher.Invoke(new Action(() =>
            {
                _connectedPeers.RemoveAll(x => x.SenderUuid == e.SenderUuid);
                _headersByPeerGuid.Remove(e.SenderUuid);
                EventsLogger($"Exited: {e.SenderName} {e.SenderUuid.ToShortString6()}");
                UpdateAndShowGroups();
            }));
        }

        private void ZyreStopEvent(object sender, ZyreEventStop e)
        {
            Application.Current.Dispatcher.Invoke(new Action(() =>
            {
                _connectedPeers.RemoveAll(x => x.SenderUuid == e.SenderUuid);
                _headersByPeerGuid.Remove(e.SenderUuid);
                EventsLogger($"Stopped: {e.SenderName} {e.SenderUuid.ToShortString6()}");
                UpdateAndShowGroups();
            }));
        }

        private void ZyreEnterEvent(object sender, ZyreEventEnter e)
        {
            Application.Current.Dispatcher.Invoke(new Action(() => { 
                var peer = new Peer(e.SenderName, e.SenderUuid, e.Address);
                _connectedPeers.Add(peer);
                var headers = new ObservableCollection<Header>();
                _headersByPeerGuid[e.SenderUuid] = headers;
                EventsLogger($"Entered: {e.SenderName} {e.SenderUuid.ToShortString6()} at {e.Address} with {e.Headers.Count} headers");
                if (e.Headers.Count > 0)
                {
                    var sb = new StringBuilder($"Headers: " + Environment.NewLine);
                    foreach (var pair in e.Headers)
                    {
                        sb.AppendLine(pair.Key + "|" + pair.Value);
                        var header = new Header(pair.Key, pair.Value);
                        headers.Add(header);
                    }
                    EventsLogger(sb.ToString());
                }
                UpdateAndShowGroups();
            }));
        }

        private void DisplayTitle()
        {
            titleTxt.Text = $"Zyre Node: {_name} {_uuid}";
            if (!string.IsNullOrEmpty(_endpoint))
            {
                titleTxt.Text += $" -- listening at {_endpoint}";
            }
        }

        private void btnStart_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Dispatcher.Invoke(new Action(() =>
            {
                btnStart.IsEnabled = false;
                btnStop.IsEnabled = true;
                _zyre.Start();
                _endpoint = _zyre.EndPoint(); // every time we start, we bind our RouterSocket to a new port
                DisplayTitle();
            }));
        }

        private void btnStop_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Dispatcher.Invoke(new Action(() =>
            {
                btnStop.IsEnabled = false;
                btnStart.IsEnabled = true;
                _zyre.Stop();
                _endpoint = null;  // every time we start, we bind our RouterSocket to a new port
                DisplayTitle();
                _connectedPeers.Clear();
            }));
        }

        private void btnAddHeader_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Dispatcher.Invoke(new Action(() =>
            {
                if (string.IsNullOrEmpty(txtHeaderKey.Text))
                {
                    MessageBox.Show("Missing header Key");
                    return;
                }
                if (string.IsNullOrEmpty(txtHeaderValue.Text))
                {
                    MessageBox.Show("Missing header Value");
                    return;
                }
                _zyre.SetHeader(txtHeaderKey.Text, txtHeaderValue.Text);
            }));
        }

        public void MessageLogger(string str)
        {
            //Application.Current.Dispatcher.Invoke(new Action(() =>
            //{
                var msg = $"[MSG] {DateTime.Now.ToString("h:mm:ss.fff")} {str}";
                //infoBar.Text += System.Environment.NewLine + msg;
                System.Diagnostics.Debug.WriteLine(msg);
                //Logger(rtbMessages, msg);
            //}));
        }

        public void NodeLogger(string str)
        {
            //Application.Current.Dispatcher.Invoke(new Action(() =>
            //{
                var threadId = Thread.CurrentThread.ManagedThreadId;
                var msg = $"Thd:{threadId} {DateTime.Now.ToString("h:mm:ss.fff")} ({_name}) {str}";
                //infoBar.Text += System.Environment.NewLine + msg;
                System.Diagnostics.Debug.WriteLine(msg);
                //Logger(rtbNodeLog, msg);
            //}));
        }

        public void EventsLogger(string str)
        {
            //Application.Current.Dispatcher.Invoke(new Action(() =>
            //{
                var msg = $"[EVT] {DateTime.Now.ToString("h:mm:ss.fff")} ({_name}) {str}";
                //infoBar.Text += System.Environment.NewLine + msg;
                //Logger(rtbEventsLog, msg);
                System.Diagnostics.Debug.WriteLine (msg);
            //}));
        }

        private void Join_Clicked(object sender, RoutedEventArgs e)
        {
            Application.Current.Dispatcher.Invoke(new Action(() =>
            {
                var groupName = txtGroupName.Text;
                if (string.IsNullOrEmpty(groupName))
                {
                    MessageBox.Show("You must enter a group name");
                    return;
                }
                _zyre.Join(groupName);
                Thread.Sleep(10);
                UpdateAndShowGroups();
            }));
        }

        private void Leave_Clicked(object sender, RoutedEventArgs e)
        {
            Application.Current.Dispatcher.Invoke(new Action(() =>
            {
                var groupName = txtGroupName.Text;
                if (string.IsNullOrEmpty(groupName))
                {
                    MessageBox.Show("You must enter a group name");
                    return;
                }
                _zyre.Leave(groupName);
                Thread.Sleep(10);
                UpdateAndShowGroups();
            }));
        }

        private void Whisper_Clicked(object sender, RoutedEventArgs e)
        {
            Application.Current.Dispatcher.Invoke(new Action(() =>
            {
                var chatMessage = txtWhisperMessage.Text;
                if (string.IsNullOrEmpty(chatMessage))
                {
                    MessageBox.Show("You must enter a chat message.");
                    return;
                }
                Guid guid = Guid.NewGuid();

                if (peerBindingSource.SelectedItem == null)
                {
                    MessageBox.Show("You must select a row in the Peer Groups list");
                    return;
                }
                var peer = peerBindingSource.SelectedItem as Peer;
                if (peer == null)
                {
                    MessageBox.Show("You must select a valid peer in the Peer Groups list");
                    return;
                }
                else
                {
                    guid = peer.SenderUuid;
                }
                /*
                var selectedRowCount = peerDataGridView.Rows.GetRowCount(DataGridViewElementStates.Selected);
                if (selectedRowCount == 0)
                {
                    MessageBox.Show("You must select a row in the Peer Groups list");
                    return;
                }
                var uuid = peerDataGridView.SelectedRows[0].Cells[2].Value.ToString();
                if (!Guid.TryParse(uuid, out guid))
                {
                    MessageBox.Show("Unable to convert SenderUuid to Guid");
                    return;
                }
                */
                var msg = new NetMQMessage();
                msg.Append(chatMessage);
                _zyre.Whisper(guid, msg);
            }));
        }

        private void Shout_Clicked(object sender, RoutedEventArgs e)
        {
            Application.Current.Dispatcher.Invoke(new Action(() =>
            {
                var chatMessage = txtShoutMessage.Text;
                if (string.IsNullOrEmpty(chatMessage))
                {
                    MessageBox.Show("You must enter a chat message.");
                    return;
                }
                string? groupName = null;
                if (peerBindingSource.SelectedItem == null)
                {
                    MessageBox.Show("You must select a row in the Peer Groups list");
                    return;
                }
                var peer = peerBindingSource.SelectedItem as Peer;
                if (peer == null)
                {
                    MessageBox.Show("You must select a valid peer in the Peer Groups list");
                    return;
                }
                else
                {
                    groupName = peer.SenderName;
                }

                /*
                var selectedRowCount = peerGroupDataGridView.Rows.GetRowCount(DataGridViewElementStates.Selected);
                if (selectedRowCount == 0)
                {
                    MessageBox.Show("You must select a row in the Connected Peers list");
                    return;
                }
                var groupName = peerGroupDataGridView.SelectedRows[0].Cells[0].Value.ToString();
                */
                var msg = new NetMQMessage();
                msg.Append(chatMessage);
                _zyre.Shout(groupName, msg);
            }));
        }

        private void UpdateAndShowGroups()
        {
            //if (InvokeRequired)
            //{
            //    BeginInvoke(new MethodInvoker(UpdateAndShowGroups));
            //}
            //else
            {
                var ownGroups = _zyre.OwnGroups();
                _ownGroups.Clear();
                foreach (var ownGroup in ownGroups)
                {
                    _ownGroups.Add(new Group(ownGroup));
                }
                var peerGroups = _zyre.PeerGroups();
                _peerGroups.Clear();
                foreach (var peerGroup in peerGroups)
                {
                    _peerGroups.Add(new Group(peerGroup));
                }

                //peerBindingSource.ResetBindings(false);
                //ownGroupBindingSource.ResetBindings(false);
                //peerGroupBindingSource.ResetBindings(false);
            }
        }

        private void UpdateMessageReceived(string msg)
        {
            /*
            if (InvokeRequired)
            {
                BeginInvoke(new MethodInvoker(() => UpdateMessageReceived(msg)));
            }
            else
            {
                MessageLogger(msg);
            }
            */
            MessageLogger(msg);
        }

        /*
        private void peerDataGridView_SelectionChanged(object sender, EventArgs e)
        {
            UpdatePeerHeaders();
        }
        */

        private void UpdatePeerHeaders()
        {
            if (peerBindingSource.SelectedItem == null)
            {
                MessageBox.Show("You must select a row in the Peer Groups list");
                return;
            }

            Guid guid = Guid.NewGuid();
            var peer = peerBindingSource.SelectedItem as Peer;
            if (peer == null)
            {
                MessageBox.Show("You must select a valid peer in the Peer Groups list");
                return;
            }
            else
            {
                guid = peer.SenderUuid;
                ObservableCollection<Header> headers;
                if (!_headersByPeerGuid.TryGetValue(guid, out headers))
                {
                    throw new Exception("Unexpected failure to find peer headers");
                }
                headerBindingSource.ItemsSource = headers;
            }
            /*
            if (InvokeRequired)
            {
                BeginInvoke(new MethodInvoker(UpdatePeerHeaders));
            }
            else
            {
                if (peerDataGridView.CurrentCell == null)
                {
                    return;
                }
                var rowIndex = peerDataGridView.CurrentCell.RowIndex;
                var uuid = peerDataGridView.Rows[rowIndex].Cells[2].Value.ToString();
                Guid guid;
                if (!Guid.TryParse(uuid, out guid))
                {
                    MessageBox.Show("Unable to convert SenderUuid to Guid");
                    return;
                }
                List<Header> headers;
                if (!_headersByPeerGuid.TryGetValue(guid, out headers))
                {
                    throw new Exception("Unexpected failure to find peer headers");
                }
                headerBindingSource.DataSource = headers;
                headerBindingSource.ResetBindings(false);
            }
            */
        }

        private void peerBindingSource_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdatePeerHeaders();
        }

        private void ownGroupBindingSource_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void peerGroupBindingSource_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void headerBindingSource_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Application.Current.Dispatcher.Invoke(new Action(() =>
            {
                btnStop.IsEnabled = false;
                btnStart.IsEnabled = true;
                _zyre.Stop();
                _endpoint = null;  // every time we start, we bind our RouterSocket to a new port
                DisplayTitle();
                _connectedPeers.Clear();
                System.Windows.Application.Current.Shutdown();
            }));
        }

        private void btnName_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(nameTb.Text))
            {
                _zyre.SetName(nameTb.Text);
            }
            _name = _zyre.Name();
        }

        /*
        /// <summary>
        /// Provide generic error handling for a DataGridView error
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void peerDataGridView_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            var dgv = (DataGridView)sender;
            var senderName = dgv.Name;
            var senderError = senderName + "_DataError()";
            MessageBox.Show("Error happened " + e.Context.ToString() + "\n" + e.Exception, senderError);

            if (e.Context == DataGridViewDataErrorContexts.Commit)
            {
                MessageBox.Show("Commit error", senderError);
            }
            if (e.Context == DataGridViewDataErrorContexts.CurrentCellChange)
            {
                MessageBox.Show("Cell change", senderError);
            }
            if (e.Context == DataGridViewDataErrorContexts.Parsing)
            {
                MessageBox.Show("Parsing error", senderError);
            }
            if (e.Context == DataGridViewDataErrorContexts.LeaveControl)
            {
                MessageBox.Show("Leave control error", senderError);
            }

            if ((e.Exception) is System.Data.ConstraintException)
            {
                var view = (DataGrid)sender;
                view.Rows[e.RowIndex].ErrorText = "an error";
                view.Rows[e.RowIndex].Cells[e.ColumnIndex].ErrorText = "an error";

                e.ThrowException = false;
            }
        }
        */
    }
}