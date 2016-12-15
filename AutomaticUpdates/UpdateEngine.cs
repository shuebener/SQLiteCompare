using System;
using System.Threading;
using System.IO;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Reflection;
using System.Net;
using System.Diagnostics;
using Common;

namespace AutomaticUpdates
{
    /// <summary>
    /// This class is responsible for managing the automatic updates process
    /// from the point of view of the SQLite Compare application.
    /// </summary>
    public class UpdateEngine
    {
        #region Events
        /// <summary>
        /// Fired when the process of checking for updates has finished
        /// </summary>
        public static event CheckForUpdatesCompletedEventHandler CheckForUpdatesCompleted;

        /// <summary>
        /// Fired when the process of downloading software updates has finished
        /// </summary>
        public static event DownloadUpdatesCompletedEventHandler DownloadUpdatesCompleted;
        #endregion

        #region Public Properties
        /// <summary>
        /// Checks if the update engine is busy or not
        /// </summary>
        public static bool IsBusy
        {
            get 
            { 
                lock(typeof(UpdateEngine))
                    return _isBusy; 
            }
        }

        /// <summary>
        /// Checks if the application will soon re-install from the updated software installer.
        /// </summary>
        public static bool IsReInstalling
        {
            get
            {
                return _updatesFilePath != null;
            }
        }
        #endregion

        #region Public Methods

        /// <summary>
        /// Prepares to restart the application
        /// </summary>
        /// <param name="fpath"></param>
        public static void PrepareForInstall(string fpath)
        {
            _updatesFilePath = fpath;
        }

        /// <summary>
        /// Apply pending updates by spawning a helper process to overwrite
        /// the necessary files, remove the unneeded files.
        /// </summary>
        public static void ApplyPendingUpdates()
        {
            // No updates are pending
            if (_updatesFilePath == null)
                return;

            // Invoke the installer program and exit
            Process p = new Process();
            ProcessStartInfo psi = new ProcessStartInfo(_updatesFilePath);
            p.StartInfo = psi;
            p.Start();

            Environment.Exit(0);
        }

        /// <summary>
        /// Check for software updates asynchronously
        /// </summary>
        public static void CheckForUpdatesAsync()
        {
            WaitCallback wc = delegate
            {
                if (CheckForUpdatesCompleted != null)
                    CheckForUpdatesCompleted(null, new CheckForUpdatesCompletedEventArgs(null, new ApplicationException("Automatic updates are no longer supported"), false, null));

                //try
                //{
                //    List<VersionUpdateInfo> res = CheckForUpdates();
                //    if (CheckForUpdatesCompleted != null)
                //        CheckForUpdatesCompleted(null, new CheckForUpdatesCompletedEventArgs(res, null, false, null));
                //}
                //catch (UserCancellationException uce)
                //{
                //    if (CheckForUpdatesCompleted != null)
                //        CheckForUpdatesCompleted(null, new CheckForUpdatesCompletedEventArgs(null, null, true, null));
                //}
                //catch (Exception ex)
                //{
                //    if (CheckForUpdatesCompleted != null)
                //        CheckForUpdatesCompleted(null, new CheckForUpdatesCompletedEventArgs(null, ex, false, null));
                //} // catch
            };
            ThreadPool.QueueUserWorkItem(wc);
        }

        /// <summary>
        /// Download software updates asynchronously
        /// </summary>
        public static void DownloadUpdatesAsync()
        {
            WaitCallback wc = delegate
            {
                try
                {
                    string res = DownloadUpdates();
                    if (DownloadUpdatesCompleted != null)
                        DownloadUpdatesCompleted(null, new DownloadUpdatesCompletedEventArgs(res, null, false, null));
                }
                catch (UserCancellationException uce)
                {
                    if (DownloadUpdatesCompleted != null)
                        DownloadUpdatesCompleted(null, new DownloadUpdatesCompletedEventArgs(null, null, true, null));
                }
                catch (Exception ex)
                {
                    if (DownloadUpdatesCompleted != null)
                        DownloadUpdatesCompleted(null, new DownloadUpdatesCompletedEventArgs(null, ex, false, null));
                }
            };
            ThreadPool.QueueUserWorkItem(wc);
        }

        /// <summary>
        /// Cancel any ongoing activity
        /// </summary>
        public static void CancelAsync()
        {
            _cancelled = true;
            lock (typeof(UpdateEngine))
            {
                if (_webclient != null)
                    _webclient.CancelAsync();
            } // lock
        }

