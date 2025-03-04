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
                .Append(canvasGroup.DOFade(1f, 0.05f))
                .AppendInterval(0.75f)
                .Append(canvasGroup.DOFade(0f, 0.20f))
                .SetLink(gameObject)
                .SetEase(Ease.Linear);
        }
    }
}