using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoudnessMeter.Models
{
    public class DecibelValue
    {
        public int Time { get; }
        public float Value { get; }

        public DecibelValue(int Time, float value)
        {
            this.Time = Time;
            this.Value = value;
        }
    }
}
