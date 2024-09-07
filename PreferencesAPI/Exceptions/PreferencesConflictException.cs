namespace RedisCacheWithRateLimitingWebAPI.MainAPI.Exceptions
{
    public class PreferencesConflictException : Exception
    {
        public PreferencesConflictException() { }

        public PreferencesConflictException(string message)
            : base(message) { }

        public PreferencesConflictException(string message, Exception inner)
            : base(message, inner) { }
    }
}