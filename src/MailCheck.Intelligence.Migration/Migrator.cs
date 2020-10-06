using MailCheck.Common.Data.Migration.Factory;
using MailCheck.Common.Data.Migration.UpgradeEngine;

namespace MailCheck.Intelligence.Migration
{
    public class Migrator
    {
        public static int Main()
        {
            IUpgradeEngine upgradeEngine = UpgradeEngineFactory.Create(UpgradeEngineFactory.DatabaseType.PostgreSql);

            return upgradeEngine.PerformUpgrade().Result;
        }
    }
}