        /// <summary>
        /// Downloads the software updates file from the server and store it into a temporary
        /// file.
        /// </summary>
        /// <returns>The path to the temporary updates file</returns>
        public static string DownloadUpdates()
        {
            lock (typeof(UpdateEngine))
            {
                if (_isBusy)
                    throw new InvalidOperationException("UpdateEngine is busy");
                _isBusy = true;
            } // lock

            _cancelled = false;
            try
            {
                try
                {
                    lock (typeof(UpdateEngine))
                        _webclient = new WebClient();

                    // Allocate a temporary name for the updates file
                    string fpath = Path.GetTempPath() + "/" + INSTALLER_FILE_NAME;
                    if (File.Exists(fpath))
                        File.Delete(fpath);

                    // Download the file from the server
                    _webclient.DownloadFile(INSTALLER_FILE_URL, fpath);

                    return fpath;
                }
                catch (System.Net.WebException wex)
                {
                    if (wex.Status == WebExceptionStatus.RequestCanceled)
                        throw new UserCancellationException();
                    else
                        throw;
                }
                finally
                {
                    lock (typeof(UpdateEngine))
                        _webclient = null;
                } // finally
            }
            finally
            {
                lock (typeof(UpdateEngine))
                    _isBusy = false;
            } // finally
        }

        /// <summary>
        /// Checks the website for a new software update.
        /// </summary>
        /// <returns>NULL - in case there are no updates, The list of version update info objects in case there 
        /// are updates</returns>
        /// <exception cref="UserCancalledException">In case the user chose to cancel the operation</exception>
        /// <exception cref="InvalidOperationException">In case another operation is already active</exception>
        public static List<VersionUpdateInfo> CheckForUpdates()
        {
            lock (typeof(UpdateEngine))
            {
                if (_isBusy)
                    throw new InvalidOperationException("UpdateEngine is busy");
                _isBusy = true;
            } // lock

            _cancelled = false;
            try
            {
                // Get my version number string
                string version = GetSoftwareVersion();

                // Download the last_version.txt file with the latest version string
                List<VersionUpdateInfo> vlist = null;
                try
                {
                    lock (typeof(UpdateEngine))
                        _webclient = new WebClient();
                    using (Stream s = _webclient.OpenRead(CHANGES_FILE_URL))
                    {
                        vlist = ParseChanges(s);
                    } // using

                    if (vlist == null)
                        throw new ApplicationException("failed to load the changes.txt file from the web server.");
                }
                catch (System.Net.WebException wex)
                {
                    if (wex.Status == WebExceptionStatus.RequestCanceled)
                        throw new UserCancellationException();
                    else
                        throw;
                }
                finally
                {
                    lock (typeof(UpdateEngine))
                        _webclient = null;
                } // finally

                // If the latest version is the same - there are no updates available
                if (vlist[vlist.Count - 1].Version == version)
                {
                    // Notify that there are no updates available
                    return null;
                }
                else
                {
                    // Prepare a sublist from the version update info list that contains all the software updates
                    // from the current installation version to the latest version.
                    bool found = false;
                    List<VersionUpdateInfo> res = new List<VersionUpdateInfo>();
                    for (int i = 0; i < vlist.Count; i++)
                    {
                        if (vlist[i].Version == version)
                            found = true;
                        else if (found)
                            res.Add(vlist[i]);
                    } // for 

                    // Notify that there are updates available
                    return res;
                } // else
            }
            finally
            {
                lock (typeof(UpdateEngine))
                    _isBusy = false;
            } // finally
        }
        #endregion

