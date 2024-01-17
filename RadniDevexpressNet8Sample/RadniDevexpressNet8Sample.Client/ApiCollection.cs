using System;

namespace CommonBlazor.Infrastructure
{
    public class ApiCollection
    {
        public class Api
        {
            public string Name { get; set; }
            public string Url { get; set; }
        }

        public static Api[] Items
        {
            get => _items;
            set
            {
                if (_items is not null)
                    throw new InvalidOperationException("Items have already been initialized.");

                _items = value;
            }
        }

        private static Api[] _items;
    }
}
