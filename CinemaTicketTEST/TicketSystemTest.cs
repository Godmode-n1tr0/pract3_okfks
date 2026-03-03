using CinemaTicketSystem;
using System.Reflection.Metadata;
using System.Runtime.InteropServices.Marshalling;

namespace CinemaTicketTEST
{
    public class TicketSystemTest
    {

        private readonly ITicketPriceCalculator _calculator;

        #region Проверка корректных вычислений

        public TicketSystemTest()
        {
            _calculator = new TicketPriceCalculator();
        }


        [Fact]
        public void TestTicket_FromNotDiscount()
        {
            int ishod = 300;
            var request = new TicketRequest
            {
                Age = 30,
                IsStudent = false,
                IsVip = false,
                Day = DayOfWeek.Monday,
                SessionTime = new TimeSpan(18, 0, 0)
            };

            var result = _calculator.CalculatePrice(request);

            Assert.Equal(ishod, result);
        }

        [Fact]
        public void TestTicket_ChildDiscount()
        {
            int ishod = 0;
            var request = new TicketRequest
            {
                Age = 5,
                IsStudent = false,
                IsVip = false,
                Day = DayOfWeek.Monday,
                SessionTime = new TimeSpan(18, 0, 0)
            };

            var result = _calculator.CalculatePrice(request);

            Assert.Equal(ishod, result);
        }

        [Fact]
        public void TestTicket_ChildDiscount_For6To17()
        {
            int ishod = 180;
            var request = new TicketRequest
            {
                Age = 15,
                IsStudent = false,
                IsVip = false,
                Day = DayOfWeek.Monday,
                SessionTime = new TimeSpan(18, 0, 0)
            };

            var result = _calculator.CalculatePrice(request);

            Assert.Equal(ishod, result);
        }

        [Fact]
        public void TestTicket_StudentDiscount_For18To25()
        {
            int ishod = 240;

            var request = new TicketRequest
            {
                Age = 19,
                IsStudent = true,
                IsVip = false,
                Day = DayOfWeek.Monday,
                SessionTime = new TimeSpan(18, 0, 0)
            };

            var result = _calculator.CalculatePrice(request);

            Assert.Equal(ishod, result);
        }

        [Fact]
        public void TestTicket_PensionerDiscount_For65Plus()
        {
            int ishod = 150;
            var request = new TicketRequest
            {
                Age = 70,
                IsStudent = false,
                IsVip = false,
                Day = DayOfWeek.Monday,
                SessionTime = new TimeSpan(18, 0, 0)
            };

            var result = _calculator.CalculatePrice(request);

            Assert.Equal(ishod, result);
        }

        [Fact]
        public void TestTicket_WednesdayDiscount()
        {
            int ishod = 210;
            var request = new TicketRequest
            {
                Age = 30,
                IsStudent = false,
                IsVip = false,
                Day = DayOfWeek.Wednesday,
                SessionTime = new TimeSpan(18, 0, 0)
            };

            var result = _calculator.CalculatePrice(request);

            Assert.Equal(ishod, result);
        }

        [Fact]
        public void TestTicket_ForMorningSessionBefore12()
        {
            int ishod = 255;

            var request = new TicketRequest
            {
                Age = 30,
                IsStudent = false,
                IsVip = false,
                Day = DayOfWeek.Monday,
                SessionTime = new TimeSpan(10, 0, 0)
            };

            var result = _calculator.CalculatePrice(request);

            Assert.Equal(ishod, result);
        }

        [Fact]
        public void TestTicket_WhenIsVipTrue()
        {
            int ishod = 600;

            var request = new TicketRequest
            {
                Age = 30,
                IsStudent = false,
                IsVip = true,
                Day = DayOfWeek.Monday,
                SessionTime = new TimeSpan(13, 0, 0)
            };

            var result = _calculator.CalculatePrice(request);

            Assert.Equal(ishod, result);
        }

        [Fact]
        public void TestTicket_SeveralDiscountsHaveBeenUsed()
        {
            int ishod = 150;

            var request = new TicketRequest
            {
                Age = 65,
                IsStudent = false,
                IsVip = false,
                Day = DayOfWeek.Wednesday,
                SessionTime = new TimeSpan(10, 0, 0)
            };

            var result = _calculator.CalculatePrice(request);

            Assert.Equal(ishod, result);
        }

