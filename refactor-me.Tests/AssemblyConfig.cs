using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace refactor_me.Tests
{
    [TestClass]
    public class AssemblyConfig
    {
        [AssemblyInitialize]
        public static void AssemblyInitialize(TestContext testContext)
        {
            //  Define data directory
            AppDomain.CurrentDomain.SetData("DataDirectory", System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Database"));
        }
    }
}
