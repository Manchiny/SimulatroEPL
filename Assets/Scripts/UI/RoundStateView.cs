using SimulatorEPL.Events;
using TMPro;
using UnityEngine;

namespace SimulatorEPL.UI
{
    public class RoundStateView : MonoBehaviour
    {
        [SerializeField]
        private TextMeshProUGUI stateText;

        private int roundCounter;

        private void Awake()
        {
            Messenger<RoundState>.AddListener(AppEvent.RoundStateChanged, OnRoundStateChanged);
            OnRoundStateChanged(RoundState.None);
        }

        private void OnDestroy()
        {
            Messenger<RoundState>.RemoveListener(AppEvent.RoundStateChanged, OnRoundStateChanged);
        }

        private void OnRoundStateChanged(RoundState state)
        {
            string text = string.Empty;

            if (state == RoundState.FirstTime)
            {
                roundCounter++;
                text = $"Round: {roundCounter} Time: 1";
            }
            else if (state == RoundState.HalfTime)
            {
                text = $"Round: {roundCounter} Half-time";
            }
            else if (state == RoundState.SecondTime)
            {
                text = $"Round: {roundCounter} Time: 2";
            }
            else if (state == RoundState.FullTime)
            {
                text = $"Round: {roundCounter} Full time";
            }

            stateText.text = text;
        }
    }
}
