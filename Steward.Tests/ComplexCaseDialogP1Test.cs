using Microsoft.Bot.Builder.Dialogs;
using System.Threading.Tasks;
// <copyright file="ComplexCaseDialogP1Test.cs">Copyright ©  2016</copyright>

using System;
using Microsoft.Pex.Framework;
using Microsoft.Pex.Framework.Validation;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Steward.Dialogs;

namespace Steward.Dialogs.Tests
{
    /// <summary>This class contains parameterized unit tests for ComplexCaseDialogP1</summary>
    [TestClass]
    [PexClass(typeof(ComplexCaseDialogP1))]
    [PexAllowedExceptionFromTypeUnderTest(typeof(ArgumentException), AcceptExceptionSubtypes = true)]
    [PexAllowedExceptionFromTypeUnderTest(typeof(InvalidOperationException))]
    public partial class ComplexCaseDialogP1Test
    {

        /// <summary>Test stub for StartAsync(IDialogContext)</summary>
        [PexMethod]
        public Task StartAsyncTest(IDialogContext context)
        {
            Task result = ComplexCaseDialogP1.StartAsync(context);
            return result;
            // TODO: add assertions to method ComplexCaseDialogP1Test.StartAsyncTest(IDialogContext)
        }
    }
}
