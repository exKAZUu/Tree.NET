﻿#region License

// Copyright (C) 2011-2014 Kazunori Sakamoto
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//     http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

#endregion

using System;
using System.Linq;
using System.Xml.Linq;
using FluentAssertions;
using NUnit.Framework;

namespace TreeDotNet.Tests {
    [TestFixture]
    public class NodeTest {
        [Test]
        public void OperateRoot() {
            var root = new StringNode("a");
            root.PrevsFromFirst().Should().HaveCount(0);
            root.NextsFromSelf().Should().HaveCount(0);
            root.PrevsFromFirstAndSelf().Should().Equal(Enumerable.Repeat(root, 1));
            root.NextsFromSelfAndSelf().Should().Equal(Enumerable.Repeat(root, 1));
            root.PrevsFromSelf().Should().HaveCount(0);
            root.NextsFromLast().Should().HaveCount(0);
            root.PrevsFromSelfAndSelf().Should().Equal(Enumerable.Repeat(root, 1));
            root.NextsFromLastAndSelf().Should().Equal(Enumerable.Repeat(root, 1));
        }

        [Test]
        public void Create1Node() {
            var node = new StringNode("a");
            node.ToString().Should().Be("a\n".NormalizeNewLine());
            string.Join("", node.Descendants().Select(n => n.Value)).Should().Be("");
        }

        [Test]
        public void Create2Nodes() {
            var node = new StringNode("a");
            node.AddFirst(new StringNode("b"));
            node.ToString().Should().Be("a\n  b\n".NormalizeNewLine());
            string.Join("", node.Descendants().Select(n => n.Value)).Should().Be("b");
        }

        [Test]
        public void Create3Nodes() {
            var node = new StringNode("a");
            node.AddLast(new StringNode("b"));
            node.AddFirst(new StringNode("c"));
            node.ToString().Should().Be("a\n  c\n  b\n".NormalizeNewLine());
            string.Join("", node.Descendants().Select(n => n.Value)).Should().Be("cb");
        }

        [Test]
        public void Create4Nodes() {
            var node = new StringNode("a");
            node.AddLast(new StringNode("b"));
            node.AddFirst(new StringNode("c"));
            node.AddLast(new StringNode("d"));
            node.ToString().Should().Be("a\n  c\n  b\n  d\n".NormalizeNewLine());
            string.Join("", node.Descendants().Select(n => n.Value)).Should().Be("cbd");
        }

