﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WtsTool.CommandOptions;

namespace WtsTool
{
    public static class RunCommand
    {
        public static int Package(PackageOptions options, TextWriter output, TextWriter error)
        {
            var action = InferPackageAction(options);

            switch (action)
            {
                case PackageTask.Create:
                    PackageWorker.Create(options.CreateNew, options.OutFile, options.CertThumbprint, output, error);
                    break;
                case PackageTask.Extract:
                    PackageWorker.Extract(options.Extract, options.DestionationDir, output, error);
                    break;
                case PackageTask.Info:
                    PackageWorker.GetInfo(options.Info, output, error);
                    break;
                default:
                    error.WriteLine($"Unexpected action '{action.ToString()}'");
                    return 1;
            }

            return 0;
        }

        public static int Publish(RemoteSourcePublishOptions publishOpts, TextWriter output, TextWriter error)
        {
            if (publishOpts.Config)
            {
                RemoteSourceWorker.PublishConfig(publishOpts, output, error);
            }
            else
            {
                RemoteSourceWorker.PublishContent(publishOpts, output, error);
            }

            return 0;
        }

        public static int Download(RemoteSourceDownloadOptions downloadOpts, TextWriter output, TextWriter error)
        {
            if (downloadOpts.Config)
            {
                RemoteSourceWorker.DownloadConfig(downloadOpts, output, error);
            }
            else
            {
                RemoteSourceWorker.DownloadContent(downloadOpts, output, error);
            }

            return 0;
        }

        public static int List(RemoteSourceListOptions listOpts, TextWriter output, TextWriter error)
        {
            if (listOpts.Summary && !listOpts.Detailed)
            {
                RemoteSourceWorker.ListSummaryInfo(listOpts, output, error);
            }

            if (listOpts.Detailed)
            {
                RemoteSourceWorker.ListDetailedVersions(listOpts, output, error);
            }

            return 0;
        }

        private static object InferPackageAction(PackageOptions options)
        {
            if (!string.IsNullOrEmpty(options.Info))
            {
                return PackageTask.Info;
            }

            if (!string.IsNullOrEmpty(options.CreateNew))
            {
                return PackageTask.Create;
            }

            if (!string.IsNullOrEmpty(options.Extract))
            {
                return PackageTask.Extract;
            }

            return PackageTask.None;
        }
    }
}
