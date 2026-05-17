// Copyright 2026 UNN-IASR
using Stateless;

namespace BugPro
{
    public enum BugState { New, Analysis, Fix, Closed, Reopened, Returned }
    public enum BugTrigger { Analyze, AssignToFix, Reject, NeedMoreInfo, VerifyFixed, FixRejected, Reopen, ProvideMoreInfo }

    public class Bug
    {
        private readonly StateMachine<BugState, BugTrigger> _machine;
        private BugState _state;
        public string Title { get; }
        public string Description { get; }
        public string? AdditionalInfo { get; private set; }

        public Bug(string title, string description)
        {
            if (string.IsNullOrWhiteSpace(title))
                throw new ArgumentException("Title cannot be empty", nameof(title));
            
            Title = title;
            Description = description;
            _state = BugState.New;
            AdditionalInfo = null;
            
            _machine = new StateMachine<BugState, BugTrigger>(() => _state, s => _state = s);
            ConfigureTransitions();
        }

        private void ConfigureTransitions()
        {
            _machine.Configure(BugState.New)
                .OnEntry(() => Console.WriteLine($"[BUG] Entered state: New"))
                .Permit(BugTrigger.Analyze, BugState.Analysis);

            _machine.Configure(BugState.Analysis)
                .OnEntry(() => Console.WriteLine($"[BUG] Entered state: Analysis"))
                .OnExit(() => Console.WriteLine($"[BUG] Exited state: Analysis"))
                .Permit(BugTrigger.AssignToFix, BugState.Fix)
                .Permit(BugTrigger.Reject, BugState.Closed)
                .Permit(BugTrigger.NeedMoreInfo, BugState.Returned);

            _machine.Configure(BugState.Fix)
                .OnEntry(() => Console.WriteLine($"[BUG] Entered state: Fix"))
                .Permit(BugTrigger.VerifyFixed, BugState.Closed)
                .Permit(BugTrigger.FixRejected, BugState.Returned);

            _machine.Configure(BugState.Closed)
                .OnEntry(() => Console.WriteLine($"[BUG] Entered state: Closed"))
                .Permit(BugTrigger.Reopen, BugState.Reopened);

            _machine.Configure(BugState.Reopened)
                .OnEntry(() => Console.WriteLine($"[BUG] Entered state: Reopened"))
                .Permit(BugTrigger.Analyze, BugState.Analysis);

            _machine.Configure(BugState.Returned)
                .OnEntry(() => Console.WriteLine($"[BUG] Entered state: Returned"))
                .Permit(BugTrigger.ProvideMoreInfo, BugState.Analysis);
        }

        public void Fire(BugTrigger trigger)
        {
            if (!_machine.CanFire(trigger))
                throw new InvalidOperationException($"Cannot fire {trigger} in state {_state}");
            
            _machine.Fire(trigger);
        }

        public void SetAdditionalInfo(string info)
        {
            AdditionalInfo = info;
        }

        public BugState GetCurrentState() => _state;
        public bool CanFire(BugTrigger trigger) => _machine.CanFire(trigger);
        
        public override string ToString()
        {
            return $"Bug: {Title} | State: {_state} | Info: {AdditionalInfo ?? "None"}";
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("=== Bug Workflow Demo ===");
            var bug = new Bug("Critical Bug", "App crashes on startup");
            Console.WriteLine(bug);
            
            bug.Fire(BugTrigger.Analyze);
            bug.Fire(BugTrigger.AssignToFix);
            bug.Fire(BugTrigger.VerifyFixed);
            
            Console.WriteLine($"Final state: {bug.GetCurrentState()}");
        }
    }
}
