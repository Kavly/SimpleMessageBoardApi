using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using BoardApi.Model;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace BoardApi.Controllers
{
    [Route("api/[controller]")]
    public class MessageBoardController : Controller
    {

        private static List<Message> Messages = new List<Message>();
        private static Mutex mutex = new Mutex();

        /// <summary>
        /// Constructor taking parameter for testing purposes
        /// </summary>
        /// <param name="messages"></param>
        public MessageBoardController(List<Message> messages = null)
        {
            if(messages != null)
                Messages = messages;
        }

        // Get api/messages
        [HttpGet]
        [Route("")]
        public ActionResult<IList<Message>> Get()
        {
            var messages = Messages.Where(msg => msg.IsDeleted == false).ToList();
            return Ok(messages);
        }

        // Post api/messages
        [HttpPost]
        public ActionResult<Message> Post(Message message)
        {
            if (message == null)
                return BadRequest();
            else if (String.IsNullOrEmpty(message.Text) || message.UserId == default(int))
                return BadRequest("Not a valid message or userId");
            else
            {
                // synchronized so that only one thread can enter.
                mutex.WaitOne();
                var msg = new Message
                {
                    Id = Messages.Count + 1,
                    Text = message.Text,
                    UserId = message.UserId,
                    Date = DateTime.Now
                };

                Messages.Add(msg);

                // release the mutex
                mutex.ReleaseMutex();

                return Ok(msg);
            }
        }

        // Put api/messages/id
        [HttpPut("{postId}")]
        public ActionResult<Message> Put(int id, Message message)
        {
            if (message == null || id <= 0)
                return BadRequest();
            else if (String.IsNullOrEmpty(message.Text) || message.UserId == default(int))
                return BadRequest("Not a valid message or userId");
            else
            {
                var userId = message.UserId;

                if (!TryGetMessage(id, out var msg))
                    return NotFound();
                else if (msg.UserId != userId)
                    return BadRequest("Not allowed for user");
                else
                {
                    msg.Text = message.Text;
                    return Ok(msg);
                }
            }
        }

        // Delete api/messages/id
        [HttpDelete("{id}")]
        public ActionResult<Message> Delete(int id, int userId)
        {
            if (id <= 0 || userId == default(int))
                return BadRequest();
            else
            {
                if (!TryGetMessage(id, out var msg))
                    return NotFound();
                else if (msg.UserId != userId)
                    return BadRequest("Not allowed for user");
                else
                {
                    msg.IsDeleted = true;
                    return Ok(msg);
                }
            }
        }

        private bool TryGetMessage(int postId, out Message message)
        {
            var msg = Messages.ElementAtOrDefault(postId - 1);

            if (msg != null && msg.IsDeleted == false)
            {
                message = msg;
                return true;
            }
            else
            {
                message = null;
                return false;
            }
        }
    }
}
