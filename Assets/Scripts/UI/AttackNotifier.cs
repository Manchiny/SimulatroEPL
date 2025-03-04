using System.Text;
using TMPro;
using UnityEngine;

namespace SimulatorEPL.UI
{
    public class AttackNotifier : MonoBehaviour
    {
        [SerializeField]
        private TextMeshProUGUI text;

        private const int maxAttackLevel = 3;

        private GameSide? targetAttackSide;
        private int currentAttackLevel;

        public void Simulate()
        {
            if (targetAttackSide.HasValue)
            {
                AddAttackValue(targetAttackSide.Value);
                return;
            }

            int random = Random.Range(1, 3);
            AddAttackValue((GameSide)random);
        }

        public void SetTargetAttackSide(GameSide? attackSide, bool force)
        {
            targetAttackSide = attackSide;

            if (force && (!attackSide.HasValue || attackSide.Value == GameSide.None))
            {
                currentAttackLevel = 0;
                UpdateView();
                return;
            }

            if (!force && targetAttackSide.HasValue && targetAttackSide != GameSide.None )
            {
                if ((targetAttackSide.Value == GameSide.Home && currentAttackLevel < 0) || (targetAttackSide.Value == GameSide.Away && currentAttackLevel > 0))
                    currentAttackLevel = 0;

                AddAttackValue(targetAttackSide.Value);
            }
        }

        private void AddAttackValue(GameSide attackSide)
        {
            if (attackSide == GameSide.None)
            {
                if (currentAttackLevel < 0)
                    currentAttackLevel++;
                else if (currentAttackLevel > 0)
                    currentAttackLevel--;
            }
            else if (attackSide == GameSide.Home)
            {
                currentAttackLevel = Clamp(currentAttackLevel + 1);
            }
            else if (attackSide == GameSide.Away)
            {
                currentAttackLevel = Clamp(currentAttackLevel - 1);
            }

            UpdateView();
        }

        private void UpdateView()
        {
            if (currentAttackLevel == 0)
            {
                text.text = string.Empty;
                return;
            }

            StringBuilder builder = new StringBuilder();
            char symbol = currentAttackLevel < 0 ? '<' : '>';

            for (int i = 0; i < Mathf.Abs(currentAttackLevel); i++)
                builder.Append(symbol);

            text.text = builder.ToString();
        }

        private int Clamp(int value) => Mathf.Clamp(value, -maxAttackLevel, maxAttackLevel);
    }
}
