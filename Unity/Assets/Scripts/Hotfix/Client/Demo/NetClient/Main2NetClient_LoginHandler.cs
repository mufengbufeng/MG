﻿using System;
using System.Net;
using System.Net.Sockets;

namespace ET.Client
{
    [MessageHandler(SceneType.NetClient)]
    public class Main2NetClient_LoginHandler : MessageHandler<Scene, Main2NetClient_Login, NetClient2Main_Login>
    {
        protected override async ETTask Run(Scene root, Main2NetClient_Login request, NetClient2Main_Login response)
        {
            string account = request.Account;
            string password = request.Password;
            // 创建一个ETModel层的Session
            root.RemoveComponent<RouterAddressComponent>();
            // 获取路由跟realmDispatcher地址
            RouterAddressComponent routerAddressComponent =
                    root.AddComponent<RouterAddressComponent, string, int>(ConstValue.RouterHttpHost, ConstValue.RouterHttpPort);
            await routerAddressComponent.Init();
            root.AddComponent<NetComponent, AddressFamily, NetworkProtocol>(routerAddressComponent.RouterManagerIPAddress.AddressFamily,
                NetworkProtocol.UDP);
            root.GetComponent<FiberParentComponent>().ParentFiberId = request.OwnerFiberId;

            NetComponent netComponent = root.GetComponent<NetComponent>();

            IPEndPoint realmAddress = routerAddressComponent.GetRealmAddress(account);

            R2C_LoginAccount r2CLogin;

            using (Session session = await netComponent.CreateRouterSession(realmAddress, account, password))
            {
                C2R_LoginAccount c2RLoginAccount = C2R_LoginAccount.Create();
                c2RLoginAccount.AccountName = account;
                c2RLoginAccount.Password = password;
                r2CLogin = (R2C_LoginAccount)await session.Call(c2RLoginAccount);
                if (r2CLogin.Error == ErrorCode.ERR_Success)
                {
                    root.AddComponent<SessionComponent>().Session = session;
                }
                else
                {
                    session?.Dispose();
                    Log.Error("登录失败，错误码：" + r2CLogin.Error);
                }

                Log.Info("登录成功，Token：" + r2CLogin.Token);
                response.Token = r2CLogin.Token;
                response.Error = r2CLogin.Error;
            }
        }
    }
}