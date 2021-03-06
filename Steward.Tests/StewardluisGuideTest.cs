using Microsoft.Bot.Builder.Luis.Models;
using Microsoft.Bot.Builder.Dialogs;
using System.Threading.Tasks;
// <copyright file="StewardluisGuideTest.cs">Copyright ©  2016</copyright>

using System;
using Microsoft.Pex.Framework;
using Microsoft.Pex.Framework.Validation;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Steward.Ai;

namespace Steward.Ai.Tests
{
    /// <summary>This class contains parameterized unit tests for StewardluisGuide</summary>
    [TestClass]
    [PexClass(typeof(StewardluisGuide))]
    [PexAllowedExceptionFromTypeUnderTest(typeof(ArgumentException), AcceptExceptionSubtypes = true)]
    [PexAllowedExceptionFromTypeUnderTest(typeof(InvalidOperationException))]
    public partial class StewardluisGuideTest
    {

        /// <summary>Test stub for GetGreetings(IDialogContext, LuisResult)</summary>
        [PexMethod]
        public Task GetGreetingsTest(
            [PexAssumeUnderTest]StewardluisGuide target,
            IDialogContext context,
            LuisResult result
        )
        {
            Task result01 = target.GetGreetings(context, result);
            return result01;
            // TODO: add assertions to method StewardluisGuideTest.GetGreetingsTest(StewardluisGuide, IDialogContext, LuisResult)
        }

        /// <summary>Test stub for AskComplexResponse(IDialogContext, LuisResult)</summary>
        [PexMethod]
        public Task AskComplexResponseTest(
            [PexAssumeUnderTest]StewardluisGuide target,
            IDialogContext context,
            LuisResult result
        )
        {
            Task result01 = target.AskComplexResponse(context, result);
            return result01;
            // TODO: add assertions to method StewardluisGuideTest.AskComplexResponseTest(StewardluisGuide, IDialogContext, LuisResult)
        }

        /// <summary>Test stub for NoIntent(IDialogContext, LuisResult)</summary>
        [PexMethod]
        public Task NoIntentTest(
            [PexAssumeUnderTest]StewardluisGuide target,
            IDialogContext context,
            LuisResult result
        )
        {
            Task result01 = target.NoIntent(context, result);
            return result01;
            // TODO: add assertions to method StewardluisGuideTest.NoIntentTest(StewardluisGuide, IDialogContext, LuisResult)
        }
    }
}
