using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;

namespace ConsoleAppTest
{
    public class ValueInRange<T, G, V> where T : IComparable<T>
    {
        private IEnumerable<(G rangeValue, V returnValue)> _rangeAndReturnValues;

        public V this[T value] => _rangeAndReturnValues.First(values => _predicate(value, values.rangeValue)).returnValue;

        private Func<T, G, bool> _predicate;

        public ValueInRange(IEnumerable<(G rangeValue, V returnValue)> rangeAndReturnValues, Func<T, G, bool> predicate)
        {
            _rangeAndReturnValues = rangeAndReturnValues;
            _predicate = predicate;
        }
    }
}
