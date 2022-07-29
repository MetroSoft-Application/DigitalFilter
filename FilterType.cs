using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DigitalFilter
{
    /// <summary>
    /// フィルターの種類
    /// </summary>
    public enum FilterType
    {
        LowPassFilter,
        HighPassFilter,
        BandPassFilter,
        BandStopFilter,
        AllPassFilter,
        MovingAverageFilter
    }
}
