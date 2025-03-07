//MIT License

//Copyright (c) 2025 Adita

//Permission is hereby granted, free of charge, to any person obtaining a copy
//of this software and associated documentation files (the "Software"), to deal
//in the Software without restriction, including without limitation the rights
//to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
//copies of the Software, and to permit persons to whom the Software is
//furnished to do so, subject to the following conditions:

//The above copyright notice and this permission notice shall be included in all
//copies or substantial portions of the Software.

//THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
//FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
//AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
//LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
//OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
//SOFTWARE.

using System.ComponentModel.DataAnnotations;

namespace Adita.PlexNet.ComponentModel.DataAnnotations.DataAnnotations
{
    /// <summary>
    /// Provides an attribute that compares the range of a property of type <see cref="IComparable"/> 
    /// within a minimum and maximum from two other properties of type <see cref="IComparable"/>.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter, AllowMultiple = false, Inherited = true)]
    public sealed class CompareRangeAttribute : ValidationAttribute
    {
        #region Constructors
        /// <summary>
        /// Initialize a new instance of <see cref="CompareRangeAttribute"/> using specified <paramref name="minPropertyName"/>, <paramref name="maxPropertyName"/>.
        /// </summary>
        /// <param name="minPropertyName">The property name to get the minimum value.</param>
        /// <param name="maxPropertyName">The property name to get the maximum value.</param>
        public CompareRangeAttribute(string minPropertyName, string maxPropertyName)
        {
            MinPropertyName = minPropertyName;
            MaxPropertyName = maxPropertyName;
        }
        #endregion Constructors

        #region Public properties
        /// <summary>
        /// Gets the property name for the minimum value.
        /// </summary>
        public string MinPropertyName
        {
            get;
        }
        /// <summary>
        /// Gets the property name for the maximum value.
        /// </summary>
        public string MaxPropertyName
        {
            get;
        }
        /// <inheritdoc/>
        public override bool RequiresValidationContext => true;
        #endregion Public properties

        #region Protected methods
        /// <summary>
        /// Validates the specified <paramref name="value"/> with respect to the current <see cref="CompareRangeAttribute"/>.
        /// </summary>
        /// <param name="value">The value to validate.</param>
        /// <param name="validationContext">The context information about the validation operation.</param>
        /// <returns>An instance of the <see cref="ValidationResult" /> class.</returns>
        /// <exception cref="InvalidOperationException">Property that has <see cref="MinPropertyName"/> is not found.</exception>
        /// <exception cref="InvalidOperationException">Property that has <see cref="MaxPropertyName"/> is not found.</exception>
        /// <exception cref="NotSupportedException">Property value of <see cref="MinPropertyName"/> is not <see cref="IComparable"/>.</exception>
        /// <exception cref="NotSupportedException">Property value of <see cref="MaxPropertyName"/> is not <see cref="IComparable"/>.</exception>
        /// <exception cref="InvalidOperationException">Property value of <see cref="MinPropertyName"/> is greater than property value of <see cref="MaxPropertyName"/>.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="validationContext"/> is <see langword="null"/>.</exception>
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            ArgumentNullException.ThrowIfNull(validationContext);

            var instance = validationContext.ObjectInstance;

            var minPropertyInfo = instance.GetType().GetProperty(MinPropertyName) ?? throw new InvalidOperationException($"{MinPropertyName} property not found.");
            var maxPropertyInfo = instance.GetType().GetProperty(MaxPropertyName) ?? throw new InvalidOperationException($"{MaxPropertyName} property not found.");

            if (minPropertyInfo.GetValue(instance) is not IComparable minValue)
            {
                throw new NotSupportedException($"The type of '{MinPropertyName}' property does not implements {nameof(IComparable)}.");
            }

            if (maxPropertyInfo.GetValue(instance) is not IComparable maxValue)
            {
                throw new NotSupportedException($"The type of '{MaxPropertyName}' property does not implements {nameof(IComparable)}.");
            }

            if (minValue.CompareTo(maxValue) > 0)
            {
                throw new InvalidOperationException($"The value of '{MinPropertyName}' property cannot be greater than value of {MaxPropertyName} property.");
            }

            //validates
            if (value is not IComparable comparableValue)
            {
                throw new NotSupportedException($"The type of '{validationContext.MemberName}' does not implements {nameof(IComparable)}.");
            }

            if (comparableValue.CompareTo(minValue) >= 0 && comparableValue.CompareTo(maxValue) <= 0)
            {
                return ValidationResult.Success;
            }

            return new ValidationResult(string.Format(ErrorMessageString, minValue, maxValue));
        }
        #endregion Protected methods
    }
}
