﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace MailCheck.AggregateReport.DomainDateProviderSubdomain.Dao {
    using System;
    
    
    /// <summary>
    ///   A strongly-typed resource class, for looking up localized strings, etc.
    /// </summary>
    // This class was auto-generated by the StronglyTypedResourceBuilder
    // class via a tool like ResGen or Visual Studio.
    // To add or remove a member, edit your .ResX file then rerun ResGen
    // with the /str option, or rebuild your VS project.
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "16.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    internal class DateDomainProviderSubdomainAggregatorDaoResources {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal DateDomainProviderSubdomainAggregatorDaoResources() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("MailCheck.AggregateReport.DomainDateProviderSubdomain.Dao.DateDomainProviderSubdo" +
                            "mainAggregatorDaoResources", typeof(DateDomainProviderSubdomainAggregatorDaoResources).Assembly);
                    resourceMan = temp;
                }
                return resourceMan;
            }
        }
        
        /// <summary>
        ///   Overrides the current thread's CurrentUICulture property for all
        ///   resource lookups using this strongly typed resource class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Globalization.CultureInfo Culture {
            get {
                return resourceCulture;
            }
            set {
                resourceCulture = value;
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to 
        ///spf_pass_dkim_pass_none = spf_pass_dkim_pass_none + VALUES(spf_pass_dkim_pass_none),
        ///spf_pass_dkim_fail_none = spf_pass_dkim_fail_none + VALUES(spf_pass_dkim_fail_none),
        ///spf_fail_dkim_pass_none = spf_fail_dkim_pass_none + VALUES(spf_fail_dkim_pass_none),
        ///spf_fail_dkim_fail_none = spf_fail_dkim_fail_none + VALUES(spf_fail_dkim_fail_none),
        ///spf_pass_dkim_pass_quarantine = spf_pass_dkim_pass_quarantine + VALUES(spf_pass_dkim_pass_quarantine),
        ///spf_pass_dkim_fail_quarantine = spf_pass_dkim_fail_quarantine  [rest of string was truncated]&quot;;.
        /// </summary>
        internal static string UpdateStatements {
            get {
                return ResourceManager.GetString("UpdateStatements", resourceCulture);
            }
        }
    }
}
