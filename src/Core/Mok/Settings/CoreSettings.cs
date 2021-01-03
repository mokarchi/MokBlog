using System;
using System.Collections.Generic;
using System.Text;

namespace Mok.Settings
{
    /// <summary>
    /// Core settings for the site.
    /// </summary>
    public class CoreSettings : ISettings
    {
        /// <summary>
        /// Title of the blog. Default "Fanray".
        /// </summary>
        public string Title { get; set; } = "Mok";

        /// <summary>
        /// Default theme "Clarity".
        /// </summary>
        /// <remarks>
        /// This is the theme's folder name.
        /// </remarks>
        public string Theme { get; set; } = "Clarity";

        /// <summary>
        /// Gets or sets the timezone id for the site. Default "UTC".
        /// </summary>
        /// <remarks>
        /// To learn more about timezone id <see cref="System.TimeZoneInfo.Id"/> and <see cref="http://stackoverflow.com/a/7908482/32240"/>
        /// </remarks>
        public string TimeZoneId { get; set; } = "UTC";

        /// <summary>
        /// Has the setup happened, true will let system skip setup, false will forward to setup page.
        /// </summary>
        public bool SetupDone { get; set; } = false;
    }
}
