using UnityEngine;
using UnityEngine.UI;

namespace SimulatorEPL.UI.Express
{
    public class AllBetsView : MonoBehaviour
    {
        [SerializeField]
        private RectTransform viewsHolder;
        //[SerializeField]
        //private MatchExpressView expressViewPrefab;
        [Space]
        [SerializeField]
        private LayoutElement layoutElement;
        [SerializeField]
        private CanvasGroup canvasGroup;

        //  private readonly List<MatchExpressView> views = new List<MatchExpressView>();

        public void SetVisible(bool visible)
        {
            layoutElement.ignoreLayout = !visible;
            canvasGroup.alpha = visible ? 1f : 0f;
            canvasGroup.blocksRaycasts = visible;
        }
    }
}
