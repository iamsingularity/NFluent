﻿// // --------------------------------------------------------------------------------------------------------------------
// // <copyright file="DynamicTests.cs" company="">
// //   Copyright 2013 Thomas PIERRAIN
// //   Licensed under the Apache License, Version 2.0 (the "License");
// //   you may not use this file except in compliance with the License.
// //   You may obtain a copy of the License at
// //       http://www.apache.org/licenses/LICENSE-2.0
// //   Unless required by applicable law or agreed to in writing, software
// //   distributed under the License is distributed on an "AS IS" BASIS,
// //   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// //   See the License for the specific language governing permissions and
// //   limitations under the License.
// // </copyright>
// // --------------------------------------------------------------------------------------------------------------------

#if !DOTNET_20 && !DOTNET_30 && !DOTNET_35 && !DOTNET_40
namespace NFluent.Tests
{
    using NUnit.Framework;

    [TestFixture]
    public class DynamicTests
    {
        class Command
        {
            internal dynamic Subject { get; set; }
        }

        [Test]
        public void CanCheckNulls()
        {
            var cmd = new Command();
            dynamic sut = "test";

            Check.ThatDynamic(sut).IsNotNull();
            // this check fails
            Check.ThatCode(() => { Check.ThatDynamic(cmd.Subject).IsNotNull(); }).Throws<FluentCheckException>();
        }

          // see GH #280
        [Test]
        public void SupportWithCustomMessage()
        {
            var cmd = new Command();
            dynamic sut = "test";

            AssertCheckFails(
                ()=>Check.WithCustomMessage("cool").ThatDynamic(cmd.Subject).IsNotNull(), 
                "cool",
                "The checked dynamic is null whereas it must not.",
                "The checked dynamic:",
                "\t[null]");
            // this check fails
            AssertCheckFails(
                ()=>Check.WithCustomMessage("cool").ThatDynamic(cmd.Subject).IsSameReferenceAs("tes"), 
                "cool",
                "The checked dynamic is not the expected reference.",
                "The checked dynamic:",
                "\t[null]",
                "The expected dynamic:",
                "\t[\"tes\"]");
            AssertCheckFails(
                ()=>Check.WithCustomMessage("cool").ThatDynamic(sut).IsEqualTo("tes"), 
                "cool",
                "The checked dynamic is not equal to the expected one.",
                "The checked dynamic:",
                "\t[\"test\"]",
                "The expected dynamic:",
                "\t[\"tes\"]");
            AssertCheckFails(
                ()=>Check.WithCustomMessage("cool").ThatDynamic(sut).Not.IsSameReferenceAs(sut), 
                "cool",
                "The checked dynamic is the expected reference whereas it must not.",
                "The checked dynamic:",
                "\t[\"test\"]",
                "The expected dynamic: different from",
                "\t[\"test\"]");
        }

        static public void AssertCheckFails(System.Action test, params string[] message)
        {
            try
            {
                test();
                Assert.Fail("Assertion should have been raised!");
            }
            catch(FluentCheckException e)
            {
                if (message.Length==1)
                {
                    Assert.AreEqual(message[0], e.Message);
                }
                else
                {
                    var builder = new System.Text.StringBuilder();
                    for(var i=0; i<message.Length; i++)
                    {
                        if  (i>0)
                        {
                            builder.Append(System.Environment.NewLine);
                        }
                        builder.Append(message[i]);
                    }
                    Assert.AreEqual(builder.ToString(), e.Message);
                }
            }
        }

        [Test]
        public void AndWorks()
        {
            var cmd = new Command();
            dynamic sut = "test";

            Check.ThatDynamic(sut).IsNotNull().And.IsEqualTo("test");
            // this check fails
            Check.ThatCode(() => { Check.ThatDynamic(cmd.Subject).IsNotNull(); }).Throws<FluentCheckException>();
        }

        [Test]
        public void CanCheckReference()
        {
            dynamic sut = "test";

            Check.ThatDynamic(sut).IsSameReferenceAs(sut);
            Check.ThatCode(() => { Check.ThatDynamic(sut).IsSameReferenceAs("tes"); }).Throws<FluentCheckException>();
        }

        [Test]
        public void CanCheckEquality()
        {
            dynamic sut = "test";

            Check.ThatDynamic(sut).IsEqualTo(sut);

            Check.ThatCode(() => { Check.ThatDynamic(sut).IsEqualTo("tes"); }).Throws<FluentCheckException>();
        }

        [Test]
        public void NotWorks()
        {
            var cmd = new Command();
            dynamic sut = "test";

            Check.ThatDynamic(cmd.Subject).Not.IsNotNull();
            // this check fails
            AssertCheckFails(
                () => Check.ThatDynamic(sut).Not.IsNotNull(), 
                "",
                "The checked dynamic is not null whereas it must.",
                "The checked dynamic:",
                "\t[\"test\"]"
                );

            Check.ThatDynamic(sut).Not.IsEqualTo("tes");
            AssertCheckFails(
                () => Check.ThatDynamic(sut).Not.IsEqualTo(sut), 
                "",
                "The checked dynamic is equal to the expected one whereas it must not.",
                "The checked dynamic:",
                "\t[\"test\"]",
                "The expected dynamic: different from",
                "\t[\"test\"]"
                );

            Check.ThatDynamic(sut).Not.IsSameReferenceAs("tes");

            AssertCheckFails(
                () => Check.ThatDynamic(sut).Not.IsSameReferenceAs(sut), 
                "",
                "The checked dynamic is the expected reference whereas it must not.",
                "The checked dynamic:",
                "\t[\"test\"]",
                "The expected dynamic: different from",
                "\t[\"test\"]"
                );
        }

    }
}
#endif