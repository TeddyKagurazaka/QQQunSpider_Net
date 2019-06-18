using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace qqQunUnitTest
{
    [TestClass]
    public class qunTest
    {
        [TestMethod]
        public void SearchForQun()
        {
            var qunModule = getQQQunModule();
            var result = qunModule.RequestQun("github");
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void SearchForUserInfo()
        {
            var qunModule = getQQQunModule();
            var result = qunModule.RequestUserJoinedGroup(qqQunSupport.qqQunModule.UserGroupType.Join);
            var result2 = qunModule.RequestUserJoinedGroup(qqQunSupport.qqQunModule.UserGroupType.Manage);
            Assert.IsNotNull(result);
            Assert.IsNotNull(result2);
        }

        public qqQunSupport.qqQunModule getQQQunModule()
        {
            var AuthModule = new qqQunSupport.authModule();
            AuthModule.startAuth();

            Assert.IsNotNull(AuthModule.skey, "sKey为空!");
            Assert.IsFalse(AuthModule.cookieContainer.Count == 0, "Cookie 为空!");

            var QunModule = new qqQunSupport.qqQunModule(AuthModule);
            return QunModule;
        }
    }
}
