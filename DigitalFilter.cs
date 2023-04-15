using System;
using System.Collections.Generic;
using System.Linq;
using static System.Math;

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
        public readonly double omega;
        public readonly double alpha;
        public readonly double q;
        public readonly double a0;
        public readonly double a1;
        public readonly double a2;
        public readonly double b0;
        public readonly double b1;
        public readonly double b2;
        public readonly int averageNum;
        public readonly FilterType internalFilter;
        private Queue<double> buffer;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="controlHz">制御周波数(Hz)</param>
        /// <param name="cutoffHz">カットオフ周波数(Hz) MovingAverageFilterの場合は移動平均数</param>
        /// <param name="filterType">フィルターの種類</param>
        /// <param name="bandWidth">帯域幅(Default = 1octave)</param>
        public DigitalFilter(double controlHz,
                             double cutoffHz,
                             FilterType filterType,
                             double bandWidth = 1.0f)
        {
            internalFilter = filterType;
            omega = 2.0f * PI * cutoffHz / controlHz;
            q = 1.0f / Sqrt(2);
            switch (internalFilter)
            {
                case FilterType.LowPassFilter_1st:
                    alpha = Sin(omega) / (1.0 + Sin(omega));
                    b0 = alpha;
                    b1 = alpha;
                    a0 = 1.0 + alpha;
                    a1 = -Cos(omega) / (1.0 + alpha);
                    break;

                case FilterType.HighPassFilter_1st:
                    alpha = Sin(omega) / (1.0 + Sin(omega));
                    b0 = 1.0 / (1.0 + alpha);
                    b1 = -1.0 / (1.0 + alpha);
                    a0 = 1.0;
                    a1 = -Cos(omega) / (1.0 + alpha);
                    break;

                case FilterType.LowPassFilter_2nd:
                    alpha = Sin(omega) / (2.0f * q);
                    a0 = 1.0f + alpha;
                    a1 = -2.0f * Cos(omega);
                    a2 = 1.0f - alpha;
                    b0 = (1.0f - Cos(omega)) / 2.0f;
                    b1 = 1.0f - Cos(omega);
                    b2 = (1.0f - Cos(omega)) / 2.0f;
                    break;

                case FilterType.HighPassFilter_2nd:
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
                                2.0f * bandWidth * omega / Sin(omega);
                    a0 = 1.0f + alpha;
                    a1 = -2.0f * Cos(omega);
                    a2 = 1.0f - alpha;
                    b0 = alpha;
                    b1 = 0.0f;
                    b2 = -alpha;
                    break;

                case FilterType.BandStopFilter:
                    alpha = Sin(omega) * Sinh(Log(2)) /
                                2.0f * bandWidth * omega / Sin(omega);
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
                    averageNum = (int)cutoffHz;
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(filterType), filterType, null);
            }
        }

        /// <summary>
        /// 入力値に対するフィルタ適用値を返す
        /// </summary>
        /// <param name="input">フィルタ対象値</param>
        /// <returns>フィルタ適用値</returns>
        public double FilterControl(double input)
        {
            double output;
            switch (internalFilter)
            {
                case FilterType.LowPassFilter_1st:
                case FilterType.HighPassFilter_1st:
                    output = (b0 / a0 * input) +
                             (b1 / a0 * in1) -
                             (a1 / a0 * out1);
                    in1 = input;
                    out1 = output;
                    return output;

                case FilterType.MovingAverageFilter:
                    buffer.Enqueue(input);
                    if (buffer.Count > averageNum)
                    {
                        buffer.Dequeue();
                    }
                    return buffer.Sum() / averageNum;

                default:
                    output = (b0 / a0 * input) +
                             (b1 / a0 * in1) +
                             (b2 / a0 * in2) -
                             (a1 / a0 * out1) -
                             (a2 / a0 * out2);
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