using System;
using System.Collections.Generic;
using System.Text;

namespace AutomaticUpdates
{
    /// <summary>
    /// This class describes the various updates that occured from the last version to
    /// the current version. It also includes the version string of the current version
    /// and the version string of the previous version.
    /// </summary>
    [Serializable]
    public class VersionUpdateInfo
    {
        #region Constructors
        public VersionUpdateInfo()
        {
        }

        public VersionUpdateInfo(string version, DateTime releaseDate)
        {
            _version = version;
            _releaseDate = releaseDate;
        }
        #endregion

        #region Public Properties
        public string Version
        {
            get { return _version; }
            set { _version = value; }
        }

        public DateTime ReleaseDate
        {
            get { return _releaseDate; }
            set { _releaseDate = value; }
        }

        public List<BugFixInfo> FixedBugs
        {
            get { return _fixedBugs; }
            set { _fixedBugs = value; }
        }

        public List<FeatureInfo> AddedFeatures
        {
            get { return _addedFeatures; }
            set { _addedFeatures = value; }
        }
        #endregion

        #region Private Variables
        private string _version;
        private DateTime _releaseDate;
        private List<BugFixInfo> _fixedBugs = new List<BugFixInfo>();
        private List<FeatureInfo> _addedFeatures = new List<FeatureInfo>();
        #endregion
    }
}
