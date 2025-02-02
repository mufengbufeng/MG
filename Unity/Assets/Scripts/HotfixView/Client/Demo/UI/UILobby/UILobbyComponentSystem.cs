﻿using UnityEngine;
using UnityEngine.UI;

namespace ET.Client
{
    [EntitySystemOf(typeof(UILobbyComponent))]
    [FriendOf(typeof(UILobbyComponent))]
    public static partial class UILobbyComponentSystem
    {
        [EntitySystem]
        private static void Awake(this UILobbyComponent self)
        {
            ReferenceCollector rc = self.GetParent<UI>().GameObject.GetComponent<ReferenceCollector>();

            self.enterMap = rc.Get<GameObject>("EnterMap");
            self.enterMap.GetComponent<Button>().onClick.AddListener(() => { self.EnterMap().Coroutine(); });
            self.EnterUIDemoBtn = rc.Get<GameObject>("EnterUIDemoBtn").GetComponent<Button>();
            self.EnterUIDemoBtn.onClick.AddListener(self.OnEnterUIDemoClick);
        }

        public static void OnEnterUIDemoClick(this UILobbyComponent self)
        {
            self.DoEnterUIDemo().Coroutine();
        }

        public static async ETTask DoEnterUIDemo(this UILobbyComponent self)
        {
            await UIHelper.Create(self.Root(), UIType.UIDemo, UILayer.Mid);
        }

        public static async ETTask EnterMap(this UILobbyComponent self)
        {
            Scene root = self.Root();
            await EnterMapHelper.EnterMapAsync(root);
            await UIHelper.Remove(root, UIType.UILobby);
        }
    }
}