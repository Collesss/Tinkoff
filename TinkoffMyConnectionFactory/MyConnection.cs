using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using Tinkoff.Trading.OpenApi.Network;

namespace TinkoffMyConnectionFactory
{
    public class MyConnection : Connection, IConnection<IContext>
    {
        //private IContext _context;

        public MyConnection(string baseUri, string webSocketBaseUri, string token, HttpClient httpClient) : 
            base(baseUri, webSocketBaseUri, token, httpClient) 
        {
            Context = new MyContext(this);
        }

        public new IContext Context { get; }

        //IContext IConnection<IContext>.Context => throw new NotImplementedException();
    }
}
