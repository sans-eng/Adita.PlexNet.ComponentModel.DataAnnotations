﻿using Adita.PlexNet.ComponentModel.DataAnnotations.DataAnnotations;
using Adita.PlexNet.ComponentModel.DataAnnotations.Test.Models;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;

namespace Adita.PlexNet.ComponentModel.DataAnnotations.Test.DataAnnotations
{
    [TestClass]
    public class CompareRangeAttributeTest
    {
        public int MinRange { get; } = 10;
        public int MaxRange { get; } = 20;
        public int InvalidMaxRange { get; } = 5;
        public DummyType InvalidMinRangeType { get; } = new();
        public DummyType InvalidMaxRangeType { get; } = new();


        [TestMethod]
        public void CanValidate()
        {
            CompareRangeAttribute attribute = new(nameof(MinRange), nameof(MaxRange)) {ErrorMessage = "Range must be between {0} to {1}" };

            var result1 = attribute.GetValidationResult(15, new ValidationContext(this));
            Assert.IsTrue(result1 == ValidationResult.Success);

            var result2 = attribute.GetValidationResult(50, new ValidationContext(this));
            if(result2 != null)
            {
                Debug.WriteLine(result2.ErrorMessage);
            }
            Assert.IsFalse(result2 == ValidationResult.Success);
        }

        [TestMethod]
        public void CanThrowNotSupportedException()
        {
            CompareRangeAttribute attribute1 = new(nameof(InvalidMinRangeType), nameof(InvalidMaxRangeType));
            Assert.ThrowsException<NotSupportedException>(() => attribute1.GetValidationResult(22, new ValidationContext(this)));

            CompareRangeAttribute attribute2 = new(nameof(MinRange), nameof(InvalidMaxRangeType));
            Assert.ThrowsException<NotSupportedException>(() => attribute2.GetValidationResult(22, new ValidationContext(this)));

            CompareRangeAttribute attribute3 = new(nameof(InvalidMinRangeType), nameof(MaxRange));
            Assert.ThrowsException<NotSupportedException>(() => attribute3.GetValidationResult(22, new ValidationContext(this)));

            CompareRangeAttribute attribute4 = new(nameof(MinRange), nameof(MaxRange));
            Assert.ThrowsException<NotSupportedException>(() => attribute4.GetValidationResult(new DummyType(), new ValidationContext(this)));
        }

        [TestMethod]
        public void CanThrowInvalidOperationException()
        {
            CompareRangeAttribute attribute1 = new("MissingProperty", nameof(MaxRange));
            Assert.ThrowsException<InvalidOperationException>(() => attribute1.GetValidationResult(22, new ValidationContext(this)));

            CompareRangeAttribute attribute2 = new(nameof(MinRange), "MissingProperty");
            Assert.ThrowsException<InvalidOperationException>(() => attribute2.GetValidationResult(22, new ValidationContext(this)));

            CompareRangeAttribute attribute3 = new(nameof(MinRange), nameof(InvalidMaxRange));
            Assert.ThrowsException<InvalidOperationException>(() => attribute3.GetValidationResult(22, new ValidationContext(this)));
        }
    }
}
