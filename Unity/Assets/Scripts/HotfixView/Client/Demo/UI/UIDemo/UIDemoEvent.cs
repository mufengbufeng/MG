using System;
using UnityEngine;

namespace ET.Client
{
    [UIEvent(UIType.UIDemo)]
    public class UIDemoEvent : AUIEvent
    {
        public override async ETTask<UI> OnCreate(UIComponent uiComponent, UILayer uiLayer)
        {
            try
            {
                string assetsName = $"Assets/Bundles/UI/Demo/UIDemo/{UIType.UIDemo}.prefab";
                GameObject bundleGameObject =
                        await uiComponent.Scene().GetComponent<ResourcesLoaderComponent>().LoadAssetAsync<GameObject>(assetsName);
                GameObject gameObject = UnityEngine.Object.Instantiate(bundleGameObject, uiComponent.UIGlobalComponent.GetLayer((int)uiLayer));
                UI ui = uiComponent.AddChild<UI, string, GameObject>(UIType.UIDemo, gameObject);
                ui.AddComponent<UIDemoViewComponent>();
                return ui;
            }
            catch (Exception e)
            {
                Log.Error(e);
                throw;
            }
        }

        public override void OnRemove(UIComponent uiComponent)
        {
        }
    }
}