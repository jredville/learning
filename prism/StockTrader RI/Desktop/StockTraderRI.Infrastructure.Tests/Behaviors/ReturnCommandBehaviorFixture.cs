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
using System.Windows.Controls;
using System.Windows.Input;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using StockTraderRI.Infrastructure.Behaviors;

namespace StockTraderRI.Infrastructure.Tests.Behaviors
{
    [TestClass]
    public class ReturnCommandBehaviorFixture
    {
        [TestMethod]
        public void ShouldExecuteCommandOnEnter()
        {
            var textBox = new TextBox();
            textBox.Text = "MyParameter";
            textBox.AcceptsReturn = true;
            var command = new MockCommand();
            Assert.IsTrue(textBox.AcceptsReturn);

            TestableReturnCommandBehavior behavior = new TestableReturnCommandBehavior(textBox);
            Assert.IsFalse(textBox.AcceptsReturn);
            behavior.Command = command;

            behavior.InvokeKeyPressed(Key.Enter);

            Assert.IsTrue(command.ExecuteCalled);
            Assert.AreEqual("MyParameter", command.ExecuteParameter);
        }

        [TestMethod]
        public void ShouldNotExecuteCommandOnKeyDifferentThanEnter()
        {
            var textBox = new TextBox();
            textBox.Text = "MyParameter";
            TestableReturnCommandBehavior behavior = new TestableReturnCommandBehavior(textBox);
            var command = new MockCommand();
            behavior.Command = command;

            behavior.InvokeKeyPressed(Key.Space);

            Assert.IsFalse(command.ExecuteCalled);
        }

        [TestMethod]
        public void ShouldSetEmptyTextAfterCommandExecution()
        {
            var textBox = new TextBox();
            var command = new MockCommand();

            TestableReturnCommandBehavior behavior = new TestableReturnCommandBehavior(textBox);
            behavior.Command = command;
            behavior.DefaultTextAfterCommandExecution = "MyDefaultText";

            behavior.InvokeKeyPressed(Key.Enter);

            Assert.AreEqual(string.Empty, textBox.Text);
        }
    }

    internal class MockCommand : ICommand
    {
        public bool ExecuteCalled;
        public object ExecuteParameter;
        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            ExecuteCalled = true;
            ExecuteParameter = parameter;
        }
    }

    internal class TestableReturnCommandBehavior : ReturnCommandBehavior
    {
        public TestableReturnCommandBehavior(TextBox textBox)
            : base(textBox)
        {
        }

        public void InvokeKeyPressed(Key keyPressed)
        {
            base.KeyPressed(keyPressed);
        }
    }
}