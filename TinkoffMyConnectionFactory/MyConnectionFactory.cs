using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using Tinkoff.Trading.OpenApi.Network;

namespace TinkoffMyConnectionFactory
{
    public class MyConnectionFactory
    {
        public static IConnection<IContext> GetConnection(string token)
        {
            HttpClient client = new HttpClient();
            client.DefaultRequestVersion = new Version(1, 1);
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            return new MyConnection("https://api-invest.tinkoff.ru/openapi/", "wss://api-invest.tinkoff.ru/openapi/md/v1/md-openapi/ws", token, client);
        }
    }
}
