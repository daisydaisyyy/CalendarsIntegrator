using CalendarsIntegrator.Core.Abstracts;
using CalendarsIntegrator.CoreTests;

namespace Test
{
    [TestClass]
    public class IntegratorTest1
    {
        [TestMethod]
        public void TestLeftToRightSync()
        {

            // Arrange
            // creare un input con 5 entry

            TestSink input = new CalendarsIntegrator.CoreTests.TestSink();
            // creare un output senza nessuna entry
            TestSink output = new CalendarsIntegrator.CoreTests.TestSink();
            var search = new CalendarsIntegrator.DefaultSearch()
            {
                Emails = new[] { "test@test.com" },
                From = new DateTime(2022, 1, 1),
                To = new DateTime(2022, 12, 31)
            };
            var integrator = new CalendarsIntegrator.Core.Concretes.Integrator(new ISink[] { input } , new ISink[] { output },search);


            // Act #1
            integrator.Sync();


            // Assert #1
            
            Assert.AreEqual(input.allEventsList.Count, output.allEventsList.Count);
            

            // Act #2
            output.Insert(new CalendarsIntegrator.Core.Concretes.CalendarEntry(Convert.ToDateTime("2022/04/04"), Convert.ToDateTime("2022/04/14"), "test@output.com", "TEST EVENT 4", "TEST EVENT 4 MUST GET DELETED", "", "TEST:TEST"));

            integrator.Sync();

            // Assert #2
            
            Assert.AreEqual(input.allEventsList.Count, output.allEventsList.Count);
            
            
        }
    }
}