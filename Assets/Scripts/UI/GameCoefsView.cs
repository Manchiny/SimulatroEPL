using TMPro;
using UnityEngine;

namespace SimulatorEPL.UI
{
    public class GameCoefsView : MonoBehaviour
    {
        [SerializeField]
        private TextMeshProUGUI winHome;
        [SerializeField]
        private TextMeshProUGUI winAway;
        [SerializeField]
        private TextMeshProUGUI draw;
        [Space]
        [SerializeField]
        private TextMeshProUGUI handicapHomeAvg;
        [SerializeField]
        private TextMeshProUGUI handicapHome;
        [SerializeField]
        private TextMeshProUGUI handicapAwayAvg;
        [SerializeField]
        private TextMeshProUGUI handicapAway;
        [Space]
        [SerializeField]
        private TextMeshProUGUI nextScoreHome;
        [SerializeField]
        private TextMeshProUGUI nextScoreAway;
        [Space]
        [SerializeField]
        private TextMeshProUGUI totalSmallAdv;
        [SerializeField]
        private TextMeshProUGUI totalSmallOver;
        [SerializeField]
        private TextMeshProUGUI totalSmallUnder;
        [Space]
        [SerializeField]
        private TextMeshProUGUI totalBigAdv;
        [SerializeField]
        private TextMeshProUGUI totalBigOver;
        [SerializeField]
        private TextMeshProUGUI totalBigUnder;

        public void SetGameCoef(MatchCoefs coefs)
        {
            winHome.text = GetCoefString(coefs.winHome);
            winAway.text = GetCoefString(coefs.winAway);
            draw.text = GetCoefString(coefs.draw);

            handicapHomeAvg.text = coefs.HandyHomeAvg > 0 ? $"+{coefs.HandyHomeAvg}" : coefs.HandyHomeAvg.ToString("N1");
            handicapHome.text = GetCoefString(coefs.handyHome);
            handicapAwayAvg.text = coefs.HandyAwayAvg > 0 ? $"+{coefs.HandyAwayAvg}" : coefs.HandyAwayAvg.ToString("N1");
            handicapAway.text = GetCoefString(coefs.handyAway);

            nextScoreHome.text = GetCoefString(coefs.nextScoreHome);
            nextScoreAway.text = GetCoefString(coefs.nextScoreAway);

            totalSmallAdv.text = coefs.TotalSmallAdv.ToString("N1");
            totalSmallUnder.text = GetCoefString(coefs.totalSmallUnder);
            totalSmallOver.text = GetCoefString(coefs.totalSmallOver);

            totalBigAdv.text = coefs.TotalBigAdv.ToString("N1");
            totalBigOver.text = GetCoefString(coefs.totalBigOver);
            totalBigUnder.text = GetCoefString(coefs.totalBigUnder);
        }

        private string GetCoefString(double coef)
        {
            return (coef <= 1 || coef > 20) ? "-" : coef.ToString("N2");
        }
    }
}
