using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Management;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using WindowsInput;
using WindowsInput.Native;
using Newtonsoft.Json;

// todo lost/rad - start with captains mode
// todo brian - check for file when starting and check for fileopenexception when stopping

// key from key: https://inputsimulator.codeplex.com/discussions/233208

// todo mlh : remove inputsimulator dependency???

namespace TGNS
{
    public partial class Form1 : Form
    {
        readonly Configuration _config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
        private readonly IList<LogEntry> _logEntries = new List<LogEntry>();
        private WebServer _ws;
        private FileSystemInfo _recordingFileSystemInfo;
        private readonly ObsRecordingStatusChanger _obsRecordingStatusChanger = new ObsRecordingStatusChanger();

        readonly Func<IEnumerable<FileSystemInfo>> _getFileSystemInfos;
        private static readonly string VersionDescriptor = "v0.11";
        private readonly ManagementEventWatcher _processStopEventWatcher;


        public Form1()
        {
            InitializeComponent();
            _getFileSystemInfos = () => new DirectoryInfo(VideoPath).GetFileSystemInfos().Where(x => x.Extension.Equals($".{VideoExtWithoutPeriod}"));

            _processStopEventWatcher = new ManagementEventWatcher(new WqlEventQuery("SELECT * FROM Win32_ProcessStopTrace")); // http://stackoverflow.com/a/1986856/116895
            _processStopEventWatcher.EventArrived += processStopEventWatcher_EventArrived;
            _processStopEventWatcher.Start();
        }

        private void processStopEventWatcher_EventArrived(object sender, EventArrivedEventArgs e)
        {
            if (AutoCloseObsCheckBox.Checked)
            {
                var processName = e.NewEvent.Properties["ProcessName"].Value;
                if (processName.Equals("NS2.exe"))
                {
                    var obsIsReadyForButtonClicks = _obsRecordingStatusChanger.IsReadyForButtonClicks();
                    if (obsIsReadyForButtonClicks)
                    {
                        var obsIsRecording = _obsRecordingStatusChanger.IsRecording();
                        Log($"NS2 exit detected. {(obsIsRecording ? "Stopping recording and c" : "C")}losing OBS...");
                        if (obsIsRecording)
                        {
                            _obsRecordingStatusChanger.StopRecording();
                            Thread.Sleep(3000);
                        }
                        _obsRecordingStatusChanger.ClickExitButton();
                    }
                }
            }
        }

        string LogText
        {
            get
            {
                string result = null;
                if (LogTextBox.InvokeRequired)
                {
                    LogTextBox.Invoke(new MethodInvoker(delegate { result = LogTextBox.Text; }));
                }
                else
                {
                    result = LogTextBox.Text;
                }
                return result;
            }
            set
            {
                if (LogTextBox.InvokeRequired)
                {
                    LogTextBox.Invoke(new MethodInvoker(delegate { LogTextBox.Text = value; }));
                }
                else
                {
                    LogTextBox.Text = value;
                }
            }
        }

        string VideoPath
        {
            get
            {
                string result = null;
                if (VideoPathTextBox.InvokeRequired)
                {
                    VideoPathTextBox.Invoke(new MethodInvoker(delegate { result = VideoPathTextBox.Text; }));
                }
                else
                {
                    result = VideoPathTextBox.Text;
                }
                return result;
            }
            set
            {
                if (VideoPathTextBox.InvokeRequired)
                {
                    VideoPathTextBox.Invoke(new MethodInvoker(delegate { VideoPathTextBox.Text = value; }));
                }
                else
                {
                    VideoPathTextBox.Text = value;
                }
            }
        }

        string ObsAutoStartFilename
        {
            get
            {
                string result = null;
                if (ObsAutoStartFilenameTextBox.InvokeRequired)
                {
                    ObsAutoStartFilenameTextBox.Invoke(new MethodInvoker(delegate { result = ObsAutoStartFilenameTextBox.Text; }));
                }
                else
                {
                    result = ObsAutoStartFilenameTextBox.Text;
                }
                return result;
            }
            set
            {
                if (ObsAutoStartFilenameTextBox.InvokeRequired)
                {
                    ObsAutoStartFilenameTextBox.Invoke(new MethodInvoker(delegate { ObsAutoStartFilenameTextBox.Text = value; }));
                }
                else
                {
                    ObsAutoStartFilenameTextBox.Text = value;
                }
            }
        }

