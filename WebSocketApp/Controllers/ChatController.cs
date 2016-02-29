using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;

namespace WebSocketApp.Controllers
{
    public class ChatController : ApiController
    {
        public HttpResponseMessage Get()
        {
            if (HttpContext.Current.IsWebSocketRequest || HttpContext.Current.IsWebSocketRequestUpgrading)
            {
                HttpContext.Current.AcceptWebSocketRequest(HandlerWebSocket);
            }
            return new HttpResponseMessage(HttpStatusCode.SwitchingProtocols);
        }

        private async Task HandlerWebSocket(WebSocketContext context)
        {
            var maxMessageSize = 1024;
            var buffer = new byte[maxMessageSize];
            var webSocket = context.WebSocket;
            while (webSocket.State == WebSocketState.Open)
            {
                var segment = new ArraySegment<byte>(buffer);
                var reviceResult = await webSocket.ReceiveAsync(segment, CancellationToken.None);
                if (reviceResult.MessageType == WebSocketMessageType.Binary)
                {
                    await webSocket.CloseAsync(WebSocketCloseStatus.InvalidMessageType, "Close", CancellationToken.None);
                }
                else if (reviceResult.MessageType == WebSocketMessageType.Close)
                {
                    await webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Close", CancellationToken.None);
                }
                else
                {
                    var message = Encoding.UTF8.GetString(segment.Array, 0, reviceResult.Count);
                    var sendMessage = $"{message} {DateTime.Now.ToLocalTime()}";
                    var messageSegment = new ArraySegment<byte>(Encoding.UTF8.GetBytes(sendMessage));

                    await webSocket.SendAsync(messageSegment, WebSocketMessageType.Text, true, CancellationToken.None);
                }
            }
        }
    }
}
