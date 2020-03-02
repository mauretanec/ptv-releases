/**
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
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     //
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using PTV.SoapServices.Interfaces.Lingsoft;

namespace LingsoftProd
{
    [System.CodeDom.Compiler.GeneratedCodeAttribute("dotnet-svcutil", "1.0.0.0")]
    [System.ServiceModel.ServiceContractAttribute(Namespace="https://llso.lingsoft.fi/api/v2/", ConfigurationName="Lingsoft.LLSOrdersPort")]
    public interface LLSOrdersPort
    {
        
        [System.ServiceModel.OperationContractAttribute(Action="https://llso.lingsoft.fi/api/v2/#OrderStatus", ReplyAction="*")]
        [System.ServiceModel.XmlSerializerFormatAttribute(SupportFaults=true)]
        System.Threading.Tasks.Task<LingsoftProd.OrderStatusResponse> OrderStatusAsync(LingsoftProd.OrderStatusRequest request);
        
        [System.ServiceModel.OperationContractAttribute(Action="https://llso.lingsoft.fi/api/v2/#OrderStatuses", ReplyAction="*")]
        [System.ServiceModel.XmlSerializerFormatAttribute(SupportFaults=true)]
        System.Threading.Tasks.Task<LingsoftProd.OrderStatusesResponse> OrderStatusesAsync(LingsoftProd.OrderStatusesRequest request);
        
        [System.ServiceModel.OperationContractAttribute(Action="https://llso.lingsoft.fi/api/v2/#CancelOrder", ReplyAction="*")]
        [System.ServiceModel.XmlSerializerFormatAttribute(SupportFaults=true)]
        System.Threading.Tasks.Task<LingsoftProd.CancelOrderResponse> CancelOrderAsync(LingsoftProd.CancelOrderRequest request);
        
        [System.ServiceModel.OperationContractAttribute(Action="https://llso.lingsoft.fi/api/v2/#QueryWorkId", ReplyAction="*")]
        [System.ServiceModel.XmlSerializerFormatAttribute(SupportFaults=true)]
        System.Threading.Tasks.Task<LingsoftProd.QueryWorkIdResponse> QueryWorkIdAsync(LingsoftProd.QueryWorkIdRequest request);
        
        [System.ServiceModel.OperationContractAttribute(Action="https://llso.lingsoft.fi/api/v2/#UpdateOrder", ReplyAction="*")]
        [System.ServiceModel.XmlSerializerFormatAttribute(SupportFaults=true)]
        System.Threading.Tasks.Task<LingsoftProd.UpdateOrderResponse> UpdateOrderAsync(LingsoftProd.UpdateOrderRequest request);
        
        [System.ServiceModel.OperationContractAttribute(Action="https://llso.lingsoft.fi/api/v2/#NewOrder", ReplyAction="*")]
        [System.ServiceModel.XmlSerializerFormatAttribute(SupportFaults=true)]
        System.Threading.Tasks.Task<LingsoftProd.OrderResponse> NewOrderAsync(LingsoftProd.NewOrderRequest request);
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("dotnet-svcutil", "1.0.0.0")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace = "https://llso.lingsoft.fi/api/v2/")]
    public partial class Status : IStatus
    {
        
        private string orderIDField;
        
        private string stateField;
        
        private string contactField;
        
        private string phoneNumberField;
        
        private string validationResultField;
        
        private string fileUrlField;
        
        private string quoteField;
        
        private string deadlineField;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified, IsNullable=true)]
        public string orderID
        {
            get
            {
                return this.orderIDField;
            }
            set
            {
                this.orderIDField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified, IsNullable=true)]
        public string state
        {
            get
            {
                return this.stateField;
            }
            set
            {
                this.stateField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified, IsNullable=true)]
        public string contact
        {
            get
            {
                return this.contactField;
            }
            set
            {
                this.contactField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified, IsNullable=true)]
        public string phoneNumber
        {
            get
            {
                return this.phoneNumberField;
            }
            set
            {
                this.phoneNumberField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified, IsNullable=true)]
        public string validationResult
        {
            get
            {
                return this.validationResultField;
            }
            set
            {
                this.validationResultField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified, IsNullable=true)]
        public string fileUrl
        {
            get
            {
                return this.fileUrlField;
            }
            set
            {
                this.fileUrlField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified, IsNullable=true)]
        public string quote
        {
            get
            {
                return this.quoteField;
            }
            set
            {
                this.quoteField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified, IsNullable=true)]
        public string deadline
        {
            get
            {
                return this.deadlineField;
            }
            set
            {
                this.deadlineField = value;
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("dotnet-svcutil", "1.0.0.0")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace = "https://llso.lingsoft.fi/api/v2/")]
    public partial class trgLang
    {
        
        private string trgLang1Field;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("trgLang", Form=System.Xml.Schema.XmlSchemaForm.Unqualified, IsNullable=true)]
        public string trgLang1
        {
            get
            {
                return this.trgLang1Field;
            }
            set
            {
                this.trgLang1Field = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("dotnet-svcutil", "1.0.0.0")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="https://llso.lingsoft.fi/api/v2/")]
    public partial class Order : IOrder
    {
        
        private string orderIDField;
        
        private string endCustomerField;
        
        private string ordererAccountField;
        
        private string emailField;
        
        private string pmField;
        
        private string deadlineField;
        
        private string clientRefField;
        
        private string clientInfoField;
        
        private string vendorInstructionsField;
        
        private System.Nullable<int> workTypeField;
        
        private System.Nullable<int> docTypeField;
        
        private string orderTypeField;
        
        private string filesUrlField;
        
        private string downloadedUrlField;
        
        private string analysisField;
        
        private string projectReadyCallbackUrlField;
        
        private string nameField;
        
        private string srcLangField;
        
        private trgLang[] trgLangListField;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified, IsNullable=true)]
        public string orderID
        {
            get
            {
                return this.orderIDField;
            }
            set
            {
                this.orderIDField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified, IsNullable=true)]
        public string endCustomer
        {
            get
            {
                return this.endCustomerField;
            }
            set
            {
                this.endCustomerField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified, IsNullable=true)]
        public string ordererAccount
        {
            get
            {
                return this.ordererAccountField;
            }
            set
            {
                this.ordererAccountField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified, IsNullable=true)]
        public string email
        {
            get
            {
                return this.emailField;
            }
            set
            {
                this.emailField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified, IsNullable=true)]
        public string pm
        {
            get
            {
                return this.pmField;
            }
            set
            {
                this.pmField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified, IsNullable=true)]
        public string deadline
        {
            get
            {
                return this.deadlineField;
            }
            set
            {
                this.deadlineField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified, IsNullable=true)]
        public string clientRef
        {
            get
            {
                return this.clientRefField;
            }
            set
            {
                this.clientRefField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified, IsNullable=true)]
        public string clientInfo
        {
            get
            {
                return this.clientInfoField;
            }
            set
            {
                this.clientInfoField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified, IsNullable=true)]
        public string vendorInstructions
        {
            get
            {
                return this.vendorInstructionsField;
            }
            set
            {
                this.vendorInstructionsField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified, IsNullable=true)]
        public System.Nullable<int> workType
        {
            get
            {
                return this.workTypeField;
            }
            set
            {
                this.workTypeField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified, IsNullable=true)]
        public System.Nullable<int> docType
        {
            get
            {
                return this.docTypeField;
            }
            set
            {
                this.docTypeField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified, IsNullable=true)]
        public string orderType
        {
            get
            {
                return this.orderTypeField;
            }
            set
            {
                this.orderTypeField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified, IsNullable=true)]
        public string filesUrl
        {
            get
            {
                return this.filesUrlField;
            }
            set
            {
                this.filesUrlField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified, IsNullable=true)]
        public string downloadedUrl
        {
            get
            {
                return this.downloadedUrlField;
            }
            set
            {
                this.downloadedUrlField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified, IsNullable=true)]
        public string analysis
        {
            get
            {
                return this.analysisField;
            }
            set
            {
                this.analysisField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified, IsNullable=true)]
        public string projectReadyCallbackUrl
        {
            get
            {
                return this.projectReadyCallbackUrlField;
            }
            set
            {
                this.projectReadyCallbackUrlField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified, IsNullable=true)]
        public string name
        {
            get
            {
                return this.nameField;
            }
            set
            {
                this.nameField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified, IsNullable=true)]
        public string srcLang
        {
            get
            {
                return this.srcLangField;
            }
            set
            {
                this.srcLangField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlArrayAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified, IsNullable=true)]
        [System.Xml.Serialization.XmlArrayItemAttribute("item", Form=System.Xml.Schema.XmlSchemaForm.Unqualified, IsNullable=false)]
        public trgLang[] trgLangList
        {
            get
            {
                return this.trgLangListField;
            }
            set
            {
                this.trgLangListField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("dotnet-svcutil", "1.0.0.0")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="https://llso.lingsoft.fi/api/v2/")]
    public partial class StatusArray
    {
        
        private Status[] statusesField;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlArrayAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified, IsNullable=true)]
        [System.Xml.Serialization.XmlArrayItemAttribute("item", Form=System.Xml.Schema.XmlSchemaForm.Unqualified, IsNullable=false)]
        public Status[] statuses
        {
            get
            {
                return this.statusesField;
            }
            set
            {
                this.statusesField = value;
            }
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("dotnet-svcutil", "1.0.0.0")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.ServiceModel.MessageContractAttribute(WrapperName="OrderStatus", WrapperNamespace="https://llso.lingsoft.fi/api/v2/", IsWrapped=true)]
    public partial class OrderStatusRequest
    {
        
        [System.ServiceModel.MessageBodyMemberAttribute(Namespace="https://llso.lingsoft.fi/api/v2/", Order=0)]
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string llsoWorkID;
        
        public OrderStatusRequest()
        {
        }
        
        public OrderStatusRequest(string llsoWorkID)
        {
            this.llsoWorkID = llsoWorkID;
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("dotnet-svcutil", "1.0.0.0")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.ServiceModel.MessageContractAttribute(WrapperName="OrderStatusResponse", WrapperNamespace="https://llso.lingsoft.fi/api/v2/", IsWrapped=true)]
    public partial class OrderStatusResponse : IOrderStatusResponse
    {
        
        [System.ServiceModel.MessageBodyMemberAttribute(Namespace="https://llso.lingsoft.fi/api/v2/", Order=0)]
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public LingsoftProd.Status OrderStatusResult;

        IStatus IOrderStatusResponse.OrderStatusResult
        {
            get { return OrderStatusResult; }
        }

        public OrderStatusResponse()
        {
        }
        
        public OrderStatusResponse(LingsoftProd.Status OrderStatusResult)
        {
            this.OrderStatusResult = OrderStatusResult;
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("dotnet-svcutil", "1.0.0.0")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.ServiceModel.MessageContractAttribute(WrapperName="OrderStatuses", WrapperNamespace="https://llso.lingsoft.fi/api/v2/", IsWrapped=true)]
    public partial class OrderStatusesRequest
    {
        
        [System.ServiceModel.MessageBodyMemberAttribute(Namespace="https://llso.lingsoft.fi/api/v2/", Order=0)]
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string llsoWorkIDs;
        
        public OrderStatusesRequest()
        {
        }
        
        public OrderStatusesRequest(string llsoWorkIDs)
        {
            this.llsoWorkIDs = llsoWorkIDs;
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("dotnet-svcutil", "1.0.0.0")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.ServiceModel.MessageContractAttribute(WrapperName="OrderStatusesResponse", WrapperNamespace="https://llso.lingsoft.fi/api/v2/", IsWrapped=true)]
    public partial class OrderStatusesResponse
    {
        
        [System.ServiceModel.MessageBodyMemberAttribute(Namespace="https://llso.lingsoft.fi/api/v2/", Order=0)]
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public LingsoftProd.StatusArray OrderStatusesResult;
        
        public OrderStatusesResponse()
        {
        }
        
        public OrderStatusesResponse(LingsoftProd.StatusArray OrderStatusesResult)
        {
            this.OrderStatusesResult = OrderStatusesResult;
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("dotnet-svcutil", "1.0.0.0")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.ServiceModel.MessageContractAttribute(WrapperName="CancelOrder", WrapperNamespace="https://llso.lingsoft.fi/api/v2/", IsWrapped=true)]
    public partial class CancelOrderRequest
    {
        
        [System.ServiceModel.MessageBodyMemberAttribute(Namespace="https://llso.lingsoft.fi/api/v2/", Order=0)]
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string llsoWorkID;
        
        public CancelOrderRequest()
        {
        }
        
        public CancelOrderRequest(string llsoWorkID)
        {
            this.llsoWorkID = llsoWorkID;
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("dotnet-svcutil", "1.0.0.0")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.ServiceModel.MessageContractAttribute(WrapperName="CancelOrderResponse", WrapperNamespace="https://llso.lingsoft.fi/api/v2/", IsWrapped=true)]
    public partial class CancelOrderResponse : ICancelOrderResponse
    {
        
        [System.ServiceModel.MessageBodyMemberAttribute(Namespace="https://llso.lingsoft.fi/api/v2/", Order=0)]
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public bool CancelOrderResult { get; set; }
        
        public CancelOrderResponse()
        {
        }
        
        public CancelOrderResponse(bool CancelOrderResult)
        {
            this.CancelOrderResult = CancelOrderResult;
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("dotnet-svcutil", "1.0.0.0")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.ServiceModel.MessageContractAttribute(WrapperName="QueryWorkId", WrapperNamespace="https://llso.lingsoft.fi/api/v2/", IsWrapped=true)]
    public partial class QueryWorkIdRequest
    {
        
        [System.ServiceModel.MessageBodyMemberAttribute(Namespace="https://llso.lingsoft.fi/api/v2/", Order=0)]
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string orderID;
        
        public QueryWorkIdRequest()
        {
        }
        
        public QueryWorkIdRequest(string orderID)
        {
            this.orderID = orderID;
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("dotnet-svcutil", "1.0.0.0")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.ServiceModel.MessageContractAttribute(WrapperName="QueryWorkIdResponse", WrapperNamespace="https://llso.lingsoft.fi/api/v2/", IsWrapped=true)]
    public partial class QueryWorkIdResponse
    {
        
        [System.ServiceModel.MessageBodyMemberAttribute(Namespace="https://llso.lingsoft.fi/api/v2/", Order=0)]
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string QueryWorkIdResult;
        
        public QueryWorkIdResponse()
        {
        }
        
        public QueryWorkIdResponse(string QueryWorkIdResult)
        {
            this.QueryWorkIdResult = QueryWorkIdResult;
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("dotnet-svcutil", "1.0.0.0")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.ServiceModel.MessageContractAttribute(WrapperName="UpdateOrder", WrapperNamespace="https://llso.lingsoft.fi/api/v2/", IsWrapped=true)]
    public partial class UpdateOrderRequest
    {
        
        [System.ServiceModel.MessageBodyMemberAttribute(Namespace="https://llso.lingsoft.fi/api/v2/", Order=0)]
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string llsoWorkID;
        
        [System.ServiceModel.MessageBodyMemberAttribute(Namespace="https://llso.lingsoft.fi/api/v2/", Order=1)]
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public LingsoftProd.Order order;
        
        public UpdateOrderRequest()
        {
        }
        
        public UpdateOrderRequest(string llsoWorkID, LingsoftProd.Order order)
        {
            this.llsoWorkID = llsoWorkID;
            this.order = order;
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("dotnet-svcutil", "1.0.0.0")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.ServiceModel.MessageContractAttribute(WrapperName="UpdateOrderResponse", WrapperNamespace="https://llso.lingsoft.fi/api/v2/", IsWrapped=true)]
    public partial class UpdateOrderResponse : IUpdateOrderResponse
    {
        
        [System.ServiceModel.MessageBodyMemberAttribute(Namespace="https://llso.lingsoft.fi/api/v2/", Order=0)]
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string UpdateOrderResult { get; set; }

        public UpdateOrderResponse()
        {
        }
        
        public UpdateOrderResponse(string UpdateOrderResult)
        {
            this.UpdateOrderResult = UpdateOrderResult;
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("dotnet-svcutil", "1.0.0.0")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.ServiceModel.MessageContractAttribute(WrapperName="NewOrder", WrapperNamespace="https://llso.lingsoft.fi/api/v2/", IsWrapped=true)]
    public partial class NewOrderRequest
    {
        
        [System.ServiceModel.MessageBodyMemberAttribute(Namespace="https://llso.lingsoft.fi/api/v2/", Order=0)]
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public LingsoftProd.Order order;
        
        public NewOrderRequest()
        {
        }
        
        public NewOrderRequest(LingsoftProd.Order order)
        {
            this.order = order;
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("dotnet-svcutil", "1.0.0.0")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.ServiceModel.MessageContractAttribute(WrapperName="NewOrderResponse", WrapperNamespace="https://llso.lingsoft.fi/api/v2/", IsWrapped=true)]
    public partial class OrderResponse : IOrderResponse
    {

        [System.ServiceModel.MessageBodyMemberAttribute(Namespace = "https://llso.lingsoft.fi/api/v2/", Order = 0)]
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string NewOrderResult { get; set; }

        public OrderResponse()
        {
        }
        
        public OrderResponse(string NewOrderResult)
        {
            this.NewOrderResult = NewOrderResult;
        }

    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("dotnet-svcutil", "1.0.0.0")]
    public interface LLSOrdersPortChannel : LingsoftProd.LLSOrdersPort, System.ServiceModel.IClientChannel
    {
    }

    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("dotnet-svcutil", "1.0.0.0")]
    public partial class LingsoftClient : System.ServiceModel.ClientBase<LingsoftProd.LLSOrdersPort>, LingsoftProd.LLSOrdersPort, ILingsoftClientManager
    {
        
    /// <summary>
    /// Implement this partial method to configure the service endpoint.
    /// </summary>
    /// <param name="serviceEndpoint">The endpoint to configure</param>
    /// <param name="clientCredentials">The client credentials</param>
    static partial void ConfigureEndpoint(System.ServiceModel.Description.ServiceEndpoint serviceEndpoint, System.ServiceModel.Description.ClientCredentials clientCredentials);
        
        public LingsoftClient() : 
                base(LingsoftClient.GetDefaultBinding(), LingsoftClient.GetDefaultEndpointAddress())
        {
            this.Endpoint.Name = EndpointConfiguration.LLSOrdersPort.ToString();
            ConfigureEndpoint(this.Endpoint, this.ClientCredentials);
        }
        
        public LingsoftClient(EndpointConfiguration endpointConfiguration) : 
                base(LingsoftClient.GetBindingForEndpoint(endpointConfiguration), LingsoftClient.GetEndpointAddress(endpointConfiguration))
        {
            this.Endpoint.Name = endpointConfiguration.ToString();
            ConfigureEndpoint(this.Endpoint, this.ClientCredentials);
        }
        
        public LingsoftClient(EndpointConfiguration endpointConfiguration, string remoteAddress) : 
                base(LingsoftClient.GetBindingForEndpoint(endpointConfiguration), new System.ServiceModel.EndpointAddress(remoteAddress))
        {
            this.Endpoint.Name = endpointConfiguration.ToString();
            ConfigureEndpoint(this.Endpoint, this.ClientCredentials);
        }
        
        public LingsoftClient(EndpointConfiguration endpointConfiguration, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(LingsoftClient.GetBindingForEndpoint(endpointConfiguration), remoteAddress)
        {
            this.Endpoint.Name = endpointConfiguration.ToString();
            ConfigureEndpoint(this.Endpoint, this.ClientCredentials);
        }
        
        public LingsoftClient(System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(binding, remoteAddress)
        {
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        System.Threading.Tasks.Task<LingsoftProd.OrderStatusResponse> LingsoftProd.LLSOrdersPort.OrderStatusAsync(LingsoftProd.OrderStatusRequest request)
        {
            return base.Channel.OrderStatusAsync(request);
        }
        
        public System.Threading.Tasks.Task<LingsoftProd.OrderStatusResponse> OrderStatusAsync(string llsoWorkID)
        {
            LingsoftProd.OrderStatusRequest inValue = new LingsoftProd.OrderStatusRequest();
            inValue.llsoWorkID = llsoWorkID;
            return ((LingsoftProd.LLSOrdersPort)(this)).OrderStatusAsync(inValue);
        }


        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        System.Threading.Tasks.Task<LingsoftProd.OrderStatusesResponse> LingsoftProd.LLSOrdersPort.OrderStatusesAsync(LingsoftProd.OrderStatusesRequest request)
        {
            return base.Channel.OrderStatusesAsync(request);
        }
        
        public System.Threading.Tasks.Task<LingsoftProd.OrderStatusesResponse> OrderStatusesAsync(string llsoWorkIDs)
        {
            LingsoftProd.OrderStatusesRequest inValue = new LingsoftProd.OrderStatusesRequest();
            inValue.llsoWorkIDs = llsoWorkIDs;
            return ((LingsoftProd.LLSOrdersPort)(this)).OrderStatusesAsync(inValue);
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        System.Threading.Tasks.Task<LingsoftProd.CancelOrderResponse> LingsoftProd.LLSOrdersPort.CancelOrderAsync(LingsoftProd.CancelOrderRequest request)
        {
            return base.Channel.CancelOrderAsync(request);
        }
        
        public System.Threading.Tasks.Task<LingsoftProd.CancelOrderResponse> CancelOrderAsync(string llsoWorkID)
        {
            LingsoftProd.CancelOrderRequest inValue = new LingsoftProd.CancelOrderRequest();
            inValue.llsoWorkID = llsoWorkID;
            return ((LingsoftProd.LLSOrdersPort)(this)).CancelOrderAsync(inValue);
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        System.Threading.Tasks.Task<LingsoftProd.QueryWorkIdResponse> LingsoftProd.LLSOrdersPort.QueryWorkIdAsync(LingsoftProd.QueryWorkIdRequest request)
        {
            return base.Channel.QueryWorkIdAsync(request);
        }
        
        public System.Threading.Tasks.Task<LingsoftProd.QueryWorkIdResponse> QueryWorkIdAsync(string orderID)
        {
            LingsoftProd.QueryWorkIdRequest inValue = new LingsoftProd.QueryWorkIdRequest();
            inValue.orderID = orderID;
            return ((LingsoftProd.LLSOrdersPort)(this)).QueryWorkIdAsync(inValue);
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        System.Threading.Tasks.Task<LingsoftProd.UpdateOrderResponse> LingsoftProd.LLSOrdersPort.UpdateOrderAsync(LingsoftProd.UpdateOrderRequest request)
        {
            return base.Channel.UpdateOrderAsync(request);
        }
        
        public System.Threading.Tasks.Task<LingsoftProd.UpdateOrderResponse> UpdateOrderAsync(string llsoWorkID, LingsoftProd.Order order)
        {
            LingsoftProd.UpdateOrderRequest inValue = new LingsoftProd.UpdateOrderRequest();
            inValue.llsoWorkID = llsoWorkID;
            inValue.order = order;
            return ((LingsoftProd.LLSOrdersPort)(this)).UpdateOrderAsync(inValue);
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        System.Threading.Tasks.Task<LingsoftProd.OrderResponse> LingsoftProd.LLSOrdersPort.NewOrderAsync(LingsoftProd.NewOrderRequest request)
        {
            return base.Channel.NewOrderAsync(request);
        }
        
        public System.Threading.Tasks.Task<LingsoftProd.OrderResponse> NewOrderAsync(LingsoftProd.Order order)
        {
            LingsoftProd.NewOrderRequest inValue = new LingsoftProd.NewOrderRequest();
            inValue.order = order;
            return ((LingsoftProd.LLSOrdersPort)(this)).NewOrderAsync(inValue);
        }
        
        public virtual System.Threading.Tasks.Task OpenAsync()
        {
            return System.Threading.Tasks.Task.Factory.FromAsync(((System.ServiceModel.ICommunicationObject)(this)).BeginOpen(null, null), new System.Action<System.IAsyncResult>(((System.ServiceModel.ICommunicationObject)(this)).EndOpen));
        }
        
        public virtual System.Threading.Tasks.Task CloseAsync()
        {
            return System.Threading.Tasks.Task.Factory.FromAsync(((System.ServiceModel.ICommunicationObject)(this)).BeginClose(null, null), new System.Action<System.IAsyncResult>(((System.ServiceModel.ICommunicationObject)(this)).EndClose));
        }
        
        private static System.ServiceModel.Channels.Binding GetBindingForEndpoint(EndpointConfiguration endpointConfiguration)
        {
            if ((endpointConfiguration == EndpointConfiguration.LLSOrdersPort))
            {
                System.ServiceModel.BasicHttpBinding result = new System.ServiceModel.BasicHttpBinding();
                result.MaxBufferSize = int.MaxValue;
                result.ReaderQuotas = System.Xml.XmlDictionaryReaderQuotas.Max;
                result.MaxReceivedMessageSize = int.MaxValue;
                result.AllowCookies = true;
                result.Security.Mode = System.ServiceModel.BasicHttpSecurityMode.Transport;
                return result;
            }
            throw new System.InvalidOperationException(string.Format("Could not find endpoint with name \'{0}\'.", endpointConfiguration));
        }
        
        private static System.ServiceModel.EndpointAddress GetEndpointAddress(EndpointConfiguration endpointConfiguration)
        {
            if ((endpointConfiguration == EndpointConfiguration.LLSOrdersPort))
            {
                return new System.ServiceModel.EndpointAddress("https://llso.lingsoft.fi/api/v2/");
            }
            throw new System.InvalidOperationException(string.Format("Could not find endpoint with name \'{0}\'.", endpointConfiguration));
        }
        
        private static System.ServiceModel.Channels.Binding GetDefaultBinding()
        {
            return LingsoftClient.GetBindingForEndpoint(EndpointConfiguration.LLSOrdersPort);
        }
        
        private static System.ServiceModel.EndpointAddress GetDefaultEndpointAddress()
        {
            return LingsoftClient.GetEndpointAddress(EndpointConfiguration.LLSOrdersPort);
        }
        
        public enum EndpointConfiguration
        {
            
            LLSOrdersPort,
        }
        
        //New methods
        public IOrderResponse NewOrder(IOrder order)
        {
            return NewOrderAsync(order as LingsoftProd.Order).Result;
        }

        public IOrderStatusResponse OrderStatus(string llsoWorkID)
        {
            return OrderStatusAsync(llsoWorkID).Result;
        }

        public IUpdateOrderResponse UpdateOrder(string llsoWorkID, IOrder order)
        {
            return UpdateOrderAsync(llsoWorkID, order as LingsoftProd.Order).Result;
        }
        
        public ICancelOrderResponse CancelOrder(string llsoWorkID)
        {
            return CancelOrderAsync(llsoWorkID).Result;
        }
        
        //New methods
    }
}
