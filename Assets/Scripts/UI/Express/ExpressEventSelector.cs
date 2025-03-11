using SimulatorEPL.Events;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace SimulatorEPL.UI.Express
{
    public class ExpressEventSelector : MonoBehaviour
    {
        [SerializeField]
        private TextMeshProUGUI textCoef;
        [SerializeField]
        private Button buttonSelect;
        [SerializeField]
        private Color colorDefault;
        [SerializeField]
        private Color colorSelected;

        public double Coef { get; private set; }

        public bool Selected { get; private set; }

        public void Init(double coef)
        {
            Coef = coef;
            textCoef.text = Coef.ToString("N2");
            SetSelected(false);
        }

        private void Awake()
        {
            buttonSelect.onClick.AddListener(OnButtonClicked);
            Init(0);
        }

        private void OnButtonClicked()
        {
            SetSelected(!Selected);
        }    

        private void SetSelected(bool selected)
        {
            Selected = selected;
            buttonSelect.image.color = Selected ? colorSelected : colorDefault;
            Messenger<ExpressEventSelector>.Broadcast(AppEvent.ExpressEventSelectChanged, this);
        }
    }
}
