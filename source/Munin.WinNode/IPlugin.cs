﻿using Munin.WinNode.Commands;

namespace Munin.WinNode
{
    /// <summary>
    /// Interface defining characteristics of plugins for the Munin Node
    /// </summary>
    public interface IPlugin
    {
        /// <summary>
        /// Gets the name of the plugin. This value is used with the <see cref="FetchCommand"/>
        /// and <see cref="ConfigCommand"/>
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Returns <c>true</c> if this plugin applies to the system that the node is hosted on.
        /// </summary>
        bool IsApplicable { get; }

        /// <summary>
        /// Returns the configuration for the plugin from a <see cref="ConfigCommand"/> request.
        /// </summary>
        /// <returns></returns>
        string GetConfiguration();

        /// <summary>
        /// Returns the values of the plugin from a <see cref="FetchCommand"/> request.
        /// </summary>
        /// <returns></returns>
        string GetValues();
    }
}
