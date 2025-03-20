using FizzWare.NBuilder;

namespace DoaMais.Tests.Utils
{
    public static class TestsUtils
    {
        public static T CreateMockedObject<T>() where T : class
        {
            return Builder<T>.CreateNew().Build();
        }

        public static IList<T> CreateMockedList<T>(int count) where T : class
        {
            return Builder<T>.CreateListOfSize(count).Build();
        }

        public static Guid CreateMockedGuid()
        {
            return Builder<Guid>.CreateNew().Build();
        }

        public static string CreateMockedString()
        {
            return "MockedString";
        }

        public static string CreateMockedEmail()
        {
            return "email@example.com";
        }

        public static DateTime CreateMockedDateTime()
        {
            return Builder<DateTime>.CreateNew().Build();
        }

        public static T CreateMockedEnum<T>() where T : struct
        {
            return Builder<T>.CreateNew().Build();
        }

        public static string CreateMockedStringWithLenght(int lenght)
        {
            return new string('a', lenght);
        }

        public static int CreateMockedInt()
        {
            return 1;
        }
    }
}
