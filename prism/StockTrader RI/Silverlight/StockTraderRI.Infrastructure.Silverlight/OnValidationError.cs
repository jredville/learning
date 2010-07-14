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
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace StockTraderRI.Infrastructure
{
    /// <summary>
    /// Provides a way to apply a tooltip and set the background of a control to a different color when an exception occurs in a binding.
    /// </summary>
    /// <remarks>
    /// This is to workaround the lack of Style triggers and ErrorTemplate that is present in WPF and not in Silverlight.
    /// This should not be taken as guidance on how to do validation.
    /// </remarks>
    public static class OnValidationError
    {
        /// <summary>
        /// <see cref="Brush"/> value to set on <see cref="Control.Background"/> when a validation error ocurr.
        /// </summary>
        public static readonly DependencyProperty ToggleBackgroundProperty = DependencyProperty.RegisterAttached(
                "ToggleBackground", typeof(Brush), typeof(OnValidationError), new PropertyMetadata(ToggleBackgroundPropertyChanged));

        /// <summary>
        /// When <see langword="true"/> sets the validation error message as tooltip over the control.
        /// </summary>
        public static readonly DependencyProperty ShowToolTipProperty = DependencyProperty.RegisterAttached(
                "ShowToolTip", typeof(bool), typeof(OnValidationError), new PropertyMetadata(ShowToolTipPropertyChanged));

        #region Private attached properties to control lifetime of attached behaviors

        private static readonly DependencyProperty MonitorBindingValidationErrorsBehaviorProperty = DependencyProperty.RegisterAttached(
                "MonitorBindingValidationErrorsBehavior", typeof(MonitorBindingValidationErrorsBehavior), typeof(OnValidationError), null);

        private static readonly DependencyProperty ToggleBackgroundOnValidationBehaviorProperty = DependencyProperty.RegisterAttached(
                "ToggleBackgroundOnValidationBehavior", typeof(ToggleBackgroundOnValidationBehavior), typeof(OnValidationError), null);

        private static readonly DependencyProperty ShowToolTipOnValidationBehaviorProperty = DependencyProperty.RegisterAttached(
                "ShowToolTipOnValidationBehavior", typeof(ShowToolTipOnValidationBehavior), typeof(OnValidationError), null);

        #endregion

        #region Public wrappers around the Attached Properties

        /// <summary>
        /// Gets the <see cref="Brush"/> value that is set on <see cref="Control.Background"/> when a validation error ocurr.
        /// </summary>
        /// <param name="dependencyObject"><see cref="Control"/> on which the <see cref="ToggleBackgroundProperty"/> property is set.</param>
        /// <returns>Value of the <see cref="ToggleBackgroundProperty"/> property.</returns>
        public static Brush GetToggleBackground(DependencyObject dependencyObject)
        {
            return dependencyObject.GetValue(ToggleBackgroundProperty) as Brush;
        }

        /// <summary>
        /// Sets the <see cref="Brush"/> value that is set on <see cref="Control.Background"/> when a validation error ocurr.
        /// </summary>
        /// <param name="dependencyObject"><see cref="Control"/> on which the <see cref="ToggleBackgroundProperty"/> property will be set.</param>
        /// <param name="value">Value to set to the <see cref="ToggleBackgroundProperty"/> property.</param>
        public static void SetToggleBackground(DependencyObject dependencyObject, Brush value)
        {
            dependencyObject.SetValue(ToggleBackgroundProperty, value);
        }

        /// <summary>
        /// Gets the value for the <see cref="ShowToolTipProperty"/> on the <paramref name="dependencyObject"/>.
        /// </summary>
        /// <param name="dependencyObject"><see cref="Control"/> on which the <see cref="ShowToolTipProperty"/> property is set.</param>
        /// <returns>Value of the <see cref="ShowToolTipProperty"/> property.</returns>
        public static bool GetShowToolTip(DependencyObject dependencyObject)
        {
            return (bool)(dependencyObject.GetValue(ShowToolTipProperty) ?? false);
        }

        /// <summary>
        /// Sets the value for the <see cref="ShowToolTipProperty"/> on the <paramref name="dependencyObject"/>.
        /// </summary>
        /// <param name="dependencyObject"><see cref="Control"/> on which the <see cref="ShowToolTipProperty"/> property will be set.</param>
        /// <param name="value">Value to set to the <see cref="ShowToolTipProperty"/> property.</param>
        public static void SetShowToolTip(DependencyObject dependencyObject, bool value)
        {
            dependencyObject.SetValue(ShowToolTipProperty, value);
        }

        #endregion

        /// <summary>
        /// Sets the value of <see cref="MonitorBindingValidationErrorsBehavior.Target"/> property to <paramref name="element"/>.
        /// </summary>
        /// <param name="element">Element whose <see cref="FrameworkElement.BindingValidationError"/> event will be handled.</param>
        public static void MonitorBindingValidationErrors(FrameworkElement element)
        {
            var behavior = DependencyPropertyHelper.GetOrAddValue<MonitorBindingValidationErrorsBehavior>(
                    element, MonitorBindingValidationErrorsBehaviorProperty);
            behavior.Target = element;
        }

        private static void ToggleBackgroundPropertyChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        {
            var element = dependencyObject as Control;
            if (element != null)
            {
                MonitorBindingValidationErrors(element);

                var behavior = DependencyPropertyHelper.GetOrAddValue<ToggleBackgroundOnValidationBehavior>(
                        element, ToggleBackgroundOnValidationBehaviorProperty);
                behavior.Target = element;
                behavior.ErrorBrush = e.NewValue as Brush;
            }
        }

        private static void ShowToolTipPropertyChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        {
            var element = dependencyObject as FrameworkElement;
            if (element != null)
            {
                if ((bool)e.NewValue == true)
                {
                    MonitorBindingValidationErrors(element);
                    var behavior = DependencyPropertyHelper.GetOrAddValue<ShowToolTipOnValidationBehavior>(
                            element, ShowToolTipOnValidationBehaviorProperty);
                    behavior.Target = element;
                }
            }
        }

        internal class MonitorBindingValidationErrorsBehavior
        {
            private FrameworkElement target;

            public FrameworkElement Target
            {
                get
                {
                    return this.target;
                }

                set
                {
                    if (this.target != value)
                    {
                        Debug.Assert(this.target == null);
                        this.target = value;
                        this.Attach();
                    }
                }
            }

            private void Attach()
            {
                this.Target.BindingValidationError += this.OnElementValidationError;
            }

            private void OnElementValidationError(object sender, ValidationErrorEventArgs args)
            {
                ObservableCollection<ValidationError> errors = Validation.GetErrors(this.Target);

                if (args.Action == ValidationErrorEventAction.Added)
                {
                    if (!errors.Contains(args.Error))
                    {
                        errors.Add(args.Error);
                    }
                }
                else
                {
                    errors.Remove(args.Error);
                }
            }
        }

        internal class ToggleBackgroundOnValidationBehavior
        {
            private Control target;
            private Brush originalBrush;
            private bool hasErrors;

            public Control Target
            {
                get
                {
                    return this.target;
                }

                set
                {
                    if (this.target != value)
                    {
                        Debug.Assert(this.target == null);
                        this.target = value;
                        this.Attach();
                    }
                }
            }

            public Brush ErrorBrush { get; set; }

            private void Attach()
            {
                ObservableCollection<ValidationError> errors = Validation.GetErrors(this.Target);
                errors.CollectionChanged += this.ErrorsCollectionChanged;
                this.RefreshBackground();
            }

            private void ErrorsCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
            {
                this.RefreshBackground();
            }

            private void RefreshBackground()
            {
                Control targetControl = this.Target;
                ObservableCollection<ValidationError> errors = Validation.GetErrors(targetControl);
                if (!this.hasErrors && errors.Count > 0)
                {
                    this.hasErrors = true;
                    this.originalBrush = targetControl.Background;
                    targetControl.Background = this.ErrorBrush;
                }
                else if (this.hasErrors && errors.Count == 0)
                {
                    this.hasErrors = false;
                    targetControl.Background = this.originalBrush;
                }
            }
        }

        internal class ShowToolTipOnValidationBehavior
        {
            private DependencyObject target;
            private bool hasErrors;
            private object originalToolTip;

            public DependencyObject Target
            {
                get
                {
                    return this.target;
                }

                set
                {
                    if (this.target != value)
                    {
                        Debug.Assert(this.target == null);
                        this.target = value;
                        this.Attach();
                    }
                }
            }

            private void Attach()
            {
                DependencyObject targetControl = this.Target;
                ObservableCollection<ValidationError> errors = Validation.GetErrors(targetControl);
                errors.CollectionChanged += this.ErrorsCollectionChanged;

                #region Workaround for a known issue in ToolTip
                // There is a known issue in Silverlight if setting the ToolTip for the first time on a control that
                // has the mouse over it. As soon as you move the mouse out of the control, an exception is thrown.
                // To workaround the issue, we set the ToolTip to an invisible ToolTip if none was already specified.
                object toolTip = ToolTipService.GetToolTip(targetControl);
                if (toolTip == null)
                {
                    ToolTipService.SetToolTip(targetControl, new ToolTip { Content = "InvisibleToolTip", Opacity = 0 });
                }
                #endregion

                this.RefreshToolTip();
            }

            private void ErrorsCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
            {
                this.RefreshToolTip();
            }

            private void RefreshToolTip()
            {
                ObservableCollection<ValidationError> errors = Validation.GetErrors(this.Target);
                if (errors.Count > 0)
                {
                    if (!this.hasErrors)
                    {
                        this.hasErrors = true;
                        this.originalToolTip = ToolTipService.GetToolTip(this.Target);
                    }

                    ToolTipService.SetToolTip(this.Target, errors[0].Exception.Message);
                }
                else if (this.hasErrors)
                {
                    ToolTipService.SetToolTip(this.Target, this.originalToolTip);
                }
            }
        }
    }
}