        string VideoExt
        {
            get
            {
                string result = null;
                if (VideoExtTextBox.InvokeRequired)
                {
                    VideoExtTextBox.Invoke(new MethodInvoker(delegate { result = VideoExtTextBox.Text; }));
                }
                else
                {
                    result = VideoExtTextBox.Text;
                }
                return result;
            }
            set
            {
                if (VideoExtTextBox.InvokeRequired)
                {
                    VideoExtTextBox.Invoke(new MethodInvoker(delegate { VideoExtTextBox.Text = value; }));
                }
                else
                {
                    VideoExtTextBox.Text = value;
                }
            }
        }

        private void Log(string message)
        {
            //if (!_logEntries.Any() || !_logEntries.First().Message.Equals(message))
            //{
                _logEntries.Insert(0, new LogEntry {When = DateTime.Now, Message = message});
            //}
            var lines = string.Join(Environment.NewLine, _logEntries.Select(x => $"{x.When.ToString("[HH:mm:ss]")}: {x.Message}"));
            LogText = lines;
        }

        string VideoExtWithoutPeriod => VideoExt.Replace(".", string.Empty);












        string ObsAutoStartFilenameConfig
        {
            get
            {
                var existingConfig = _config.AppSettings.Settings["ObsAutoStartFilename"];
                var result = existingConfig != null && !string.IsNullOrWhiteSpace(existingConfig.Value) ? existingConfig.Value : @"C:\Program Files (x86)\OBS\OBS.exe";
                return result;
            }
            set
            {
                var existingConfig = _config.AppSettings.Settings["ObsAutoStartFilename"];
                if (existingConfig != null)
                {
                    existingConfig.Value = value;
                }
                else
                {
                    _config.AppSettings.Settings.Add("ObsAutoStartFilename", value);
                }
            }
        }

        string VideoPathConfig
        {
            get
            {
                var existingConfig = _config.AppSettings.Settings["VideoPath"];
                var result = existingConfig != null && !string.IsNullOrWhiteSpace(existingConfig.Value) ? existingConfig.Value : @"C:\path\to\recorded\videos";
                return result;
            }
            set
            {
                var existingConfig = _config.AppSettings.Settings["VideoPath"];
                if (existingConfig != null)
                {
                    existingConfig.Value = value;
                }
                else
                {
                    _config.AppSettings.Settings.Add("VideoPath", value);
                }
            }
        }

        string VideoExtConfig
        {
            get
            {
                var existingConfig = _config.AppSettings.Settings["VideoExt"];
                var result = existingConfig != null && !string.IsNullOrWhiteSpace(existingConfig.Value) ? existingConfig.Value : "flv";
                return result;
            }
            set
            {
                var existingConfig = _config.AppSettings.Settings["VideoExt"];
                if (existingConfig != null)
                {
                    existingConfig.Value = value;
                }
                else
                {
                    _config.AppSettings.Settings.Add("VideoExt", value);
                }
            }
        }

        string ShowIconConfig
        {
            get
            {
                var existingConfig = _config.AppSettings.Settings["ShowIcon"];
                var result = existingConfig != null && !string.IsNullOrWhiteSpace(existingConfig.Value) ? existingConfig.Value : bool.FalseString;
                return result;
            }
            set
            {
                var existingConfig = _config.AppSettings.Settings["ShowIcon"];
                if (existingConfig != null)
                {
                    existingConfig.Value = value;
                }
                else
                {
                    _config.AppSettings.Settings.Add("ShowIcon", value);
                }
            }
        }

        string CasterModeConfig
        {
            get
            {
                var existingConfig = _config.AppSettings.Settings["CasterMode"];
                var result = existingConfig != null && !string.IsNullOrWhiteSpace(existingConfig.Value) ? existingConfig.Value : bool.FalseString;
                return result;
            }
            set
            {
                var existingConfig = _config.AppSettings.Settings["CasterMode"];
                if (existingConfig != null)
                {
                    existingConfig.Value = value;
                }
                else
                {
                    _config.AppSettings.Settings.Add("CasterMode", value);
                }
            }
        }

