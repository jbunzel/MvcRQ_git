using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Web.Http;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Mvc5RQ;
using Mvc5RQ.Controllers;

namespace Mvc5RQ.Tests.Controllers
{
    [TestClass]
    public class ValuesControllerTest
    {
        [TestMethod]
        public void Get()
        {
            // Anordnen
            RQDSController controller = new RQDSController();

            // Aktion ausführen
            IEnumerable<string> result = null; //controller.get();

            // Bestätigen
            Assert.IsNotNull(result);
            Assert.AreEqual(2, result.Count());
            Assert.AreEqual("value1", result.ElementAt(0));
            Assert.AreEqual("value2", result.ElementAt(1));
        }

        [TestMethod]
        public void GetById()
        {
            // Anordnen
            RQDSController controller = new RQDSController();

            // Aktion ausführen
            string result = null; // controller.Get(5);

            // Bestätigen
            Assert.AreEqual("value", result);
        }

        [TestMethod]
        public void Post()
        {
            // Anordnen
            RQDSController controller = new RQDSController();

            // Aktion ausführen
            // controller.Post("value");

            // Bestätigen
        }

        [TestMethod]
        public void Put()
        {
            // Anordnen
            RQDSController controller = new RQDSController();

            // Aktion ausführen
            //controller.Put(5, "value");

            // Bestätigen
        }

        [TestMethod]
        public void Delete()
        {
            // Anordnen
            RQDSController controller = new RQDSController();

            // Aktion ausführen
            //controller.Delete(5);

            // Bestätigen
        }
    }
}