        [Test]
        public void CreateTreeAndTraverse() {
            var a = new StringNode("a");
            var b = a.AddFirst(new StringNode("b"));
            var c = a.AddLast(new StringNode("c"));
            var d = a.AddFirst(new StringNode("d"));
            var e = a.AddFirst(new StringNode("e"));
            var f = b.AddFirst(new StringNode("f"));
            var g = b.AddFirst(new StringNode("g"));
            var h = g.AddLast("h");
            var i = f.AddLast("i");
            var j = h.AddNext("j");
            var k = h.AddPrevious("k");
            var l = i.AddPrevious("l");
            var m = i.AddNext("m");
            a.ToString()
                    .Should()
                    .Be(
                            "a\n  e\n  d\n  b\n    g\n      k\n      h\n      j\n    f\n      l\n      i\n      m\n  c\n"
                                    .NormalizeNewLine());

            string.Join("", a.Descendants().Select(n => n.Value)).Should().Be("edbgkhjflimc");
            string.Join("", e.Descendants().Select(n => n.Value)).Should().Be("");
            string.Join("", d.Descendants().Select(n => n.Value)).Should().Be("");
            string.Join("", b.Descendants().Select(n => n.Value)).Should().Be("gkhjflim");
            string.Join("", c.Descendants().Select(n => n.Value)).Should().Be("");

            string.Join("", a.DescendantsAndSelf().Select(n => n.Value)).Should()
                    .Be("aedbgkhjflimc");
            string.Join("", e.DescendantsAndSelf().Select(n => n.Value)).Should().Be("e");
            string.Join("", d.DescendantsAndSelf().Select(n => n.Value)).Should().Be("d");
            string.Join("", b.DescendantsAndSelf().Select(n => n.Value)).Should().Be("bgkhjflim");
            string.Join("", c.DescendantsAndSelf().Select(n => n.Value)).Should().Be("c");

            string.Join("", a.Descendants(2).Select(n => n.Value)).Should().Be("edbgfc");
            string.Join("", e.Descendants(2).Select(n => n.Value)).Should().Be("");
            string.Join("", d.Descendants(2).Select(n => n.Value)).Should().Be("");
            string.Join("", b.Descendants(2).Select(n => n.Value)).Should().Be("gkhjflim");
            string.Join("", c.Descendants(2).Select(n => n.Value)).Should().Be("");
            string.Join("", b.Descendants(0).Select(n => n.Value)).Should().Be("");

            string.Join("", a.DescendantsAndSelf(2).Select(n => n.Value)).Should()
                    .Be("aedbgfc");
            string.Join("", e.DescendantsAndSelf(2).Select(n => n.Value)).Should().Be("e");
            string.Join("", d.DescendantsAndSelf(2).Select(n => n.Value)).Should().Be("d");
            string.Join("", b.DescendantsAndSelf(2).Select(n => n.Value)).Should().Be("bgkhjflim");
            string.Join("", c.DescendantsAndSelf(2).Select(n => n.Value)).Should().Be("c");
            string.Join("", b.DescendantsAndSelf(0).Select(n => n.Value)).Should().Be("b");

            string.Join("", a.Siblings().Select(n => n.Value)).Should().Be("");
            string.Join("", k.Siblings().Select(n => n.Value)).Should().Be("hj");
            string.Join("", h.Siblings().Select(n => n.Value)).Should().Be("kj");
            string.Join("", j.Siblings().Select(n => n.Value)).Should().Be("kh");
            string.Join("", i.Siblings().Select(n => n.Value)).Should().Be("lm");

            string.Join("", a.SiblingsAndSelf().Select(n => n.Value)).Should().Be("a");
            string.Join("", k.SiblingsAndSelf().Select(n => n.Value)).Should().Be("khj");
            string.Join("", h.SiblingsAndSelf().Select(n => n.Value)).Should().Be("khj");
            string.Join("", j.SiblingsAndSelf().Select(n => n.Value)).Should().Be("khj");
            string.Join("", i.SiblingsAndSelf().Select(n => n.Value)).Should().Be("lim");

            string.Join("", a.Siblings(1).Select(n => n.Value)).Should().Be("");
            string.Join("", k.Siblings(1).Select(n => n.Value)).Should().Be("h");
            string.Join("", h.Siblings(1).Select(n => n.Value)).Should().Be("kj");
            string.Join("", j.Siblings(1).Select(n => n.Value)).Should().Be("h");
            string.Join("", i.Siblings(1).Select(n => n.Value)).Should().Be("lm");
            string.Join("", i.Siblings(0).Select(n => n.Value)).Should().Be("");

            string.Join("", a.SiblingsAndSelf(1).Select(n => n.Value)).Should().Be("a");
            string.Join("", k.SiblingsAndSelf(1).Select(n => n.Value)).Should().Be("kh");
            string.Join("", h.SiblingsAndSelf(1).Select(n => n.Value)).Should().Be("khj");
            string.Join("", j.SiblingsAndSelf(1).Select(n => n.Value)).Should().Be("hj");
            string.Join("", i.SiblingsAndSelf(1).Select(n => n.Value)).Should().Be("lim");
            string.Join("", i.SiblingsAndSelf(0).Select(n => n.Value)).Should().Be("i");

            string.Join("", i.Ancestors().Select(n => n.Value)).Should().Be("fba");
            string.Join("", i.Ancestors(3).Select(n => n.Value)).Should().Be("fba");
            string.Join("", i.Ancestors(2).Select(n => n.Value)).Should().Be("fb");
            string.Join("", i.Ancestors(1).Select(n => n.Value)).Should().Be("f");
            string.Join("", i.Ancestors(0).Select(n => n.Value)).Should().Be("");

            string.Join("", i.AncestorsAndSelf().Select(n => n.Value)).Should().Be("ifba");
            string.Join("", i.AncestorsAndSelf(3).Select(n => n.Value)).Should().Be("ifba");
            string.Join("", i.AncestorsAndSelf(2).Select(n => n.Value)).Should().Be("ifb");
            string.Join("", i.AncestorsAndSelf(1).Select(n => n.Value)).Should().Be("if");
            string.Join("", i.AncestorsAndSelf(0).Select(n => n.Value)).Should().Be("i");

            string.Join("", f.AncestorsAndSiblingsAfterSelf().Select(n => n.Value)).Should().Be("c");
            string.Join("", f.AncestorsAndSiblingsAfterSelfAndSelf().Select(n => n.Value)).Should().Be("fc");
            string.Join("", f.AncestorsAndSiblingsBeforeSelf().Select(n => n.Value)).Should().Be("gbdea");
            string.Join("", f.AncestorsAndSiblingsBeforeSelfAndSelf().Select(n => n.Value)).Should().Be("fgbdea");

            string.Join("", h.AncestorsAndSiblingsAfterSelf().Select(n => n.Value)).Should().Be("jfc");
            string.Join("", h.AncestorsAndSiblingsAfterSelfAndSelf().Select(n => n.Value)).Should().Be("hjfc");
            string.Join("", h.AncestorsAndSiblingsBeforeSelf().Select(n => n.Value)).Should().Be("kgbdea");
            string.Join("", h.AncestorsAndSiblingsBeforeSelfAndSelf().Select(n => n.Value)).Should().Be("hkgbdea");


            Assert.That(b.Ancestors(), Is.EqualTo(new[] { a }));
            Assert.That(b.AncestorsAndSelf(), Is.EqualTo(new[] { b, a }));
            Assert.That(b.Children(), Is.EqualTo(new[] { g, f }));
            Assert.That(b.ChildrenAndSelf(), Is.EqualTo(new[] { b, g, f }));
            Assert.That(b.ChildrenCount, Is.EqualTo(2));
            Assert.That(b.NextsFromSelf(), Is.EqualTo(new[] { c }));
            Assert.That(b.NextsFromSelfAndSelf(), Is.EqualTo(new[] { b, c }));
            Assert.That(b.NextsFromLast(), Is.EqualTo(new[] { c }));
            Assert.That(b.NextsFromLastAndSelf(), Is.EqualTo(new[] { c, b }));
            Assert.That(b.PrevsFromFirst(), Is.EqualTo(new[] { e, d }));
            Assert.That(b.PrevsFromFirstAndSelf(), Is.EqualTo(new[] { e, d, b }));
            Assert.That(b.PrevsFromSelf(), Is.EqualTo(new[] { d, e }));
            Assert.That(b.PrevsFromSelfAndSelf(), Is.EqualTo(new[] { b, d, e }));
            Assert.That(b.DescendantsOfFirstChild(), Is.EqualTo(new[] { g, k }));
            Assert.That(b.DescendantsOfFirstChildAndSelf(), Is.EqualTo(new[] { b, g, k }));

            Assert.That(e.Ancestors(), Is.EqualTo(new[] { a }));
            Assert.That(e.AncestorsAndSelf(), Is.EqualTo(new[] { e, a }));
            Assert.That(e.Children(), Is.EqualTo(new StringNode[0]));
            Assert.That(e.ChildrenAndSelf(), Is.EqualTo(new[] { e }));
            Assert.That(e.ChildrenCount, Is.EqualTo(0));
            Assert.That(e.NextsFromSelf(), Is.EqualTo(new[] { d, b, c }));
            Assert.That(e.NextsFromSelfAndSelf(), Is.EqualTo(new[] { e, d, b, c }));
            Assert.That(e.NextsFromLast(), Is.EqualTo(new[] { c, b, d }));
            Assert.That(e.NextsFromLastAndSelf(), Is.EqualTo(new[] { c, b, d, e }));
            Assert.That(e.PrevsFromFirst(), Is.EqualTo(new StringNode[0]));
            Assert.That(e.PrevsFromFirstAndSelf(), Is.EqualTo(new[] { e }));
            Assert.That(e.PrevsFromSelf(), Is.EqualTo(new StringNode[0]));
            Assert.That(e.PrevsFromSelfAndSelf(), Is.EqualTo(new[] { e }));
            Assert.That(e.DescendantsOfFirstChild(), Is.EqualTo(new StringNode[0]));
            Assert.That(e.DescendantsOfFirstChildAndSelf(), Is.EqualTo(new[] { e }));
        }