        string AutoCloseObsConfig
        {
            get
            {
                var existingConfig = _config.AppSettings.Settings["AutoCloseObs"];
                var result = existingConfig != null && !string.IsNullOrWhiteSpace(existingConfig.Value) ? existingConfig.Value : bool.FalseString;
                return result;
            }
            set
            {
                var existingConfig = _config.AppSettings.Settings["AutoCloseObs"];
                if (existingConfig != null)
                {
                    existingConfig.Value = value;
                }
                else
                {
                    _config.AppSettings.Settings.Add("AutoCloseObs", value);
                }
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            ObsAutoStartFilenameTextBox.Text = ObsAutoStartFilenameConfig;
            VideoPathTextBox.Text = VideoPathConfig;
            VideoExtTextBox.Text = VideoExtConfig;
            ShowIconCheckbox.Checked = ShowIconConfig == bool.TrueString;
            CasterModeCheckBox.Checked = CasterModeConfig == bool.TrueString;
            AutoCloseObsCheckBox.Checked = AutoCloseObsConfig == bool.TrueString;

            Action<string> autolaunchObs = detectionDescriptor =>
            {
                if (!string.IsNullOrWhiteSpace(ObsAutoStartFilename))
                {
                    var processes = Process.GetProcesses();
                    var ns2IsRunning = processes.Any(x => string.Equals(x.ProcessName, "NS2", StringComparison.InvariantCultureIgnoreCase));
                    if (ns2IsRunning)
                    {
                        var obsIsRunning = processes.Any(x => string.Equals(x.ProcessName, "OBS", StringComparison.InvariantCultureIgnoreCase));
                        if (!obsIsRunning)
                        {
                            if (File.Exists(ObsAutoStartFilename))
                            {
                                var processStartInfo = new ProcessStartInfo(ObsAutoStartFilename) { WindowStyle = ProcessWindowStyle.Minimized };
                                Log($"{detectionDescriptor} detected. AutoLaunching {ObsAutoStartFilename}...");
                                Process.Start(processStartInfo);
                            }
                        }
                    }
                }
            };

            Log(VersionDescriptor);

            Action listenForGamesIfReady = () =>
            {
                var videoPathExists = Directory.Exists(VideoPath);
                var extensionHasValue = !string.IsNullOrWhiteSpace(VideoExtWithoutPeriod);
                if (videoPathExists && extensionHasValue)
                {
                    if (_ws == null)
                    {
                        _ws = new WebServer(request =>
                        {
                            if (request.Url.AbsolutePath.EndsWith("record_start"))
                            {
                                Log("TGNS game started.");
                                StartRecording();
                            }
                            else if (request.Url.AbsolutePath.EndsWith("record_end"))
                            {
                                Log("TGNS game ended.");
                                EndRecording(request, true);
                            }
                            else if (request.Url.AbsolutePath.EndsWith("record_prep"))
                            {
                                autolaunchObs("TGNS connection");
                                Thread.Sleep(2000);
                                if (_obsRecordingStatusChanger.IsRecording())
                                {
                                    EndRecording(request, false);
                                }
                            }
                            var response = new Dictionary<string, object> {{"trhVersion", VersionDescriptor}};
                            if (ShowIconCheckbox.Checked)
                            {
                                if (_obsRecordingStatusChanger.IsReadyForButtonClicks())
                                {
                                    response["showIcon"] = true;
                                }
                                else
                                {
                                    Log("Did not show scoreboard icon. OBS is not ready.");
                                }
                            }
                            if (CasterModeCheckBox.Checked)
                            {
                                if (_obsRecordingStatusChanger.IsReadyForButtonClicks())
                                {
                                    response["casterMode"] = true;
                                }
                                else
                                {
                                    Log("Did not enable Caster Mode. OBS is not ready.");
                                }
                            }
                            return JsonConvert.SerializeObject(response);
                        }, exception =>
                        {
                            Log($"Unexpected error. Message: {exception.Message} | Stacktrace: ${exception.StackTrace}");
                        }
                        , "http://localhost:8467/tgns/");
                        _ws.Run();
                        Log("Listening for TGNS game start/stops...");
                    }
                }
                else
                {
                    Log($"Specify '{FolderLabel.Text.Replace(":", string.Empty).Trim()}' (that exists) and '{ExtensionLabel.Text.Replace(":", string.Empty).Trim()}' values above.");
                }
            };

            VideoPathTextBox.LostFocus += (o, args) => { listenForGamesIfReady(); };
            VideoExtTextBox.LostFocus += (o, args) => { listenForGamesIfReady(); };
            listenForGamesIfReady();
            autolaunchObs("NS2");
        }

        //protected override void OnResize(EventArgs e)
        //{
        //    base.OnResize(e);
        //    if (WindowState == FormWindowState.Minimized)
        //    {
        //        // this.ShowInTaskbar = false;
        //        Hide();
        //    }
        //}

        //private void ApplicationMainForm_Resize(object sender, EventArgs e)
        //{
        //    if (this.WindowState == FormWindowState.Minimized)
        //    {
        //        // this.ShowInTaskbar = false;
        //        Hide();
        //    }
        //}



        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing)
            {
                ShowInTaskbar = false;
                Hide();
                e.Cancel = true;
            }
            else
            {
                _processStopEventWatcher?.Stop();
                base.OnFormClosing(e);
            }
        }

