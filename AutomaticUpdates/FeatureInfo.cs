using System;
using System.Collections.Generic;
using System.Text;

namespace AutomaticUpdates
{
    /// <summary>
    /// Describes a feature that was added to the software
    /// </summary>
    [Serializable]
    public class FeatureInfo
    {
        public FeatureInfo()
        {
        }

        /// <summary>
        /// Creates a new feature info object with the specified impact and description
        /// </summary>
        /// <param name="impact">The impact of the feature</param>
        /// <param name="description">The description of the feature</param>
        public FeatureInfo(FeatureImpact impact, string description)
        {
            _impact = impact;
            _description = description;
        }

        /// <summary>
        /// Get the impact of the feature on the system
        /// </summary>
        public FeatureImpact Impact
        {
            get { return _impact; }
            set { _impact = value; }
        }

        /// <summary>
        /// Get the description of the feature
        /// </summary>
        public string Description
        {
            get { return _description; }
            set { _description = value; }
        }

        private FeatureImpact _impact;
        private string _description;
    }

    /// <summary>
    /// Describes the impart of the added feature
    /// </summary>
    public enum FeatureImpact
    {
        None = 0,

        Minor = 1,

        Major = 2,
    }
}
