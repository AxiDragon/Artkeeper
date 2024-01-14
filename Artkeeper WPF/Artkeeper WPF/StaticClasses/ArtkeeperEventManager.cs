using System;
using System.Collections.Generic;

namespace Artkeeper.StaticClasses
{
    public class ArtkeeperEventManager
    {
        public static Dictionary<EventCollection, Action> EVENT_COLLECTION = new Dictionary<EventCollection, Action>();
        public static Dictionary<EventCollection, Action<object>> EVENT_COLLECTION_PARAM = new Dictionary<EventCollection, Action<object>>();

        public static void Initialize()
        {
            EVENT_COLLECTION.Clear();
            EVENT_COLLECTION_PARAM.Clear();

            foreach (EventCollection eventCollection in Enum.GetValues(typeof(EventCollection)))
            {
                if (!EVENT_COLLECTION.ContainsKey(eventCollection))
                {
                    EVENT_COLLECTION.Add(eventCollection, Empty);
                }
                if (!EVENT_COLLECTION_PARAM.ContainsKey(eventCollection))
                {
                    EVENT_COLLECTION_PARAM.Add(eventCollection, Empty);
                }
            }
        }

        private static void Empty()
        {
        }

        private static void Empty(object _value)
        {
        }
    }
}