using static System.Math;
using System.Collections.Generic;

namespace DigitalFilter
{
    public class DigitalFilter
    {
        private double in1 = 0.0f;
        private double in2 = 0.0f;
        private double out1 = 0.0f;
        private double out2 = 0.0f;
        private double omega;
        private double alpha;
        private double q;
        private double bandWidth = 1.0f;
        private double a0;
        private double a1;
        private double a2;
        private double b0;
        private double b1;
        private double b2;
        private int averageNum;
        private Queue<double> buffer = new Queue<double>();

        FilterType internalFilter;
        public DigitalFilter(double secControl, double cutoff, FilterType filter)
        {
            internalFilter = filter;
            switch (internalFilter)
            {
                case FilterType.LowPassFilter:
                    q = 1.0f / Sqrt(2);
                    omega = 2.0f * PI * cutoff / secControl;
                    alpha = Sin(omega) / (2.0f * q);
                    a0 = 1.0f + alpha;
                    a1 = -2.0f * Cos(omega);
                    a2 = 1.0f - alpha;
                    b0 = (1.0f - Cos(omega)) / 2.0f;
                    b1 = 1.0f - Cos(omega);
                    b2 = (1.0f - Cos(omega)) / 2.0f;
                    break;

                case FilterType.HighPassFilter:
                    q = 1.0f / Sqrt(2);
                    omega = 2.0f * PI * cutoff / secControl;
                    alpha = Sin(omega) / (2.0f * q);
                    a0 = 1.0f + alpha;
                    a1 = -2.0f * Cos(omega);
                    a2 = 1.0f - alpha;
                    b0 = (1.0 + Cos(omega)) / 2.0f;
                    b1 = -(1.0f + Cos(omega));
                    b2 = (1.0f + Cos(omega)) / 2.0f;
                    break;

                case FilterType.BandPassFilter:
                    omega = 2.0f * PI * cutoff / secControl;
                    alpha = Sin(omega) * Sinh(Log(2)) /
                                2.0f * bandWidth * omega / Sin(omega);
                    a0 = 1.0f + alpha;
                    a1 = -2.0f * Cos(omega);
                    a2 = 1.0f - alpha;
                    b0 = alpha;
                    b1 = 0.0f;
                    b2 = -alpha;
                    break;

                case FilterType.BandStopFilter:
                    omega = 2.0f * PI * cutoff / secControl;
                    alpha = Sin(omega) * Sinh(Log(2)) /
                                2.0f * bandWidth * omega / Sin(omega);
                    a0 = 1.0f + alpha;
                    a1 = -2.0f * Cos(omega);
                    a2 = 1.0f - alpha;
                    b0 = 1.0f;
                    b1 = -2.0f * Cos(omega);
                    b2 = 1.0f;
                    break;

                case FilterType.MovingAverageFilter:
                    averageNum = (int)cutoff;
                    break;
            }
        }
        public double FilterControl(double input)
        {
            switch (internalFilter)
            {
                case FilterType.MovingAverageFilter:
                    buffer.Enqueue(input);
                    double sum = 0.0f;
                    if (buffer.Count > averageNum)
                    {
                        buffer.Dequeue();
                    }
                    foreach (double data in buffer)
                    {
                        sum += data;
                    }
                    return (sum / averageNum);

                default:
                    double output = b0 / a0 * input +
                                            b1 / a0 * in1 +
                                            b2 / a0 * in2 -
                                            a1 / a0 * out1 -
                                            a2 / a0 * out2;
                    in2 = in1;   
                    in1 = input; 
                    out2 = out1;   
                    out1 = output; 

                    return output;
            }
        }
        public void bufferClear()
        {
            buffer.Clear();
        }
    }
}