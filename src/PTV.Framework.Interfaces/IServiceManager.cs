﻿/**
 * The MIT License
 * Copyright (c) 2016 Population Register Centre (VRK)
 * 
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 * 
 * The above copyright notice and this permission notice shall be included in
 * all copies or substantial portions of the Software.
 * 
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
 * THE SOFTWARE.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;

namespace PTV.Framework.Interfaces
{
    public interface IServiceManager
    {
        IServiceResultWrap CallService(Func<IServiceResultWrap> serviceCall, Dictionary<Type, string> resultMessages, IMessage okMessage = null);
        
        Task<IServiceResultWrap> CallServiceAsync(Func<Task<IServiceResultWrap>> serviceCall, Dictionary<Type, string> resultMessages, IMessage okMessage = null);

        void CallRService(Func<IResolveManager, IClientProxy, IMessage> serviceCall,
            Dictionary<Type, string> resultMessages, IHubCallerClients clients, IMessage okMessage = null, Action<IClientProxy> sendRecieveMessage = null);

        /// <summary>
        /// Call action, i.e. service, and wrap the exceptions and result into generic result envelope
        /// </summary>
        /// <param name="serviceCall"></param>
        /// <param name="resultMessages"></param>
        /// <returns></returns>
        Task<IServiceResultWrap> CallServiceInSync(Func<IServiceResultWrap> serviceCall, Dictionary<Type, string> resultMessages, IMessage okMessage = null);
    }
}
