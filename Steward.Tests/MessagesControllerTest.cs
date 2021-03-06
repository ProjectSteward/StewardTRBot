// <copyright file="MessagesControllerTest.cs">Copyright ©  2016</copyright>
using System;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Bot.Connector;
using Microsoft.Pex.Framework;
using Microsoft.Pex.Framework.Validation;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Steward.Controllers;

namespace Steward.Controllers.Tests
{
    /// <summary>This class contains parameterized unit tests for MessagesController</summary>
    [PexClass(typeof(MessagesController))]
    [PexAllowedExceptionFromTypeUnderTest(typeof(InvalidOperationException))]
    [PexAllowedExceptionFromTypeUnderTest(typeof(ArgumentException), AcceptExceptionSubtypes = true)]
    [TestClass]
    public partial class MessagesControllerTest
    {
        /// <summary>Test stub for Post(Activity)</summary>
        [PexMethod]
        public Task<HttpResponseMessage> PostTest(
            [PexAssumeUnderTest]MessagesController target,
            Activity activity
        )
        {
            Task<HttpResponseMessage> result = target.Post(activity);
            return result;
            // TODO: add assertions to method MessagesControllerTest.PostTest(MessagesController, Activity)
        }
    }
}
