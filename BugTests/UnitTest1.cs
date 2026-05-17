// Copyright 2026 UNN-IASR
using Microsoft.VisualStudio.TestTools.UnitTesting;
using BugPro;

namespace BugTests
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void Bug_CreatedInNewState()
        {
            var bug = new Bug("Test", "Description");
            Assert.AreEqual(BugState.New, bug.GetCurrentState());
        }

        [TestMethod]
        public void New_To_Analysis()
        {
            var bug = new Bug("T", "D");
            bug.Fire(BugTrigger.Analyze);
            Assert.AreEqual(BugState.Analysis, bug.GetCurrentState());
        }

        [TestMethod]
        public void Analysis_To_Fix()
        {
            var bug = new Bug("T", "D");
            bug.Fire(BugTrigger.Analyze);
            bug.Fire(BugTrigger.AssignToFix);
            Assert.AreEqual(BugState.Fix, bug.GetCurrentState());
        }

        [TestMethod]
        public void Analysis_To_Closed()
        {
            var bug = new Bug("T", "D");
            bug.Fire(BugTrigger.Analyze);
            bug.Fire(BugTrigger.Reject);
            Assert.AreEqual(BugState.Closed, bug.GetCurrentState());
        }

        [TestMethod]
        public void Analysis_To_Returned()
        {
            var bug = new Bug("T", "D");
            bug.Fire(BugTrigger.Analyze);
            bug.Fire(BugTrigger.NeedMoreInfo);
            Assert.AreEqual(BugState.Returned, bug.GetCurrentState());
        }

        [TestMethod]
        public void Fix_To_Closed()
        {
            var bug = new Bug("T", "D");
            bug.Fire(BugTrigger.Analyze);
            bug.Fire(BugTrigger.AssignToFix);
            bug.Fire(BugTrigger.VerifyFixed);
            Assert.AreEqual(BugState.Closed, bug.GetCurrentState());
        }

        [TestMethod]
        public void Fix_To_Returned()
        {
            var bug = new Bug("T", "D");
            bug.Fire(BugTrigger.Analyze);
            bug.Fire(BugTrigger.AssignToFix);
            bug.Fire(BugTrigger.FixRejected);
            Assert.AreEqual(BugState.Returned, bug.GetCurrentState());
        }

        [TestMethod]
        public void Closed_To_Reopened()
        {
            var bug = new Bug("T", "D");
            bug.Fire(BugTrigger.Analyze);
            bug.Fire(BugTrigger.Reject);
            bug.Fire(BugTrigger.Reopen);
            Assert.AreEqual(BugState.Reopened, bug.GetCurrentState());
        }

        [TestMethod]
        public void Reopened_To_Analysis()
        {
            var bug = new Bug("T", "D");
            bug.Fire(BugTrigger.Analyze);
            bug.Fire(BugTrigger.Reject);
            bug.Fire(BugTrigger.Reopen);
            bug.Fire(BugTrigger.Analyze);
            Assert.AreEqual(BugState.Analysis, bug.GetCurrentState());
        }

        [TestMethod]
        public void Returned_To_Analysis()
        {
            var bug = new Bug("T", "D");
            bug.Fire(BugTrigger.Analyze);
            bug.Fire(BugTrigger.NeedMoreInfo);
            bug.Fire(BugTrigger.ProvideMoreInfo);
            Assert.AreEqual(BugState.Analysis, bug.GetCurrentState());
        }

        [TestMethod]
        public void CannotFireInvalidTransition()
        {
            var bug = new Bug("T", "D");
            Assert.IsFalse(bug.CanFire(BugTrigger.VerifyFixed));
        }

        [TestMethod]
        public void FullWorkflow_HappyPath()
        {
            var bug = new Bug("Bug", "Desc");
            bug.Fire(BugTrigger.Analyze);
            bug.Fire(BugTrigger.AssignToFix);
            bug.Fire(BugTrigger.VerifyFixed);
            Assert.AreEqual(BugState.Closed, bug.GetCurrentState());
        }
    }
}
