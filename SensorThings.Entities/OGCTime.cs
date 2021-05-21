using System;

namespace SensorThings.Entities
{
    public class OGCTime
    {
        private readonly DateTime _begin;
        private readonly DateTime _end;

        public OGCTime(DateTime instant)
        {
            _begin = instant;
        }

        public OGCTime(DateTime begin, DateTime end)
        {
            _begin = begin;
            _end = end;
        }
    }

    // TODO: This class needs a custom JSON converter
}
