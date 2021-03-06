﻿using JetBrains.Annotations;
using Nuke.Common.IO;

namespace Models
{
    public class CommonDir
    {
        public CommonDir()
        {
        }

        private CommonDir(string name)
        {

            Name = name;
        }

        public string Name { get; set; }
        public static CommonDir Build { get; } = new CommonDir("build");
        public static CommonDir Merge { get; } = new CommonDir("merge");
        public static CommonDir Nuget { get; } = new CommonDir("nuget");
        public static CommonDir Ready { get; } = new CommonDir("ready");
        public static CommonDir Zip { get; } = new CommonDir("zip");



        public static AbsolutePath operator /(
            AbsolutePath path1,
            [CanBeNull] CommonDir commonDir)
        {
            return path1 / (commonDir?.Name ?? string.Empty);
        }


    }
}