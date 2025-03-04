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
        private TextMeshProUGUI handicapHome;
        [SerializeField]
        private TextMeshProUGUI handicapAway;
        [Space]
        [SerializeField]
        private TextMeshProUGUI totalMore;
        [SerializeField]
        private TextMeshProUGUI totalLess;

        public void SetGameCoef(GameCoefs coefs)
        {
            winHome.text = $"{coefs.winHome:N2}";
            winAway.text = $"{coefs.winAway:N2}";
            draw.text = $"{coefs.draw:N2}";

            handicapHome.text = $"{coefs.handicapHome:N2}";
            handicapAway.text = $"{coefs.handicapAway:N2}";

            totalMore.text = $"{coefs.totalMore:N2}";
            totalLess.text = $"{coefs.totalLess:N2}";
        }
    }
}
