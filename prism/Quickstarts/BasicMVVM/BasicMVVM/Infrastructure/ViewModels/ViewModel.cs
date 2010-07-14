//===================================================================================
// Microsoft patterns & practices
// Composite Application Guidance for Windows Presentation Foundation and Silverlight
//===================================================================================
// Copyright (c) Microsoft Corporation.  All rights reserved.
// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY
// OF ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT
// LIMITED TO THE IMPLIED WARRANTIES OF MERCHANTABILITY AND
// FITNESS FOR A PARTICULAR PURPOSE.
//===================================================================================
// The example companies, organizations, products, domain names,
// e-mail addresses, logos, people, places, and events depicted
// herein are fictitious.  No association with any real company,
// organization, product, domain name, email address, logo, person,
// places, or events is intended or should be inferred.
//===================================================================================
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace BasicMVVM.Infrastructure.ViewModels
{
    /// <summary>
    /// Base class for view models.
    /// </summary>
    /// <remarks>
    /// This class provides basic support for implementing the <see cref="INotifyPropertyChanged"/> and 
    /// <see cref="INotifyDataErrorInfo"/> interfaces.
    /// </remarks>
    public class ViewModel : INotifyPropertyChanged, INotifyDataErrorInfo
    {
        private readonly Dictionary<string, IEnumerable<string>> errors = new Dictionary<string, IEnumerable<string>>();
        private event EventHandler<DataErrorsChangedEventArgs> errorsChanged;

        /// <summary>
        /// Raised when a property on this object has a new value.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Occurs when the validation errors have changed for a property or for the entire object.
        /// </summary>
        event EventHandler<DataErrorsChangedEventArgs> INotifyDataErrorInfo.ErrorsChanged
        {
            [method: SuppressMessage("Microsoft.Design", "CA1033", Justification = "Doesn't need to access event methods.")]
            add { this.errorsChanged += value; }
            [method: SuppressMessage("Microsoft.Design", "CA1033", Justification = "Doesn't need to access event methods.")]
            remove { this.errorsChanged -= value; }
        }

        bool INotifyDataErrorInfo.HasErrors
        {
            get { return this.HasErrors; }
        }

        protected bool HasErrors
        {
            get { return this.errors.Any(); }
        }

        IEnumerable INotifyDataErrorInfo.GetErrors(string propertyName)
        {
            return this.GetErrors(propertyName);
        }

        protected virtual IEnumerable GetErrors(string propertyName)
        {
            IEnumerable<string> error;
            if (this.errors.TryGetValue(propertyName ?? string.Empty, out error))
            {
                return error;
            }

            return null;
        }

        protected virtual void SetError(string propertyName, string error)
        {
            this.SetErrors(propertyName, new List<string>() { error });
        }

        protected virtual void SetError<T>(Expression<Func<T>> propertyExpresssion, string error)
        {
            var propertyName = ExtractPropertyName(propertyExpresssion);
            this.SetError(propertyName, error);
        }

        protected virtual void ClearErrors(string propertyName)
        {
            this.SetErrors(propertyName, new List<string>());
        }

        protected virtual void ClearErrors<T>(Expression<Func<T>> propertyExpresssion)
        {
            var propertyName = ExtractPropertyName(propertyExpresssion);
            this.ClearErrors(propertyName);
        }

        protected virtual void SetErrors<T>(Expression<Func<T>> propertyExpresssion, IEnumerable<string> propertyErrors)
        {
            var propertyName = ExtractPropertyName(propertyExpresssion);
            this.SetErrors(propertyName, propertyErrors);
        }

        protected virtual void SetErrors(string propertyName, IEnumerable<string> propertyErrors)
        {
            if (propertyErrors.Any(error => error == null))
            {
                throw new ArgumentException("Errors messages should not be null", "propertyErrors");
            }

            var propertyNameKey = propertyName ?? string.Empty;

            IEnumerable<string> currentPropertyErrors;
            if (this.errors.TryGetValue(propertyNameKey, out currentPropertyErrors))
            {
                if (!AreErrorCollectionsEqual(currentPropertyErrors, propertyErrors))
                {
                    if (propertyErrors.Any())
                    {
                        this.errors[propertyNameKey] = propertyErrors;
                    }
                    else
                    {
                        this.errors.Remove(propertyNameKey);
                    }

                    this.RaiseErrorsChanged(propertyNameKey);
                }
            }
            else
            {
                if (propertyErrors.Any())
                {
                    this.errors[propertyNameKey] = propertyErrors;
                    this.RaiseErrorsChanged(propertyNameKey);
                }
            }
        }

        /// <summary>
        /// Raises this object's ErrorsChangedChanged event.
        /// </summary>
        /// <param name="propertyName">The property that has new errors.</param>
        protected virtual void RaiseErrorsChanged(string propertyName)
        {
            EventHandler<DataErrorsChangedEventArgs> handler = this.errorsChanged;
            if (handler != null)
            {
                handler(this, new DataErrorsChangedEventArgs(propertyName));
            }
        }

        /// <summary>
        /// Raises this object's ErrorsChangedChanged event.
        /// </summary>
        /// <typeparam name="T">The type of the property that has errors</typeparam>
        /// <param name="propertyExpresssion">A Lambda expression representing the property that has new errors.</param>
        protected virtual void RaiseErrorsChanged<T>(Expression<Func<T>> propertyExpresssion)
        {
            var propertyName = ExtractPropertyName(propertyExpresssion);
            this.RaiseErrorsChanged(propertyName);
        }

        /// <summary>
        /// Raises this object's PropertyChanged event.
        /// </summary>
        /// <param name="propertyName">The property that has a new value.</param>
        protected virtual void RaisePropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = this.PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        /// <summary>
        /// Raises this object's PropertyChanged event for each of the properties.
        /// </summary>
        /// <param name="propertyNames">The properties that have a new value.</param>
        protected void RaisePropertyChanged(params string[] propertyNames)
        {
            foreach (var name in propertyNames)
            {
                this.RaisePropertyChanged(name);
            }
        }


        /// <summary>
        /// Raises this object's PropertyChanged event.
        /// </summary>
        /// <typeparam name="T">The type of the property that has a new value</typeparam>
        /// <param name="propertyExpresssion">A Lambda expression representing the property that has a new value.</param>
        protected void RaisePropertyChanged<T>(Expression<Func<T>> propertyExpresssion)
        {
            var propertyName = ExtractPropertyName(propertyExpresssion);
            this.RaisePropertyChanged(propertyName);
        }

        private string ExtractPropertyName<T>(Expression<Func<T>> propertyExpresssion)
        {
            if (propertyExpresssion == null)
            {
                throw new ArgumentNullException("propertyExpression");
            }

            var memberExpression = propertyExpresssion.Body as MemberExpression;
            if (memberExpression == null)
            {
                throw new ArgumentException("The expression is not a member access expression.", "propertyExpression");
            }

            var property = memberExpression.Member as PropertyInfo;
            if (property == null)
            {
                throw new ArgumentException("The member access expression does not access a property.", "propertyExpression");
            }

            if (!property.DeclaringType.IsAssignableFrom(this.GetType()))
            {
                throw new ArgumentException("The referenced property belongs to a different type.", "propertyExpression");
            }

            var getMethod = property.GetGetMethod(true);
            if (getMethod == null)
            {
                // this shouldn't happen - the expression would reject the property before reaching this far
                throw new ArgumentException("The referenced property does not have a get method.", "propertyExpression");
            }

            if (getMethod.IsStatic)
            {
                throw new ArgumentException("The referenced property is a static property.", "propertyExpression");
            }

            return memberExpression.Member.Name;
        }

        private static bool AreErrorCollectionsEqual(IEnumerable<string> propertyErrors, IEnumerable<string> currentPropertyErrors)
        {
            var equals = currentPropertyErrors.Zip(propertyErrors, (current, newError) => current == newError);
            return propertyErrors.Count() == currentPropertyErrors.Count() && equals.All(b => b);
        }
    }
}