        [Test]
        public void TraverseSingles() {
            var a = new StringNode("a");
            var b = new StringNode("b");
            var c = new StringNode("c");
            var d = new StringNode("d");
            var e = new StringNode("e");
            var f = new StringNode("f");
            var g = new StringNode("g");
            // a - b - c - d - e
            //   - g         - f
            a.AddFirst(b);
            a.AddLast(g);
            b.AddFirst(c);
            c.AddFirst(d);
            d.AddFirst(e);
            d.AddLast(f);

            string.Join("", b.DescendantsOfSingle().Select(n => n.Value)).Should()
                    .Be("cd");
            string.Join("", b.DescendantsOfSingleAndSelf().Select(n => n.Value)).Should()
                    .Be("bcd");
            string.Join("", c.DescendantsOfSingle().Select(n => n.Value)).Should()
                    .Be("d");
            string.Join("", c.DescendantsOfSingleAndSelf().Select(n => n.Value)).Should()
                    .Be("cd");

            string.Join("", b.AncestorsWithSingleChild().Select(n => n.Value)).Should()
                    .Be("");
            string.Join("", b.AncestorsWithSingleChildAndSelf().Select(n => n.Value)).Should()
                    .Be("b");
            string.Join("", c.AncestorsWithSingleChild().Select(n => n.Value)).Should()
                    .Be("b");
            string.Join("", c.AncestorsWithSingleChildAndSelf().Select(n => n.Value)).Should()
                    .Be("cb");
            string.Join("", d.AncestorsWithSingleChild().Select(n => n.Value)).Should()
                    .Be("cb");
            string.Join("", d.AncestorsWithSingleChildAndSelf().Select(n => n.Value)).Should()
                    .Be("dcb");
            string.Join("", e.AncestorsWithSingleChild().Select(n => n.Value)).Should()
                    .Be("");
            string.Join("", e.AncestorsWithSingleChildAndSelf().Select(n => n.Value)).Should()
                    .Be("e");
        }

        [Test]
        public void UseExtensionMethodsForIEnumerable() {
            new XElement[0].Descendants("test").Descendants("test");
            new StringNode[0].Descendants("test").Descendants("test");
        }
    }

    public static class StringExtensionForTest {
        public static string NormalizeNewLine(this string text) {
            return text.Replace("\n", Environment.NewLine);
        }
    }
}