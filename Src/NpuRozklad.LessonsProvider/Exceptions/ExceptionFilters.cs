using System;

namespace NpuRozklad.LessonsProvider.Exceptions
{
    public static class ExceptionFilters
    {
        public static bool DeserializationOrFetchException(Exception e)
        {
            return e is DeserializationException || e is NpuServerFetchException;
        }
    }
}