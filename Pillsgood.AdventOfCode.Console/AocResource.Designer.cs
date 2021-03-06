﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Pillsgood.AdventOfCode.Console {
    using System;
    
    
    /// <summary>
    ///   A strongly-typed resource class, for looking up localized strings, etc.
    /// </summary>
    // This class was auto-generated by the StronglyTypedResourceBuilder
    // class via a tool like ResGen or Visual Studio.
    // To add or remove a member, edit your .ResX file then rerun ResGen
    // with the /str option, or rebuild your VS project.
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "4.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    internal class AocResource {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal AocResource() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("Pillsgood.AdventOfCode.Console.AocResource", typeof(AocResource).Assembly);
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
        ///   Looks up a localized string similar to -------------- {0} -.
        /// </summary>
        internal static string Header {
            get {
                return ResourceManager.GetString("Header", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to -- {0} --------------.
        /// </summary>
        internal static string HeaderLong {
            get {
                return ResourceManager.GetString("HeaderLong", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Advent of Code.
        /// </summary>
        internal static string Title {
            get {
                return ResourceManager.GetString("Title", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to int y={0};.
        /// </summary>
        internal static string YearFormat0 {
            get {
                return ResourceManager.GetString("YearFormat0", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to /*{0}*/.
        /// </summary>
        internal static string YearFormat1 {
            get {
                return ResourceManager.GetString("YearFormat1", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to 0x0000|{0}.
        /// </summary>
        internal static string YearFormat2 {
            get {
                return ResourceManager.GetString("YearFormat2", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to sub y{{{0}}}.
        /// </summary>
        internal static string YearFormat3 {
            get {
                return ResourceManager.GetString("YearFormat3", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to {{:year {0}}}.
        /// </summary>
        internal static string YearFormat4 {
            get {
                return ResourceManager.GetString("YearFormat4", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to {{year=&gt;{0}}}.
        /// </summary>
        internal static string YearFormat5 {
            get {
                return ResourceManager.GetString("YearFormat5", resourceCulture);
            }
        }
    }
}
