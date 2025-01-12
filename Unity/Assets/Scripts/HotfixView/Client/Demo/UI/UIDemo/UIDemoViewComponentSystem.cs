using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace ET.Client
{
    [EntitySystemOf(typeof(UIDemoViewComponent))]
    [FriendOfAttribute(typeof(ET.Client.UIDemoViewComponent))]
    public static partial class UIDemoViewComponentSystem
    {
        [EntitySystem]
        private static void Awake(this ET.Client.UIDemoViewComponent self)
        {
            Log.Info("UIDemoViewComponent Awake");
            self.BindUI();
            self.BindEvent();
            self.ChangeState(UIDemoState.End);
        }

        private static void BindUI(this ET.Client.UIDemoViewComponent self)
        {
            Log.Info("UIDemoViewComponent BindUI");
            self.RootGo = self.GetParent<UI>().GameObject.GetComponent<RectTransform>();
            var rc = self.RootGo.GetComponent<ReferenceCollector>();
            self.CubeBtn = rc.Get<GameObject>("CubeBtn").GetComponent<Button>();
            self.TimeText = rc.Get<GameObject>("TimeText").GetComponent<TextMeshProUGUI>();
            self.ScoresText = rc.Get<GameObject>("ScoresText").GetComponent<TextMeshProUGUI>();
            self.StartBtn = rc.Get<GameObject>("StartBtn").GetComponent<Button>();
            self.CubeImage = self.CubeBtn.GetComponent<Image>();
        }

        private static void BindEvent(this ET.Client.UIDemoViewComponent self)
        {
            self.CubeBtn.onClick.AddListener(self.OnCubeBtnClick);
            self.StartBtn.onClick.AddListener(self.onStartBtnClick);
        }

        private static void ChangeState(this ET.Client.UIDemoViewComponent self, UIDemoState state)
        {
            switch (state)
            {
                case UIDemoState.Start:
                    self.RefreshToStart().Coroutine();
                    break;
                case UIDemoState.End:
                    self.RefreshToEnd();
                    break;
            }

            self.State = state;
        }

        private static async ETTask RefreshToStart(this UIDemoViewComponent self)
        {
            if (self.State == UIDemoState.Start)
            {
                // 防止重复点击
                return;
            }

            self.StartBtn.gameObject.SetActive(false);
            self.CubeBtn.gameObject.SetActive(true);
            self.TimeText.gameObject.SetActive(true);
            self.ScoresText.gameObject.SetActive(true);
            self.TimeText.text = "Time: 60s";
            self.ScoresText.text = "Scores: 0";
            var timeComponent = self.Root().GetComponent<TimerComponent>();
            self.GameCancellationToken = new ETCancellationToken();
            int gameTimer = self.GameTimer;
            await timeComponent.WaitAsync(1000, self.GameCancellationToken);
            while (gameTimer > 0)
            {
                self.TimeText.text = $"Time: {gameTimer--}s";
                await timeComponent.WaitAsync(1000, self.GameCancellationToken);
            }

            // await timeComponent.WaitAsync(self.GameTimer * 1000, self.GameCancellationToken);
            self.ChangeState(UIDemoState.End);
        }

        private static void RefreshToEnd(this UIDemoViewComponent self)
        {
            self.StartBtn.gameObject.SetActive(true);
            self.CubeBtn.gameObject.SetActive(false);
            self.TimeText.gameObject.SetActive(false);
            self.ScoresText.gameObject.SetActive(false);
        }

        private static void OnCubeBtnClick(this ET.Client.UIDemoViewComponent self)
        {
            self.DoCubeBtnClick().Coroutine();
        }

        private static async ETTask DoCubeBtnClick(this ET.Client.UIDemoViewComponent self)
        {
            Log.Info("OnCubeBtnClick");
            if (self.State != UIDemoState.Start) return;

            await self.HandleCubeAnimation();
            self.UpdateScore();
            self.UpdateCubePosition();
            self.UpdateCubeColor();
        }

        private static void UpdateScore(this ET.Client.UIDemoViewComponent self)
        {
            self.ScoresText.text = $"分数：{self.Scores++}";
        }

        // 随机改变Cube位置
        private static void UpdateCubePosition(this ET.Client.UIDemoViewComponent self)
        {
            RectTransform rt = self.CubeBtn.GetComponent<RectTransform>();
            Vector2 minPos = new(0 + rt.rect.width / 2, 0 - rt.rect.height );
            Vector2 maxPos = new(self.RootGo.rect.width - rt.rect.width / 2, self.RootGo.rect.height - rt.rect.height);

            rt.anchoredPosition = new Vector2(Random.Range(minPos.x, maxPos.x),
                -Random.Range(minPos.y, maxPos.y));
            rt.position = new Vector3(rt.position.x, rt.position.y, rt.position.z);
        }

        // 随机改变Cube颜色
        private static void UpdateCubeColor(this ET.Client.UIDemoViewComponent self)
        {
            Color color = new(Random.Range(0, 1f), Random.Range(0, 1f), Random.Range(0, 1f), 1);
            Color show = new(color.r, color.g, color.b, 1);

            DOTween.To(() => self.CubeImage.color, x => self.CubeImage.color = x, color, 0.2f);
            // DOTween.To(() => self.CubeImage.color, x => self.CubeImage.color = x, show, 0.2f);
        }

        // 隐藏Cube
        private static async ETTask HandleCubeAnimation(this ET.Client.UIDemoViewComponent self)
        {
            Color originColor = self.CubeImage.color;
            Color hide = new(originColor.r, originColor.g, originColor.b, 0);

            ETTask task = ETTask.Create();
            DOTween.To(() => self.CubeImage.color, x => self.CubeImage.color = x, hide, 0.2f)
                    .OnComplete(() => { task.SetResult(); })
                    .Play();
            await task;
        }

        private static void onStartBtnClick(this ET.Client.UIDemoViewComponent self)
        {
            Log.Info("onStartBtnClick");
            self.ChangeState(UIDemoState.Start);
        }

        [EntitySystem]
        private static void Destroy(this ET.Client.UIDemoViewComponent self)
        {
            Log.Info("UIDemoViewComponent Destroy");
        }
    }
}