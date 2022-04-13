﻿using System.Collections.Generic;

using NUnit.Framework;

namespace Stott.Optimizely.Csp.Test.TestCases
{
    public static class CommonTestCases
    {
        public static IEnumerable<TestCaseData> EmptyNullOrWhitespaceStrings
        {
            get
            {
                yield return new TestCaseData((string)null);
                yield return new TestCaseData(string.Empty);
                yield return new TestCaseData(" ");
            }
        }
    }
}
