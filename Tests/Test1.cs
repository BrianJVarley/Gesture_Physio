﻿using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyoTestv4.Tests
{
    [TestFixture]
    public class Test1
    {
        [Test]
        public void TestAppVM()
        {

            HomeViewModel vm = new HomeViewModel();
            string actualName = vm.Name;
            Assert.AreEqual("Home Page", actualName);


        }


    }
}
