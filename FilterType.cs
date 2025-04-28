namespace DigitalFilter
{
    /// <summary>
    /// フィルターの種類を表します。
    /// </summary>
    public enum FilterType
    {
        /// <summary>
        /// 1次ローパスフィルター。低周波数成分を通過させ、高周波数成分を減衰させます。
        /// </summary>
        LowPassFilter_1st,

        /// <summary>
        /// 2次ローパスフィルター。1次よりも急峻な減衰特性を持ちます。
        /// </summary>
        LowPassFilter_2nd,

        /// <summary>
        /// 1次ハイパスフィルター。高周波数成分を通過させ、低周波数成分を減衰させます。
        /// </summary>
        HighPassFilter_1st,

        /// <summary>
        /// 2次ハイパスフィルター。1次よりも急峻な減衰特性を持ちます。
        /// </summary>
        HighPassFilter_2nd,

        /// <summary>
        /// バンドパスフィルター。特定の周波数帯域のみを通過させます。
        /// </summary>
        BandPassFilter,

        /// <summary>
        /// バンドストップフィルター。特定の周波数帯域のみを減衰させます。
        /// </summary>
        BandStopFilter,

        /// <summary>
        /// オールパスフィルター。全ての周波数を通過させますが、位相を変更します。
        /// </summary>
        AllPassFilter,

        /// <summary>
        /// 移動平均フィルター。データの平滑化に使用されます。
        /// </summary>
        MovingAverageFilter
    }
}