        #region Private Methods
        private static List<VersionUpdateInfo> ParseChanges(Stream s)
        {
            Regex versionRx = new Regex(@"^\[((\d+)\.(\d+)\.(\d+)\.(\d+))\]");
            Regex propRx = new Regex(@"^((\w+)(\((\w+)\))?)\s*\=\s*(.*)$");
            Regex dateRx = new Regex(@"(\d\d\d\d)\-(\d\d)\-(\d\d)");

            List<VersionUpdateInfo> res = new List<VersionUpdateInfo>();
            using (StreamReader reader = new StreamReader(s))
            {
                VersionUpdateInfo vinfo = null;
                while (!reader.EndOfStream)
                {
                    string line = reader.ReadLine();
                    Match m = versionRx.Match(line);
                    if (m.Success)
                    {
                        if (vinfo != null)
                            res.Add(vinfo);

                        vinfo = new VersionUpdateInfo();
                        vinfo.Version = m.Groups[1].Value;
                    }
                    else
                    {
                        m = propRx.Match(line);
                        if (m.Success)
                        {
                            string pname = m.Groups[2].Value.ToLower();
                            if (pname == "bug")
                            {
                                if (m.Groups[4].Success)
                                {
                                    string sev = m.Groups[4].Value.ToLower();
                                    BugSeverity severity = BugSeverity.None;
                                    if (sev == "minor")
                                        severity = BugSeverity.Minor;
                                    else if (sev == "major")
                                        severity = BugSeverity.Major;
                                    else if (sev == "critical")
                                        severity = BugSeverity.Critical;
                                    if (severity != BugSeverity.None)
                                    {
                                        BugFixInfo bug = new BugFixInfo(severity, m.Groups[5].Value);
                                        vinfo.FixedBugs.Add(bug);
                                    }
                                } // if
                            }
                            else if (pname == "feature")
                            {
                                if (m.Groups[4].Success)
                                {
                                    string imp = m.Groups[4].Value.ToLower();
                                    FeatureImpact impact = FeatureImpact.None;
                                    if (imp == "minor")
                                        impact = FeatureImpact.Minor;
                                    else if (imp == "major")
                                        impact = FeatureImpact.Major;
                                    if (impact != FeatureImpact.None)
                                    {
                                        FeatureInfo feature = new FeatureInfo(impact, m.Groups[5].Value);
                                        vinfo.AddedFeatures.Add(feature);
                                    }
                                } // if
                            }
                            else if (pname == "release")
                            {
                                string datestr = m.Groups[5].Value;
                                m = dateRx.Match(datestr);
                                if (m.Success)
                                {
                                    int year = int.Parse(m.Groups[1].Value);
                                    int month = int.Parse(m.Groups[2].Value);
                                    int day = int.Parse(m.Groups[3].Value);
                                    vinfo.ReleaseDate = new DateTime(year, month, day);
                                }
                            }
                        }
                    } // else
                } // while
                if (vinfo != null)
                    res.Add(vinfo);
            } // using

            if (res.Count == 0)
                return null;
            else
                return res;
        }

        private static void CheckCancelled()
        {
            if (_cancelled)
                throw new UserCancellationException();
        }

        private static string GetSoftwareVersion()
        {
            Assembly assembly = Assembly.GetExecutingAssembly();
            string version = assembly.GetName().Version.ToString();
            return version;
        }
        #endregion

        #region Private Constants
        private const string WEBSITE_URL = "http://sqlitecompare.com";
        private const string SOFTWARE_URL = WEBSITE_URL + "/software";
        private const string CHANGES_FILE_URL = SOFTWARE_URL + "/changes.txt";
        private const string INSTALLER_FILE_URL = SOFTWARE_URL + "/SQLiteCompareSetup.exe";
        private const string INSTALLER_FILE_NAME = "SQLiteCompareSetup.exe";
        #endregion

        #region Private Variables
        private static bool _isBusy;
        private static bool _cancelled;
        private static WebClient _webclient;
        private static string _updatesFilePath;
        #endregion
    }

    public class DownloadUpdatesCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs
    {
        public DownloadUpdatesCompletedEventArgs(string result, 
            Exception error, bool cancelled, object userState)
            : base(error, cancelled, userState)
        {
            _fpath = result;
        }

        public string Result
        {
            get { return _fpath; }
        }

        private string _fpath = null;
    }
    public delegate void DownloadUpdatesCompletedEventHandler(object sender, DownloadUpdatesCompletedEventArgs e);

    public class CheckForUpdatesCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs
    {
        public CheckForUpdatesCompletedEventArgs(List<VersionUpdateInfo> result, 
            Exception error, bool cancelled, object userState)
            : base(error, cancelled, userState)
        {
            _updates = result;
        }

        public List<VersionUpdateInfo> Result
        {
            get { return _updates; }
        }

        private List<VersionUpdateInfo> _updates = null;
    }

    public delegate void CheckForUpdatesCompletedEventHandler(object sender, 
        CheckForUpdatesCompletedEventArgs e);
}
