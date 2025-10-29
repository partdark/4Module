using Application;
using Applications.Services;
using Domain.Entitties;
using Domain.Interfaces;
using Moq;
using Repository;
using System.Threading.Tasks;

namespace xUnit_Test_Project
{
    public class UnitTest1
    {
        [Fact]
        public void Test1()
        {
            var resualt = Math.Add(1, 2);
            var expected = 1 + 2;

            Assert.Equal(expected, resualt);
        }

        [Theory]
        [InlineData(1, 2, 3)]
        [InlineData(5, 5, 10)]
        public void TestAdd(double a, double b, double expected)
        {
            var result = Math.Add(a, b);
            Assert.Equal(expected, result);
        }
    }

    
    public  class BookReposotoryTest
    {
        private readonly Mock<IBookRepository> _bookMock;
        private readonly Mock<IAuthorRepository> _authorMock;
        private readonly IBookService _bookService;


        public BookReposotoryTest()
        {
            _bookMock = new Mock<IBookRepository>();
            _authorMock = new Mock<IAuthorRepository>();
            _bookService = new BookService(_bookMock.Object, _authorMock.Object);

        }


        [Fact]
        public async Task GetByIdtest()
        {
            var bookId = Guid.NewGuid();
            var expectedBook = new Book
            {
                Id = bookId,
                Authors = new List<Author>(),
                Title = "testTitle",
                Year = 1985
            };
            _bookMock.Setup(b => b.GetByIdAsync(bookId)).ReturnsAsync(expectedBook);

            var result = await _bookService.GetByIdAsync(bookId);

            Assert.NotNull(result);
            Assert.Equal(expectedBook.Id, result.Id);
            Assert.Equal(expectedBook.Title, result.Title);
            Assert.Equal(expectedBook.Year, result.Year);
            _bookMock.Verify(r => r.GetByIdAsync(bookId), Times.Once);

        }
        [Fact]
        public async Task DeleteByIdtest()
        {
            var bookId = Guid.NewGuid();
            var expectedBook = new Book
            {
                Id = bookId,
                Authors = new List<Author>(),
                Title = "testTitle",
                Year = 1985
            };
            _bookMock.Setup(b => b.GetByIdAsync(bookId)).ReturnsAsync(expectedBook);
             _bookMock.Setup(b => b.DeleteAsync(It.IsAny<Guid>())).ReturnsAsync(true);

            await _bookService.DeleteAsync(bookId);

            _bookMock.Verify(r => r.DeleteAsync(bookId), Times.Once);

        }

    }


    public static class Math
    {
        public static double Add(double a, double b) => a + b;


    }
}