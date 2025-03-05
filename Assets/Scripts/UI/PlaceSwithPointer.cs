using DG.Tweening;
using UnityEngine;

namespace SimulatorEPL.UI
{
    public class PlaceSwithPointer : MonoBehaviour
    {
        [SerializeField]
        private CanvasGroup canvasGroupArrowUp;
        [SerializeField]
        private CanvasGroup canvasGroupArrowDown;

        private Tween arrowAnimation;

        private void Awake()
        {
            canvasGroupArrowUp.alpha = canvasGroupArrowDown.alpha = 0f;
        }

        public void ShowDirection(bool isUp)
        {
            arrowAnimation?.Kill();

            if (isUp)
                canvasGroupArrowDown.alpha = 0f;
            else
                canvasGroupArrowUp.alpha = 0f;

            CanvasGroup canvasGroup = isUp ? canvasGroupArrowUp : canvasGroupArrowDown;

            arrowAnimation = DOTween.Sequence()
                .SetLink(gameObject)
                .SetEase(Ease.Linear)
                .Append(canvasGroup.DOFade(1f, 0.2f))
                .AppendInterval(0.8f)
                .Append(canvasGroup.DOFade(0f, 0.3f));
        }
    }
}
