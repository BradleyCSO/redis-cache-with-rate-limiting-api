namespace RedisCacheWithRateLimitingWebAPI.Exceptions
{
    public class PreferencesBadRequestException : Exception
    {
        public PreferencesBadRequestException() { }

        public PreferencesBadRequestException(string message)
            : base(message) { }

        public PreferencesBadRequestException(string message, Exception inner)
            : base(message, inner) { }
    }
}