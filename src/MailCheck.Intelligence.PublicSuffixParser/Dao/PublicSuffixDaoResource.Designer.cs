﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace MailCheck.Intelligence.PublicSuffixParser.Dao {
    using System;
    
    
    /// <summary>
    ///   A strongly-typed resource class, for looking up localized strings, etc.
    /// </summary>
    // This class was auto-generated by the StronglyTypedResourceBuilder
    // class via a tool like ResGen or Visual Studio.
    // To add or remove a member, edit your .ResX file then rerun ResGen
    // with the /str option, or rebuild your VS project.
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "15.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    internal class PublicSuffixDaoResource {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal PublicSuffixDaoResource() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("MailCheck.Intelligence.PublicSuffixParser.Dao.PublicSuffixDaoResource", typeof(PublicSuffixDaoResource).Assembly);
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
        ///   Looks up a localized string similar to INSERT INTO public.public_suffix(suffix) VALUES.
        /// </summary>
        internal static string InsertPublicSuffix {
            get {
                return ResourceManager.GetString("InsertPublicSuffix", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to ON CONFLICT (suffix) DO NOTHING;.
        /// </summary>
        internal static string InsertPublicSuffixOnConflict {
            get {
                return ResourceManager.GetString("InsertPublicSuffixOnConflict", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to (@suffix_{0}).
        /// </summary>
        internal static string InsertPublicSuffixValueFormatString {
            get {
                return ResourceManager.GetString("InsertPublicSuffixValueFormatString", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to REFRESH MATERIALIZED VIEW CONCURRENTLY public_suffix_mv;.
        /// </summary>
        internal static string RefreshMaterializedView {
            get {
                return ResourceManager.GetString("RefreshMaterializedView", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to DELETE FROM public_suffix;.
        /// </summary>
        internal static string Truncate {
            get {
                return ResourceManager.GetString("Truncate", resourceCulture);
            }
        }
    }
}