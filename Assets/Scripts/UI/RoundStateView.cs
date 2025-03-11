using SimulatorEPL.Events;
using TMPro;
using UnityEngine;

namespace SimulatorEPL.UI
{
    public class RoundStateView : MonoBehaviour
    {
        [SerializeField]
        private TextMeshProUGUI stateText;

        private void Awake()
        {
            OnRoundStateChanged(0, RoundState.None);
            Messenger<int, RoundState>.AddListener(AppEvent.RoundStateChanged, OnRoundStateChanged);
        }

        private void OnDestroy()
        {
            Messenger<int, RoundState>.RemoveListener(AppEvent.RoundStateChanged, OnRoundStateChanged);
        }

        private void OnRoundStateChanged(int round, RoundState state)
        {
            string text = string.Empty;

            if (state == RoundState.Started)
            {
                //  text = $"Round: {round}     Time: 1";
                text = $"Round: {round}";
            }
            //else if (state == RoundState.HalfTime)
            //{
            //    text = $"Round: {round}     Half-time";
            //}
            //else if (state == RoundState.SecondTime)
            //{
            //    text = $"Round: {round}     Time: 2";
            //}
            else if (state == RoundState.Finished)
            {
                text = $"Round: {round}     Finished";
            }

            stateText.text = text;
        }
    }
}
