// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace Microsoft.WindowsAppSDK
{
    // Release information
    public class Release
    {
        public const ushort Major = 1;
        public const ushort Minor = 1;
        public const ushort Patch = 2;
        public const uint MajorMinor = 0x00010001;

        public const string Channel = "";

        public const string VersionTag = "";
        public const string VersionShortTag = "";

        public const string FormattedVersionTag = "";
        public const string FormattedVersionShortTag = "";
    }

    // Runtime information
    namespace Runtime
    {
        public class Identity
        {
            public const string Publisher = "CN=Microsoft Corporation, O=Microsoft Corporation, L=Redmond, S=Washington, C=US";
            public const string PublisherId = "8wekyb3d8bbwe";
        }

        public class Version
        {
            public const ushort Major = 1002;
            public const ushort Minor = 543;
            public const ushort Build = 1943;
            public const ushort Revision = 0;
            public const ulong UInt64 = 0x03EA021F07970000;
            public const string DotQuadString = "1002.543.1943.0";
        }

        namespace Packages
        {
            public class Framework
            {
                public const string PackageFamilyName = "Microsoft.WindowsAppRuntime.1.1_8wekyb3d8bbwe";
            }
            public class Main
            {
                public const string PackageFamilyName = "MicrosoftCorporationII.WinAppRuntime.Main.1.1_8wekyb3d8bbwe";
            }
            public class Singleton
            {
                public const string PackageFamilyName = "MicrosoftCorporationII.WinAppRuntime.Singleton_8wekyb3d8bbwe";
            }
            namespace DDLM
            {
                public class X86
                {
                    public const string PackageFamilyName = "Microsoft.WinAppRuntime.DDLM.1002.543.1943.0-x8_8wekyb3d8bbwe";
                }
                public class X64
                {
                    public const string PackageFamilyName = "Microsoft.WinAppRuntime.DDLM.1002.543.1943.0-x6_8wekyb3d8bbwe";
                }
                public class Arm64
                {
                    public const string PackageFamilyName = "Microsoft.WinAppRuntime.DDLM.1002.543.1943.0-a6_8wekyb3d8bbwe";
                }
            }
        }
    }
}
