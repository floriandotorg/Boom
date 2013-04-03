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

        public double Value
        {
            get
            {
                return _max * Math.Sin(_sineValue);
            }

            set
            {
                _sineValue = Math.Asin(value / _max);
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

        public void Inc()
        {
            _sineValue += _inc;
        }

        public void Dec()
        {
            _sineValue -= +_inc;
        }
    }
}
