using static System.Math;
using System.Collections.Generic;
using System.Linq;

namespace DigitalFilter
{
    /// <summary>
    /// 双二次変換を用いたデジタルフィルタ
    /// </summary>
    public class DigitalFilter
    {
        private double in1 = 0.0f;
        private double in2 = 0.0f;
        private double out1 = 0.0f;
        private double out2 = 0.0f;
        private Queue<double> buffer;
        public readonly double omega;
        public readonly double alpha;
        public readonly double internalBandWidth;
        public readonly double q;
        public readonly double a0;
        public readonly double a1;
        public readonly double a2;
        public readonly double b0;
        public readonly double b1;
        public readonly double b2;
        public readonly int averageNum;
        public readonly FilterType internalFilter;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="secControl">制御周期(sec)</param>
        /// <param name="cutoff">カットオフ周波数(Hz)</param>
        /// <param name="filterType">フィルターの種類</param>
        /// <param name="bandWidth">帯域幅(Default = 1octave)</param>
        public DigitalFilter(double secControl, double cutoff, FilterType filterType, double bandWidth = 1.0f)
        {
            internalBandWidth = bandWidth;
            internalFilter = filterType;
            omega = 2.0f * PI * cutoff / secControl;
            q = 1.0f / Sqrt(2);
            switch (internalFilter)
            {
                case FilterType.LowPassFilter:
                    alpha = Sin(omega) / (2.0f * q);
                    a0 = 1.0f + alpha;
                    a1 = -2.0f * Cos(omega);
                    a2 = 1.0f - alpha;
                    b0 = (1.0f - Cos(omega)) / 2.0f;
                    b1 = 1.0f - Cos(omega);
                    b2 = (1.0f - Cos(omega)) / 2.0f;
                    break;

                case FilterType.HighPassFilter:
                    alpha = Sin(omega) / (2.0f * q);
                    a0 = 1.0f + alpha;
                    a1 = -2.0f * Cos(omega);
                    a2 = 1.0f - alpha;
                    b0 = (1.0 + Cos(omega)) / 2.0f;
                    b1 = -(1.0f + Cos(omega));
                    b2 = (1.0f + Cos(omega)) / 2.0f;
                    break;

                case FilterType.BandPassFilter:
                    alpha = Sin(omega) * Sinh(Log(2)) /
                                2.0f * internalBandWidth * omega / Sin(omega);
                    a0 = 1.0f + alpha;
                    a1 = -2.0f * Cos(omega);
                    a2 = 1.0f - alpha;
                    b0 = alpha;
                    b1 = 0.0f;
                    b2 = -alpha;
                    break;

                case FilterType.BandStopFilter:
                    alpha = Sin(omega) * Sinh(Log(2)) /
                                2.0f * internalBandWidth * omega / Sin(omega);
                    a0 = 1.0f + alpha;
                    a1 = -2.0f * Cos(omega);
                    a2 = 1.0f - alpha;
                    b0 = 1.0f;
                    b1 = -2.0f * Cos(omega);
                    b2 = 1.0f;
                    break;

                case FilterType.AllPassFilter:
                    alpha = Sin(omega) / (2.0f * q);
                    a0 = 1.0f + alpha;
                    a1 = -2.0f * Cos(omega);
                    a2 = 1.0f - alpha;
                    b0 = 1.0f - alpha;
                    b1 = -2.0f * Cos(omega);
                    b2 = 1.0f + alpha;
                    break;

                case FilterType.MovingAverageFilter:
                    buffer = new Queue<double>();
                    averageNum = (int)cutoff;
                    break;
            }
        }

        /// <summary>
        /// 入力値に対するフィルタ適用値を返す
        /// </summary>
        /// <param name="input">フィルタ対象値</param>
        /// <returns>フィルタ適用値</returns>
        public double FilterControl(double input)
        {
            switch (internalFilter)
            {
                case FilterType.MovingAverageFilter:
                    buffer.Enqueue(input);
                    if (buffer.Count > averageNum)
                    {
                        buffer.Dequeue();
                    }
                    buffer.Sum();
                    return (buffer.Sum() / averageNum);

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

        /// <summary>
        /// キューバッファをクリアする
        /// </summary>
        public void BufferClear()
        {
            if (buffer != null)
            {
                buffer.Clear();
            }
        }
    }
}