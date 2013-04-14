using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Boom
{
    class SineValue
    {
        private double _max;
        private double _sineValue;
        private double _inc;
        private bool _reserve = false;

        public double Value
        {
            get
            {
                if (_reserve)
                {
                    return _max * (1 - Math.Cos(_sineValue));
                }
                else
                {
                    return _max * Math.Sin(_sineValue);
                }
            }

            set
            {
                if (_reserve)
                {
                    _sineValue = Math.Acos(1 - value / _max);
                }
                else
                {
                    _sineValue = Math.Asin(value / _max);
                }
            }
        }

        public bool IsMax
        {
            get
            {
                return _sineValue >= Math.PI / 2.0;
            }
        }

        public bool IsMin
        {
            get
            {
                return _sineValue <= 0.0;
            }
        }

        public SineValue(double max, int steps)
        {
            _max = max;
            _inc = (Math.PI / 2.0) / (double)steps;
            _sineValue = 0;
        }

        public bool Inc()
        {
            _sineValue += _inc;
            return IsMax;
        }

        public bool Dec()
        {
            _sineValue -= +_inc;
            return IsMin;
        }

        public void Reverse()
        {
            double value = Value;
            _reserve = !_reserve;
            Value = value;
        }
    }
}
