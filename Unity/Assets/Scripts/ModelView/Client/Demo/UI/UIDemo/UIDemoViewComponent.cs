using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace ET.Client
{
    [ComponentOf(typeof(UI))]
    public class UIDemoViewComponent : Entity, IAwake, IDestroy
    {
        public RectTransform RootGo;
        public Button CubeBtn;
        public Image CubeImage;
        public Button StartBtn;
        public TextMeshProUGUI TimeText;
        public TextMeshProUGUI ScoresText;

        public UIDemoState State = UIDemoState.End;
        public readonly int GameTimer = 60; // s
        public int Scores = 0;

        public ETCancellationToken GameCancellationToken;
    }

    public enum UIDemoState
    {
        Start,
        End,
    }
}