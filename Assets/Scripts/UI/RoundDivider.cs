using TMPro;
using UnityEngine;

namespace SimulatorEPL.UI
{
    public class RoundDivider : MonoBehaviour
{
        [SerializeField]
        private TextMeshProUGUI textRound;

        public void SetRoundNumber(int roundNumber)
        {
            textRound.text = $"Round: {roundNumber}";
        }
    }
}
