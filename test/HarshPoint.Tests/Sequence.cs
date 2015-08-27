using Moq;
using Moq.Language;
using Moq.Language.Flow;
using System;
using System.Linq.Expressions;
using System.Runtime.InteropServices;
using Xunit;

namespace HarshPoint.Tests
{
    public class Sequence : IDisposable
    {
        private Int32 sequenceStep;
        private Int32 sequenceLength;

        public ISetup<TMock> SetupInSequence<TMock>(
            Mock<TMock> mock,
            Expression<Action<TMock>> expression
        )
            where TMock : class
            => SetupInSequenceCore(
                mock,
                m => m.Setup(expression),
                (s, c) => s.Callback(c)
            );

        public ISetup<TMock, TResult> SetupInSequence<TMock, TResult>(
            Mock<TMock> mock,
            Expression<Func<TMock, TResult>> expression
        )
            where TMock : class
            => SetupInSequenceCore(
                mock,
                m => m.Setup(expression),
                (s, c) => s.Callback(c)
            );

        private TSetup SetupInSequenceCore<TMock, TSetup>(
            Mock<TMock> mock,
            Func<ISetupConditionResult<TMock>, TSetup> createSetup,
            Action<TSetup, Action> setCallback
        )
            where TMock : class
        {
            var expectationPosition = sequenceLength++;

            var setup = createSetup(
                mock.When(() => expectationPosition == sequenceStep)
            );

            setCallback(setup, () => sequenceStep++);

            return setup;
        }

        public void VerifyFinished()
        {
            Assert.True(
                sequenceLength == sequenceStep,
                "Sequence not completed."
            );
        }

        private static Boolean IsInException()
            => Marshal.GetExceptionPointers() != IntPtr.Zero
                || Marshal.GetExceptionCode() != 0;

        void IDisposable.Dispose()
        {
            if (!IsInException())
            {
                VerifyFinished();
            }
        }
    }

    public static class SequenceExtensions
    {
        public static ISetup<TMock> SetupInSequence<TMock>(
            this Mock<TMock> mock,
            Sequence sequence,
            Expression<Action<TMock>> expression
        )
            where TMock : class
        {
            if (sequence == null)
            {
                throw Logger.Fatal.ArgumentNull(nameof(sequence));
            }

            return sequence.SetupInSequence(mock, expression);
        }

        public static ISetup<TMock, TResult> SetupInSequence<TMock, TResult>(
            this Mock<TMock> mock,
            Sequence sequence,
            Expression<Func<TMock, TResult>> expression
        )
            where TMock : class
        {
            if (sequence == null)
            {
                throw Logger.Fatal.ArgumentNull(nameof(sequence));
            }

            return sequence.SetupInSequence(mock, expression);
        }

        private static readonly HarshLogger Logger
            = HarshLog.ForContext(typeof(MockSequenceHelper));
    }
}
