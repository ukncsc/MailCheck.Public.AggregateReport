﻿using MailCheck.Common.Data.Migration.Factory;
using MailCheck.Common.Data.Migration.UpgradeEngine;

namespace MailCheck.AggregateReport.Migration
{
    public class Migrator
    {
        public static int Main()
        {
            IUpgradeEngine upgradeEngine = UpgradeEngineFactory.Create();

            return upgradeEngine.PerformUpgrade().Result;
        }
    }
}
