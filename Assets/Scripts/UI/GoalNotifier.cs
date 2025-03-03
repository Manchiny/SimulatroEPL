using DG.Tweening;
using UnityEngine;

namespace SimulatorEPL.UI
{
    public class GoalNotifier : MonoBehaviour
    {
        [SerializeField]
        private CanvasGroup canvasGroup;

        private Tween showHideAnimation;

        private void Awake()
        {
            canvasGroup.alpha = 0f;
        }

        public void Show()
        {
            showHideAnimation?.Kill();

            showHideAnimation = DOTween.Sequence()
                .Append(canvasGroup.DOFade(1f, 0.25f))
                .AppendInterval(1f)
                .Append(canvasGroup.DOFade(0f, 0.25f))
                .SetLink(gameObject)
                .SetEase(Ease.Linear);
        }
    }
}