        [Fact]
        public void TestTicket_PriceAfterDiscountPensionerPlusVipTrue()
        {
            int ishod = 300;

            var request = new TicketRequest
            {
                Age = 70,
                IsStudent = false,
                IsVip = true,
                Day = DayOfWeek.Monday,
                SessionTime = new TimeSpan(18, 0, 0)
            };

            var result = _calculator.CalculatePrice(request);

            Assert.Equal(ishod, result);
        }

        #endregion

        #region Проверка граничных значений

        [Theory]
        [InlineData(0)]
        [InlineData(120)]
        public void TestTicket_HandleExtremeAges(int age)
        {
            var request = new TicketRequest
            {
                Age = age,
                IsStudent = false,
                IsVip = false,
                Day = DayOfWeek.Monday,
                SessionTime = new TimeSpan(18, 0, 0)
            };

            var exception = Record.Exception(() => _calculator.CalculatePrice(request));

            Assert.Null(exception);
        }

        [Theory]
        [InlineData(5, 0)]
        [InlineData(6, 180)]
        [InlineData(17, 180)]
        [InlineData(18, 240)]
        [InlineData(25, 240)]
        [InlineData(26, 300)]
        [InlineData(64, 300)]
        [InlineData(65, 150)]
        [InlineData(120, 150)]

        public void TestTicket_HandleAgeBoundaries(int age, int expectedPrice)
        {
            var request = new TicketRequest
            {
                Age = age,
                IsStudent = age >= 18 && age <= 25,
                IsVip = false,
                Day = DayOfWeek.Monday,
                SessionTime = new TimeSpan(18, 0, 0)
            };

            var result = _calculator.CalculatePrice(request);

            Assert.Equal(expectedPrice, result);
        }

        [Theory]
        [InlineData(11, 59, 255)]
        [InlineData(12, 0, 300)]
        public void TestTicket_HandleMorningDiscountTimeBoundaries(int hour, int minute, int expectedPrice)
        {
            var request = new TicketRequest
            {
                Age = 30,
                IsStudent = false,
                IsVip = false,
                Day = DayOfWeek.Monday,
                SessionTime = new TimeSpan(hour, minute, 0)
            };

            var result = _calculator.CalculatePrice(request);

            Assert.Equal(expectedPrice, result);
        }
        #endregion

        #region Проверка исключений

        [Fact]
        public void TestTicket_ShouldThrowArgumentNullException_WhenRequestIsNull()
        {
            TicketRequest request = null;

            Assert.Throws<ArgumentNullException>(() => _calculator.CalculatePrice(request));
        }

        [Theory]
        [InlineData(-1)]
        [InlineData(121)]

        public void TestTicket_ShouldThrowArgumentOutOfRangeException_WhenAgeIsInvalid(int invalidAge)
        {
            var request = new TicketRequest
            {
                Age = invalidAge,
                IsStudent = false,
                IsVip = false,
                Day = DayOfWeek.Monday,
                SessionTime = new TimeSpan(18, 0, 0)
            };

            Assert.Throws<ArgumentOutOfRangeException>(()  => _calculator.CalculatePrice(request));
        }
        #endregion

        #region комбинированные сценарии

        [Fact]
        public void TestTicket_ShouldHandleComplexScenario_WithMultipleDiscountsAndVip()
        {
            int ishod = 360;

            var request = new TicketRequest
            {
                Age = 17,
                IsStudent = false,
                IsVip = true,
                Day = DayOfWeek.Wednesday,
                SessionTime = new TimeSpan(10, 0, 0)
            };

            var result = _calculator.CalculatePrice(request);

            Assert.Equal(ishod, result);
        }

        [Fact]
        public void TestTicket_ShoudlRountToNearestInteger()
        {
            int ishod = 210;
            int ishod2 = 0;

            var request = new TicketRequest
            {
                Age = 30,
                IsStudent = false,
                IsVip = false,
                Day = DayOfWeek.Wednesday,
                SessionTime = new TimeSpan(10, 0, 0)
            };

            var result = _calculator.CalculatePrice(request);

            Assert.Equal(ishod, result);
            Assert.Equal(ishod2, result % 1);
        }

        #endregion
    }
}