        private void KeysHelpLinkLabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start("https://msdn.microsoft.com/en-us/library/windows/desktop/dd375731(v=vs.85).aspx");
        }

        private void LearnMoreLinkLabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start("http://rr.tacticalgamer.com/Replay/RecordingHelper");
        }

        private void StartKeyTestButton_Click(object sender, EventArgs e)
        {
            StartRecording();
        }

        void ModifyRecordingState(string actionDescription, Action action)
        {
            if (_obsRecordingStatusChanger.IsReadyForButtonClicks())
            {
                //Log($"Recording button text: '{_obsRecordingStatusChanger.GetRecordingButtonText()}'");
                action();
            }
            else
            {
                Log($"Failed to {actionDescription} recording. OBS Classic window not found! Is it running (disable 'Minimize to Notification Area' in Setting > General)?");
            }
        }

        void StartRecording()
        {
            ModifyRecordingState("start", () =>
            {
                var isRecording = _obsRecordingStatusChanger.IsRecording();
                if (isRecording)
                {
                    Log("Recording already in progress. No action taken.");
                }
                else
                {
                    var fileSystemInfos = Directory.Exists(VideoPath) ? _getFileSystemInfos().ToList() : new List<FileSystemInfo>();
                    _obsRecordingStatusChanger.StartRecording();
                    isRecording = _obsRecordingStatusChanger.IsRecording();
                    if (isRecording)
                    {
                        var message = "Recording started.";
                        var updatedFileSystemInfos = Directory.Exists(VideoPath) ? _getFileSystemInfos().ToList() : new List<FileSystemInfo>();
                        var newFileSystemInfos = updatedFileSystemInfos.Where(x => !fileSystemInfos.Any(y => x.Name.Equals(y.Name))).ToList();
                        if (newFileSystemInfos.Count == 1)
                        {
                            _recordingFileSystemInfo = newFileSystemInfos.First();
                            message = message + $" Recording output file detected ({_recordingFileSystemInfo.Name}).";
                        }
                        else
                        {
                            message = message + " No new recording output file detected.";
                        }
                        Log(message);
                    }
                    else
                    {
                        Log("Tried to start recording, but failed. Try running as administrator ( https://i.imgur.com/llOEq16.png ).");
                    }
                }
            });
        }

        void EndRecording(HttpListenerRequest request, bool shouldRenameOutputFile)
        {
            ModifyRecordingState("stop", () =>
            {
                var isRecording = _obsRecordingStatusChanger.IsRecording();
                if (isRecording)
                {
                    _obsRecordingStatusChanger.StopRecording();
                    isRecording = _obsRecordingStatusChanger.IsRecording();
                    if (isRecording)
                    {
                        Log("Failed to stop recording.");

                    }
                    else
                    {
                        Log("Recording stopped.");
                        if (shouldRenameOutputFile)
                        {
                            if (_recordingFileSystemInfo != null)
                            {
                                var mapName = request != null ? request.QueryString["m"] : "test_mapname";
                                mapName = mapName.Replace("ns2_", "");
                                mapName = mapName.Replace("tgns_", "");
                                var buildNumber = request != null ? request.QueryString["b"] : "test_build_number";
                                var teamName = request != null ? request.QueryString["team"] : "test_teamname";
                                var durationInMinutes = Math.Round(Convert.ToDecimal(request != null ? request.QueryString["d"] : "15")/60);
                                var who = Path.GetInvalidFileNameChars().Aggregate(request != null ? request.QueryString["n"] : "test_playername", (current, c) => current.Replace(c.ToString(), string.Empty));
                                if (string.IsNullOrWhiteSpace(who))
                                {
                                    who = request != null ? request.QueryString["i"] : "test_ns2id";
                                }

                                var when = request != null ? request.QueryString["t"] : Convert.ToInt64((DateTime.Now - new DateTime(1970, 1, 1)).TotalSeconds).ToString(CultureInfo.InvariantCulture);
                                var whenLong = Convert.ToInt64(when);
                                var whenFormatted = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Local).AddSeconds(whenLong).ToString("MMMM dd, yyyy");
                                var destFileName = $@"{VideoPath}\{when} TGNS {mapName} {durationInMinutes}m {teamName} on {whenFormatted} build {buildNumber} by {who}.{VideoExtWithoutPeriod}";
                                Action renameFile = () =>
                                {
                                    Log("Renaming video file...");
                                    File.Move(_recordingFileSystemInfo.FullName, destFileName);
                                    var oldDisplay = _recordingFileSystemInfo.FullName.Replace(VideoPath, string.Empty);
                                    var destFileNameDisplay = destFileName.Replace(VideoPath, string.Empty);
                                    if (oldDisplay.StartsWith(@"\"))
                                    {
                                        oldDisplay = oldDisplay.Substring(1);
                                    }
                                    if (destFileNameDisplay.StartsWith(@"\"))
                                    {
                                        destFileNameDisplay = destFileNameDisplay.Substring(1);
                                    }
                                    Log("Rename successful.");
                                    Log($"Old: {oldDisplay}");
                                    Log($"New: {destFileNameDisplay}");
                                };
                                try
                                {
                                    renameFile();
                                }
                                catch (Exception e)
                                {
                                    Log($"Problem renaming file. Message: {e.Message} -- will try again in 3 seconds.");
                                    Thread.Sleep(3000);
                                    renameFile();
                                }
                                finally
                                {
                                    _recordingFileSystemInfo = null;
                                }
                            }
                            else
                            {
                                Log("Recording output file unknown. Skipping filename rename operation.");
                            }
                        }
                    }
                }
                else
                {
                    Log("No recording in progress. No action taken.");
                }
            });
        }

        private void EndKeyTestButton_Click(object sender, EventArgs e)
        {
            EndRecording(null, true);
        }

        private void ClearLogButton_Click(object sender, EventArgs e)
        {
            _logEntries.Clear();
            LogText = string.Empty;
        }

        //private void LaunchObsTimer_Tick(object sender, EventArgs e)
        //{
        //    if (!string.IsNullOrWhiteSpace(ObsAutoStartFilename))
        //    {
        //        var ns2IsRunning = Process.GetProcesses().Any(x => x.ProcessName == "ns2");
        //        if (ns2IsRunning)
        //        {
        //            var obsIsRunning = Process.GetProcesses().Any(x => x.ProcessName == "OBS");
        //            if (!obsIsRunning)
        //            {
        //                if (File.Exists(ObsAutoStartFilename))
        //                {
        //                    var processStartInfo = new ProcessStartInfo(ObsAutoStartFilename) { WindowStyle = ProcessWindowStyle.Minimized };
        //                    Log($"NS2 detected. AutoLaunching {ObsAutoStartFilename}...");
        //                    Process.Start(processStartInfo);
        //                    Thread.Sleep(5000);
        //                }
        //            }
        //        }
        //    }
        //}

        private void notifyIcon1_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                ShowMe();
            }
        }

        private void ExitMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        public void ShowMe()
        {
            WindowState = WindowState == FormWindowState.Maximized ? WindowState : FormWindowState.Normal;
            // get our current "TopMost" value (ours will always be false though)
            bool top = TopMost;
            //// make our form jump to the top of everything
            TopMost = true;
            //// set it back to whatever it was
            TopMost = top;
            Show();
            Activate();
            ShowInTaskbar = true;
        }

        private void ShowMenuItem_Click(object sender, EventArgs e)
        {
            ShowMe();
        }

        private void ObsAutoStartFilenameTextBox_TextChanged(object sender, EventArgs e)
        {
            ObsAutoStartFilenameConfig = ObsAutoStartFilenameTextBox.Text;
            _config.Save(ConfigurationSaveMode.Modified);
        }

        private void VideoPathTextBox_TextChanged(object sender, EventArgs e)
        {
            VideoPathConfig = VideoPathTextBox.Text;
            _config.Save(ConfigurationSaveMode.Modified);
        }

        private void VideoExtTextBox_TextChanged(object sender, EventArgs e)
        {
            VideoExtConfig = VideoExtTextBox.Text;
            _config.Save(ConfigurationSaveMode.Modified);
        }

        private void ShowIconCheckbox_CheckedChanged(object sender, EventArgs e)
        {
            ShowIconConfig = ShowIconCheckbox.Checked ? bool.TrueString : bool.FalseString;
            _config.Save(ConfigurationSaveMode.Modified);
        }

        private void CasterModeCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            CasterModeConfig = CasterModeCheckBox.Checked ? bool.TrueString : bool.FalseString;
            _config.Save(ConfigurationSaveMode.Modified);
        }

        private void AutoCloseObsCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            AutoCloseObsConfig = AutoCloseObsCheckBox.Checked ? bool.TrueString : bool.FalseString;
            _config.Save(ConfigurationSaveMode.Modified);
        }
    }

    public class WebServer // https://codehosting.net/blog/BlogEngine/post/Simple-C-Web-Server.aspx
    {
        private readonly HttpListener _listener = new HttpListener();
        private readonly Func<HttpListenerRequest, string> _responderMethod;
        private readonly Action<Exception> _exceptionAction;

        public WebServer(string[] prefixes, Func<HttpListenerRequest, string> method, Action<Exception> exceptionAction)
        {
            if (!HttpListener.IsSupported)
                throw new NotSupportedException(
                    "Needs Windows XP SP2, Server 2003 or later.");

            // URI prefixes are required, for example 
            // "http://localhost:8080/index/".
            if (prefixes == null || prefixes.Length == 0)
                throw new ArgumentException("prefixes");

            // A responder method is required
            if (method == null)
                throw new ArgumentException("method");
            if (exceptionAction == null)
                throw new ArgumentException("exceptionAction");

            foreach (string s in prefixes)
                _listener.Prefixes.Add(s);

            _responderMethod = method;
            _exceptionAction = exceptionAction;
            _listener.Start();
        }

        public WebServer(Func<HttpListenerRequest, string> method, Action<Exception> exceptionAction, params string[] prefixes)
            : this(prefixes, method, exceptionAction)
        { }

        public void Run()
        {
            ThreadPool.QueueUserWorkItem((o) =>
            {
                try
                {
                    while (_listener.IsListening)
                    {
                        ThreadPool.QueueUserWorkItem((c) =>
                        {
                            var ctx = c as HttpListenerContext;
                            try
                            {
                                string rstr = _responderMethod(ctx.Request);
                                byte[] buf = Encoding.UTF8.GetBytes(rstr);
                                ctx.Response.ContentLength64 = buf.Length;
                                ctx.Response.OutputStream.Write(buf, 0, buf.Length);
                            }
                            catch (Exception e)
                            {
                                _exceptionAction(e);
                            }
                            finally
                            {
                                // always close the stream
                                ctx.Response.OutputStream.Close();
                            }
                        }, _listener.GetContext());
                    }
                }
                catch (Exception e)
                {
                    _exceptionAction(e);
                } // suppress any exceptions
            });
        }

        public void Stop()
        {
            _listener.Stop();
            _listener.Close();
        }

    }

    public class LogEntry
    {
        public DateTime When { get; set; }
        public string Message { get; set; }
    }
}
